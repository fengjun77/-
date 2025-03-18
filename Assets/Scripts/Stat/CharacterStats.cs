using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

//增益类型
public enum StatType
{
    strength,
    agility,
    intelegence,
    vitality,
    damage,
    critChance,
    critPower,
    health,
    armor,
    evasion,
    magicRes,
    fireDamage,
    iceDamage,
    lightningDamage
}

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("主要属性")]
    //力量
    public Stat strength;      //一点力量属性加一点伤害和一点暴击伤害
    //敏捷
    public Stat agility;       //一点敏捷属性加1%闪避率和1%暴击率
    //智慧
    public Stat intelligence;  //一点智力属性加三点魔法抗性
    //活力
    public Stat vitality;

    [Header("进攻属性")]
    //基础伤害
    public Stat damage;
    //暴击率
    public Stat critChance;
    //暴击伤害
    public Stat critPower;

    [Header("防御属性")]
    //生命值
    public Stat maxHealth;
    //护甲值
    public Stat armor;
    //闪避率
    public Stat evasion;
    //魔法抗性
    public Stat magicResistance;

    [Header("魔法属性")]
    //火焰伤害
    public Stat fireDamage;
    //冰冻伤害
    public Stat iceDamage;
    //雷电伤害
    public Stat lightningDamage;

    //是否被点燃
    public bool isIgnited;  //在一段时间内持续造成火焰伤害
    //是否被冰冻
    public bool isChilled;  //降低20%的护甲 并且移动速度减慢
    //是否被麻痹
    public bool isShocked;  //减少命中率

    //总燃烧计时器
    private float ignitedTimer;
    //冰冻计时器
    private float chilledTimer;
    //麻痹计时器
    private float shockedTimer;

    //每段燃烧间隔
    private float ignitedDamageCooldown = .3f;
    //每段燃烧计时器
    private float ignitedDamageTimer;
    //燃烧伤害
    private int igniteDamage;
    [SerializeField] private GameObject shockStrikePrefab;
    //电击伤害
    private int shockDamage;

    public int currentHealth;

    public System.Action onHealthChanged;

    public bool isDead {  get; private set; }
    private bool isVulnerable;
    private bool isInvincible; 

    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();

        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        ignitedDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;

        if (chilledTimer < 0)
            isChilled = false;

        if (shockedTimer < 0)
            isShocked = false;

        if(isIgnited)
            ApplyIgniteDamage();
    }

    public void MakeVulnerableFor(float _duration)
    {
        StartCoroutine(VulnerableCorutine(_duration));
    }

    private IEnumerator VulnerableCorutine(float _duration)
    {
        isVulnerable = true;

        yield return new WaitForSeconds(_duration);

        isVulnerable = false;
    }

    //增加增益
    public virtual void IncreaseStatBy(int _modifier,float _duration,Stat _statToModify)
    {
        StartCoroutine(StatModCoroutine(_modifier,_duration,_statToModify));
    }
    
    //协程 获取的增益，并在一段时间后消失
    IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);
        yield return new WaitForSeconds(_duration);
        _statToModify.RemoveModifier(_modifier);
    }

    //物理伤害
    public virtual void DoDamage(CharacterStats _targetStats,float _percentage = 1)
    {
        bool critHit = false;

        if (CanAvoidAttack(_targetStats))
            return;

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);

        //总伤害     基础伤害 + 力量
        int totalDamage = damage.GetValue() + strength.GetValue();

        //是否暴击
        if (CanCrit())
        {
            totalDamage = CalculatCriticalDamage(totalDamage);
            critHit = true;
        }

        fx.CreateHitFX(_targetStats.transform,critHit);

        //根据护甲值减少伤害
        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(Mathf.RoundToInt(totalDamage * _percentage));

    }

    //魔法伤害
    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();

        int totalMagicalDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();
        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);

        _targetStats.TakeDamage(totalMagicalDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0)
            return;

        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;

        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < .3f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilment(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < .41f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilment(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if(Random.value < .72f && _lightningDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilment(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }

        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        if (canApplyShock)
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightningDamage * .4f));

        _targetStats.ApplyAilment(canApplyIgnite,canApplyChill, canApplyShock);
     }

    //目标魔法抗性检测
    private static int CheckTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        //总魔法伤害 -= 魔法抗性 + （智力属性 * 3）
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }

    //异常状态检测 如果存在异常状态 则不添加 如果不存在 则施加新的异常状态
    public void ApplyAilment(bool _ignite, bool _chill, bool _shock)
    {
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;

        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = 2;

            fx.IgniteFxFor(2);
        }

        if(_chill && canApplyChill)
        {
            chilledTimer = 2;
            isChilled = _chill;

            //减少20%的移动速度 持续两秒
            GetComponent<Entity>().SlowEntityBy(.2f, 2);
            fx.ChillFxFor(2);
        }

        if (_shock && canApplyShock)
        {

            if (!isShocked)
            {
                ApplyShock(_shock);
            }
            else
            {
                if (GetComponent<Player>() != null)
                    return;
                HitNearesTargetWithShockStrike();
            }
        }
    }

    //造成感电状态
    public void ApplyShock(bool _shock)
    {
        if (isShocked)
            return;

        shockedTimer = 2;
        isShocked = _shock;

        fx.ShockFxFor(2);
    }

    //电击最近的目标
    private void HitNearesTargetWithShockStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 15);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }

            if (closestEnemy == null)
                closestEnemy = transform;
        }

        if (closestEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);

            newShockStrike.GetComponent<ShockStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    //造成燃烧伤害
    private void ApplyIgniteDamage()
    {
        if (ignitedDamageTimer < 0)
        {
            DecreaseHealthBy(igniteDamage);

            if (currentHealth <= 0 && !isDead)
                Die();

            ignitedDamageTimer = ignitedDamageCooldown;
        }
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;
    public void SetupShockStrikeDamage(int _damage) => shockDamage = _damage;

    //目标护甲检测
    protected int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if(_targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        else
            totalDamage -= _targetStats.armor.GetValue();

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    public virtual void OnEvasion()
    {

    }

    public virtual void OnSpeed()
    {

    }

    //能否闪避攻击
    protected bool CanAvoidAttack(CharacterStats _targetStats)
    {
        //总闪避率   闪避值 + 敏捷值
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        //如果自己被麻痹了 则增加目标的闪避率
        if (isShocked)
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {
            _targetStats.OnEvasion();
            _targetStats.OnSpeed(); 
            return true;
        }

        return false;
    }

    //受到伤害计算
    public virtual void TakeDamage(int _damage)
    {
        if (isInvincible)
            return;
        
        DecreaseHealthBy(_damage);
        
        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");

        if (currentHealth <= 0 && !isDead)
            Die();

    }

    //增加实时血量
    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHealth += _amount;

        if(currentHealth > GetMaxHealthValue())
            currentHealth = GetMaxHealthValue();
        
        onHealthChanged?.Invoke();
    }

    //减少实时血量
    protected virtual void DecreaseHealthBy(int _damage)
    {
        if (isVulnerable)
            _damage = Mathf.RoundToInt(_damage * 1.2f);

        currentHealth -= _damage;

        if(_damage > 0)
            fx.CreatePopUpText(_damage.ToString());

        onHealthChanged?.Invoke();        
    }

    //死亡相关
    protected virtual void Die()
    {
        isDead = true;
    }

    public void KillEntity()
    {
        if (!isDead)
            Die();
    }

    public void MakeInvincible(bool _invincible) => isInvincible = _invincible; 

    //能否暴击
    protected bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if(Random.Range(0, 100) < totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    //暴击伤害计算
    protected int CalculatCriticalDamage(int _damage)
    {
        //将暴击转换为百分比  （暴击伤害 + 力量) / 100 
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        
        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }

    //获取最大生命值
    public int GetMaxHealthValue()
    {
        //最大生命值 = 默认生命值 + 活力 * 5
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }


    public Stat StatOfType(StatType _statType)
    {
        if (_statType == StatType.strength)
            return strength;
        else if (_statType == StatType.agility)
            return agility;
        else if (_statType == StatType.intelegence)
            return intelligence;
        else if (_statType == StatType.vitality)
            return vitality;
        else if (_statType == StatType.damage)
            return damage;
        else if (_statType == StatType.critChance)
            return critChance;
        else if (_statType == StatType.critPower)
            return critPower;
        else if (_statType == StatType.health)
            return maxHealth;
        else if (_statType == StatType.armor)
            return armor;
        else if (_statType == StatType.evasion)
            return evasion;
        else if (_statType == StatType.magicRes)
            return magicResistance;
        else if (_statType == StatType.fireDamage)
            return fireDamage;
        else if (_statType == StatType.iceDamage)
            return iceDamage;
        else if (_statType == StatType.lightningDamage)
            return lightningDamage;

        return null;
    }
}

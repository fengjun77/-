using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

//��������
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

    [Header("��Ҫ����")]
    //����
    public Stat strength;      //һ���������Լ�һ���˺���һ�㱩���˺�
    //����
    public Stat agility;       //һ���������Լ�1%�����ʺ�1%������
    //�ǻ�
    public Stat intelligence;  //һ���������Լ�����ħ������
    //����
    public Stat vitality;

    [Header("��������")]
    //�����˺�
    public Stat damage;
    //������
    public Stat critChance;
    //�����˺�
    public Stat critPower;

    [Header("��������")]
    //����ֵ
    public Stat maxHealth;
    //����ֵ
    public Stat armor;
    //������
    public Stat evasion;
    //ħ������
    public Stat magicResistance;

    [Header("ħ������")]
    //�����˺�
    public Stat fireDamage;
    //�����˺�
    public Stat iceDamage;
    //�׵��˺�
    public Stat lightningDamage;

    //�Ƿ񱻵�ȼ
    public bool isIgnited;  //��һ��ʱ���ڳ�����ɻ����˺�
    //�Ƿ񱻱���
    public bool isChilled;  //����20%�Ļ��� �����ƶ��ٶȼ���
    //�Ƿ����
    public bool isShocked;  //����������

    //��ȼ�ռ�ʱ��
    private float ignitedTimer;
    //������ʱ��
    private float chilledTimer;
    //��Լ�ʱ��
    private float shockedTimer;

    //ÿ��ȼ�ռ��
    private float ignitedDamageCooldown = .3f;
    //ÿ��ȼ�ռ�ʱ��
    private float ignitedDamageTimer;
    //ȼ���˺�
    private int igniteDamage;
    [SerializeField] private GameObject shockStrikePrefab;
    //����˺�
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

    //��������
    public virtual void IncreaseStatBy(int _modifier,float _duration,Stat _statToModify)
    {
        StartCoroutine(StatModCoroutine(_modifier,_duration,_statToModify));
    }
    
    //Э�� ��ȡ�����棬����һ��ʱ�����ʧ
    IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);
        yield return new WaitForSeconds(_duration);
        _statToModify.RemoveModifier(_modifier);
    }

    //�����˺�
    public virtual void DoDamage(CharacterStats _targetStats,float _percentage = 1)
    {
        bool critHit = false;

        if (CanAvoidAttack(_targetStats))
            return;

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);

        //���˺�     �����˺� + ����
        int totalDamage = damage.GetValue() + strength.GetValue();

        //�Ƿ񱩻�
        if (CanCrit())
        {
            totalDamage = CalculatCriticalDamage(totalDamage);
            critHit = true;
        }

        fx.CreateHitFX(_targetStats.transform,critHit);

        //���ݻ���ֵ�����˺�
        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(Mathf.RoundToInt(totalDamage * _percentage));

    }

    //ħ���˺�
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

    //Ŀ��ħ�����Լ��
    private static int CheckTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        //��ħ���˺� -= ħ������ + ���������� * 3��
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }

    //�쳣״̬��� ��������쳣״̬ ����� ��������� ��ʩ���µ��쳣״̬
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

            //����20%���ƶ��ٶ� ��������
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

    //��ɸе�״̬
    public void ApplyShock(bool _shock)
    {
        if (isShocked)
            return;

        shockedTimer = 2;
        isShocked = _shock;

        fx.ShockFxFor(2);
    }

    //��������Ŀ��
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

    //���ȼ���˺�
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

    //Ŀ�껤�׼��
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

    //�ܷ����ܹ���
    protected bool CanAvoidAttack(CharacterStats _targetStats)
    {
        //��������   ����ֵ + ����ֵ
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        //����Լ�������� ������Ŀ���������
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

    //�ܵ��˺�����
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

    //����ʵʱѪ��
    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHealth += _amount;

        if(currentHealth > GetMaxHealthValue())
            currentHealth = GetMaxHealthValue();
        
        onHealthChanged?.Invoke();
    }

    //����ʵʱѪ��
    protected virtual void DecreaseHealthBy(int _damage)
    {
        if (isVulnerable)
            _damage = Mathf.RoundToInt(_damage * 1.2f);

        currentHealth -= _damage;

        if(_damage > 0)
            fx.CreatePopUpText(_damage.ToString());

        onHealthChanged?.Invoke();        
    }

    //�������
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

    //�ܷ񱩻�
    protected bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if(Random.Range(0, 100) < totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    //�����˺�����
    protected int CalculatCriticalDamage(int _damage)
    {
        //������ת��Ϊ�ٷֱ�  �������˺� + ����) / 100 
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        
        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }

    //��ȡ�������ֵ
    public int GetMaxHealthValue()
    {
        //�������ֵ = Ĭ������ֵ + ���� * 5
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

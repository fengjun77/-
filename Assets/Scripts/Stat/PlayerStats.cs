using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;

    protected override void Start()
    {
        base.Start();
        player = GetComponent<Player>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();

        player.Die();

        GameManager.instance.lostMoneyAmount = PlayerManager.instance.money;
        PlayerManager.instance.money = 0;

        GetComponent<PlayerItemDrop>().GenerateDrop();
    }

    //生命值减少时的逻辑
    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

        if(isDead)
            return;

        if (_damage > GetMaxHealthValue() * .25f)
        {
            player.SetupKnockbackPower(new Vector2(10, 6));
            player.fx.ScreenShake(player.fx.shakeHighDamage);

            AudioManager.instance.PlaySFX(11);
        }

        ItemData_Equipment currentArmor = Inventory.instance.GetEquipmentType(EquipmentType.Armor);

        if (currentArmor != null)
            currentArmor.ExecuteItemEffect(player.transform);

        ItemData_Equipment currentAmulet = Inventory.instance.GetEquipmentType(EquipmentType.Amulet);

        if (currentAmulet != null)
            currentAmulet.ExecuteItemEffect(player.transform);
    }

    public override void OnEvasion()
    {
        SkillManager.instance.dodge.CreateMirageOnDodge();
    }

    public override void OnSpeed()
    {
        SkillManager.instance.dodge.AddSpeed();
    }

    public void CloneDoDamage(CharacterStats _targetStats, float _multiplier)
    {
        if (CanAvoidAttack(_targetStats))
            return;

        //总伤害     基础伤害 + 力量
        int totalDamage = damage.GetValue() + strength.GetValue();
        
        if (_multiplier > 0)
            totalDamage = Mathf.RoundToInt(totalDamage * _multiplier);

        //是否暴击
        if (CanCrit())
        {
            totalDamage = CalculatCriticalDamage(totalDamage);
        }

        //根据护甲值减少伤害
        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);
    }
}

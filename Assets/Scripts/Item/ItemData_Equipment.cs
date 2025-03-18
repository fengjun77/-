using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    //武器
    Weapon,
    //护甲
    Armor,
    //护符
    Amulet,
    //药瓶
    Flask
}

[CreateAssetMenu(fileName = "新物品数据", menuName = "Data/Equipment")]
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;

    [Header("独特效果")]
    //装备效果
    public ItemEffect[] itemEffects;
    public float itemCooldown;
    [TextArea]
    public string itemEffectDescription;

    [Header("主要属性")]
    //力量
    public int strength;      //一点力量属性加一点伤害和一点暴击伤害
    //敏捷
    public int agility;       //一点敏捷属性加1%闪避率和1%暴击率
    //智慧
    public int intelligence;  //一点智力属性加三点魔法抗性
    //活力    
    public int vitality;

    [Header("进攻属性")]
    //基础伤害
    public int damage;
    //暴击率
    public int critChance;
    //暴击伤害
    public int critPower;

    [Header("防御属性")]
    //生命值
    public int maxHealth;
    //护甲值
    public int armor;
    //闪避率
    public int evasion;
    //魔法抗性
    public int magicResistance;

    [Header("魔法属性")]
    //火焰伤害
    public int fireDamage;
    //冰冻伤害
    public int iceDamage;
    //雷电伤害
    public int lightingDamage;

    [Header("工艺")]
    public List<InventoryItem> craftingMaterial;

    private int descriptionLength;

    //执行物品效果
    public void ExecuteItemEffect(Transform _enemyPosition)
    {
        foreach (var item in itemEffects)
        {
            item.ExecuteEffect(_enemyPosition);
        }
    }

    public void AddModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitality);

        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);

        playerStats.maxHealth.AddModifier(maxHealth);
        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResistance.AddModifier(magicResistance);

        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightningDamage.AddModifier(lightingDamage);
    }

    public void RemoveModifiers() 
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);
        playerStats.vitality.RemoveModifier(vitality);

        playerStats.damage.RemoveModifier(damage);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critPower.RemoveModifier(critPower);
            
        playerStats.maxHealth.RemoveModifier(maxHealth);
        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicResistance.RemoveModifier(magicResistance);

        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightningDamage.RemoveModifier(lightingDamage);
    }

    //获取装备描述
    public override string GetDescription()
    {
        sb.Length = 0;
        descriptionLength = 0;

        AddItemDescription(strength, "力量");
        AddItemDescription(agility, "敏捷");
        AddItemDescription(intelligence, "智慧");
        AddItemDescription(vitality, "活力");

        AddItemDescription(damage, "伤害");
        AddItemDescription(critChance, "暴击率");
        AddItemDescription(critPower, "暴击伤害");
        
        AddItemDescription(maxHealth, "血量");
        AddItemDescription(evasion, "闪避率");
        AddItemDescription(armor, "护甲");
        AddItemDescription(magicResistance, "魔法抗性");

        AddItemDescription(fireDamage, "火焰伤害");
        AddItemDescription(iceDamage, "冰冻伤害");
        AddItemDescription(lightingDamage, "电击伤害");

        if(descriptionLength < 5)
        {
            for (int i = 0; i < 5 - descriptionLength; i++)
            {
                sb.AppendLine();
                sb.Append(" ");
            }
        }

        if(itemEffectDescription.Length > 0)
        {
            sb.AppendLine();
            sb.Append(itemEffectDescription);
        }
            
        return sb.ToString();
    }

    //添加装备描述
    private void AddItemDescription(int _value, string _name)
    {
        if (_value != 0)
        {
            if (sb.Length > 0)
                sb.AppendLine();

            if (_value > 0)
                sb.Append("+" +  _value + " " + _name);

            descriptionLength++;
        }
    }
}

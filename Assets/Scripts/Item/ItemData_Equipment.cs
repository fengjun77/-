using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    //����
    Weapon,
    //����
    Armor,
    //����
    Amulet,
    //ҩƿ
    Flask
}

[CreateAssetMenu(fileName = "����Ʒ����", menuName = "Data/Equipment")]
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;

    [Header("����Ч��")]
    //װ��Ч��
    public ItemEffect[] itemEffects;
    public float itemCooldown;
    [TextArea]
    public string itemEffectDescription;

    [Header("��Ҫ����")]
    //����
    public int strength;      //һ���������Լ�һ���˺���һ�㱩���˺�
    //����
    public int agility;       //һ���������Լ�1%�����ʺ�1%������
    //�ǻ�
    public int intelligence;  //һ���������Լ�����ħ������
    //����    
    public int vitality;

    [Header("��������")]
    //�����˺�
    public int damage;
    //������
    public int critChance;
    //�����˺�
    public int critPower;

    [Header("��������")]
    //����ֵ
    public int maxHealth;
    //����ֵ
    public int armor;
    //������
    public int evasion;
    //ħ������
    public int magicResistance;

    [Header("ħ������")]
    //�����˺�
    public int fireDamage;
    //�����˺�
    public int iceDamage;
    //�׵��˺�
    public int lightingDamage;

    [Header("����")]
    public List<InventoryItem> craftingMaterial;

    private int descriptionLength;

    //ִ����ƷЧ��
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

    //��ȡװ������
    public override string GetDescription()
    {
        sb.Length = 0;
        descriptionLength = 0;

        AddItemDescription(strength, "����");
        AddItemDescription(agility, "����");
        AddItemDescription(intelligence, "�ǻ�");
        AddItemDescription(vitality, "����");

        AddItemDescription(damage, "�˺�");
        AddItemDescription(critChance, "������");
        AddItemDescription(critPower, "�����˺�");
        
        AddItemDescription(maxHealth, "Ѫ��");
        AddItemDescription(evasion, "������");
        AddItemDescription(armor, "����");
        AddItemDescription(magicResistance, "ħ������");

        AddItemDescription(fireDamage, "�����˺�");
        AddItemDescription(iceDamage, "�����˺�");
        AddItemDescription(lightingDamage, "����˺�");

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

    //���װ������
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

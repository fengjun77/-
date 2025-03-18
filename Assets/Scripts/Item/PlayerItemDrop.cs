using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("��ҵ���")]
    [SerializeField] private float chanceToLooseItems;
    //[SerializeField] private float chanceToLooseMaterials;

    public override void GenerateDrop()
    {
        Inventory inventory = Inventory.instance;

        //��Ҫ�Ƴ���װ��
        List<InventoryItem> itemsToUnequip = new List<InventoryItem>();
        //��Ҫ�Ƴ��Ĳ���
        //List<InventoryItem> materialsToLoose = new List<InventoryItem>();

        //��������װ��
        foreach (InventoryItem item in inventory.GetEquipmentList())
        {
            //������������㣬���Ƴ���װ��
            if (Random.Range(0, 100) <= chanceToLooseItems)
            {
                DropItem(item.data);
                itemsToUnequip.Add(item);
            }
        }

        //for (int i = 0; i < itemsToUnequip.Count; i++)
        //{
        //    inventory.UnequipItem(itemsToUnequip[i].data as ItemData_Equipment);
        //}
        

        //foreach (InventoryItem item in inventory.GetStashList())
        //{
        //    if (Random.Range(0, 100) <= chanceToLooseMaterials)
        //    {
        //        DropItem(item.data);
        //        itemsToUnequip.Add(item);
        //        materialsToLoose.Add(item);
        //    }
        //}

        //for(int i = 0;i < materialsToLoose.Count;i++)
        //{
        //    inventory.RemoveItem(materialsToLoose[i].data);
        //}
    }
}

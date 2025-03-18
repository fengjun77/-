using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("玩家掉落")]
    [SerializeField] private float chanceToLooseItems;
    //[SerializeField] private float chanceToLooseMaterials;

    public override void GenerateDrop()
    {
        Inventory inventory = Inventory.instance;

        //需要移除的装备
        List<InventoryItem> itemsToUnequip = new List<InventoryItem>();
        //需要移除的材料
        //List<InventoryItem> materialsToLoose = new List<InventoryItem>();

        //遍历所有装备
        foreach (InventoryItem item in inventory.GetEquipmentList())
        {
            //如果掉落率满足，则移除该装备
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

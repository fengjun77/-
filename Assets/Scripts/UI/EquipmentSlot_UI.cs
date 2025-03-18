using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 装备槽
/// </summary>
public class EquipmentSlot_UI : ItemSlot_UI
{
    public EquipmentType slotType;

    private void OnValidate()
    {
        gameObject.name = "装备类型 - " + slotType.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(item == null || item.data == null)
            return;

        Inventory.instance.UnequipItem(item.data as ItemData_Equipment);
        Inventory.instance.AddItem(item.data as ItemData_Equipment);
        
        ui.itemToolTip.HideToolTip();
        
        CleanUpSlot();
    }
}

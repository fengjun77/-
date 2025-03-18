using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// װ����
/// </summary>
public class EquipmentSlot_UI : ItemSlot_UI
{
    public EquipmentType slotType;

    private void OnValidate()
    {
        gameObject.name = "װ������ - " + slotType.ToString();
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

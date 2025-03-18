using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot_UI : MonoBehaviour, IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemText;

    protected UI ui;
    public InventoryItem item;

    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();
    }

    public void UpdateSlot(InventoryItem _newItem)
    {
        item = _newItem;
        itemImage.color = Color.white;

        if (item != null)
        {
            itemImage.sprite = item.data.icon;

            if (item.stackSize > 1)
            {
                itemText.text = item.stackSize.ToString();
            }
            else
            {
                itemText.text = " ";
            }
        }
    }

    //清空物品槽
    public void CleanUpSlot()
    {
        item = null;

        itemImage.sprite = null;
        itemImage.color = Color.clear;
        itemText.text = " ";
    }

    //接口 鼠标点击可以获得物品信息
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (item == null)
            return;

        //按住左Ctril 再点击可以删除物品
        if(Input.GetKey(KeyCode.LeftControl))
        {
            Inventory.instance.RemoveItem(item.data);
            return;
        }

        if (item.data.type == ItemType.Equipment)
        {
            Inventory.instance.EquipItem(item.data);
        }

        ui.itemToolTip.HideToolTip();
    }

    //鼠标进入时触发
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item == null)
            return;

        ui.itemToolTip.ShowToolTip(item.data as ItemData_Equipment);
    }

    //鼠标离开时触发
    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null)
            return;

        ui.itemToolTip?.HideToolTip();
    }
}

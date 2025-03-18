using UnityEngine.EventSystems;

public class CraftSlot_UI : ItemSlot_UI
{
    protected override void Start()
    {
        base.Start();
    }

    public void SetupCraftSlot(ItemData_Equipment _data)
    {
        if (_data == null)
            return; 

        item.data = _data;

        itemImage.sprite = _data.icon;
        itemText.text = _data.itemName;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        ui.craftWindow.SetupCraftWindow(item.data as ItemData_Equipment);
    }
}

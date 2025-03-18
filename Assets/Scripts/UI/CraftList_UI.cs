using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftList_UI : MonoBehaviour,IPointerDownHandler
{
    //合成槽父对象 用于存放所有可以合成的装备
    [SerializeField] private Transform craftSlotParent;
    //合成槽预设体
    [SerializeField] private GameObject carftSlotPrefab;

    //一个列表 存储所有可以合成的装备数据
    [SerializeField] private List<ItemData_Equipment> craftEquipment;
    
    void Start()
    {
        transform.parent.GetChild(0).GetComponent<CraftList_UI>().SetupCraftList();
        SetupDefaultCraftWindow();
    }

    public void SetupCraftList()
    {
        //删除原来的合成槽UI
        for (int i = 0; i < craftSlotParent.childCount; i++)
        {
            Destroy(craftSlotParent.GetChild(i).gameObject);
        }

        //创建对应的合成槽UI
        for (int i = 0; i < craftEquipment.Count; i++)
        {
            GameObject newSlot = Instantiate(carftSlotPrefab, craftSlotParent);
            newSlot.GetComponent<CraftSlot_UI>().SetupCraftSlot(craftEquipment[i]);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetupCraftList();
    }

    public void SetupDefaultCraftWindow()
    {
        if (craftEquipment[0] != null)
            GetComponentInParent<UI>().craftWindow.SetupCraftWindow(craftEquipment[0]);
    }
}

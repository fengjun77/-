using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftList_UI : MonoBehaviour,IPointerDownHandler
{
    //�ϳɲ۸����� ���ڴ�����п��Ժϳɵ�װ��
    [SerializeField] private Transform craftSlotParent;
    //�ϳɲ�Ԥ����
    [SerializeField] private GameObject carftSlotPrefab;

    //һ���б� �洢���п��Ժϳɵ�װ������
    [SerializeField] private List<ItemData_Equipment> craftEquipment;
    
    void Start()
    {
        transform.parent.GetChild(0).GetComponent<CraftList_UI>().SetupCraftList();
        SetupDefaultCraftWindow();
    }

    public void SetupCraftList()
    {
        //ɾ��ԭ���ĺϳɲ�UI
        for (int i = 0; i < craftSlotParent.childCount; i++)
        {
            Destroy(craftSlotParent.GetChild(i).gameObject);
        }

        //������Ӧ�ĺϳɲ�UI
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

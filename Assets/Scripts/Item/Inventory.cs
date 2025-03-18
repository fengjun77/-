using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Inventory : MonoBehaviour,ISaveManager
{
    public static Inventory instance;

    //��ʼ��Ʒ
    public List<ItemData> startingItem;

    //�洢���������ϵ�װ��
    public List<InventoryItem> equipment;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;

    //���װ��
    public List<InventoryItem> inventory;
    //�����ֵ�洢��Ʒ����ͬ��Ʒ��ѵ�
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;
    
    //�洢����
    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;

    [Header("��汳��UI")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSlotParent;
    [SerializeField] private Transform statSlotParent;

    //װ����洢��
    private ItemSlot_UI[] inventoryItemSlot;
    //��������
    private ItemSlot_UI[] stashItemSlot;
    //װ����
    private EquipmentSlot_UI[] equipmentSlot;
    //״̬��
    private StatSlot_UI[] statSlot;

    [Header("��Ʒ��ȴ")]
    private float lastTimeUsedFlask;
    private float lastTimeUseArmor;

    public float flaskCooldown;
    private float armorCooldown;

    public List<ItemData> itemDataBase;
    public List<InventoryItem> loadedItems;
    public List<ItemData_Equipment> loadedEquipment;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();

        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<ItemSlot_UI>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<ItemSlot_UI>();
        equipmentSlot = equipmentSlotParent.GetComponentsInChildren<EquipmentSlot_UI>();
        statSlot = statSlotParent.GetComponentsInChildren<StatSlot_UI>();

        StartCoroutine(DelayedAddStartingItems());
    }

    private IEnumerator DelayedAddStartingItems()
    {
        yield return new WaitForEndOfFrame(); // �ȴ� Start() ������г�ʼ�����
        AddStartingItems();
    }

    //��ӳ�ʼװ��
    private void AddStartingItems()
    {
        foreach(ItemData_Equipment item in loadedEquipment)
            EquipItem(item);

        if(loadedItems.Count > 0)
        {
            foreach(InventoryItem item in loadedItems)
            {
                for (int i = 0; i < item.stackSize; i++)
                    AddItem(item.data);
            }

            return;
        }

        for (int i = 0; i < startingItem.Count; i++)
        {
            if (startingItem[i] != null)
                AddItem(startingItem[i]);
        }
    }

    //����װ��
    public void EquipItem(ItemData _item)
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;

        InventoryItem newItem = new InventoryItem(newEquipment);

        ItemData_Equipment oldEquipment = null;

        //�����ֵ�������װ��
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            //�������װ�������Һ�׼��������װ������һ��������֮ǰ��װ�����������µ�װ�� 
            if (item.Key.equipmentType == newEquipment.equipmentType)
            {
                oldEquipment = item.Key;
            }
        }

        if (oldEquipment != null)
        {
            //�Ƴ���תװ��
            UnequipItem(oldEquipment);
            //����װ���Ż�װ������
            AddItem(oldEquipment);
        }

        //װ����װ����װ����
        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        //���װ������Ч��
        newEquipment.AddModifiers();
        //����װ����װ�������Ƴ�
        RemoveItem(_item);
    }

    //ж��װ��
    public void UnequipItem(ItemData_Equipment itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
        {
            //�Ƴ�װ����Ϣ
            equipment.Remove(value);
            //�Ƴ�װ���ֵ��еĸ�װ��
            equipmentDictionary.Remove(itemToRemove);
            //�Ƴ�װ��������������
            itemToRemove.RemoveModifiers();
        }
    }

    //������Ʒ�� �ڸ���ǰ�����������Ʒ�� (��ֹ��������Ʒ���޷����µ�����)
    private void UpdateSlotUI()
    {
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            {
                //���װ��������ͬ����װ��������װ������
                if (item.Key.equipmentType == equipmentSlot[i].slotType)
                {
                    equipmentSlot[i].UpdateSlot(item.Value);
                }
            }
        }

        for (int i = 0; i < inventoryItemSlot.Length; i++)
        {
            inventoryItemSlot[i].CleanUpSlot();
        }

        for (int i = 0; i < stashItemSlot.Length; i++)
        {
            stashItemSlot[i].CleanUpSlot();
        }

        //���¸�����Ʒʵ����ͼ�������
        for (int i = 0; i < inventory.Count; i++)
        {
            inventoryItemSlot[i].UpdateSlot(inventory[i]);
        }

        for (int i = 0; i < stash.Count; i++)
        {
            stashItemSlot[i].UpdateSlot(stash[i]);
        }

        UpdateStatsUI();
    }

    public void UpdateStatsUI()
    {
        for (int i = 0; i < statSlot.Length; i++)
        {
            statSlot[i].UpdateStatValueUI();
        }
    }

    //�����Ʒ
    public void AddItem(ItemData _item)
    {
        if (_item.type == ItemType.Equipment && CanAddItem())
        {
            AddToInventory(_item);
        }
        else if(_item.type == ItemType.Material)
        {
            AddToStash(_item);
        }

        UpdateSlotUI();
    }

    private void AddToStash(ItemData _item)
    {
        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
        }
    }

    private void AddToInventory(ItemData _item)
    {
        //������������Ʒ���� ��᷵��true �������Ӹ���Ʒʵ���Ķѵ�����
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        //���򴴽��µ���Ʒ���� ���������Ʒʵ�����뱳���У��������ֵ䷽�����
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }
    }

    //�Ƴ���Ʒ
    public void RemoveItem(ItemData _item)
    {
        if(inventoryDictionary.TryGetValue(_item,out InventoryItem value))
        {
            //�������ֻ��һ���߲��㣬���Ƴ������Ʒʵ�� �����ֵ���ɾ������Ʒ����
            if(value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(_item);
            }
            //������ٶѵ�����
            else
                value.RemoveStack();
        }

        if(stashDictionary.TryGetValue(_item, out InventoryItem stashValue))
        {
            if(stashValue.stackSize <= 1)
            {
                stash.Remove(stashValue);
                stashDictionary.Remove(_item);
            }
            else
                stashValue.RemoveStack();
        }

        UpdateSlotUI();
    }

    public bool CanAddItem()
    {
        if(inventory.Count >= inventoryItemSlot.Length)
        {
            return false;
        }

        return true;
    }

    public bool CanCraft(ItemData_Equipment _itemToCraft,List<InventoryItem> _requiredMaterials)
    {
        //��Ҫ���Ƴ��Ĳ����б�
        List<InventoryItem> materialToRemove = new List<InventoryItem>();

        for (int i = 0; i < _requiredMaterials.Count; i++)
        {
            //Ѱ�Ҳ��ϱ�������û����Ҫ�Ĳ���
            if (stashDictionary.TryGetValue(_requiredMaterials[i].data, out InventoryItem stashValue))
            {
                //���������������Ҫ������
                if(stashValue.stackSize < _requiredMaterials[i].stackSize)
                {
                    Debug.Log("û���㹻�Ĳ���");
                    return false;
                }
                //����������������� ����ӽ��Ƴ��б�
                else
                {
                    for (int j = 0; j < _requiredMaterials[i].stackSize; j++)
                    {
                        materialToRemove.Add(stashValue);
                    }
                }
            }
            //������ϱ���û�иò���
            else
            {
                Debug.Log("û���㹻�Ĳ���");
                return false;
            }
        }
        //�������Ƴ��Ĳ����б�
        for (int i = 0; i < materialToRemove.Count; i++)
        {
            RemoveItem(materialToRemove[i].data);
        }
        //��������������
        AddItem(_itemToCraft);
        Debug.Log("�������װ�� -" +  _itemToCraft.name);

        return true;
    }

    //��ȡ����װ�����б�
    public List<InventoryItem> GetEquipmentList() => equipment;

    public List<InventoryItem> GetStashList() => stash;

    //��ȡװ����Ʒ����
    public ItemData_Equipment GetEquipmentType(EquipmentType _type)
    {
        ItemData_Equipment equipedItem = null;

        //�����ֵ�������װ��
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == _type)
            {
                equipedItem = item.Key;
            }
        }

        return equipedItem;
    }

    //ʹ��ҩƿ
    public void UseFlask()
    {
        ItemData_Equipment currentFlask = GetEquipmentType(EquipmentType.Flask);

        if (currentFlask == null)
            return;

        bool canUseFlask = Time.time > lastTimeUsedFlask + flaskCooldown;

        if (canUseFlask)
        {
            flaskCooldown = currentFlask.itemCooldown;
            currentFlask.ExecuteItemEffect(null);
            lastTimeUsedFlask = Time.time;
        }
        else
            Debug.Log("ҩƿ������ȴ");
    }

    public bool CanUseArmor()
    {
        ItemData_Equipment currentArmor = GetEquipmentType(EquipmentType.Armor);

        if(Time.time > lastTimeUseArmor + armorCooldown)
        {
            armorCooldown = currentArmor.itemCooldown;
            lastTimeUseArmor = Time.time;
            return true;
        }

        Debug.Log("���ױ���������ȴ");
        return false;
    }

    public void LoadData(GameData _data)
    {
        foreach (KeyValuePair<string, int> pair in _data.inventory)
        {
            foreach(var item in itemDataBase)
            {
                if(item != null && item.itemId == pair.Key)
                {
                    InventoryItem itemToLoad = new InventoryItem(item);
                    itemToLoad.stackSize = pair.Value;

                    loadedItems.Add(itemToLoad);
                }
            }
        }

        foreach(string loadedItemId in _data.equipmentId)
        {
            foreach(var item in itemDataBase)
            {
                if(item != null && loadedItemId == item.itemId)
                {
                    loadedEquipment.Add(item as ItemData_Equipment);
                }
            }
        }

        Debug.Log("������");
    }

    public void SaveData(ref GameData _data)
    {
        _data.inventory.Clear();
        _data.equipmentId.Clear();

        foreach(KeyValuePair<ItemData,InventoryItem> pair in inventoryDictionary)
        {
            _data.inventory.Add(pair.Key.itemId,pair.Value.stackSize);
        }

        foreach(KeyValuePair<ItemData,InventoryItem> pair in stashDictionary)
        {
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        foreach(KeyValuePair<ItemData_Equipment,InventoryItem> pair in equipmentDictionary)
        {
            _data.equipmentId.Add(pair.Key.itemId);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Fill up item data base")]
    private void FillUpItemDataBase() => itemDataBase = new List<ItemData>(GetItemDataBase());

    private List<ItemData> GetItemDataBase()
    {
        List<ItemData> itemDataBase = new List<ItemData>();
        string[] assetNames = AssetDatabase.FindAssets("", new[] { "Assets/Data/Items"});

        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOpath);
            itemDataBase.Add(itemData);
        }

        return itemDataBase;
    }
#endif
}

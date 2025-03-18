using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Inventory : MonoBehaviour,ISaveManager
{
    public static Inventory instance;

    //初始物品
    public List<ItemData> startingItem;

    //存储穿戴在身上的装备
    public List<InventoryItem> equipment;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;

    //库存装备
    public List<InventoryItem> inventory;
    //利用字典存储物品，相同物品则堆叠
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;
    
    //存储杂物
    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;

    [Header("库存背包UI")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSlotParent;
    [SerializeField] private Transform statSlotParent;

    //装备类存储栏
    private ItemSlot_UI[] inventoryItemSlot;
    //杂物类栏
    private ItemSlot_UI[] stashItemSlot;
    //装备栏
    private EquipmentSlot_UI[] equipmentSlot;
    //状态栏
    private StatSlot_UI[] statSlot;

    [Header("物品冷却")]
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
        yield return new WaitForEndOfFrame(); // 等待 Start() 里的所有初始化完成
        AddStartingItems();
    }

    //添加初始装备
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

    //穿戴装备
    public void EquipItem(ItemData _item)
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;

        InventoryItem newItem = new InventoryItem(newEquipment);

        ItemData_Equipment oldEquipment = null;

        //遍历字典里所有装备
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            //如果存在装备，并且和准备穿戴的装备类型一样，则换下之前的装备，穿戴上新的装备 
            if (item.Key.equipmentType == newEquipment.equipmentType)
            {
                oldEquipment = item.Key;
            }
        }

        if (oldEquipment != null)
        {
            //移除旧转装备
            UnequipItem(oldEquipment);
            //将旧装备放回装备背包
            AddItem(oldEquipment);
        }

        //装备新装备到装备栏
        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        //添加装备增益效果
        newEquipment.AddModifiers();
        //将新装备从装备背包移除
        RemoveItem(_item);
    }

    //卸下装备
    public void UnequipItem(ItemData_Equipment itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
        {
            //移除装备信息
            equipment.Remove(value);
            //移除装备字典中的盖装备
            equipmentDictionary.Remove(itemToRemove);
            //移除装备所带来的增益
            itemToRemove.RemoveModifiers();
        }
    }

    //更新物品槽 在更新前先清空所有物品槽 (防止出现有物品槽无法更新的问题)
    private void UpdateSlotUI()
    {
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            {
                //如果装备类型相同，则将装备穿戴进装备槽中
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

        //更新各个物品实例的图像和数量
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

    //添加物品
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
        //如果存在这个物品数据 则会返回true 并且增加改物品实例的堆叠数量
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        //否则创建新的物品数据 并将这个物品实例加入背包中，最后加入字典方便查找
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }
    }

    //移除物品
    public void RemoveItem(ItemData _item)
    {
        if(inventoryDictionary.TryGetValue(_item,out InventoryItem value))
        {
            //如果数量只有一或者不足，则移除这个物品实例 并在字典中删除该物品数据
            if(value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(_item);
            }
            //否则减少堆叠数量
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
        //需要被移除的材料列表
        List<InventoryItem> materialToRemove = new List<InventoryItem>();

        for (int i = 0; i < _requiredMaterials.Count; i++)
        {
            //寻找材料背包中有没有需要的材料
            if (stashDictionary.TryGetValue(_requiredMaterials[i].data, out InventoryItem stashValue))
            {
                //如果材料少于所需要的数量
                if(stashValue.stackSize < _requiredMaterials[i].stackSize)
                {
                    Debug.Log("没有足够的材料");
                    return false;
                }
                //材料满足所需的数量 则添加进移除列表
                else
                {
                    for (int j = 0; j < _requiredMaterials[i].stackSize; j++)
                    {
                        materialToRemove.Add(stashValue);
                    }
                }
            }
            //如果材料背包没有该材料
            else
            {
                Debug.Log("没有足够的材料");
                return false;
            }
        }
        //遍历需移除的材料列表
        for (int i = 0; i < materialToRemove.Count; i++)
        {
            RemoveItem(materialToRemove[i].data);
        }
        //获得所制造的武器
        AddItem(_itemToCraft);
        Debug.Log("这是你的装备 -" +  _itemToCraft.name);

        return true;
    }

    //获取穿戴装备的列表
    public List<InventoryItem> GetEquipmentList() => equipment;

    public List<InventoryItem> GetStashList() => stash;

    //获取装备物品类型
    public ItemData_Equipment GetEquipmentType(EquipmentType _type)
    {
        ItemData_Equipment equipedItem = null;

        //遍历字典里所有装备
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == _type)
            {
                equipedItem = item.Key;
            }
        }

        return equipedItem;
    }

    //使用药瓶
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
            Debug.Log("药瓶正在冷却");
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

        Debug.Log("护甲被动正在冷却");
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

        Debug.Log("库存加载");
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

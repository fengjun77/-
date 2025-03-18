using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    //掉落物品数量
    [SerializeField] private int possibleItemDrop;
    //可能掉落的物品种类
    [SerializeField] private ItemData[] possibleDrop;
    //掉落列表
    private List<ItemData> dropList = new List<ItemData>();

    [SerializeField] private GameObject dropPrefab;

    //生成掉落物
    public virtual void GenerateDrop()
    {
        //遍历所有可能掉落的物品
        for (int i = 0; i < possibleDrop.Length; i++)
        {
            //如果符合条件就加入掉落列表
            if(Random.Range(0,100) <= possibleDrop[i].dropChance)
                dropList.Add(possibleDrop[i]);
        }

        if (dropList.Count == 0) // 确保掉落池里至少有一个物品
            return;

        int dropCount = Mathf.Min(possibleItemDrop, dropList.Count);
        //遍历掉落列表，将物品掉落出来
        for (int i = 0; i < dropCount; i++)
        {
            ItemData randomItem = dropList[Random.Range(0,dropList.Count)];

            dropList.Remove(randomItem);
            DropItem(randomItem);
        }
    }

    protected void DropItem(ItemData _itemData)
    {
        GameObject newDrop = Instantiate(dropPrefab,transform.position,Quaternion.identity);

        Vector2 randomVelocity = new Vector2(Random.Range(-5, 5), Random.Range(15, 20));

        newDrop.GetComponent<ItemObject>().SetupItem(_itemData, randomVelocity);
    }
}

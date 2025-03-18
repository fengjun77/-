using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    //������Ʒ����
    [SerializeField] private int possibleItemDrop;
    //���ܵ������Ʒ����
    [SerializeField] private ItemData[] possibleDrop;
    //�����б�
    private List<ItemData> dropList = new List<ItemData>();

    [SerializeField] private GameObject dropPrefab;

    //���ɵ�����
    public virtual void GenerateDrop()
    {
        //�������п��ܵ������Ʒ
        for (int i = 0; i < possibleDrop.Length; i++)
        {
            //������������ͼ�������б�
            if(Random.Range(0,100) <= possibleDrop[i].dropChance)
                dropList.Add(possibleDrop[i]);
        }

        if (dropList.Count == 0) // ȷ���������������һ����Ʒ
            return;

        int dropCount = Mathf.Min(possibleItemDrop, dropList.Count);
        //���������б�����Ʒ�������
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

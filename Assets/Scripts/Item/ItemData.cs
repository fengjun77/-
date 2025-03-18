using System.Text;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif
public enum ItemType
{
    Equipment,

    Material
}

[CreateAssetMenu(fileName = "����Ʒ����", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType type;
    public string itemId;

    [Range(0, 100)]
    public float dropChance;

    protected StringBuilder sb = new StringBuilder();

    private void OnValidate()
    {
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        itemId = AssetDatabase.AssetPathToGUID(path);
#endif
    }

    public virtual string GetDescription()
    {
        return "";
    }
}

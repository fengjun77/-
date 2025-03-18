using UnityEngine;

/// <summary>
/// 装备物品效果
/// </summary>
public class ItemEffect : ScriptableObject
{
    //执行效果
    public virtual void ExecuteEffect(Transform _enemyPosition)
    {
        Debug.Log("Effect");
    }
}

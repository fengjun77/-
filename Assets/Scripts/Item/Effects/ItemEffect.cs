using UnityEngine;

/// <summary>
/// װ����ƷЧ��
/// </summary>
public class ItemEffect : ScriptableObject
{
    //ִ��Ч��
    public virtual void ExecuteEffect(Transform _enemyPosition)
    {
        Debug.Log("Effect");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Freeze effect", menuName = "Data/Item effect/Freeze effect")]

public class Freeze_Effect : ItemEffect
{
    [SerializeField] private float duration;

    public override void ExecuteEffect(Transform _transform)
    {
        if(!Inventory.instance.CanUseArmor())
            return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, 3);

        foreach (var hit in colliders)
        {
            hit.GetComponent<Enemy>()?.FreezeTimeFor(duration);
        }
    }
}

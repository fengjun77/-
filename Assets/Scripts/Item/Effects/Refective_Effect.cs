using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Reflective effect", menuName = "Data/Item effect/Reflective effect")]
public class Refective_Effect : ItemEffect
{
    [Range(0f, 1f)]
    [SerializeField] private float reflectivePercentage;
    [SerializeField] private float distance;

    public override void ExecuteEffect(Transform _transform)
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        int damge = Mathf.RoundToInt(playerStats.GetMaxHealthValue() * reflectivePercentage);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, distance);

        foreach (var hit in colliders)
        {
            hit.GetComponent<EnemyStats>()?.TakeDamage(damge);
        }
    }
}

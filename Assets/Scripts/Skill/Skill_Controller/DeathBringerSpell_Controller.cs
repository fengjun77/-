using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerSpell_Controller : MonoBehaviour
{
    [SerializeField] private Transform check;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private LayerMask whatIsPlayer;

    private CharacterStats myStats;

    public void SetupSpell(CharacterStats _stats) => myStats = _stats;

    private void AnimationTrigger()
    {
        //λ�� ��Χ Ŀ���
        Collider2D[] colliders = Physics2D.OverlapBoxAll(check.position,boxSize,whatIsPlayer);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {
                hit.GetComponent<Entity>().SetupKnockbackDir(transform);
                //myStats.DoDamage(hit.GetComponent<CharacterStats>());
                myStats.DoMagicalDamage(hit.GetComponent<CharacterStats>());
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(check.position,boxSize);
    }

    private void SelfDestroy() => Destroy(gameObject);
}

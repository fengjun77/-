using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();

    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        AudioManager.instance.PlaySFX(2);

        //�������¼�����ʱ�������ڹ�����ײ���ڵ�����
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);
    
        foreach(var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats target = hit.GetComponent<EnemyStats>();

                player.stats.DoDamage(target);

                // inventory��ȡ���� ������ƷЧ��
                ItemData_Equipment weaponData = Inventory.instance.GetEquipmentType(EquipmentType.Weapon);
                ItemData_Equipment amuletData = Inventory.instance.GetEquipmentType(EquipmentType.Amulet);

                if (amuletData != null)
                {
                    amuletData.ExecuteItemEffect(target.transform);
                }

                if (weaponData != null)
                    weaponData.ExecuteItemEffect(target.transform);


            }
        }
    }

    private void ThrowSword()
    {
        SkillManager.instance.sword.CreateSword();
    }
}

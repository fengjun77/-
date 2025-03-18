using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal effect", menuName = "Data/Item effect/Heal effect")]
public class Heal_Effect : ItemEffect
{
    [Range(0f, 1f)]
    [SerializeField] private float healPercent;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        //���Ƶ�Ѫ��
        int healAmount = Mathf.RoundToInt(playerStats.GetMaxHealthValue() * healPercent);
        //�ָ�Ѫ��
        playerStats.IncreaseHealthBy(healAmount);

    }
}

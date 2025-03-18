using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff effect", menuName = "Data/Item effect/Buff effect")]
public class Buff_Effect : ItemEffect
{
    private PlayerStats stats;
    //增益类型
    [SerializeField] private StatType buffType;
    //增益数
    [SerializeField] private int buffAmount;
    //增益时间
    [SerializeField] private float buffDuration;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        stats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        stats.IncreaseStatBy(buffAmount,buffDuration,stats.StatOfType(buffType));
    }

}

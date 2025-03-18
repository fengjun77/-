using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBrinerTeleportState : EnemyState
{
    private Enemy_DeathBriner enemy;

    public DeathBrinerTeleportState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DeathBriner enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.stats.MakeInvincible(true);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.stats.MakeInvincible(false);
    }

    public override void Update()
    {
        base.Update();

        //����������ͺ�
        if(triggerCalled)
        {
            //������Խ���ʩ��
            if(enemy.CanDoSpellCast())
                stateMachine.ChangeState(enemy.castState);
            else
                stateMachine.ChangeState(enemy.battleState);
        }
    }
}

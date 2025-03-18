using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBrinerIdleState : EnemyState
{
    private Enemy_DeathBriner enemy;
    private Transform player;

    public DeathBrinerIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DeathBriner enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime;
        player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();

        AudioManager.instance.PlaySFX(5, enemy.transform);
    }

    public override void Update()
    {
        base.Update();

        if (Vector2.Distance(player.position,enemy.transform.position) < 15)
        {
            enemy.bossFightBegun = true;
        }

        if (Input.GetKeyDown(KeyCode.V))
            stateMachine.ChangeState(enemy.teleportState);

        if(stateTimer < 0 && enemy.bossFightBegun)
            stateMachine.ChangeState(enemy.battleState);
    }
}

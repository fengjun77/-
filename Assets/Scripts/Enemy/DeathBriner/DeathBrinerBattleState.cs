using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBrinerBattleState : EnemyState
{
    private Enemy_DeathBriner enemy;
    private Transform player;

    private int moveDir;

    public DeathBrinerBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DeathBriner enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;

        if (player.GetComponent<PlayerStats>().isDead)
            stateMachine.ChangeState(enemy.idleState);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsGroundDetected())
        {
            if (enemy.IsPlayerDetected())
            {
                stateTimer = enemy.battleTime;

                if (enemy.IsPlayerDetected().distance < enemy.atkDistance)
                {
                    if(CanAttack())
                        stateMachine.ChangeState(enemy.attackState);
                    else
                        stateMachine.ChangeState(enemy.idleState);
                }
            }

            if (player.position.x > enemy.transform.position.x)
                moveDir = 1;
            else if (player.position.x < enemy.transform.position.x)
                moveDir = -1;

            //如果怪物接近玩家了，则停下来进行攻击，不会继续接近
            if (enemy.IsPlayerDetected() && enemy.IsPlayerDetected().distance < enemy.atkDistance - .5f)
                return;

            enemy.SetVelocity(enemy.moveSpeed * moveDir, enemy.rb.velocity.y);
        }
        else
            stateMachine.ChangeState(enemy.idleState);
    }

    private bool CanAttack()
    {
        if (Time.time >= enemy.lastTimeAttacked + enemy.atkCoolDown)
        {
            enemy.lastTimeAttacked = Time.time;
            return true;
        }

        return false;
    }
}

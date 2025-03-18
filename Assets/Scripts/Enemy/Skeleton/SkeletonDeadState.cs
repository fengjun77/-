using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonDeadState : EnemyState
{
    private Enemy_Skeleton enemy;
    private float fadeSpeed = 3;
    public SkeletonDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        AudioManager.instance.PlaySFX(8);

        enemy.rb.isKinematic = true;

        stateTimer = 1.5f;
    }

    public override void Update()
    {
        base.Update();

        enemy.rb.velocity = new Vector2(0, -1);
        if(stateTimer < 0)
        {
            Color color = enemy.sr.color;
            color.a -= fadeSpeed * Time.deltaTime;

            enemy.sr.color = color;

            if (color.a <= 0)
                enemy.isDead = true;
        }
    }
}

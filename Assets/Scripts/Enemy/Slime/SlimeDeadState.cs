using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeDeadState : EnemyState
{
    private Enemy_Slime enemy;
    private float fadeSpeed = 3;

    public SlimeDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_Slime _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
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
        if (stateTimer < 0)
        {
            Color color = enemy.sr.color;
            color.a -= fadeSpeed * Time.deltaTime;

            enemy.sr.color = color;

            if (color.a <= 0)
                enemy.isDead = true;
        }
    }
}

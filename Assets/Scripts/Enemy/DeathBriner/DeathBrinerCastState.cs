using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBrinerCastState : EnemyState
{
    private Enemy_DeathBriner enemy;
    //施法次数
    private int amountOfSpells;
    //施法计时器
    private float spellTimer;
    public DeathBrinerCastState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DeathBriner enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();

        amountOfSpells = enemy.amountOfSpells;
        //因为有一个起手动作，所以第一次施法需要多0.5秒
        spellTimer = .5f;

        enemy.stats.iceDamage.AddModifier(20);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.lastTimeCast = Time.time;
        enemy.stats.iceDamage.RemoveModifier(20);
    }

    public override void Update()
    {
        base.Update();

        spellTimer -= Time.deltaTime;

        //如果可以施法则开始施法，否则传送
        if(CanCast())
        {
            enemy.CastSpell();
            amountOfSpells--;
        }
        

        if(amountOfSpells <= 0)
            stateMachine.ChangeState(enemy.teleportState);
    }


    private bool CanCast()
    {
        if(amountOfSpells > 0 && spellTimer < 0)
        {
            spellTimer = enemy.spellCooldown;
            return true;
        }

        return false;
    }
}

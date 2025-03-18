using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlackholeState : PlayerState
{
    private float flyTime = .4f;
    private bool skillUsed;

    private float defaultGravity;

    public PlayerBlackholeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();

        defaultGravity = player.rb.gravityScale;

        startTime = flyTime;
        skillUsed = false;
        rb.gravityScale = 0;

    }

    public override void Exit()
    {
        base.Exit();

        player.rb.gravityScale = defaultGravity;
        player.MakeTransparent(false);
    }

    public override void Update()
    {
        base.Update();

        if (startTime > 0)
        {
            rb.velocity = new Vector2(0, 15);
            player.fx.CreateAfterImage(player.fx.afterImageJumpPrefab);
        }
        if(startTime < 0)
        {
            rb.velocity = new Vector2(0, -.1f);

            if(!skillUsed)
            {
                if(player.skill.blackhole.CanUseSkill())
                    skillUsed = true;
            }
        }

        if (player.skill.blackhole.SkillCompleted())
            stateMachine.ChangeState(player.airState);
    }
}

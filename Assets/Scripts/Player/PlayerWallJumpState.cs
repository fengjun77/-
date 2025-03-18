using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        startTime = 1;

        player.SetVelocity(5 * -player.facingDir,player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(startTime < 0)
            stateMachine.ChangeState(player.airState);

        if(player.IsGroundDetected())
            stateMachine.ChangeState(player.idleState);

        if(player.IsWallDetected())
            stateMachine.ChangeState(player.wallSlideState);
    }
}

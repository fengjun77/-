using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrounedState : PlayerState
{
    public PlayerGrounedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword() && player.skill.sword.swordUnlocked)
            stateMachine.ChangeState(player.aimSword);

        if (Input.GetKeyDown(KeyCode.R) && player.skill.blackhole.blackholeUnlock)
        {
            if(player.skill.blackhole.cooldownTimer > 0)
            {
                player.fx.CreatePopUpText("������ȴ��");
                return;
            }
            stateMachine.ChangeState(player.blackHole);
        }

        if (Input.GetKeyDown(KeyCode.Z) && player.skill.parry.parryUnlocked && player.skill.parry.cooldownTimer < 0)
        {
            player.skill.parry.CanUseSkill();

            stateMachine.ChangeState(player.counterAttack);
        }
        else if (Input.GetKeyDown(KeyCode.Z) && player.skill.parry.cooldownTimer > 0)
            player.fx.CreatePopUpText("������ȴ��");

        if (Input.GetKeyDown(KeyCode.Mouse0))
            stateMachine.ChangeState(player.primaryAttack);

        if(!player.IsGroundDetected())
            stateMachine.ChangeState(player.airState);

        if(Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
            stateMachine.ChangeState(player.jumpState);
    }

    private bool HasNoSword()
    {
        if (!player.sword)
            return true;

        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();
        return false;
    }
}

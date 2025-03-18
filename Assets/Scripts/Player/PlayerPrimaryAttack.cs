using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttack : PlayerState
{
    public int comboCounter {  get; private set; }
    private float LastTimeAttack;
    private int comboWindow = 2;

    public PlayerPrimaryAttack(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //���������������2�����߹������ʱ�䳬�����趨��comboWindow ��������Ϊ��һ�ι���
        if (comboCounter > 2 || Time.time >= LastTimeAttack + comboWindow)
            comboCounter = 0;

        player.anim.SetInteger("ComboCounter", comboCounter);

        startTime = .1f;
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .1f);

        comboCounter++;
        LastTimeAttack = Time.time;
    }

    public override void Update()
    {
        base.Update();

        if(startTime < 0)
            player.SetZeroVelocity();

        if (triggleCalled)
            stateMachine.ChangeState(player.idleState);
    }
}

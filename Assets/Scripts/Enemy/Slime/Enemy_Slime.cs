using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlimeType
{
    big,
    medium,
    small
}

public class Enemy_Slime : Enemy
{
    [Header("史莱姆分裂相关")]
    //史莱姆类型
    [SerializeField] private SlimeType slimeType;
    //分裂数量
    [SerializeField] private int slimesToCreate;
    //史莱姆预设体
    [SerializeField] private GameObject slimePrefab;
    //史莱姆分裂后弹射力量
    [SerializeField] private Vector2 minCreationVelocity;
    [SerializeField] private Vector2 maxCreationVelocity;

    private EnemyStats enemyStats;

    public SlimeIdleState idleState {  get; private set; }
    public SlimeMoveState moveState { get; private set; }
    public SlimeStunnedState stunnedState { get; private set; }
    public SlimeBattleState battleState { get; private set; }
    public SlimeAttackState attackState { get; private set; }
    public SlimeDeadState deadState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        SetupDefaultFacingDir(-1);

        idleState = new SlimeIdleState(this,stateMachine,"Idle",this);
        moveState = new SlimeMoveState(this, stateMachine, "Move", this);
        attackState = new SlimeAttackState(this, stateMachine, "Attack", this);
        stunnedState = new SlimeStunnedState(this, stateMachine, "Stunned", this);
        battleState = new SlimeBattleState(this, stateMachine, "Move", this);
        deadState = new SlimeDeadState(this, stateMachine, "Dead", this);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Init(idleState);
        enemyStats = GetComponent<EnemyStats>();
    }

    protected override void Update()
    {
        base.Update();

        if (isDead)
            Destroy(gameObject);
    }

    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            stateMachine.ChangeState(stunnedState);
            return true;
        }
        return false;
    }

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);

        if (slimeType == SlimeType.small)
            return;

        CreateSlime(slimesToCreate,slimePrefab);
    }

    //创建分裂的史莱姆
    private void CreateSlime(int _amountOfSlime,GameObject _slimePrefab)
    {
        for (int i = 0; i < _amountOfSlime; i++)
        {
            GameObject newSlime = Instantiate(slimePrefab,transform.position, Quaternion.identity);

            //获取创建的史莱姆身上的组件来控制史莱姆分裂出去;
            newSlime.GetComponent<Enemy_Slime>().SetupSlime(facingDir,enemyStats.level);
        }
    }

    public void SetupSlime(int _facingDir,int _level)
    {
        if (_facingDir != facingDir)
            Flip();

        if (GetComponent<EnemyStats>().level >= 2)
        {
            _level = GetComponent<EnemyStats>().level - 1;
        }
        else
            _level = GetComponent<EnemyStats>().level;

        float xVelocity = Random.Range(minCreationVelocity.x,minCreationVelocity.y);
        float yVelocity = Random.Range(maxCreationVelocity.x,maxCreationVelocity.y);
        
        //激发僵直，无法移动
        isKnocked = true;

        GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * -_facingDir, yVelocity);

        //延迟取消僵直状态
        Invoke("CancelKnockback", 1.5f);
    }

    private void CancelKnockback() => isKnocked = false;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_DeathBriner : Enemy
{
    #region 状态
    public DeathBrinerAttackState attackState { get; private set; }
    public DeathBrinerBattleState battleState { get; private set; }
    public DeathBrinerCastState castState { get; private set; }
    public DeathBrinerDeadState deadState { get; private set; }
    public DeathBrinerIdleState idleState { get; private set; }
    public DeathBrinerTeleportState teleportState { get; private set; }

    #endregion

    public bool bossFightBegun;

    [Header("传送")]
    [SerializeField] private BoxCollider2D arena;
    [SerializeField] private Vector2 surroundingCheckSize;

    [Header("施法")]
    [SerializeField] private GameObject spellPrefab;
    public int amountOfSpells;
    public float spellCooldown;
    public float lastTimeCast;
    //施法整体冷却时间
    [SerializeField] private float spellStateCooldown;

    //传送机会
    public float chanceToTeleport;
    //默认传送机会
    public float defaultChanceToTeleport = 25;

    protected override void Awake()
    {
        base.Awake();

        SetupDefaultFacingDir(-1);

        idleState = new DeathBrinerIdleState(this, stateMachine, "Idle", this);
        battleState = new DeathBrinerBattleState(this, stateMachine, "Move", this);
        attackState = new DeathBrinerAttackState(this, stateMachine, "Attack", this);
        deadState = new DeathBrinerDeadState(this, stateMachine, "Dead", this);
        teleportState = new DeathBrinerTeleportState(this, stateMachine, "Teleport", this);
        castState = new DeathBrinerCastState(this, stateMachine, "Cast", this);
    }

    protected override void Start()
    {
        base.Start();

        //初始化状态
        stateMachine.Init(idleState);
    }

    protected override void Update()
    {
        base.Update();

        if(isDead)
            Destroy(gameObject);
    }

    //死亡切换状态
    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }

    //施法逻辑
    public void CastSpell()
    {
        Player player = PlayerManager.instance.player;

        float xOffset = 0;

        if(player.rb.velocity.x != 0)
            xOffset = player.facingDir * Random.Range(1, 3);

        Vector3 spellPosition = new Vector3(player.transform.position.x + xOffset, player.transform.position.y + 1.5f);

        GameObject newSpell = Instantiate(spellPrefab, spellPosition, Quaternion.identity);
        newSpell.GetComponent<DeathBringerSpell_Controller>().SetupSpell(stats);
    }

    //寻找合适位置放置对象
    public void FindPosition()
    {
        //随机获取一个坐标，在arena碰撞体内 加偏移防止对象靠近边界
        float x = Random.Range(arena.bounds.min.x + 3,arena.bounds.max.x -3);
        float y = Random.Range(arena.bounds.min.y + 3,arena.bounds.max.y -3);

        transform.position = new Vector3(x,y);
        //所在高度减去离地面的高度再加上对象碰撞体一半的高度，使对象站在地上
        transform.position = new Vector3(transform.position.x, transform.position.y - GroundBelow().distance + (cd.size.y / 2));
    
        if(!GroundBelow() || SomethingIsAround())
        {
            Debug.Log("寻找新位置");
            FindPosition();
        }
    }

    //检查对象下方是否有地面
    private RaycastHit2D GroundBelow() => Physics2D.Raycast(transform.position, Vector2.down, 100, whatIsGround);
    //检查对象周围是否有障碍物
    private bool SomethingIsAround() => Physics2D.BoxCast(transform.position, surroundingCheckSize, 0, Vector2.zero, 0, whatIsGround);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawLine(transform.position,new Vector3(transform.position.x, transform.position.y - GroundBelow().distance));
        Gizmos.DrawWireCube(transform.position, surroundingCheckSize);
    }

    //是否可以传送
    public bool CanTeleport()
    {
        if(Random.Range(0,100) <= chanceToTeleport)
        {
            chanceToTeleport = defaultChanceToTeleport;
            return true;
        }

        return false;
    }

    //是否可以施法攻击
    public bool CanDoSpellCast()
    {
        if(Time.time >= lastTimeCast + spellStateCooldown)
        {
            return true;
        }

        return false;
    }
}

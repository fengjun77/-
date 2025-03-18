using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_DeathBriner : Enemy
{
    #region ״̬
    public DeathBrinerAttackState attackState { get; private set; }
    public DeathBrinerBattleState battleState { get; private set; }
    public DeathBrinerCastState castState { get; private set; }
    public DeathBrinerDeadState deadState { get; private set; }
    public DeathBrinerIdleState idleState { get; private set; }
    public DeathBrinerTeleportState teleportState { get; private set; }

    #endregion

    public bool bossFightBegun;

    [Header("����")]
    [SerializeField] private BoxCollider2D arena;
    [SerializeField] private Vector2 surroundingCheckSize;

    [Header("ʩ��")]
    [SerializeField] private GameObject spellPrefab;
    public int amountOfSpells;
    public float spellCooldown;
    public float lastTimeCast;
    //ʩ��������ȴʱ��
    [SerializeField] private float spellStateCooldown;

    //���ͻ���
    public float chanceToTeleport;
    //Ĭ�ϴ��ͻ���
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

        //��ʼ��״̬
        stateMachine.Init(idleState);
    }

    protected override void Update()
    {
        base.Update();

        if(isDead)
            Destroy(gameObject);
    }

    //�����л�״̬
    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }

    //ʩ���߼�
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

    //Ѱ�Һ���λ�÷��ö���
    public void FindPosition()
    {
        //�����ȡһ�����꣬��arena��ײ���� ��ƫ�Ʒ�ֹ���󿿽��߽�
        float x = Random.Range(arena.bounds.min.x + 3,arena.bounds.max.x -3);
        float y = Random.Range(arena.bounds.min.y + 3,arena.bounds.max.y -3);

        transform.position = new Vector3(x,y);
        //���ڸ߶ȼ�ȥ�����ĸ߶��ټ��϶�����ײ��һ��ĸ߶ȣ�ʹ����վ�ڵ���
        transform.position = new Vector3(transform.position.x, transform.position.y - GroundBelow().distance + (cd.size.y / 2));
    
        if(!GroundBelow() || SomethingIsAround())
        {
            Debug.Log("Ѱ����λ��");
            FindPosition();
        }
    }

    //�������·��Ƿ��е���
    private RaycastHit2D GroundBelow() => Physics2D.Raycast(transform.position, Vector2.down, 100, whatIsGround);
    //��������Χ�Ƿ����ϰ���
    private bool SomethingIsAround() => Physics2D.BoxCast(transform.position, surroundingCheckSize, 0, Vector2.zero, 0, whatIsGround);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawLine(transform.position,new Vector3(transform.position.x, transform.position.y - GroundBelow().distance));
        Gizmos.DrawWireCube(transform.position, surroundingCheckSize);
    }

    //�Ƿ���Դ���
    public bool CanTeleport()
    {
        if(Random.Range(0,100) <= chanceToTeleport)
        {
            chanceToTeleport = defaultChanceToTeleport;
            return true;
        }

        return false;
    }

    //�Ƿ����ʩ������
    public bool CanDoSpellCast()
    {
        if(Time.time >= lastTimeCast + spellStateCooldown)
        {
            return true;
        }

        return false;
    }
}

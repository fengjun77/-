using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("碰撞检测相关")]
    public Transform attackCheck;
    public float attackCheckRadius;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;


    [Header("击退相关")]
    [SerializeField] protected Vector2 knockbackPower;
    [SerializeField] protected Vector2 knockbackOffset;
    [SerializeField] protected float knockbackDuration;
    protected bool isKnocked;
    public int knockbackDir { get; private set; }
    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;

    #region Components
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public SpriteRenderer sr { get; private set; }
    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D cd { get; private set; }
    #endregion

    public System.Action onFlipped;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void Update()
    {

    }

    public virtual void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {

    }

    protected virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1;
    }

    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        //如果被击退，处于僵直无法移动
        if (isKnocked)
            return;

        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }

    public void SetZeroVelocity()
    {
        //如果被击退，处于僵直无法移动
        if (isKnocked)
            return;

        rb.velocity = Vector2.zero;
    }

    #region 翻转相关
    public void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        onFlipped?.Invoke();
    }

    public void FlipController(float _x)
    {
        if (_x > 0 && !facingRight || _x < 0 && facingRight)
            Flip();
    }

    public void SetupDefaultFacingDir(int _direction)
    {
        facingDir = _direction;
        if (facingDir == -1)
            facingRight = false;
    }
    #endregion

    public virtual void DamageImpact() => StartCoroutine("HitKnockback");


    public virtual void SetupKnockbackDir(Transform _damageDirection)
    {
        //如果攻击方的x轴坐标值比自己的大，那么说明它在自己的右边,那么就该往左击退
        if (_damageDirection.position.x > transform.position.x)
            knockbackDir = -1;
        else if (_damageDirection.position.x < transform.position.x)
            knockbackDir = 1;
    }

    public void SetupKnockbackPower(Vector2 _power) => knockbackPower = _power;

    //受击逻辑 协程
    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;

        float xOffset = Random.Range(knockbackOffset.x, knockbackOffset.y);

        if (knockbackPower.x > 0 || knockbackPower.y > 0)
            rb.velocity = new Vector2((knockbackPower.x + xOffset) * knockbackDir, knockbackPower.y);

        yield return new WaitForSeconds(knockbackDuration);

        isKnocked = false;

        SetupZeroKnockbackPower();
    }

    protected virtual void SetupZeroKnockbackPower()
    {

    }

    #region 碰撞检测相关
    public bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDir, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion

    public void MakeTransparent(bool _transparent)
    {
        if (_transparent)
            sr.color = Color.clear;
        else
            sr.color = Color.white;
    }

    public virtual void DestoryMyself(GameObject _gameObject)
    {
        Destroy(_gameObject);
    }

    public virtual void Die()
    {

    }
}

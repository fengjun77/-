using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private bool canRotate = true;
    private bool isReturning;

    private float freezeTimeDuration;
    private float returnSpeed = 16;

    [Header("�������")]
    //�������  �ɽ��ڵ����м���������
    //�����ٶ�
    [SerializeField] private float bounceSpeed;
    //�Ƿ�������
    private bool isBouncing;
    //��������
    private int bounceAmount;
    //����Ŀ������
    private List<Transform> enemyTarget;
    private int targetIndex;

    [Header("�������")]
    //��͸����
    private float pierceAmount;

    [Header("��ת���")]
    //��Զ�������
    private float maxTravelDistance;
    //��תʱ��
    private float spinDuration;
    //��ת��ʱ��
    private float spinTimer;
    //�Ƿ�ֹͣ
    private bool wasStopped;
    private bool isSpinning;

    private float hitTimer;
    private float hitCooldown;

    private float spinDirection;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void SetupSword(Vector2 _dir,float _swordGravity,Player _player,float _freezeTimeDuration,float _returnSpeed)
    {
        rb.velocity = _dir;
        rb.gravityScale = _swordGravity;
        returnSpeed = _returnSpeed;
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;

        if(pierceAmount <= 0 )
            anim.SetBool("Rotate",true);

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);

        Invoke("DestroyMe", 4);
    }

    public void SetupBounce(bool _isBouncing,int _amountOfBounces,float _bounceSpeed)
    {
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounces;
        bounceSpeed = _bounceSpeed;

        enemyTarget = new List<Transform>();
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public void SetupSpin(bool _isSpinning,float _maxTravelDistance,float _spinDuration,float _hitCooldown)
    {
        isSpinning = _isSpinning;
        spinDuration = _spinDuration;
        maxTravelDistance = _maxTravelDistance;
        hitCooldown = _hitCooldown;

        spinTimer = spinDuration;
    }


    private void Update()
    {
        if (canRotate)
            transform.right = rb.velocity;

        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 1)
                SwordCaught();
        }

        BounceLogic();
        SpinLogic();
    }

    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }

            if (wasStopped && spinTimer > 0)
            {
                spinTimer -= Time.deltaTime;

                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);

                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpinning = false;
                }

                hitTimer -= Time.deltaTime;

                if (hitTimer < 0)
                {
                    hitTimer = hitCooldown;
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                        {
                            SwordSkillDamage(hit.GetComponent<Enemy>());
                        }
                    }
                }
            }
        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
    }

    private void BounceLogic()
    {
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .1f)
            {
                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());

                targetIndex++;
                bounceAmount--;

                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }

                if (targetIndex >= enemyTarget.Count)
                    targetIndex = 0;
            }
        }
    }

    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.parent = null;
        isReturning = true;

        anim.SetBool("Rotate", true);
    }

    public void SwordCaught()
    {
        //�����ӽ�
        player.CatchSword();

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)
            return;

        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            SwordSkillDamage(enemy);
        }

        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                        enemyTarget.Add(hit.transform);
                }
            }
        }

        StuckInto(collision);
    }

    private void SwordSkillDamage(Enemy enemy)
    {
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();

        //�����˺�
        if (player.skill.sword.swordType == SwordType.Bounce)
            player.stats.DoDamage(enemyStats, .8f);
        //��͸�˺�
        else if (player.skill.sword.swordType == SwordType.Pierce)
            player.stats.DoDamage(enemyStats, 2.0f);
        //��ת�˺�
        else if (player.skill.sword.swordType == SwordType.Spin)
            player.stats.DoDamage(enemyStats, .6f);
        //��ͨ�˺�
        else
            player.stats.DoDamage(enemyStats, 1.4f);

        if (player.skill.sword.timeStopUnlocked)
            enemy.FreezeTimeFor(freezeTimeDuration);

        if (player.skill.sword.vulnerableUnlocked)
            enemyStats.MakeVulnerableFor(freezeTimeDuration + 1);

        //�ҵ��������͵�װ��
        ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipmentType(EquipmentType.Amulet);
        //������� ��ִ�ж�Ӧ��Ч��
        if (equipedAmulet != null)
            equipedAmulet.ExecuteItemEffect(enemy.transform);
    }

    //������������壬��ֹͣ�˶�����ʾ��������
    private void StuckInto(Collider2D collision)
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        if(isSpinning)
        {
            StopWhenSpinning();
            return;
        }

        canRotate = false;
        cd.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        if (isBouncing && enemyTarget.Count > 0)
            return;
        
        anim.SetBool("Rotate", false);
        transform.parent = collision.transform;
    }
}

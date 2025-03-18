using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private Animator anim;
    private Player player;
    [SerializeField] private float colorLosingSpeed;
    private float cloneTimer;
    private float attackMultiplier;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = .8f;
    private Transform closestEnemy;
    private int facingDir;
    
    private bool canDuplicate;
    private float chanceToDuplicate;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        player = PlayerManager.instance.player;
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if(cloneTimer < 0)
        {
            sr.color = new Color(1,1,1,sr.color.a - (Time.deltaTime * colorLosingSpeed));
        }

        if(sr.color.a <= 0)
            Destroy(gameObject);
    }

    public void SetupClone(Transform _newTransform,float _cloneDuration,bool _canAttack,Vector3 _offset,Transform _closestEnmey,bool _canDuplicate,float _chanceToDuplicate,float _attackMultiplier)
    {
        if (_canAttack)
            anim.SetInteger("AttackNumber", Random.Range(1, 3));

        transform.position = _newTransform.position + _offset;
        cloneTimer = _cloneDuration;
        closestEnemy = _closestEnmey;
        canDuplicate = _canDuplicate;
        chanceToDuplicate = _chanceToDuplicate;
        attackMultiplier = _attackMultiplier;

        FaceClosestTarget();
    }

    private void AnimationTrigger()
    {
        cloneTimer = -.1f;
    }

    private void AttackTrigger()
    {
        //检测这个事件触发时，所有在攻击碰撞器内的物体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                //PlayerManager.instance.player.stats.DoDamage(hit.GetComponent<CharacterStats>());

                hit.GetComponent<Entity>().SetupKnockbackDir(transform);

                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                EnemyStats enemyStats = hit.GetComponent<EnemyStats>();

                playerStats.CloneDoDamage(enemyStats, attackMultiplier);

                if(player.skill.clone.canApplyOnHitEffect)
                {
                    // inventory获取武器 调用物品效果
                    ItemData_Equipment weaponData = Inventory.instance.GetEquipmentType(EquipmentType.Weapon);

                    if (weaponData != null)
                        weaponData.ExecuteItemEffect(hit.transform);
                }

                if(canDuplicate)
                {
                    if (Random.Range(0, 100) < chanceToDuplicate)
                    {
                        SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(.5f * facingDir, 0));
                    }
                }
            }
        }
    }

    //面向最近的目标
    private void FaceClosestTarget()
    {
        
        if (closestEnemy != null)
        {
            
            if (transform.position.x > closestEnemy.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }
}

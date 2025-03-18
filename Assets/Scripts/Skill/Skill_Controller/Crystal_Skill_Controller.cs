using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();

    private float crystalExitTimer;

    private bool canExplode;

    private float moveSpeed;
    private bool canMove;

    private float growSpeed = 5;
    private bool canGrow;

    private Transform closestTarget;
    public void SetupCrystal(float _crystalDuration, float _moveSpeed, bool _canMove, bool _canExplode, Transform _closestTarget)
    {
        crystalExitTimer = _crystalDuration;
        moveSpeed = _moveSpeed;
        canExplode = _canExplode;
        canMove = _canMove;
        closestTarget = _closestTarget;
    }

    private void Update()
    {
        crystalExitTimer -= Time.deltaTime;

        if (crystalExitTimer < 0)
        {
            ChangeCrystal();
        }

        if (canMove && closestTarget != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, closestTarget.position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, closestTarget.position) < 1)
                ChangeCrystal();
        }

        if (canGrow)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), growSpeed * Time.deltaTime);
    }

    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                hit.GetComponent<Entity>().SetupKnockbackDir(transform);

                PlayerManager.instance.player.stats.DoMagicalDamage(hit.GetComponent<CharacterStats>());

                //找到护符类型的装备
                ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipmentType(EquipmentType.Amulet);
                //如果存在 则执行对应的效果
                if (equipedAmulet != null)
                    equipedAmulet.ExecuteItemEffect(hit.transform);
            }
        }
    }

    public void ChangeCrystal()
    {
        if (canExplode)
        {
            canGrow = true;
            anim.SetTrigger("Explode");
        }
        else
        {
            SelfDestroy();
        }
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
}

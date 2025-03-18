using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    private ItemDrop myDropSystem;
    public Stat coinsDropAmount;

    [Header("等级调整")]
    [SerializeField] public int level = 1;

    [Range(0f, 1f)]
    [SerializeField] private float percentageModifier = .25f;

    protected override void Start()
    {
        coinsDropAmount.SetDefaultValue(10);
        
        Modify(damage);
        Modify(maxHealth);
        Modify(coinsDropAmount);
        
        base.Start();

        enemy = GetComponent<Enemy>();
        myDropSystem = GetComponent<ItemDrop>();
    }

    private void Modify(Stat _stat)
    {
        //根据等级提升增加相同比例的数值
        for (int i = 1; i < level; i++)
        {
            float modifier = _stat.GetValue() * percentageModifier;
        
            _stat.AddModifier(Mathf.RoundToInt(modifier));
        }
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);

        if (GetComponent<Enemy_Skeleton>() != null)
            AudioManager.instance.PlaySFX(10);
    }

    protected override void Die()
    {
        base.Die();
        enemy.Die();

        PlayerManager.instance.money += coinsDropAmount.GetValue();
        myDropSystem.GenerateDrop();
    }
}

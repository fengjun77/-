using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private Animator anim;
    private ItemDrop chestDrop;
    private bool isOpen;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        chestDrop = GetComponent<ItemDrop>();
    }

    private List<Enemy> enemiesInTrigger = new List<Enemy>();
    private bool playerInTrigger = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            playerInTrigger = true;
        }

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemiesInTrigger.Add(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            playerInTrigger = false;
        }

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemiesInTrigger.Remove(enemy);
        }
    }

    private void Update()
    {
        // 清理已死亡的敌人
        enemiesInTrigger.RemoveAll(enemy => enemy == null || enemy.isDead);

        // 检查是否满足打开箱子的条件
        if (!isOpen && playerInTrigger && enemiesInTrigger.Count == 0)
        {
            OpenChest();
            isOpen = true;
        }
    }

    private void OpenChest()
    {
        anim.SetBool("Open", true);
        chestDrop.GenerateDrop();
        Destroy(gameObject, 2f);
    }
}

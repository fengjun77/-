using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostMoneyController : MonoBehaviour
{
    public int money;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerManager.instance.money += money;
        Destroy(this.gameObject);
    }
}

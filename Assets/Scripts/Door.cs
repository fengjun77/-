using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public BoxCollider2D box;
    [SerializeField] private Enemy_DeathBriner enemy;

    // Update is called once per frame
    void Update()
    {
        if(enemy.bossFightBegun)
            box.isTrigger = false;
    }
}

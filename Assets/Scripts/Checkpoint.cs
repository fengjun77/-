using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator anim;
    public string checkpointID;
    public bool actived;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    //Éú³É´æµµµãID
    [ContextMenu("Generate checkpoint id")]
    private void GenerateId()
    {
        checkpointID = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            ActivateCheckPoint();
        }
    }

    public void ActivateCheckPoint()
    {
        if (actived)
            return;

        AudioManager.instance.PlaySFX(3);

        actived = true;
        anim.SetBool("active", true);
    }
}

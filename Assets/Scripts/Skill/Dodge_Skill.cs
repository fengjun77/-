using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dodge_Skill : Skill
{
    [Header("…¡±‹")]
    [SerializeField] private SkillTreeSlot_UI unlockDodgeButton;
    [SerializeField] private int evasionAmount;
    public bool dodgeUnlocked;

    [Header("æµœÒ…¡±‹")]
    [SerializeField] private SkillTreeSlot_UI unlockMirageDodgeButton;
    public bool dodgeMirageUnlocked;

    [Header("…¡±‹æ≤÷π")]
    [SerializeField] private SkillTreeSlot_UI unlockDodgeSpeedButton;
    [SerializeField] private int dodgeSpeed;
    public bool dodgeSpeedUnlocked;

    protected override void Start()
    {
        base.Start();

        unlockDodgeButton.GetComponent<Button>().onClick.AddListener(UnlockDodge);
        unlockMirageDodgeButton.GetComponent<Button>().onClick.AddListener(UnlockMirageDodge);
        unlockDodgeSpeedButton.GetComponent<Button>().onClick.AddListener(UnlockDodgeSpeed);
    }

    protected override void CheckUnlock()
    {
        UnlockDodge();
        UnlockMirageDodge();
        UnlockDodgeSpeed();
    }

    private void UnlockDodge()
    {
        if (unlockDodgeButton.unlocked)
        {
            if (dodgeUnlocked)
            {
                return;
            }
            player.stats.evasion.AddModifier(evasionAmount);
            Inventory.instance.UpdateStatsUI();
            dodgeUnlocked = true;
        }
    }

    private void UnlockMirageDodge()
    {
        if(unlockMirageDodgeButton.unlocked)
            dodgeMirageUnlocked = true;
    }

    private void UnlockDodgeSpeed()
    {
        if(unlockDodgeSpeedButton.unlocked)
            dodgeSpeedUnlocked = true;
    }

    public void CreateMirageOnDodge()
    {
        if (dodgeMirageUnlocked)
            SkillManager.instance.clone.CreateClone(player.transform, new Vector3(2 * player.facingDir,0));
    }

    public void AddSpeed()
    {
        if (dodgeSpeedUnlocked)
            StartCoroutine(SpeedCoroutine(dodgeSpeed));
    }

    IEnumerator SpeedCoroutine(int _second)
    {
        player.moveSpeed += 4;
        yield return new WaitForSeconds(_second);
        player.moveSpeed -= 4;
    }
}

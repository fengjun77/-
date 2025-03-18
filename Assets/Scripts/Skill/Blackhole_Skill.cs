using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blackhole_Skill : Skill
{
    [SerializeField] private SkillTreeSlot_UI blackholeUnlockButton;
    public bool blackholeUnlock {  get; private set; }
    [SerializeField] private GameObject blackholePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;
    [SerializeField] private float cloneCooldown;
    [SerializeField] private int attackAmount;
    [SerializeField] private float blackholeDuration;

    Blackhole_Skill_Controller currentBlackhole;

    private void UnlockBlackhole()
    {
        if(blackholeUnlockButton.unlocked)
            blackholeUnlock = true;
    }

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        GameObject newBlackhole = Instantiate(blackholePrefab,player.transform.position,Quaternion.identity);

        currentBlackhole = newBlackhole.GetComponent<Blackhole_Skill_Controller>();

        currentBlackhole.SetupBlackhole(maxSize,growSpeed,shrinkSpeed,attackAmount,cloneCooldown,blackholeDuration);
    }

    protected override void Start()
    {
        base.Start();

        blackholeUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBlackhole);
    }

    protected override void CheckUnlock()
    {
        UnlockBlackhole();
    }

    protected override void Update()
    {
        base.Update();
    }

    public bool SkillCompleted()
    {
        if(currentBlackhole == null)
            return false;

        if (currentBlackhole.playerCanExitState)
        {
            currentBlackhole = null;
            return true;
        }

        return false;
    }

    public bool IsCooldown()
    {
        if(cooldownTimer <= 0)
            return false;

        return true;
    }
}

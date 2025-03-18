using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clone_Skill : Skill
{
    [Header("克隆相关")]
    [SerializeField] private float attackMultiplier;
    [SerializeField] private GameObject clonePrefabs;
    [SerializeField] protected float cloneDuration;
    [Space]

    [Header("克隆攻击")]
    [SerializeField] private SkillTreeSlot_UI cloneAttackUnlockButton;
    [SerializeField] private float cloneAttackMultiplier;
    [SerializeField] private bool canAttack;

    [Header("克隆攻击++")]
    [SerializeField] private SkillTreeSlot_UI aggresiveCloneUnlockButton;
    [SerializeField] private float aggresiveCloneMultiplier;
    public bool canApplyOnHitEffect {  get; private set; }

    [Header("多重克隆")]
    [SerializeField] private SkillTreeSlot_UI multipleUnlockButton;
    [SerializeField] private float multipleCloneAttackMultiplier;
    [SerializeField] private bool canDuplicate;
    [SerializeField] private float chanceToDuplicate;

    [Header("水晶克隆")]
    [SerializeField] private SkillTreeSlot_UI crystalInseadCloneUnlockButton;
    [SerializeField] private bool crystalInseadOfClone;

    protected override void Start()
    {
        base.Start();

        cloneAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        aggresiveCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAggresiveClone);
        multipleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMultiClone);
        crystalInseadCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalInstead);
    }

    protected override void CheckUnlock()
    {
        UnlockCloneAttack();
        UnlockAggresiveClone();
        UnlockMultiClone();
        UnlockCrystalInstead();
    }

    #region 解锁

    private void UnlockCloneAttack()
    {
        if (cloneAttackUnlockButton.unlocked)
        {
            canAttack = true;
            attackMultiplier = cloneAttackMultiplier;
        }
    }

    private void UnlockAggresiveClone()
    {
        if (aggresiveCloneUnlockButton.unlocked)
        {
            canApplyOnHitEffect = true;
            attackMultiplier = aggresiveCloneMultiplier;
        }
    }

    private void UnlockMultiClone()
    {
        if(multipleUnlockButton.unlocked)
        {
            canDuplicate = true;
            attackMultiplier = multipleCloneAttackMultiplier;
        }
    }

    private void UnlockCrystalInstead()
    {
        if(crystalInseadCloneUnlockButton.unlocked)
        {
            crystalInseadOfClone = true;
        }
    }

    #endregion

    public void CreateClone(Transform _clonePosition,Vector3 _offset)
    {
        if (crystalInseadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }

        GameObject newClone = Instantiate(clonePrefabs);

        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_clonePosition,cloneDuration,canAttack,_offset,FindClosestEnemy(newClone.transform), canDuplicate,chanceToDuplicate,attackMultiplier);
    }

    public void CreateCloneWithDelay(Transform _enemyTransform)
    {
            StartCoroutine(CloneDelayCorotine(_enemyTransform , new Vector3(2 * player.facingDir, 0)));
    }

    private IEnumerator CloneDelayCorotine(Transform _transform,Vector3 _offset)
    {
        yield return new WaitForSeconds(.4f);
        CreateClone(_transform, _offset);
    }
}

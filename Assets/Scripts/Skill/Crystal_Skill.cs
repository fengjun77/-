using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crystal_Skill : Skill
{
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalDuration;


    [Header("ˮ����ը")]
    [SerializeField] private SkillTreeSlot_UI unlockExplosiveButton;
    [SerializeField] private bool canExplode;

    [Header("ˮ����¡")]
    [SerializeField] private SkillTreeSlot_UI unlockCloneInsteadButton;
    [SerializeField] private bool canCreateClone;

    [Header("ˮ���ƶ�")]
    [SerializeField] private SkillTreeSlot_UI unlockMovingCrystalButton;
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool canMoveToEnemy;
    
    private GameObject currentCrystal;

    [Space]
    [Header("���ˮ�����")]
    [SerializeField] private SkillTreeSlot_UI unlockMultiStackButton;
    [SerializeField] private SkillTreeSlot_UI unlockMultiStack2Button;
    [SerializeField] private int crystalAmount;
    //һ�����ù�ˮ�������ȴ
    [SerializeField] private float crystalCooldown = 20f;
    //����ˮ����ȴʱ��
    [SerializeField] private float refillInterval = 5f;
    [SerializeField] private List<GameObject> crystalLeft;
    [SerializeField] private bool canUseMultiStacks;
    private Coroutine refillCoroutine;

    //�Ƿ��ڳ���ȴ��
    private bool isCooldownActive = false;

    [Header("ˮ������")]
    [SerializeField] private SkillTreeSlot_UI unlockCrystalButton;
    public bool crystalUnlocked;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(AutoRefillCrystals());

        unlockCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        unlockCloneInsteadButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalMirage);
        unlockExplosiveButton.GetComponent<Button>().onClick.AddListener(UnlockExplosiveCrystal);
        unlockMovingCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockMovingCrystal);
        unlockMultiStackButton.GetComponent<Button>().onClick.AddListener(UnlockMultiStack);
        unlockMultiStack2Button.GetComponent<Button>().onClick.AddListener(UnlockMultiStack2);
    }

    protected override void CheckUnlock()
    {
        UnlockCrystal();
        UnlockCrystalMirage();
        UnlockExplosiveCrystal();
        UnlockMovingCrystal();
        UnlockMultiStack();
        UnlockMultiStack2();
    }

    private void UnlockCrystal()
    {
        if (unlockCrystalButton.unlocked)
            crystalUnlocked = true;
    }

    private void UnlockCrystalMirage()
    {
        if(unlockCloneInsteadButton.unlocked)
        {
            crystalDuration += 1; 
            canCreateClone = true;
        }
    }

    private void UnlockMovingCrystal()
    {
        if(unlockMovingCrystalButton.unlocked)
        {
            canMoveToEnemy = true;
        }
    }

    private void UnlockExplosiveCrystal()
    {
        if(unlockExplosiveButton.unlocked)
        {
            cooldown += 10;
            canExplode = true;
        }
    }

    private void UnlockMultiStack()
    {
        if(unlockMovingCrystalButton.unlocked)
        {
            crystalAmount = 3;
            canUseMultiStacks = true;
        }
    }

    private void UnlockMultiStack2()
    {
        if(unlockMultiStack2Button.unlocked)
        {
            crystalAmount = 5;
            canUseMultiStacks = true;
        }
    }

    public override void UseSkill()
    {
        base.UseSkill();

        //��������˶���ˮ������û�н���λ�û�������ֱ�ӷ���
        if (CanUseMultiCrystal() && !canCreateClone)
            return;

        //���û��ˮ�����򴴽���һ��ˮ��
        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        else//����У���������ˮ���ƶ� Ҳֱ�ӷ��� û��������Ի���λ�ò�������ը
        {
            if (canMoveToEnemy)
                return;
            Vector2 playerPos = player.transform.position;
            //������ϴ���ˮ����ֱ�Ӻ�ˮ������λ��
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;


            if(canCreateClone)
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.ChangeCrystal();
            }
        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);

        Crystal_Skill_Controller crystalSkillScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();
        crystalSkillScript.SetupCrystal(crystalDuration, moveSpeed, canMoveToEnemy, canExplode, FindClosestEnemy(currentCrystal.transform));
    }

    private bool CanUseMultiCrystal()
    {
        if(isCooldownActive) 
            return true;

        if(canUseMultiStacks && crystalLeft.Count > 0)
        {
            cooldown = 0;
            GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
            GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

            crystalLeft.Remove(crystalToSpawn);

            newCrystal.GetComponent<Crystal_Skill_Controller>().SetupCrystal(crystalDuration, moveSpeed, canMoveToEnemy, canExplode, FindClosestEnemy(newCrystal.transform));

            //����洢��ˮ��Ϊ0���򲹳�ˮ��
            if (crystalLeft.Count == 0 && !isCooldownActive)
            {
                StartCoroutine(CrystalCooldown());
            }

            return true;
        }

        return false;
    }

    //����20����ȴ
    private IEnumerator CrystalCooldown()
    {
        isCooldownActive = true;
        yield return new WaitForSeconds(crystalCooldown);
        RefillCrystal(crystalAmount);
        isCooldownActive = false;
    }

    //ÿ����ָ�һ��ˮ��
    private IEnumerator AutoRefillCrystals()
    {
        while (true)
        {
            yield return new WaitForSeconds(refillInterval);
            if (crystalLeft.Count > 0 && crystalLeft.Count < crystalAmount && !isCooldownActive)
            {
                RefillCrystal(1);
            }
        }
    }

    //����ˮ��
    private void RefillCrystal(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if(crystalLeft.Count < crystalAmount)
                crystalLeft.Add(crystalPrefab);
        }
    }
}

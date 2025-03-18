using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SwordType
{
    //正常
    Regular,
    //弹跳
    Bounce,
    //穿透
    Pierce,
    //旋转
    Spin,
}

public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;

    [Header("弹跳相关")]
    [SerializeField] private SkillTreeSlot_UI bounceUnlockButton;
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;

    [Header("穿刺相关")]
    [SerializeField] private SkillTreeSlot_UI pierceUnlockButton;
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravaity;

    [Header("旋转相关")]
    [SerializeField] private SkillTreeSlot_UI spinUnlockButton;
    [SerializeField] private float hitCooldown = .35f;
    [SerializeField] private float maxTravelDistance = 7;
    [SerializeField] private float spinDuration;
    [SerializeField] private float spinGravity;

    [Header("技能信息")]
    [SerializeField] private SkillTreeSlot_UI swordUnlockButton;
    public bool swordUnlocked {  get; private set; }
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;

    [Header("被动技能")]
    [SerializeField] private SkillTreeSlot_UI timeStopUnlockButton;
    public bool timeStopUnlocked {  get; private set; }
    [SerializeField] private SkillTreeSlot_UI vulnerableUnlockButton;
    public bool vulnerableUnlocked {  get; private set; }

    private Vector2 finalDir;

    [Header("瞄准轨迹相关")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotParent;

    private GameObject[] dots;

    protected override void Start()
    {
        base.Start();

        GenerateDots();


        swordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        bounceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBounceSword);
        pierceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPierceSword);
        spinUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSpinSword);
        vulnerableUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockVulnerable);
        timeStopUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockTimeStop);
    }

    protected override void CheckUnlock()
    {
        UnlockSword();
        UnlockBounceSword();
        UnlockPierceSword();
        UnlockSpinSword();
        UnlockTimeStop();
        UnlockVulnerable();
    }

    private void SetupGravity()
    {
        if (swordType == SwordType.Bounce)
            swordGravity = bounceGravity;
        else if (swordType == SwordType.Pierce)
            swordGravity = pierceGravaity;
        else if(swordType == SwordType.Spin)
            swordGravity = spinGravity;
    }

    protected override void Update()
    {
        SetupGravity();

        if (Input.GetKeyUp(KeyCode.Mouse1))
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);

        if (Input.GetKey(KeyCode.Mouse1))
        {
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
            }
        }
    }

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab,player.transform.position,transform.rotation);
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        if (swordType == SwordType.Bounce)
            newSwordScript.SetupBounce(true, bounceAmount,bounceSpeed);
        else if (swordType == SwordType.Pierce)
            newSwordScript.SetupPierce(pierceAmount);
        else if (swordType == SwordType.Spin)
            newSwordScript.SetupSpin(true, maxTravelDistance, spinDuration,hitCooldown);


        newSwordScript.SetupSword(finalDir, swordGravity,player,freezeTimeDuration,returnSpeed);

        player.AssignNewSword(newSword);

        DotsActive(false);
    }

    #region 解锁
    private void UnlockTimeStop()
    {
        if (timeStopUnlockButton.unlocked)
            timeStopUnlocked = true;
    }

    private void UnlockVulnerable()
    {
        if(vulnerableUnlockButton.unlocked)
            vulnerableUnlocked = true;
    }

    private void UnlockSword()
    {
        if (swordUnlockButton.unlocked)
        {
            swordType = SwordType.Regular;
            swordUnlocked = true;
        }
    }

    private void UnlockBounceSword()
    {
        if(bounceUnlockButton.unlocked)
            swordType = SwordType.Bounce;
    }

    private void UnlockPierceSword()
    {
        if(pierceUnlockButton.unlocked)
            swordType = SwordType.Pierce;
    }

    private void UnlockSpinSword()
    {
        if(spinUnlockButton.unlocked)
            swordType = SwordType.Spin;
    }
    #endregion

    #region 瞄准
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;

        return direction;
    }

    public void DotsActive(bool _isActive)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    private void GenerateDots()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = GameObject.Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y) * t + .5f * (Physics2D.gravity * swordGravity) * (t * t);
        return position;
    }
    #endregion
}

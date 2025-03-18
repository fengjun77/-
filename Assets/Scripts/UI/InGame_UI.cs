using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGame_UI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;

    [SerializeField] private Image dashImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image swordImage;
    [SerializeField] private Image blackholeImage;
    [SerializeField] private Image flaskImage;

    private SkillManager skills;

    [Header("金币相关")]
    [SerializeField] private TextMeshProUGUI currentMoney;
    [SerializeField] private float coinsAmount;
    [SerializeField] private float increaseRate = 2000;

    [Header("时间相关")]
    [SerializeField] private TextMeshProUGUI currentTime;
    public float gameTime;
    void Start()
    {
        if (playerStats != null)
            playerStats.onHealthChanged += UpdateHealthUI;

        skills = SkillManager.instance;
    }

    void Update()
    {
        UpdateTimeUI();

        UpdateCoinUI();

        if (Input.GetKeyDown(KeyCode.LeftShift) && skills.dash.dashUnlocked)
            SetCooldownOf(dashImage);

        if (Input.GetKeyDown(KeyCode.Z) && skills.parry.parryUnlocked)
            SetCooldownOf(parryImage);

        if (Input.GetKeyDown(KeyCode.G) && skills.crystal.crystalUnlocked)
            SetCooldownOf(crystalImage);

        if (Input.GetKeyDown(KeyCode.Mouse1) && skills.sword.swordUnlocked)
            SetCooldownOf(swordImage);

        if (Input.GetKeyDown(KeyCode.R) && skills.blackhole.blackholeUnlock)
            SetCooldownOf(blackholeImage);

        if (Input.GetKeyDown(KeyCode.Alpha1) && Inventory.instance.GetEquipmentType(EquipmentType.Flask) != null)
            SetCooldownOf(flaskImage);

        CheckCooldownOf(dashImage, skills.dash.cooldown);
        CheckCooldownOf(parryImage, skills.parry.cooldown);
        CheckCooldownOf(crystalImage, skills.crystal.cooldown);
        CheckCooldownOf(swordImage, skills.sword.cooldown);
        CheckCooldownOf(blackholeImage, skills.blackhole.cooldown);
        CheckCooldownOf(flaskImage, Inventory.instance.flaskCooldown);
    }

    public float NowTime()
    {
        return gameTime;
    }

    private void UpdateTimeUI()
    {
        gameTime += Time.deltaTime;
        currentTime.text = gameTime.ToString("F0") + "秒";
    }

    private void UpdateCoinUI()
    {
        if (coinsAmount < PlayerManager.instance.CurrentMoneyAmount())
            coinsAmount += Time.deltaTime * increaseRate;
        else
            coinsAmount = PlayerManager.instance.CurrentMoneyAmount();

        currentMoney.text = ((int)coinsAmount).ToString();
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = playerStats.GetMaxHealthValue();
        slider.value = playerStats.currentHealth;
    }

    private void SetCooldownOf(Image _image)
    {
        if(_image.fillAmount <= 0)
            _image.fillAmount = 1;
    }

    private void CheckCooldownOf(Image _image,float _cooldown)
    {
        if (_image.fillAmount > 0)
            _image.fillAmount -= 1/_cooldown * Time.deltaTime;
    }
}

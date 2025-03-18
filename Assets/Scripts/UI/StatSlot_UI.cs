using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatSlot_UI : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    private UI ui;

    [SerializeField] private string statName;
    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private TextMeshProUGUI statNameText;

    [TextArea]
    [SerializeField] private string statDescription;
    private void OnValidate()
    {
        gameObject.name = "Stat - " + statName;

        if(statNameText != null)
            statNameText.text = statName;
            
    }

    private void Start()
    {
        UpdateStatValueUI();
        
        ui = GetComponentInParent<UI>();
    }

    public void UpdateStatValueUI()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if(playerStats != null )
        {
            statValueText.text = playerStats.StatOfType(statType).GetValue().ToString();
        }

        if(statType == StatType.health)
            statValueText.text = playerStats.GetMaxHealthValue().ToString();
    
        if(statType == StatType.damage)
            statValueText.text = (playerStats.damage.GetValue() + playerStats.strength.GetValue()).ToString();

        if (statType == StatType.critPower)
            statValueText.text = (playerStats.critPower.GetValue() + playerStats.strength.GetValue()).ToString();

        if (statType == StatType.critChance)
            statValueText.text = (playerStats.critChance.GetValue() + playerStats.agility.GetValue()).ToString();

        if (statType == StatType.evasion)
            statValueText.text = (playerStats.evasion.GetValue() + playerStats.agility.GetValue()).ToString();

        if (statType == StatType.magicRes)
            statValueText.text = (playerStats.magicResistance.GetValue() + playerStats.intelligence.GetValue() * 3).ToString();


    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.statToolTip.ShowStatToolTip(statDescription);

        Vector2 mousePosition = Input.mousePosition;

        float xOffset = 0;
        float yOffset = 0;

        if (mousePosition.x > 600)
            xOffset = -75;
        else
            xOffset = 75;

        if (mousePosition.y > 320)
            yOffset = -75;
        else
            yOffset = 75;

        ui.statToolTip.transform.position = new Vector2(mousePosition.x + xOffset, mousePosition.y + yOffset);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.statToolTip.HideStatToolTip();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillToolTip_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillPrice;

    public void ShowToolTip(string _skillName,string _skillDescription,int _price)
    {
        skillName.text = _skillName;
        skillDescription.text = _skillDescription;
        skillPrice.text = _price.ToString();

        gameObject.SetActive(true);
    }

    public void HideToolTip() => gameObject.SetActive(false);
}

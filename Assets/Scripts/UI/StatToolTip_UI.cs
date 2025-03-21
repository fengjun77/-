using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatToolTip_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI description;

    public void ShowStatToolTip(string _text)
    {
        description.text = _text;

        gameObject.SetActive(true);
    }

    public void HideStatToolTip() => gameObject.SetActive(false);

}

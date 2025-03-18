using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerManager : MonoBehaviour,ISaveManager
{
    public static PlayerManager instance;
    public Player player;

    public int money;
    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    public bool HaveMoney(int _price)
    {
        if (_price > money)
        {
            Debug.Log("Ã»ÓÐ×ã¹»µÄÇ®");
            return false;
        }
        else
        {
            money = money - _price;
            return true;
        }
    }

    public int CurrentMoneyAmount() => money;

    public void LoadData(GameData _data)
    {
        money = _data.money;
    }

    public void SaveData(ref GameData _data)
    {
        _data.money = money;
    }
}

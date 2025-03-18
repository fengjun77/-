using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int money;

    public SerializableDictionary<string, bool> skillTree;
    public SerializableDictionary<string, int> inventory;
    public List<string> equipmentId;

    public SerializableDictionary<string, bool> checkpoints;
    public string closestCheckpointID;

    public float lostMoneyX;
    public float lostMoneyY;
    public int lostMoneyAmount;

    public SerializableDictionary<string, float> volumeSettings;

    public GameData()
    {
        this.lostMoneyX = 0;
        this.lostMoneyY = 0;
        this.lostMoneyAmount = 0;

        this.money = 0;
        skillTree = new SerializableDictionary<string, bool>();
        inventory = new SerializableDictionary<string, int>();
        equipmentId = new List<string>();

        closestCheckpointID = string.Empty;
        checkpoints = new SerializableDictionary<string, bool>{};

        volumeSettings = new SerializableDictionary<string, float>();
    }
}

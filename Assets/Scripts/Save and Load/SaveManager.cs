using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private string fileName;
    
    private GameData gameData;

    private List<ISaveManager> saveManagers;

    private FileDataHandle dataHandler;

    private void Awake()
    {
        if(instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        dataHandler = new FileDataHandle(Application.persistentDataPath,fileName);
        saveManagers = FindAllSaveManagers();
        //print(Application.persistentDataPath);
        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    //加载游戏，如果没有存档，则新建一个存档
    public void LoadGame()
    {
        gameData = dataHandler.Load();

        if(this.gameData == null)
        {
            NewGame();
        }

        foreach(ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }
    }

    //保存游戏
    public void SaveGame()
    {
        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }

    //退出游戏自动保存
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<ISaveManager> FindAllSaveManagers()
    {
        IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>().OfType<ISaveManager>();

        return new List<ISaveManager>(saveManagers);
    }

    [ContextMenu("Delete save file")]
    public void DeleteSaveData()
    {
        dataHandler = new FileDataHandle(Application.persistentDataPath, fileName);
        dataHandler.Delete();
    }

    public bool HasSavedData()
    {
        if (dataHandler.Load() != null)
            return true;

        return false;
    }
}

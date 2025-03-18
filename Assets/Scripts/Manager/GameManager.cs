using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance;

    private Transform player;
    //存档点列表
    [SerializeField] private Checkpoint[] checkpoints;
    [SerializeField] private string closestCheckpointID;

    [Header("丢失的钱")]
    [SerializeField] private GameObject lostMoneyPerfab;
    public int lostMoneyAmount;
    [SerializeField] private float lostMoneyX;
    [SerializeField] private float lostMoneyY;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;

        //找到场景中所有的存档点，存入列表中
        checkpoints = FindObjectsOfType<Checkpoint>();

        player = PlayerManager.instance.player.transform;
    }

    private void Start()
    {
        Debug.Log(Application.persistentDataPath);
    }

    public void RestartScene()
    {
        SaveManager.instance.SaveGame();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void LoadData(GameData _data)
    {
        StartCoroutine(LoadWithDelay(_data));
    }

    private void LoadCheckpoints(GameData _data)
    {
        //检查所有存储的存档点信息
        foreach (KeyValuePair<string, bool> pair in _data.checkpoints)
        {
            //遍历场景上的存档点列表
            foreach (Checkpoint checkpoint in checkpoints)
            {
                //如果有id相同的，并且它的值是true 那么就激活改存档点
                if (checkpoint.checkpointID == pair.Key && pair.Value == true)
                    checkpoint.ActivateCheckPoint();
            }
        }
    }

    private void LoadLostMoney(GameData _data)
    {
        lostMoneyX = _data.lostMoneyX;
        lostMoneyY = _data.lostMoneyY;
        lostMoneyAmount = _data.lostMoneyAmount;

        if(lostMoneyAmount > 0)
        {
            GameObject newLostMoney = Instantiate(lostMoneyPerfab,new Vector3(lostMoneyX,lostMoneyY),Quaternion.identity);
            newLostMoney.GetComponent<LostMoneyController>().money = lostMoneyAmount;
        }

        lostMoneyAmount = 0;
    }

    private void PlacePlayerAtClosestCheckpoint(GameData _data)
    {
        if (_data.closestCheckpointID == null)
            return;

        closestCheckpointID = _data.closestCheckpointID;

        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (closestCheckpointID == checkpoint.checkpointID)
            {
                player.position = checkpoint.transform.position + new Vector3(0,.5f,0);
            }
        }
    }

    private IEnumerator LoadWithDelay(GameData _data)
    {
        yield return new WaitForSeconds(.1f);

        LoadCheckpoints(_data);
        PlacePlayerAtClosestCheckpoint(_data);
        LoadLostMoney(_data);
    }

    public void SaveData(ref GameData _data)
    {
        _data.lostMoneyAmount = lostMoneyAmount;
        _data.lostMoneyX = player.position.x;
        _data.lostMoneyY = player.position.y;

        if(FindClosestCheckPoint()  != null)
            _data.closestCheckpointID = FindClosestCheckPoint().checkpointID;
        
        _data.checkpoints.Clear();

        //遍历每个检查点
        foreach (Checkpoint checkpoint in checkpoints)
        {
            _data.checkpoints.Add(checkpoint.checkpointID, checkpoint.actived);
        }
    }

    //找到最近的被激活的存档点
    private Checkpoint FindClosestCheckPoint()
    {
        float chosestDistance = Mathf.Infinity;
        Checkpoint closestCheckpoint = null;

        foreach (Checkpoint checkpoint in checkpoints)
        {
            float distanceToCheckPoint = Vector2.Distance(player.position, checkpoint.transform.position);

            if (distanceToCheckPoint < chosestDistance && checkpoint.actived == true)
            {
                chosestDistance = distanceToCheckPoint;
                closestCheckpoint = checkpoint;
            }
        }

        return closestCheckpoint;
    }

    //暂停游戏
    public void PauseGame(bool _pause)
    {
        if (_pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
}

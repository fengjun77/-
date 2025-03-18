using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance;

    private Transform player;
    //�浵���б�
    [SerializeField] private Checkpoint[] checkpoints;
    [SerializeField] private string closestCheckpointID;

    [Header("��ʧ��Ǯ")]
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

        //�ҵ����������еĴ浵�㣬�����б���
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
        //������д洢�Ĵ浵����Ϣ
        foreach (KeyValuePair<string, bool> pair in _data.checkpoints)
        {
            //���������ϵĴ浵���б�
            foreach (Checkpoint checkpoint in checkpoints)
            {
                //�����id��ͬ�ģ���������ֵ��true ��ô�ͼ���Ĵ浵��
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

        //����ÿ������
        foreach (Checkpoint checkpoint in checkpoints)
        {
            _data.checkpoints.Add(checkpoint.checkpointID, checkpoint.actived);
        }
    }

    //�ҵ�����ı�����Ĵ浵��
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

    //��ͣ��Ϸ
    public void PauseGame(bool _pause)
    {
        if (_pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
}

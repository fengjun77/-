using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour,ISaveManager
{
    [Header("�������㻭��")]
    [SerializeField] private FadeScreen_UI fadeScreen;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartButton;
    [Header("��ɱBoss���㻭��")]
    [SerializeField] private GameObject successfulText;
    [SerializeField] private GameObject overButton;
    [SerializeField] private GameObject gameOverTime;
    [SerializeField] private InGame_UI inGame;
    [SerializeField] private TextMeshProUGUI gameTime;
    [SerializeField] private GameObject time;
    [Space]

    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionUI;
    [SerializeField] private GameObject inGameUI;

    public ItemToolTip_UI itemToolTip;
    public StatToolTip_UI statToolTip;
    public SkillToolTip_UI skillToolTip;
    public CraftWindow_UI craftWindow;

    [SerializeField] private VolumeSlider_UI[] volumeSettings;



    private void Awake()
    {
        SwitchTo(skillTreeUI);//���ȼ��ؼ��������Ա�֤���ܿ���ʹ��
        fadeScreen.gameObject.SetActive(true);
    }

    void Start()
    {
        SwitchTo(inGameUI);

        itemToolTip.gameObject.SetActive(false);
        statToolTip.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            SwitchWithKeyTo(characterUI);

        if (Input.GetKeyDown(KeyCode.B))
            SwitchWithKeyTo(craftUI);

        if (Input.GetKeyDown(KeyCode.K))
            SwitchWithKeyTo(skillTreeUI);

        if (Input.GetKeyDown(KeyCode.O))
            SwitchWithKeyTo(optionUI);    
    }

    //��ѡ���UI����
    public void SwitchTo(GameObject _menu)
    {
        //�ȹر����н���
        for (int i = 0; i < transform.childCount; i++)
        {
            bool isFadeScreen = transform.GetChild(i).GetComponent<FadeScreen_UI>() != null;
            
            
            if(!isFadeScreen)
                transform.GetChild(i).gameObject.SetActive(false);
        }

        CloseAllToolTips();

        //��ʾ��Ӧ����
        if(_menu != null)
        {
            AudioManager.instance.PlaySFX(4);
            _menu.SetActive(true);
        }

        if (GameManager.instance != null)
        {
            if (_menu == inGameUI)
                GameManager.instance.PauseGame(false);
            else
                GameManager.instance.PauseGame(true);
        }
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        if (_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);
            CloseAllToolTips();
            CheckForInGameUI();
            return;
        }

        
        SwitchTo(_menu);
    }

    private void CloseAllToolTips()
    {
        itemToolTip.gameObject.SetActive(false);
        statToolTip.gameObject.SetActive(false);
    }

    //������������漤��򲻿�����ϷUI ���򼤻���ϷUI
    private void CheckForInGameUI()
    {
        for(int i = 0; i < transform.childCount;i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).GetComponent<FadeScreen_UI>() == null)
                return;
        }

        SwitchTo(inGameUI);
    }


    //ת����������
    public void SwitchOnEndScreen()
    {
        fadeScreen.FadeOut();
        StartCoroutine(EndScreenCorutine());
    }

    //ת��ʤ������
    public void SwitchOnSuccessfulScreen()
    {
        fadeScreen.FadeOut();
        gameTime.text = inGame.gameTime.ToString("F2");
        StartCoroutine(SuccessfulScreenCorutine());
    }

    IEnumerator EndScreenCorutine()
    {
        yield return new WaitForSeconds(1);

        endText.SetActive(true);

        yield return new WaitForSeconds(2);

        restartButton.SetActive(true);
    }

    IEnumerator SuccessfulScreenCorutine()
    {
        yield return new WaitForSeconds(1);

        gameOverTime.SetActive(true);
        time.SetActive(true);
        successfulText.SetActive(true);

        yield return new WaitForSeconds(2);

        overButton.SetActive(true);
    }

    //���¿�ʼ��Ϸ��ť
    public void RestartGameButton() => GameManager.instance.RestartScene();

    //��ͷ���� ɾ����Ϸ�浵��ť
    public void DestroyGameDataButton()
    {
        SaveManager.instance.DeleteSaveData();
        SceneManager.LoadScene("MainMenu");
    }
    public void LoadData(GameData _data)
    {
        foreach(KeyValuePair<string,float> pair in _data.volumeSettings)
        {
            foreach(VolumeSlider_UI item in volumeSettings)
            {
                if(item.parametr == pair.Key)
                    item.LoadSlider(pair.Value);
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.volumeSettings.Clear();

        foreach(VolumeSlider_UI item in volumeSettings)
        {
            _data.volumeSettings.Add(item.parametr,item.slider.value);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour,ISaveManager
{
    [Header("死亡结算画面")]
    [SerializeField] private FadeScreen_UI fadeScreen;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartButton;
    [Header("击杀Boss结算画面")]
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
        SwitchTo(skillTreeUI);//优先加载技能树，以保证技能可以使用
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

    //打开选择的UI界面
    public void SwitchTo(GameObject _menu)
    {
        //先关闭所有界面
        for (int i = 0; i < transform.childCount; i++)
        {
            bool isFadeScreen = transform.GetChild(i).GetComponent<FadeScreen_UI>() != null;
            
            
            if(!isFadeScreen)
                transform.GetChild(i).gameObject.SetActive(false);
        }

        CloseAllToolTips();

        //显示对应界面
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

    //如果有其他界面激活，则不开启游戏UI 否则激活游戏UI
    private void CheckForInGameUI()
    {
        for(int i = 0; i < transform.childCount;i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).GetComponent<FadeScreen_UI>() == null)
                return;
        }

        SwitchTo(inGameUI);
    }


    //转到结束界面
    public void SwitchOnEndScreen()
    {
        fadeScreen.FadeOut();
        StartCoroutine(EndScreenCorutine());
    }

    //转到胜利界面
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

    //重新开始游戏按钮
    public void RestartGameButton() => GameManager.instance.RestartScene();

    //从头再来 删除游戏存档按钮
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

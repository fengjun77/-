using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private float sfxMinDistance;
    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;

    public bool playBGM;
    private int bgmIndex;
    private string sceneName;

    private bool canPlaySFX;
    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;

        Invoke("AllowSFX", 2f);
    }

    private void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
    }

    private void Update()
    {
        if(!playBGM)
            StopAllBGM();
        else
        {
            if (!bgm[bgmIndex].isPlaying)
            {
                if (sceneName == "MainMenu")
                    PlayBGM(0);
                else
                    PlayBGM(Random.Range(1, 3));
            }
            
        }
    }

    public void PlaySFX(int _sfxIndex,Transform _source = null)
    {
        if(!canPlaySFX)
            return;

        //如果音频和玩家距离超过一定限度，则不播放该音频
        if (_source != null && Vector2.Distance(PlayerManager.instance.player.transform.position, _source.position) > sfxMinDistance)
            return;

        if (_sfxIndex < sfx.Length)
        {
            sfx[_sfxIndex].pitch = Random.Range(0.85f, 1.1f);
            sfx[_sfxIndex].Play();
        }
    }

    public void StopSFX(int _index) => sfx[_index].Stop();

    //缓慢降低音效，最后停止
    public void StopSFXWithDelay(int _index)
    {
        StartCoroutine(DecreaseVolume(sfx[_index]));
    }

    private IEnumerator DecreaseVolume(AudioSource _audio)
    {
        float defaultVolume = _audio.volume;

        while (_audio.volume > .1f)
        {
            _audio.volume -= _audio.volume * .4f;
            yield return new WaitForSeconds(.15f);
            
            if (_audio.volume < .1f)
            {
                _audio.Stop();
                _audio.volume = defaultVolume;
                break;
            }
        }
    }

    public void PlayBGM(int _bgmIndex)
    {
        bgmIndex = _bgmIndex;

        StopAllBGM();
        bgm[bgmIndex].Play();
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    private void AllowSFX() => canPlaySFX = true;
}

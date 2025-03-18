using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Options_UI : MonoBehaviour
{
    public void SaveAndExit()
    {
        SaveManager.instance.SaveGame();
        SceneManager.LoadScene("MainMenu");
    }
}

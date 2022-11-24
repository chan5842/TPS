using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [HideInInspector] public int killCount;
    public Text killCountTxt;
    public static UIManager uiManager;

    void Awake()
    {
        if (uiManager == null)
            uiManager = this;
        else if (uiManager != this)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);

        LoadGameData();
    }

    void LoadGameData()
    {
        killCount = PlayerPrefs.GetInt("KILL_COUNT", 0);
        killCountTxt.text = "KILL: " + "<color=#ff0000>" + killCount.ToString("0000") + "</color>" ;
    }
    public void InkillCount()
    {
        ++killCount;
        killCountTxt.text = "KILL: " + "<color=#ff0000>" + killCount.ToString("0000") + "</color>";
        PlayerPrefs.SetInt("KILL_COUNT", killCount);
    }
}

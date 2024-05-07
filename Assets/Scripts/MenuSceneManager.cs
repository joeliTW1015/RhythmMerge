using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Audio;

public class MenuSceneManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI highestScoreText;
    [SerializeField] TextMeshProUGUI modeText;
    [SerializeField] List<string> modeNames;
    [SerializeField] GameObject settingPanel
    ;
    //temp!
    [SerializeField] AudioMixer audioMixer;

    int modeValue;
    private void Awake() 
    {
        //TODO: 針對不同模式的紀錄改變文字
        modeValue = PlayerPrefs.GetInt("gameMode", 0);
        modeText.text = modeNames[modeValue];
        if(modeValue != 3 && modeValue != 4)
            highestScoreText.text = "Best Score: " + PlayerPrefs.GetInt("highestScore" + modeValue.ToString(), 0).ToString();    
        else
            highestScoreText.text = "";
    }

    private void Start() 
    {
        //temp,之後整合進AudioManager
        audioMixer.SetFloat("MusicValue", Mathf.Log10(PlayerPrefs.GetFloat("musicValue", 0.5f)) * 20);
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ModeSwitch_Left();
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            ModeSwitch_Right();
        }
        else if(Input.GetKeyDown(KeyCode.Return))
        {
            EnterPlayScene();
        }

    }

    public void ModeSwitch_Left()
    {
        if(modeValue > 0)
        {
            modeValue --;
            modeText.text = modeNames[modeValue];

            if(modeValue != 3 && modeValue != 4)
                highestScoreText.text = "Best Score: " + PlayerPrefs.GetInt("highestScore" + modeValue.ToString(), 0).ToString();    
            else
                highestScoreText.text = "";
                
            PlayerPrefs.SetInt("gameMode", modeValue);
        }
    }

    public void ModeSwitch_Right()
    {
        if(modeValue < 4)
        {
            modeValue ++;
            modeText.text = modeNames[modeValue];

            if(modeValue != 3 && modeValue != 4)
                highestScoreText.text = "Best Score: " + PlayerPrefs.GetInt("highestScore" + modeValue.ToString(), 0).ToString();    
            else
                highestScoreText.text = "";

            PlayerPrefs.SetInt("gameMode", modeValue);
        }
    }

    public void EnterPlayScene()
    {
        //TODO: 增加模式選單邏輯
        GameManager.gameMod = modeValue;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //TODO: 轉場
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettingPanel()
    {
        settingPanel.SetActive(true);
    }
}

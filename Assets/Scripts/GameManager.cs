using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int gameMod; // 0: 60BPM 1: 100BPM  2: 經典 3 練習 4延遲調整
    TileBoard board;
    public int score; 
    public float health;
    public bool paused;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highestScoreText;
    [SerializeField] TextMeshProUGUI gameOverScoreText;
    [SerializeField] GameObject pausePanel, gameOverPanel, settingPanel;
    MusicGameManager musicGameManager;
    private void Awake() 
    {
        musicGameManager = GetComponent<MusicGameManager>();
        board = FindObjectOfType<TileBoard>();
        paused = false;

        if(gameMod != 3 && gameMod != 4)
        {
            highestScoreText.text = "Best Score: " + PlayerPrefs.GetInt("highestScore" + gameMod.ToString(), 0).ToString();
        }
    }

    private void Start() 
    {
        //TODO: 增加開場倒數
        NewGame();    
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GamePause();
        }    
    }

    void NewGame()
    {
        Debug.Log("NewGame");
        score = 0;
        health = 100;
        scoreText.text = score.ToString();
        board.ClearBoard();
        board.enabled = true;
        board.CreateTile();    
        board.CreateTile();  
        musicGameManager.enabled = true;
    }

    public void GameOver()
    {
        Debug.Log("GameOver");
        MusicGameManager.isPlaying = false;
        board.ClearBoard();
        board.enabled = false;
        musicGameManager.mainMusic.Stop();
        musicGameManager.enabled = false;
        gameOverPanel.SetActive(true);
        if(gameMod != 3 && gameMod != 4)
            gameOverScoreText.text = "SCORE: " + score.ToString();
        //TODO: 選單
        //TODO: 增加遊戲結束過場
    }


    public void RestartGame()
    {
        //TODO: 轉場
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddScore(int value)
    {
        if(gameMod == 3 || gameMod == 4)
        {
            return;
        }

        score += value;
        scoreText.text = score.ToString();
        if(score > PlayerPrefs.GetInt("highestScore" + gameMod.ToString(), 0))
        {
            PlayerPrefs.SetInt("highestScore" + gameMod.ToString(), score);
            highestScoreText.text = "Best Score: " + score.ToString();
        }
        //TODO: ScoreTextAnimation
    }

    public void GamePause()
    {
        musicGameManager.mainMusic.Pause();
        paused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void GameResume()
    {
        musicGameManager.mainMusic.Play();
        paused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void BackToMenu()
    {
        //TODO: 過場效果
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
        //temp!會有更多場景，不能用0
    }

    public void OpenSettingPanel()
    {
        settingPanel.SetActive(true);
    }
}

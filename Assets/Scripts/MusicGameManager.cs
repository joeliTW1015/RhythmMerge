using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicGameManager : MonoBehaviour
{
    public static bool isPlaying;
    public float timeModifier;
    [SerializeField] float missHealth = 10, addHealth = 5, hintTextDuration = 0.5f, shrinkTimeMod, enlargeTimeMod;
    [SerializeField] RectTransform board;
    [SerializeField] GameObject bgEffects;
    [SerializeField] Image box;
    [SerializeField] Image healthBar;
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI hintText, gameTimeText, delayTimeText, countDownText;
    [SerializeField] GameObject modifyModeObject;
    [SerializeField] AudioClip normalMusic, fastMusic;
    public AudioSource mainMusic;
    
    public float bpm;
    public bool debug;
    bool hitCurrentBeat, missCurrentBeat, boxFlag, resetFlag;
    GameManager gameManager;
    Coroutine hintTextCoroutine;
    float gameTimer;
    float nextBeatTimer;
    float colorHue;
    float musicLength;

    private void Awake() 
    {
        isPlaying = false;
        gameManager = GetComponent<GameManager>();    
        mainMusic = GetComponent<AudioSource>();
        timeModifier = PlayerPrefs.GetFloat("offsetValue", 0f) * -1;
        //決定音樂速度   
        if(GameManager.gameMod == 1)
        {
            bpm = 100;
            musicLength = 211.2f;
            mainMusic.clip = fastMusic;
        } 
        else
        {
            bpm = 60;
            musicLength = 352;
            mainMusic.clip = normalMusic;
        }
    }

    private void Start() 
    {
        //要顯示的物件
        switch (GameManager.gameMod)
        {
            case 0:
                healthBar.enabled = true;
                hintText.enabled = true;
                gameManager.scoreText.enabled = true;
                gameManager.highestScoreText.enabled = true;
                countDownText.enabled = true;
                break;

            case 1:
                healthBar.enabled = true;
                hintText.enabled = true;
                gameManager.scoreText.enabled = true;
                gameManager.highestScoreText.enabled = true;
                countDownText.enabled = true;
                break;

            case 2:
                gameManager.scoreText.enabled = true;
                gameManager.highestScoreText.enabled = true;
                break;
                
            case 3:
                hintText.enabled = true;
                countDownText.enabled = true;
                break;
            case 4:
                hintText.enabled = true;
                countDownText.enabled = true;
                modifyModeObject.SetActive(true);
                delayTimeText.text = "Timing deviation:";
                break;
            default: 
                break;
        }
        nextBeatTimer = 60/bpm; 
        colorHue = 0f;

        //開場倒數，延遲播音樂
        if(GameManager.gameMod != 2)
            StartCoroutine(CountDown());
        else
        {
            mainMusic.Play();
            isPlaying = true;
        }
       
    }

    IEnumerator CountDown()
    {
        for(int i = 0; i < 4; i++)
        {
            if(i != 3)
            {
                countDownText.text = (3 - i).ToString("0");
            }
            else
            {
                countDownText.text = "GO!";
            }
            yield return new WaitForSeconds(60/bpm);

            if(i > 1)
                SpawnBox();
        }
        mainMusic.Play();
        isPlaying = true;
        countDownText.enabled = false;
    }

    
    private void Update() 
    {

        //TODO: 血量條動畫
        if(GameManager.gameMod != 2 && GameManager.gameMod != 3 && GameManager.gameMod != 4)
        {
            healthBar.rectTransform.sizeDelta =new Vector2(gameManager.health * 1920/100, healthBar.rectTransform.rect.height);
            if(gameManager.health <= 0 && !debug)
            {
                gameManager.GameOver();
            }
        }

        if(GameManager.gameMod != 2)
        {
            gameTimer = mainMusic.time + timeModifier;
            if(nextBeatTimer == 0 && mainMusic.time > 2)
            {
                gameTimer = mainMusic.time - musicLength;
            }

            if(GameManager.gameMod == 4)
            {
                gameTimeText.text = "Beat: " + ((int)gameTimer).ToString();
            }
            
            if(gameTimer >= nextBeatTimer + 0.11f)
            {
                nextBeatTimer += 60/bpm;
                if(nextBeatTimer >= musicLength)
                {
                    nextBeatTimer = 0;
                }

                CheckMiss();   
                missCurrentBeat = false;
                boxFlag = false;
                resetFlag = false;
            }
            else if(gameTimer >= nextBeatTimer)
            {
                if(!boxFlag)
                {
                    SpawnBox();
                    boxFlag = true;
                }
            }
            else if(gameTimer >= nextBeatTimer - 0.11f)
            {
                if(!resetFlag)
                {
                    hitCurrentBeat = false;
                    resetFlag = true;
                }
            }
        }
    }

    void CheckMiss()
    {
        if(missCurrentBeat || hitCurrentBeat)
        {
            return;
        }
        else
        {
            MissTheBeat();
        }
    }
    void SpawnBox()
    {
        Image currentBox = Instantiate(box, bgEffects.transform);
        currentBox.rectTransform.sizeDelta = new Vector2(board.rect.width, board.rect.height) * 4;
        currentBox.color = Color.HSVToRGB(colorHue, 1, 1);
        colorHue += 0.71f;
        if(colorHue > 1)
        {
            colorHue -= 1;
        }
        StartCoroutine(ShrinkBox(currentBox));
    }
    
    IEnumerator ShrinkBox(Image _currentBox)
    {
        float elapsed = 0;
        Vector2 startSize = _currentBox.rectTransform.sizeDelta;
        Vector2 boardSize = new Vector2(board.rect.width, board.rect.height);
        while(elapsed <= (60/bpm) * shrinkTimeMod)
        {
            _currentBox.rectTransform.sizeDelta = Vector2.Lerp(startSize, boardSize, elapsed /((60/bpm) * shrinkTimeMod));
            elapsed += Time.deltaTime;
            yield return null;
        }
        _currentBox.rectTransform.sizeDelta = boardSize;

        //color
        Color tempColor = _currentBox.color;
        tempColor.a = 0.3f;
        background.color = tempColor;
        _currentBox.enabled = false;
        StartCoroutine(EnlaegeBox(_currentBox));
    }

    IEnumerator EnlaegeBox(Image _currentBox)
    {

        yield return new WaitForSeconds((60/bpm)*0.5f);
        _currentBox.enabled = true;
        float elapsed = 0;
        Vector2 startSize = _currentBox.rectTransform.sizeDelta;
        Vector2 enlargeSize = new Vector2(2000, 2000);
        Color tempColor = _currentBox.color;
        while(elapsed <= (60/bpm) * enlargeTimeMod)
        {
            _currentBox.rectTransform.sizeDelta = Vector2.Lerp(startSize, enlargeSize, elapsed /((60/bpm) * enlargeTimeMod));
            tempColor.a = Mathf.Lerp(1, 0, elapsed/((60/bpm) * enlargeTimeMod)) * 0.5f;
            _currentBox.color = tempColor;
            elapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(_currentBox.gameObject);
    }
    

    public bool ButtonPressed() //true表示打在拍點中間，兩倍分數
    {
        if(GameManager.gameMod == 2)
        {
            return false;
        }

        float delayTime = Mathf.Abs(gameTimer - nextBeatTimer);

        if(GameManager.gameMod == 4)
            delayTimeText.text = "Timing deviation:\n " + (gameTimer - nextBeatTimer).ToString("0.00000");

        if(delayTime <= 0.05f)
        {
            if(hintTextCoroutine != null)
                StopCoroutine(hintTextCoroutine);

            hintTextCoroutine =  StartCoroutine(ShowHintText(2));
            hitCurrentBeat = true;

            if(gameManager.health<= 95)
                gameManager.health += addHealth;
        }
        else if(delayTime < 0.1f)
        {
            if(hintTextCoroutine != null)
                StopCoroutine(hintTextCoroutine);

            hintTextCoroutine =  StartCoroutine(ShowHintText(1));
            hitCurrentBeat = true;
        }
        else if(Mathf.Abs(delayTime - 0.5f * (60/bpm)) <= 0.1f && hitCurrentBeat)
        {
            if(hintTextCoroutine != null)
                StopCoroutine(hintTextCoroutine);

            hintTextCoroutine =  StartCoroutine(ShowHintText(3));
            return true;
        }
        else
        {
            MissTheBeat();
        }

        return false;
    }

    void MissTheBeat()
    {
        Debug.Log("Miss");
        //TODO:添加其他效果
        missCurrentBeat = true;
        gameManager.health -= missHealth;

        if(hintTextCoroutine != null)
            StopCoroutine(hintTextCoroutine);

        hintTextCoroutine = StartCoroutine(ShowHintText(0));
    }
    

    //0 = miss, 1 = good, 2 = perfect, 3 = bounus hit
    IEnumerator ShowHintText(int mode)
    {
        //TODO: 根據發光調整顏色 做調整顏色介面
        if(mode == 0)
        {
            hintText.color = Color.red;
            hintText.text = "MISS!";
        }
        else if(mode == 1)
        {
            hintText.color = Color.yellow;
            hintText.text = "GOOD!";
        }
        else if(mode == 2)
        {
            hintText.color = Color.green;
            hintText.text = "PERFECT!";
        }
        else if(mode == 3)
        {
            hintText.color = Color.blue;
            hintText.text = "BONUS HIT!";
        }

        float elapsed = 0;
        Color tempColor = hintText.color;
        while(elapsed <= hintTextDuration)
        {
            tempColor.a = Mathf.Lerp(1, 0, elapsed / hintTextDuration);
            hintText.color = tempColor;
            elapsed += Time.deltaTime;
            yield return null;
        }

        hintText.text = " ";
    }

}

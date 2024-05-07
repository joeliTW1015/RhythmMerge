using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ModifyModeManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI offsetValueText;
    [SerializeField] MusicGameManager musicGameManager;
    float offsetValue;
    private void Start() 
    {
        offsetValue = PlayerPrefs.GetFloat("offsetValue", 0f);    
        offsetValueText.text = offsetValue.ToString("0.00");
    }

    public void IncreaseValue()
    {
        if(offsetValue < 0.5f)
        {
            offsetValue += 0.05f;
            offsetValueText.text = offsetValue.ToString("0.00");
            musicGameManager.timeModifier = offsetValue * -1;
            PlayerPrefs.SetFloat("offsetValue", offsetValue);

        }
    }

    public void DecreaseValue()
    {
        if(offsetValue > -0.5f)
        {
            offsetValue -= 0.05f;
            offsetValueText.text = offsetValue.ToString("0.00");
            musicGameManager.timeModifier = offsetValue * -1;
            PlayerPrefs.SetFloat("offsetValue", offsetValue);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] Slider sfx, music;
    [SerializeField] AudioMixer audioMixer;

    //TODO: 增加音效調整部分。和AudioManager連接
    private void OnEnable() 
    {
        music.value = PlayerPrefs.GetFloat("musicValue", 0.5f);
        music.onValueChanged.AddListener(SetMusicValue);
    }

    void SetMusicValue(float value)
    {
        audioMixer.SetFloat("MusicValue", Mathf.Log10(value) * 20);
    }

    public void CloseSettingPanel()
    {
        PlayerPrefs.SetFloat("musicValue", music.value);
        this.gameObject.SetActive(false);
    }
}

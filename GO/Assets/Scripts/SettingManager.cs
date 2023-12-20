using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;
    private void Awake()
    {
        musicSlider.value = StaticData.musicValue;
        soundSlider.value = StaticData.soundValue;
        SetBGMusicValue();
        SetSoundValue();
    }

    public void SetBGMusicValue()
    {
        GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>().GetMusicAudioSource().volume = musicSlider.value;
        StaticData.musicValue = musicSlider.value;
    }
    public void SetSoundValue()
    {
        GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>().GetPutStoneSoundAudioSource().volume = soundSlider.value;
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Button");
        foreach(var elem in obj)
        {
            elem.GetComponent<ButtonSoundScript>().GetMousePressSoundAudioSource().volume = soundSlider.value;
        }
        StaticData.soundValue = soundSlider.value;
    }
}

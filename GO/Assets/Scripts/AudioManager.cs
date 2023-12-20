using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgMusic;
    [SerializeField] private AudioSource putStoneSound;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;
    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        bgMusic.Play();
    }
    // Start is called before the first frame update
    void Start()
    {
        musicSlider.value = StaticData.musicValue;
        soundSlider.value = StaticData.soundValue;
    }

    public void PlayMusic()
    {
        if (bgMusic.isPlaying) return;
        bgMusic.Play();
    }

    public void StopMusic()
    {
        bgMusic.Stop();
    }
    public AudioSource GetMusicAudioSource()
    {
        return bgMusic;
    }
    public AudioSource GetPutStoneSoundAudioSource()
    {
        return putStoneSound;
    }
    public void SetSoundValue()
    {
        GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>().GetPutStoneSoundAudioSource().volume = soundSlider.value;
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Button");
        foreach (var elem in obj)
        {
            elem.GetComponent<ButtonSoundScript>().GetMousePressSoundAudioSource().volume = soundSlider.value;
        }
        StaticData.soundValue = soundSlider.value;
    }
}

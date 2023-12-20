using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSoundScript : MonoBehaviour
{
    [SerializeField] private AudioSource mousePressSound;
    // Start is called before the first frame update
    public AudioSource GetMousePressSoundAudioSource()
    {
        return mousePressSound;
    }
}

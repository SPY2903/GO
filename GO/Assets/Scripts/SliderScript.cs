using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private string type;
    public void SetValue()
    {
        if (type.Equals("music"))
        {
            StaticData.musicValue = slider.value;
        }
        if (type.Equals("sound"))
        {
            StaticData.soundValue = slider.value;
        }
    }
}

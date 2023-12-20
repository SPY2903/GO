using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticData :MonoBehaviour
{
    public static string gridSize = "9x9";
    public static string rule = "JP";
    public static string stone = "black";
    public static bool playWithAI = false;
    public static float musicValue = 1;
    public static float soundValue = 1;

    public void SetPlayWithAI()
    {
        playWithAI = true;
    }
    public void SetPlayWithP()
    {
        playWithAI = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionData : MonoBehaviour
{
    [SerializeField] private GameObject _9x9Image;
    [SerializeField] private GameObject _13x13Image;
    [SerializeField] private GameObject _19x19Image;
    [SerializeField] private GameObject JPImage;
    [SerializeField] private GameObject CNImage;
    [SerializeField] private GameObject BlackStoneBorder;
    [SerializeField] private GameObject WhiteStoneBorder;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SendData()
    {
        if (_9x9Image.activeInHierarchy)
        {
            StaticData.gridSize = "9x9";
        }
        if (_13x13Image.activeInHierarchy)
        {
            StaticData.gridSize = "13x13";
        }
        if (_19x19Image.activeInHierarchy)
        {
            StaticData.gridSize = "19x19";
        }
        if (JPImage.activeInHierarchy)
        {
            StaticData.rule = "JP";
        }
        if (CNImage.activeInHierarchy)
        {
            StaticData.rule = "CN";
        }
        if (BlackStoneBorder.activeInHierarchy)
        {
            StaticData.stone = "black";
        }
        if (WhiteStoneBorder.activeInHierarchy)
        {
            StaticData.stone = "white";
        }
    }
}

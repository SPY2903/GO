using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountingPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string[] displayContent;
    [SerializeField] private float timeDisplay = 0.5f;
    [SerializeField] private GameObject aiCountingPanel;
    int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        //InvokeRepeating("DisPlay", 0.5f, timeDisplay);
        StartCoroutine(DisPlay());
    }

    // Update is called once per frame
    void Update()
    {
    }
    IEnumerator DisPlay()
    {
        for(int i = 0; i < displayContent.Length; i++)
        {
            text.text = displayContent[i];
            yield return new WaitForSeconds(timeDisplay);
        }
        StartCoroutine(SetInActive());
    }
    IEnumerator SetInActive()
    {
        yield return new WaitForSeconds(0.25f);
        //Destroy(gameObject);
        gameObject.SetActive(false);
        GameControl gameControl = GameObject.FindGameObjectWithTag("ChessBoard").GetComponent<GameControl>();
        if (gameControl.isAiFitsrTurn)
        {
            aiCountingPanel.SetActive(true);
        }
    }
}

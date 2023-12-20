using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AiCounting : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string[] displayContent;
    [SerializeField] private float timeDisplay = 0.5f;
    private bool hasPutStone = false;
    public bool GetHasPutStone()
    {
        return hasPutStone;
    }
    public void SetHasPutStone(bool has)
    {
        hasPutStone = has;
    }
    public struct IndexOfIAndJ
    {
        public int i;
        public int j;
    }

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
    public void DisPlayPan()
    {
        StartCoroutine(DisPlay());
    }
    IEnumerator DisPlay()
    {
        for (int i = 0; i < displayContent.Length; i++)
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
        if (!gameObject.activeInHierarchy)
        {
            GameControl gameControl = GameObject.FindGameObjectWithTag("ChessBoard").GetComponent<GameControl>();
            if (gameControl.isAiFitsrTurn)
            {
                gameControl.PutRand();
            }
            if (hasPutStone && StaticData.playWithAI && !gameControl.isAiFitsrTurn)
            {
                IndexOfIAndJ index = new IndexOfIAndJ();
                index.i = gameControl.IndexToPutStone().i;
                index.j = gameControl.IndexToPutStone().j;
                Debug.Log(index.i + "~" + index.j);
                hasPutStone = false;
                gameControl.GetMatrix()[index.i, index.j].GetComponent<Stone>().PutStone();
            }
        }
    }
}

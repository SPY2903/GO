using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessboard : MonoBehaviour
{
    [SerializeField] private int row;
    [SerializeField] private int col;
    [SerializeField] private float start_X;
    [SerializeField] private float start_Y;
    [SerializeField] private float xSpace, ySpace;
    [SerializeField] private GameObject prefab;
    private GameControl gameControl;
    // Start is called before the first frame update
    void Start()
    {
        gameControl = GameObject.FindGameObjectWithTag("ChessBoard").GetComponent<GameControl>();
        gameControl.SetMatrix(row, col);
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                gameControl.GetMatrix()[i,j] = Instantiate(prefab, new Vector3( start_X + (ySpace * (j % row)), start_Y + (-xSpace * (i % col))), Quaternion.identity);
                gameControl.GetMatrix()[i, j].GetComponent<Stone>().SetIndexI(i);
                gameControl.GetMatrix()[i, j].GetComponent<Stone>().SetIndexJ(j);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public int GetRow()
    {
        return row;
    }
    public int GetCol()
    {
        return col;
    }
}

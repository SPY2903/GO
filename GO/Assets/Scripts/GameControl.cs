using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;

public class GameControl : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Sprite board9x9, board13x13, board19x19, blackStone, whiteStone;
    [SerializeField] private GameObject grid9x9, grid13x13, grid19x19;
    [SerializeField] private GameObject whiteStoneBorder;
    [SerializeField] private GameObject blackStoneBorder;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject exitPanel;
    [SerializeField] private GameObject newGamePanel;
    [SerializeField] private GameObject decidePanel;
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject countingPanel;
    [SerializeField] private GameObject aiCoutingPanel;
    [SerializeField] private GameObject passTurnPanel;
    [SerializeField] private Image winStone;
    [SerializeField] private TextMeshProUGUI txtScore;
    [SerializeField] private Button backButton;
    [SerializeField] private Button HintsButton;
    private float komi = 6.5f;
    private bool japanRules = true, chinaRules = false;
    private bool player1 = true, firstTurn = true, isBlackStone = true, isWhiteStone = false,isPlayingWithAi = false;
    public bool isAiFitsrTurn = false;
    private int row, col;
    private GameObject[,] matrix;
    private int[] dx = { -1, 0, 0, 1 };
    private int[] dy = { 0, -1, 1, 0 };
    private bool[,] visited, supportVisited;
    private int sum = 0;
    private int countBack = 0;
    private int countToEndGame = 0;
    private int lastestIndexI = 0, lastestIndexJ = 0, valueCaptured = -1;
    private float blackScore = 0f, whiteScore = 0f;
    private int blackPrisoners = 0, whitePrisoners = 0,
        blackDeadStones = 0, whiteDeadStones = 0, 
        blackTerritory = 0, whiteTerritory = 0,
        numberBlackStones = 0, numberWhiteStones = 0;
    IDictionary<int,int> sumOfLiberties = new Dictionary<int,int>();
    IDictionary<int, int> listSumOfTwoEyes = new Dictionary<int, int>();
    IDictionary<int, bool> listSingleStone = new Dictionary<int, bool>();
    private int numberOfEyes = 0;
    private int numberOfCommonTerritory = 0;
    private int countNumberOfEyesInside = 0;
    private bool isSingleStone = false;

    public struct IndexOfIAndJ
    {
        public int i;
        public int j;
    }
    public class MoveResult
    {
        public int Value { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }

    IDictionary<int, IndexOfIAndJ> listIndexOfIAndJ = new Dictionary<int, IndexOfIAndJ>();
    IDictionary<int, IndexOfIAndJ> listCapturedStones = new Dictionary<int, IndexOfIAndJ>();
    IDictionary<int, IndexOfIAndJ> listLifeStone = new Dictionary<int, IndexOfIAndJ>();
    IDictionary<int, IndexOfIAndJ> listDeadStone = new Dictionary<int, IndexOfIAndJ>();
    IDictionary<int, List<IndexOfIAndJ>> listLiberties = new Dictionary<int, List<IndexOfIAndJ>>();
    List<IndexOfIAndJ> lstLib = new List<IndexOfIAndJ>();
    private int sizeOfListCapturedStones = 0;
    private List<IndexOfIAndJ> listConnectedComponents = new List<IndexOfIAndJ>();
    private List<IndexOfIAndJ> listVisitedEyes = new List<IndexOfIAndJ>();
    private List<IndexOfIAndJ> listVisitedCommonTerritory = new List<IndexOfIAndJ>();

    void Start()
    {
        if (StaticData.gridSize.Equals("9x9"))
        {
            GetComponent<SpriteRenderer>().sprite = board9x9;
            grid9x9.SetActive(true);
        }
        if (StaticData.gridSize.Equals("13x13"))
        {
            GetComponent<SpriteRenderer>().sprite = board13x13;
            grid13x13.SetActive(true);
        }
        if (StaticData.gridSize.Equals("19x19"))
        {
            GetComponent<SpriteRenderer>().sprite = board19x19;
            grid19x19.SetActive(true);
        }
        if (StaticData.rule.Equals("JP"))
        {
            japanRules = true;
            chinaRules = false;
        }
        if (StaticData.rule.Equals("CN"))
        {
            chinaRules = true;
            japanRules = false;
        }
        if (StaticData.stone.Equals("black"))
        {
            isBlackStone = true;
            isWhiteStone = false;
        }
        if (StaticData.stone.Equals("white"))
        {
            isWhiteStone = true;
            isBlackStone = false;
        }
        if(StaticData.playWithAI && StaticData.stone.Equals("white"))
        {
            isAiFitsrTurn = true;
        }
        if (!StaticData.playWithAI)
        {
            HintsButton.interactable = false;
        }
        else
        {
            backButton.interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingPanel.activeInHierarchy)
            {
                settingPanel.SetActive(false);
            }
            if (exitPanel.activeInHierarchy)
            {
                exitPanel.SetActive(false);
            }
            if (newGamePanel.activeInHierarchy)
            {
                newGamePanel.SetActive(false);
            }
            if (decidePanel.activeInHierarchy)
            {
                decidePanel.SetActive(false);
                newGamePanel.SetActive(true);
            }
            if (selectionPanel.activeInHierarchy)
            {
                selectionPanel.SetActive(false);
                decidePanel.SetActive(true);
            }
        }
    }
    public bool GetPlayer()
    {
        return player1;
    }
    public void SetPlayer(bool player)
    {
        this.player1 = player;
    }
    public void Turn()
    {
        if (player1)
        {
            whiteStoneBorder.SetActive(true);
            blackStoneBorder.SetActive(false);
        }
        else
        {
            whiteStoneBorder.SetActive(false);
            blackStoneBorder.SetActive(true);
        }
    }
    public void NextTurn()
    {
        this.player1 = !this.player1;
        Turn();
    }
    public void Pass()
    {
        countToEndGame++;
        NextTurn();
        InitiateListCapturedStones();
        if (player1)
        {
            search(0);
            passTurnPanel.GetComponentInChildren<TextMeshProUGUI>().text = "Trắng bỏ lượt";
        }
        else
        {
            search(1);
            passTurnPanel.GetComponentInChildren<TextMeshProUGUI>().text = "Đen bỏ lượt";
        }
        StartCoroutine(DisplayPassturnPanel());
    }
    public void SetDefaultCountToEndGame()
    {
        countToEndGame = 0;
    }
    public GameObject[,] GetMatrix()
    {
        return matrix;
    }
    public GameObject GetAiCountingPanel()
    {
        return aiCoutingPanel;
    }

    public int[,] Clone()
    {
        int[,] cloneMatrix = new int[row, col];
        for(int i = 0; i < row; i++)
        {
            for(int j = 0; j < col; j++)
            {
                cloneMatrix[i, j] = -1;
            }
        }
        return cloneMatrix;
    }
    public void SetMatrix(int row, int col)
    {
        this.row = row;
        this.col = col;
        matrix = new GameObject[row, col];
    }
    public int GetRow()
    {
        return row;
    }
    public int GetCol()
    {
        return col;
    }
    public int GetValueCaptured()
    {
        return valueCaptured;
    }
    public bool GetIsSetting()
    {
        if (settingPanel.activeInHierarchy || exitPanel.activeInHierarchy ||
            newGamePanel.activeInHierarchy || decidePanel.activeInHierarchy ||
            selectionPanel.activeInHierarchy || winPanel.activeInHierarchy ||
            countingPanel.activeInHierarchy || aiCoutingPanel.activeInHierarchy ||
            passTurnPanel.activeInHierarchy) return true;
        return false;
    }
    public void SetLastestIndex(int i,int j)
    {
        this.lastestIndexI = i;
        this.lastestIndexJ = j;
    }
    public void SetDefaultCountBack()
    {
        countBack = 0;
    }
    public void SetUnavailableFitstTurn()
    {
        firstTurn = false;
    }
    public void Back()
    {
        countBack++;
        if (countBack == 1 && !firstTurn)
        {
            NextTurn(); 
            if (listCapturedStones.Count !=0)
            {
                foreach(var elem in listCapturedStones)
                {
                    Restore(elem.Value.i, elem.Value.j, valueCaptured);
                }
            }
            matrix[lastestIndexI, lastestIndexJ].GetComponent<Stone>().SetDefault();
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (matrix[i, j].GetComponent<Stone>().GetValue() != -1)
                    {
                        matrix[i, j].GetComponent<Stone>().SetLiberties(matrix[i, j].GetComponent<Stone>().Liberties(i,j));
                    }
                }
            }
        }
    }

    public void InitiateVisited()
    {
        visited = new bool[row, col];
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                visited[i, j] = false;
            }
        }
    }
    public void InitiateSuportVisited()
    {
        supportVisited = new bool[row, col];
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                supportVisited[i, j] = false;
            }
        }
    }
    public void InitiateListCapturedStones()
    {
        listCapturedStones.Clear();
        sizeOfListCapturedStones = 0;
    }

    //public void InitiateSupportMatrix()
    //{
    //    supportVisited
    //}
    IEnumerator DisplayPassturnPanel()
    {
        passTurnPanel.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        passTurnPanel.SetActive(false);
        if (StaticData.playWithAI)
        {
            aiCoutingPanel.GetComponent<AiCounting>().SetHasPutStone(true);
            aiCoutingPanel.SetActive(true);
            aiCoutingPanel.GetComponent<AiCounting>().DisPlayPan();
        }
        if (countToEndGame == 2)
        {
            EndGame();
        }
    }

    public void dfs(int i, int j, int val)
    {
        //Debug.Log(i + "-" + j);
        if(val == 0)
        {
            numberBlackStones++;
        }
        if(val == 1)
        {
            numberWhiteStones++;
        }
        sum += matrix[i, j].GetComponent<Stone>().GetLiberties();
        visited[i, j] = true;
        if (!matrix[i, j].GetComponent<Stone>().IsSingleStone())
        {
            for (int k = 0; k < 4; k++)
            {
                int newI = i + dx[k];
                int newJ = j + dy[k];
                if (newI >= 0 && newI < row && newJ >= 0 && newJ < col &&
                     matrix[newI, newJ].GetComponent<Stone>().GetValue() == -1)
                {
                    for (int kk = 0; kk < 4; kk++)
                    {
                        int newII = newI + dx[kk];
                        int newJJ = newJ + dy[kk];
                        if (newII >= 0 && newII < row && newJJ >= 0 && newJJ < col && !visited[newII, newJJ] &&
                            newII != i && newJJ != j && matrix[newII, newJJ].GetComponent<Stone>().GetValue() == val)
                        {
                            InitiateSuportVisited();
                            if(isAnElementOfConnectedComponents(newII, newJJ, val, i, j))
                            {
                                sum--;
                                if(matrix[newI, newJ].GetComponent<Stone>().IsAnEye(val))
                                {
                                    countNumberOfEyesInside++;
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            isSingleStone = true;
        }
        for (int k = 0; k < 4; k++)
        {
            int newI = i + dx[k];
            int newJ = j + dy[k];
            if (newI >= 0 && newI < row && newJ >= 0 && newJ < col &&
                !visited[newI, newJ] && matrix[newI, newJ].GetComponent<Stone>().GetValue() == val)
            {
                dfs(newI, newJ, val);
            }
            if (newI >= 0 && newI < row && newJ >= 0 && newJ < col &&
                !visited[newI, newJ] && matrix[newI, newJ].GetComponent<Stone>().GetValue() == -1)
            {
                IndexOfIAndJ index = new IndexOfIAndJ();
                index.i = newI;
                index.j = newJ;
                lstLib.Add(index);
                if (matrix[newI, newJ].GetComponent<Stone>().IsAnEye(val))
                {
                    if (!listVisitedEyes.Contains(index))
                    {
                        listVisitedEyes.Add(index);
                        numberOfEyes++;
                    }
                }
                if (matrix[newI, newJ].GetComponent<Stone>().IsCommonTerritory())
                {
                    if (!listVisitedCommonTerritory.Contains(index))
                    {
                        listVisitedCommonTerritory.Add(index);
                        numberOfCommonTerritory++;
                    }
                }
            }
        }
        
    }
    public bool isAnElementOfConnectedComponents(int i, int j,int val,int targetI,int targetJ)
    {
        bool check = false;
        if (listConnectedComponents.Count != 0)
        {
            listConnectedComponents.Clear();
        }
        connectedComponents(i, j, val);
        foreach(var elem in listConnectedComponents)
        {
            if(elem.i == targetI && elem.j == targetJ)
            {
                return true;
            }
        }
        return check;
    }
    public void connectedComponents(int i,int j, int val)
    {
        //Debug.Log(i + "-" + j);
        supportVisited[i, j] = true;
        for (int k = 0; k < 4; k++)
        {
            int newI = i + dx[k];
            int newJ = j + dy[k];
            if (newI >= 0 && newI < row && newJ >= 0 && newJ < col &&
                !supportVisited[newI, newJ] && matrix[newI, newJ].GetComponent<Stone>().GetValue() == val)
            {
                IndexOfIAndJ index = new IndexOfIAndJ();
                index.i = newI;
                index.j = newJ;
                listConnectedComponents.Add(index);
                connectedComponents(newI, newJ, val);
            }
        }
    }
    public void search(int key)
    {
        InitiateVisited();
        if(sumOfLiberties.Count != 0)
        {
            sumOfLiberties.Clear();
        }
        if (listIndexOfIAndJ.Count != 0)
        {
            listIndexOfIAndJ.Clear();
        }
        if (listLifeStone.Count != 0)
        {
            listLifeStone.Clear();
        }
        if(listDeadStone.Count != 0)
        {
            listDeadStone.Clear();
        }
        if(listVisitedEyes.Count != 0)
        {
            listVisitedEyes.Clear();
        }
        if(listVisitedCommonTerritory.Count != 0)
        {
            listVisitedCommonTerritory.Clear();
        }
        if(listSingleStone.Count != 0)
        {
            listSingleStone.Clear();
        }
        if(listLiberties.Count != 0)
        {
            listLiberties.Clear();
        }
        if(lstLib.Count != 0)
        {
            lstLib.Clear();
        }
        int count = 0;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (matrix[i, j].GetComponent<Stone>().GetValue() == key && !visited[i, j])
                {
                    sum = 0;
                    numberOfEyes = 0;
                    numberOfCommonTerritory = 0;
                    countNumberOfEyesInside = 0;
                    listVisitedEyes.Clear();
                    listVisitedCommonTerritory.Clear();
                    isSingleStone = false;
                    dfs(i, j, key);
                    count++;
                    if(countNumberOfEyesInside >= 4)
                    {
                        sum += (int)(countNumberOfEyesInside / 4);
                    }
                    //Debug.Log(sum);
                    sumOfLiberties.Add(count, sum);
                    IndexOfIAndJ index = new IndexOfIAndJ();
                    index.i = i;
                    index.j = j;
                    listIndexOfIAndJ.Add(count, index);
                    listLiberties.Add(count, lstLib);
                    if (isSingleStone)
                    {
                        listSingleStone.Add(count, true);
                    }
                    if (sum == 2)
                    {
                        if ((int)(countNumberOfEyesInside / 4) >= 2)
                        {
                            listLifeStone.Add(count, index);
                            //Debug.Log("two eyes");
                        }
                        if (numberOfCommonTerritory == 2)
                        {
                            // && !IsSingleStone
                            listLifeStone.Add(count, index);
                            //Debug.Log("2 common");
                        }
                        if(numberOfEyes == 1 && numberOfCommonTerritory == 1)
                        {
                            listLifeStone.Add(count, index);
                            //Debug.Log("1 eye 1 common");
                        }
                        //if(numberOfEyes == 2)
                        //{
                        //    listLifeStone.Add(count, index);
                        //}
                        if(numberOfEyes == 1 && (int)(countNumberOfEyesInside / 4) == 1)
                        {
                            listLifeStone.Add(count, index);
                        }
                    }
                    if(sum == numberOfEyes && sum > 2)
                    {
                        listLifeStone.Add(count, index);
                    }
                    if(sum < 5 && !isSingleStone)
                    {
                        listDeadStone.Add(count, index);
                    }
                }
            }
        }
        foreach (var elem in sumOfLiberties)
        {
            //Debug.Log($"{elem.Key} - {elem.Value}");
            InitiateVisited();
            if(elem.Value == 0)
            {
                valueCaptured = key;
                sizeOfListCapturedStones++;
                IndexOfIAndJ index = new IndexOfIAndJ();
                index.i = listIndexOfIAndJ[elem.Key].i;
                index.j = listIndexOfIAndJ[elem.Key].j;
                listCapturedStones.Add(sizeOfListCapturedStones, index);
                Capture(listIndexOfIAndJ[elem.Key].i, listIndexOfIAndJ[elem.Key].j, key);
            }
        }
        //foreach (var elem in listIndexOfIAndJ)
        //{
        //    Debug.Log($"{elem.Key} - {elem.Value.i} + {elem.Value.j}");
        //}
        //foreach(var elem in listDeadStone)
        //{
        //    Debug.Log($"{elem.Key} - {elem.Value.i} + {elem.Value.j}");
        //}
    }
    public void Capture(int i, int j, int val)
    {
        if(val == 0)
        {
            blackPrisoners++;
        }
        if(val == 1)
        {
            whitePrisoners++;
        }
        matrix[i, j].GetComponent<Stone>().SetDefault();
        visited[i, j] = true;
        for (int k = 0; k < 4; k++)
        {
            int newI = i + dx[k];
            int newJ = j + dy[k];
            if (newI >= 0 && newI < row && newJ >= 0 && newJ < col &&
                !visited[newI, newJ] && matrix[newI, newJ].GetComponent<Stone>().GetValue() == val)
            {
                Capture(newI, newJ, val);
            }
        }
    }
    public void Restore(int i, int j, int val)
    {
        if(val == 0)
        {
            matrix[i, j].GetComponent<Stone>().SetBlackStone();
        }
        if(val == 1)
        {
            matrix[i, j].GetComponent<Stone>().SetWhiteStone();
        }
        for (int k = 0; k < 4; k++)
        {
            int newI = i + dx[k];
            int newJ = j + dy[k];
            if (newI >= 0 && newI < row && newJ >= 0 && newJ < col &&
                 matrix[newI, newJ].GetComponent<Stone>().GetValue() == -1)
            {
                Restore(newI, newJ, val);
            }
        }
    }
    public void CountTerritory(int i, int j, int val, int key)
    {
        //Debug.Log(i + "-" + j);
        //if (matrix[i, j].GetComponent<Stone>().IsAnEye(0))
        //{
        //    Debug.Log(i + "-" + j);
        //}
        visited[i, j] = true;
        for (int k = 0; k < 4; k++)
        {
            int newI = i + dx[k];
            int newJ = j + dy[k];
            if (newI >= 0 && newI < row && newJ >= 0 && newJ < col &&
                !visited[newI, newJ] && matrix[newI, newJ].GetComponent<Stone>().GetValue() == matrix[i, j].GetComponent<Stone>().GetValue()
                && matrix[newI, newJ].GetComponent<Stone>().GetValue()!= val)
            {
                CountTerritory(newI, newJ, val, key);
            }
            else if (newI >= 0 && newI < row && newJ >= 0 && newJ < col &&
                !visited[newI, newJ] && matrix[newI, newJ].GetComponent<Stone>().GetValue() == val)
            {
                if (matrix[newI, newJ].GetComponent<Stone>().GetValue() == val && key == 0 &&
                    !matrix[newI, newJ].GetComponent<Stone>().IsCommonTerritory())
                {
                    if (matrix[newI, newJ].GetComponent<Stone>().GetTerritoryValue() == 1)
                    {
                        matrix[newI, newJ].GetComponent<Stone>().SetDefault();
                        whiteTerritory++;
                    }
                    else
                    {
                        matrix[newI, newJ].GetComponent<Stone>().SetBlackTerritory();
                        blackTerritory++;
                    }
                }
                if (matrix[newI, newJ].GetComponent<Stone>().GetValue() == val && key == 1 &&
                    !matrix[newI, newJ].GetComponent<Stone>().IsCommonTerritory())
                {
                    if (matrix[newI, newJ].GetComponent<Stone>().GetTerritoryValue() == 0)
                    {
                        matrix[newI, newJ].GetComponent<Stone>().SetDefault();
                        blackTerritory--;
                    }
                    else
                    {
                        matrix[newI, newJ].GetComponent<Stone>().SetWhiteTerritory();
                        whiteTerritory++;
                    }
                }
                if (matrix[newI, newJ].GetComponent<Stone>().IsCommonTerritory())
                {
                    //SetDefaultTerritory(newI, newJ);
                }
                CountTerritory(newI, newJ, val, key);
            }
        }
    }
    public void CountTerritory()
    {
        if (blackTerritory != 0)
        {
            blackTerritory = 0;
        }
        if (whiteTerritory != 0)
        {
            whiteTerritory = 0;
        }
        search(0);
        foreach (var elem in listIndexOfIAndJ)
        {
            //Debug.Log($"{elem.Key} - {elem.Value.i} + {elem.Value.j}");
            CountTerritory(elem.Value.i, elem.Value.j, -1, 0);
        }
        search(1);
        foreach (var elem in listIndexOfIAndJ)
        {
            //Debug.Log($"{elem.Key} - {elem.Value.i} + {elem.Value.j}");
            CountTerritory(elem.Value.i, elem.Value.j, -1, 1);
        }
    }
    public void PutRand()
    {
        int middleR = Mathf.CeilToInt(row / 2);
        int leftR = Mathf.CeilToInt(middleR / 2);
        int rightR = Mathf.CeilToInt((middleR + row) / 2);
        int middleC = Mathf.CeilToInt(col / 2);
        int leftC = Mathf.CeilToInt(middleR / 2);
        int rightC = Mathf.CeilToInt((middleR + col) / 2);
        int posI = UnityEngine.Random.Range(leftC, rightC);
        int posJ = UnityEngine.Random.Range(leftR, rightR);
        matrix[posI, posJ].GetComponent<Stone>().PutStone();
        isAiFitsrTurn = false;
    }
    public KeyValuePair<int,int> smallestValue(IDictionary<int,int> dict)
    {
        dict = dict.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        return dict.First();
    }


    //public bool IsTerminal()
    //{
    //    // Kiểm tra xem trạng thái hiện tại có phải là trạng thái kết thúc hay không.
    //    // Trong trò chơi cờ vây, trạng thái kết thúc xảy ra khi cả hai bên đều cảm thấy không còn nước đi có lợi cho mình.

    //    // Kiểm tra xem có nước đi hợp lệ nào cho cả hai bên không
    //    bool blackCanMove = CanMove(0); // Bên đen
    //    bool whiteCanMove = CanMove(1); // Bên trắng

    //    return !blackCanMove && !whiteCanMove;
    //}
    public bool IsTerminal()
    {
        // Kiểm tra xem trạng thái hiện tại có phải là trạng thái kết thúc hay không.
        // Trong trò chơi cờ vây, trạng thái kết thúc xảy ra khi không còn ô trống nào trên bàn cờ.

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (matrix[i, j].GetComponent<Stone>().GetValue() == -1)
                {
                    if (player1)
                    {
                        matrix[i, j].GetComponent<Stone>().PutStone();
                        if(matrix[i, j].GetComponent<Stone>().GetValue() != -1)
                        {
                            Back();
                            return false; // Nếu còn ô trống, trạng thái chưa kết thúc
                        }
                    }
                }
            }
        }

        return true; // Nếu không còn ô trống, trạng thái đã kết thúc
    }

    public bool CanMove(int player)
    {
        // Kiểm tra xem người chơi có thể thực hiện nước đi hợp lệ không
        Debug.Log(row + "-" + col);
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (matrix[i, j].GetComponent<Stone>().GetValue() == -1)
                {
                    // Kiểm tra xem có thể thực hiện nước đi hợp lệ tại ô trống này không
                    if (IsValidMove(i, j, player))
                    {
                        Debug.Log("valid");
                        return true;
                    }
                }
            }
        }

        return false; // Không có nước đi hợp lệ nào cho người chơi
    }

    public bool IsValidMove(int row, int col, int player)
    {
        // Kiểm tra xem nước đi từ người chơi tại ô này có hợp lệ không
        // (có thể cần thêm logic dựa trên quy tắc cụ thể của cờ vây)
        // Đây chỉ là một ví dụ đơn giản, bạn cần điều chỉnh theo luật chơi cụ thể của mình.
        //if (player == 1)
        //{
        //    matrix[row, col].GetComponent<Stone>().SetWhiteStone();
        //    search(1);
        //    matrix[row, col].GetComponent<Stone>().UpdateLiberties();
        //    search(0);
        //}
        //if (player == 0)
        //{
        //    matrix[row, col].GetComponent<Stone>().SetBlackStone();
        //    search(0);
        //    matrix[row, col].GetComponent<Stone>().UpdateLiberties();
        //    search(1);
        //}
        //if (matrix[row, col].GetComponent<Stone>().GetValue() == -1)
        //{
        //    Back();
        //    return false;
        //}
        //else
        //{
        //    Back();
        //    //matrix[row, col].GetComponent<Stone>().SetDefault();
        //}
        return true;
    }
    public int Evaluate()
    {
        // Hàm đánh giá giá trị của trạng thái hiện tại.
        // Trong trò chơi cờ vây, một phương pháp đơn giản có thể là đếm số lượng ô đã chiếm được bởi mỗi bên.
    
        int score = 10;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                //if (!player1)
                //{
                //    matrix[row, col].GetComponent<Stone>().PutStone();
                //    search(1);
                //    matrix[row, col].GetComponent<Stone>().UpdateLiberties();
                //    search(0);
                //}
                //if (player1)
                //{
                //    matrix[row, col].GetComponent<Stone>().PutStone();
                //    search(0);
                //    matrix[row, col].GetComponent<Stone>().UpdateLiberties();
                //    search(1);
                //}
                //if (matrix[row, col].GetComponent<Stone>().GetValue() == -1)
                //{
                //    Back();
                //}
                //else
                //{
                //    Back();
                //    if (!player1)
                //    {
                //        search(1);
                //        KeyValuePair<int,int> valuePairWSumOfLiberties = smallestValue(sumOfLiberties);
                //        search(0);
                //        KeyValuePair<int, int> valuePairB = smallestValue(sumOfLiberties);
                //    }
                //    if (player1)
                //    {
                //        search(0);
                //        search(1);
                //    }
                //}
            }
        }

        return score;
    }
    public IDictionary<int[,],int> GenerateMoves(int player)
    {
        // Hàm này sinh ra tất cả các bước đi hợp lệ từ trạng thái hiện tại của trò chơi.
        // Trong trò chơi cờ vây, các bước đi có thể là đặt đá vào ô trống.

        IDictionary<int[,],int> moves = new Dictionary<int[,],int>();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                int score = 5;
                if (player == 1)
                {
                    score = -5;
                }
                if (matrix[i, j].GetComponent<Stone>().GetValue() == -1)
                {
                    if (player == 0)
                    {
                        matrix[i, j].GetComponent<Stone>().PutStone();
                        if (matrix[i, j].GetComponent<Stone>().GetValue() != -1)
                        {
                            Back();
                            InitiateListCapturedStones();
                            int[,] newBoard = Clone(); // Tạo một bản sao của ma trận
                            newBoard[i, j] = 0; // Đặt đá của người chơi 1 (đen) vào ô trống
                            if (matrix[i, j].GetComponent<Stone>().HasNextTo(0))
                            {
                                score = 8;
                            }
                            search(0);
                            IDictionary<int, int> keyValuePairB = new Dictionary<int, int>();
                            if (sumOfLiberties.Count != 0)
                            {
                                keyValuePairB.Add(smallestValue(sumOfLiberties).Key,smallestValue(sumOfLiberties).Value);
                            }
                            search(1);
                            IDictionary<int, int> keyValuePairW = new Dictionary<int, int>();
                            if (sumOfLiberties.Count != 0)
                            {
                                keyValuePairW.Add(smallestValue(sumOfLiberties).Key, smallestValue(sumOfLiberties).Value);
                            }
                            if(keyValuePairB.Count!=0 && keyValuePairW.Count!=0 && keyValuePairB.First().Value < keyValuePairW.First().Value)
                            {
                                search(0);
                                if (!listSingleStone.ContainsKey(keyValuePairB.First().Key))
                                {
                                    IndexOfIAndJ index = new IndexOfIAndJ();
                                    index.i = i;
                                    index.j = j;
                                    if (listLiberties.ContainsKey(keyValuePairB.First().Key))
                                    {
                                        if (listLiberties[keyValuePairB.First().Key].Contains(index) && matrix[i, j].GetComponent<Stone>().HasNextTo(0))
                                        {
                                            score = 30;
                                        }
                                    }
                                }
                            }
                            else if(keyValuePairB.Count != 0 && keyValuePairW.Count != 0 && keyValuePairB.First().Value > keyValuePairW.First().Value)
                            {
                                search(1);
                                if (!listSingleStone.ContainsKey(keyValuePairB.First().Key))
                                {
                                    IndexOfIAndJ index = new IndexOfIAndJ();
                                    index.i = i;
                                    index.j = j;
                                    if (listLiberties.ContainsKey(keyValuePairB.First().Key))
                                    {
                                        if (listLiberties[keyValuePairB.First().Key].Contains(index) && matrix[i, j].GetComponent<Stone>().HasNextTo(1))
                                        {
                                            score = 20;
                                        }
                                    }
                                }
                            }else if ((keyValuePairB.Count == 0 && keyValuePairW.Count != 0) || ((keyValuePairB.Count != 0 && keyValuePairW.Count != 0) && (keyValuePairW.First().Value == keyValuePairB.First().Value)))
                            {
                                search(1);
                                IndexOfIAndJ index = new IndexOfIAndJ();
                                index.i = i;
                                index.j = j;
                                if (listLiberties.ContainsKey(keyValuePairW.First().Key))
                                {
                                    if (listLiberties[keyValuePairW.First().Key].Contains(index))
                                    {
                                        score = 10;
                                    }
                                }
                            }
                            moves.Add(newBoard,score);
                        }
                    }
                    else if(player == 1)
                    {
                        matrix[i, j].GetComponent<Stone>().PutStone();
                        if (matrix[i, j].GetComponent<Stone>().GetValue() != -1)
                        {
                            SetLastestIndex(i, j);
                            Back();
                            InitiateListCapturedStones();
                            int[,] newBoard = Clone(); // Tạo một bản sao của ma trận
                            newBoard[i, j] = 1; // Đặt đá của người chơi 2 (trắng) vào ô trống
                            if (matrix[i, j].GetComponent<Stone>().HasNextTo(1))
                            {
                                score = -8;
                            }
                            //Debug.Log("visited : " + i + "-" + j);
                            search(1);
                            IDictionary<int, int> keyValuePairW = new Dictionary<int, int>();
                            if (sumOfLiberties.Count != 0)
                            {
                                keyValuePairW.Add(smallestValue(sumOfLiberties).Key, smallestValue(sumOfLiberties).Value);
                            }
                            search(0);
                            IDictionary<int, int> keyValuePairB = new Dictionary<int, int>();
                            if (sumOfLiberties.Count != 0)
                            {
                                keyValuePairB.Add(smallestValue(sumOfLiberties).Key, smallestValue(sumOfLiberties).Value);
                            }
                            if (keyValuePairW.Count != 0 && keyValuePairB.Count != 0 && keyValuePairW.First().Value < keyValuePairB.First().Value)
                            {
                                search(1);
                                if (!listSingleStone.ContainsKey(keyValuePairW.First().Key))
                                {
                                    IndexOfIAndJ index = new IndexOfIAndJ();
                                    index.i = i;
                                    index.j = j;
                                    if (listLiberties.ContainsKey(keyValuePairB.First().Key))
                                    {
                                        if (listLiberties[keyValuePairW.First().Key].Contains(index) && matrix[i, j].GetComponent<Stone>().HasNextTo(1))
                                        {
                                            score = -30;
                                        }
                                    }
                                }
                            }
                            else if (keyValuePairW.Count != 0 && keyValuePairB.Count != 0 && keyValuePairW.First().Value > keyValuePairB.First().Value)
                            {
                                search(0);
                                if (!listSingleStone.ContainsKey(keyValuePairW.First().Key))
                                {
                                    IndexOfIAndJ index = new IndexOfIAndJ();
                                    index.i = i;
                                    index.j = j;
                                    if (listLiberties.ContainsKey(keyValuePairW.First().Key))
                                    {
                                        if (listLiberties[keyValuePairW.First().Key].Contains(index) && matrix[i, j].GetComponent<Stone>().HasNextTo(0))
                                        {
                                            score = -20;
                                        }
                                    }
                                }
                            }
                            else if ((keyValuePairW.Count == 0 && keyValuePairB.Count != 0) || ((keyValuePairW.Count != 0 && keyValuePairB.Count != 0) && (keyValuePairW.First().Value ==keyValuePairB.First().Value)))
                            {
                                search(0);
                                IndexOfIAndJ index = new IndexOfIAndJ();
                                index.i = i;
                                index.j = j;
                                if (listLiberties.ContainsKey(keyValuePairB.First().Key))
                                {
                                    if (listLiberties[keyValuePairB.First().Key].Contains(index))
                                    {
                                        score = -10;
                                    }
                                }
                            }
                            moves.Add(newBoard, score);
                        }
                    }
                }
            }
        }
        //int k = 1;
        //foreach (var elem in moves)
        //{
        //    Debug.Log(elem.Value);
        //    for (int i = 0; i < row; i++)
        //    {
        //        string str = "";
        //        for (int j = 0; j < col; j++)
        //        {
        //            str += elem.Key[i, j] + "~";
        //        }
        //        Debug.Log(str);
        //    }
        //    Debug.Log(k + "------------------------");
        //    k++;
        //}
        return moves;
    }
    public int GetRow(int[,] board)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (player1)
                {
                    if (board[i, j] == 0)
                    {
                        return i;
                    }
                }
                else
                {
                    if (board[i, j] == 1)
                    {
                        return i;
                    }
                }

            }
        }

        return -1; // Không tìm thấy quân cờ
    }

    public int GetColumn(int[,] board)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (player1)
                {
                    if (board[i, j] == 0)
                    {
                        return j;
                    }
                }
                else
                {
                    if (board[i, j] == 1)
                    {
                        return j;
                    }
                }
            }
        }

        return -1; // Không tìm thấy quân cờ
    }

    public MoveResult AlphaBeta(KeyValuePair<int[,],int> InitBoard, int depth)
    {
        int alpha = int.MinValue;
        int beta = int.MaxValue;
        MoveResult bestMove = MaxValue(InitBoard, alpha, beta, depth);

        return bestMove;
    }

    public MoveResult MaxValue(KeyValuePair<int[,], int> boardAndEvaluate, int alpha, int beta, int currentDepth)
    {
        int v = int.MinValue;
        MoveResult bestMove = new MoveResult();

        if (IsGameOver(boardAndEvaluate.Key) || currentDepth == 0)
        {
            bestMove.Value = boardAndEvaluate.Value;
            return bestMove;
        }
        int val = 1;
        if (player1)
        {
            val = 0;
        }
        foreach (var child in GenerateMoves(val)) // Assume 1 is for black
        {
            MoveResult childResult = MinValue(child, alpha, beta, currentDepth - 1);

            if (childResult.Value > v)
            {
                v = childResult.Value;
                bestMove = new MoveResult { Value = v, Row = GetRow(child.Key), Column = GetColumn(child.Key) };
            }

            alpha = Math.Max(alpha, v);

            if (alpha >= beta)
                break;
        }

        return bestMove;
    }

    public MoveResult MinValue(KeyValuePair<int[,], int> boardAndEvaluate, int alpha, int beta, int currentDepth)
    {
        int v = int.MaxValue;
        MoveResult bestMove = new MoveResult();

        if (IsGameOver(boardAndEvaluate.Key) || currentDepth == 0)
        {
            bestMove.Value = boardAndEvaluate.Value;
            return bestMove;
        }
        int val = 0;
        if (player1)
        {
            val = 1;
        }
        foreach (var child in GenerateMoves(val)) // Assume 2 is for white
        {
            MoveResult childResult = MaxValue(child, alpha, beta, currentDepth - 1);

            if (childResult.Value < v)
            {
                v = childResult.Value;
                bestMove = new MoveResult { Value = v, Row = GetRow(child.Key), Column = GetColumn(child.Key) };
            }

            beta = Math.Min(beta, v);

            if (alpha >= beta)
                break;
        }

        return bestMove;
    }

    public bool IsGameOver(int[,] board)
    {
        IDictionary<int[,],int> movesForBlack = GenerateMoves(0);
        IDictionary<int[,],int> movesForWhite = GenerateMoves(1);

        return movesForBlack.Count == 0 && movesForWhite.Count == 0;
    }
    public MoveResult AlphaBetaWithoutRecursive(KeyValuePair<int[,],int> boardAndEvaluate, int depth)
    {
        int alpha = int.MinValue;
        int beta = int.MaxValue;

        Stack<(int[,], int, int, int, int, int)> stack = new Stack<(int[,] ,int , int, int, int, int)>();
        stack.Push((boardAndEvaluate.Key, boardAndEvaluate.Value, alpha, beta, depth, 1)); // Assume 1 is for black

        MoveResult bestMove = new MoveResult();

        while (stack.Count > 0)
        {
            var (currentBoard, currentEvaluate, currentAlpha, currentBeta, currentDepth, currentPlayer) = stack.Pop();

            if (IsGameOver(currentBoard) || currentDepth == 0)
            {
                int evalValue = currentEvaluate;
                Debug.Log(currentEvaluate);
                if (!player1 && evalValue > bestMove.Value)
                {
                    bestMove = new MoveResult { Value = evalValue, Row = GetRow(currentBoard), Column = GetColumn(currentBoard) };
                }
                else if (player1 && evalValue < bestMove.Value)
                {
                    bestMove = new MoveResult { Value = evalValue, Row = GetRow(currentBoard), Column = GetColumn(currentBoard) };
                }
            }
            else
            {
                IDictionary<int[,],int> moves;
                if (player1)
                {
                    moves = GenerateMoves(1);
                }
                else
                {
                    moves = GenerateMoves(0);
                }
                foreach (var move in moves)
                {
                    int newAlpha = currentAlpha;
                    int newBeta = currentBeta;

                    if (!player1)
                    {
                        MoveResult childResult = new MoveResult { Value = int.MinValue };
                        stack.Push((move.Key,move.Value, newAlpha, newBeta, currentDepth - 1, 2));

                        while (stack.Count > 0)
                        {
                            var (_,_, childAlpha, _, _, _) = stack.Pop();
                            if (childAlpha > childResult.Value)
                            {
                                childResult.Value = childAlpha;
                            }
                        }

                        newAlpha = Math.Max(newAlpha, childResult.Value);
                    }
                    else
                    {
                        MoveResult childResult = new MoveResult { Value = int.MaxValue };
                        stack.Push((move.Key, move.Value, newAlpha, newBeta, currentDepth - 1, 1));

                        while (stack.Count > 0)
                        {
                            var (_, _, childBeta, _, _, _) = stack.Pop();
                            if (childBeta < childResult.Value)
                            {
                                childResult.Value = childBeta;
                            }
                        }

                        newBeta = Math.Min(newBeta, childResult.Value);
                    }

                    if (newAlpha >= newBeta)
                        break;
                }
            }
        }

        return bestMove;
    }
    public MoveResult AlphaBetaWithoutRecursiveFinal(IDictionary<int[,],int> boardAndEvaluate)
    {
        int alpha = int.MinValue;
        int beta = int.MaxValue;
        MoveResult bestMove = new MoveResult();
        // White turn
        if (!player1)
        {
            foreach(var elem in boardAndEvaluate)
            {
                int val;
                if(elem.Value >= alpha)
                {
                    alpha = elem.Value;
                    val = alpha;
                }
                else
                {
                    val = Math.Min(elem.Value, alpha);
                    if (val <= alpha) alpha = val;
                }
                if(bestMove.Value > val)
                {
                    bestMove.Value = val;
                    //bestMove.Row = GetRow(elem.Key);
                    //bestMove.Column = GetColumn(elem.Key);
                    for (int i = 0; i < row; i++)
                    {
                        for (int j = 0; j < col; j++)
                        {
                            if (elem.Key[i, j] == 1) 
                            {
                                bestMove.Row = i;
                                bestMove.Column = j;
                            }
                        }
                    }
                    //Debug.Log(GetRow(elem.Key) + "-" + GetColumn(elem.Key));
                }
            }
            //Debug.Log("White turn");
        }
        // Black turn
        else
        {
            foreach (var elem in boardAndEvaluate)
            {
                int val;
                if (elem.Value >= beta)
                {
                    beta = elem.Value;
                    val = beta;
                }
                else
                {
                    val = Math.Max(elem.Value, alpha);
                    if (val >= alpha) alpha = val;
                }
                if (bestMove.Value < val)
                {
                    bestMove.Value = val;
                    for (int i = 0; i < row; i++)
                    {
                        for (int j = 0; j < col; j++)
                        {
                            if (elem.Key[i, j] == 0)
                            {
                                bestMove.Row = i;
                                bestMove.Column = j;
                            }
                        }
                    }
                }
            }
            //Debug.Log("Black turn");
        }
        //Debug.Log(bestMove.Value);
        return bestMove;
    }
    public IndexOfIAndJ IndexToPutStone()
    {
        IDictionary<int[,], int> InitBoard;
        if (player1)
        {
            InitBoard = GenerateMoves(0);
        }
        else
        {
            InitBoard = GenerateMoves(1);
        }
        //int[,] mtr = Clone();
        //InitBoard = new Dictionary<int[,],int>();
        //InitBoard.Add(mtr, 0);
        MoveResult moveResult = AlphaBetaWithoutRecursiveFinal(InitBoard);
        IndexOfIAndJ index = new IndexOfIAndJ();
        index.i = moveResult.Row;
        index.j = moveResult.Column;
        return index;
    }
    public void DebugAlphaBetaValue()
    {
        IDictionary<int[,], int> InitBoard;
        if (player1)
        {
            InitBoard = GenerateMoves(0);
        }
        else
        {
            InitBoard = GenerateMoves(1);
        }
        //int[,] mtr = Clone();
        //InitBoard = new Dictionary<int[,],int>();
        //InitBoard.Add(mtr, 0);
        MoveResult moveResult = AlphaBetaWithoutRecursiveFinal(InitBoard);
        Debug.Log(moveResult.Row + "-" + moveResult.Column);

    }
    public void TerritoryScoring()
    {
        CountTerritory();
        //////Debug.Log(blackTerritory + "-" + whiteTerritory);
        blackScore = blackTerritory - blackPrisoners - blackDeadStones;
        whiteScore = whiteTerritory - whitePrisoners - whiteDeadStones + komi;
        if (whiteScore - blackScore > 0)
        {
            //Debug.Log("White win : " + (whiteScore - blackScore).ToString());
            winStone.sprite = whiteStone;
            txtScore.text = "SCORE : " + (whiteScore - blackScore).ToString();
        }
        else
        {
            Debug.Log("Black win : " + (blackScore - whiteScore).ToString());
            winStone.sprite = blackStone;
            txtScore.text = "SCORE : " + (blackScore - whiteScore).ToString();
        }
        winPanel.SetActive(true);
        //SearchDeadStones();
    }
    public void AreaScoring()
    {
        if (numberBlackStones != 0)
        {
            numberBlackStones = 0;
        }
        if (numberWhiteStones != 0)
        {
            numberWhiteStones = 0;
        }
        search(0);
        search(1);
        //Debug.Log(numberBlackStones + "-" + numberWhiteStones);
        Debug.Log(blackPrisoners + "-" + whitePrisoners);
        blackScore = blackTerritory + numberBlackStones;
        whiteScore = whiteTerritory + numberWhiteStones + komi;
    }
    public void EndGame()
    {
        if (japanRules)
        {
            TerritoryScoring();
        }
        if (chinaRules)
        {
            AreaScoring();
        }
    }
}

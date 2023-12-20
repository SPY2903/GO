using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    [SerializeField] private Sprite blackSprite;
    [SerializeField] private Sprite whiteSprite;
    [SerializeField] private Sprite blackTerritory;
    [SerializeField] private Sprite whiteTerritory;
    private SpriteRenderer sprite;
    private GameControl gameControl;
    private int indexI;
    private int indexJ;
    [SerializeField] private int value = -1;
    private int liberties = 4;
    private int numberOfStonesArround = 0;
    private AudioSource putStoneSound;
    private int territoryValue = -1;
    private bool canPut = false;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        gameControl = GameObject.FindGameObjectWithTag("ChessBoard").GetComponent<GameControl>();
        putStoneSound = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>().GetPutStoneSoundAudioSource();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        if (!gameControl.GetIsSetting())
        {
            PutStone();
            putStoneSound.Play();
            if (canPut && StaticData.playWithAI)
            {
                gameControl.GetAiCountingPanel().GetComponent<AiCounting>().SetHasPutStone(true);
                gameControl.GetAiCountingPanel().SetActive(true);
                gameControl.GetAiCountingPanel().GetComponent<AiCounting>().DisPlayPan();
            }
        }
    }
    public void PutStone()
    {
        if (sprite.sprite == null && !gameControl.GetIsSetting())
        {
            gameControl.SetUnavailableFitstTurn();
            gameControl.SetDefaultCountToEndGame();
            gameControl.SetDefaultCountBack();
            //gameControl.InitiateVisited();
            gameControl.SetLastestIndex(indexI, indexJ);

            int searchVal = -1;
            gameControl.NextTurn();
            //gameControl.SetPlayer(!gameControl.GetPlayer());
            if (gameControl.GetPlayer())
            {
                sprite.sprite = whiteSprite;
                value = 1;
                searchVal = 0;
            }
            else
            {
                sprite.sprite = blackSprite;
                value = 0;
                searchVal = 1;
            }
            if (gameControl.GetValueCaptured() == value)
            {
                gameControl.InitiateListCapturedStones();
            }
            UpdateLiberties();
            if (searchVal == 1)
            {
                gameControl.search(1);
                UpdateLiberties();
                gameControl.search(0);
            }
            if (searchVal == 0)
            {
                gameControl.search(0);
                UpdateLiberties();
                gameControl.search(1);
            }
            if (value == -1)
            {
                gameControl.Back();
                canPut = false;
            }
            else
            {
                canPut = true;
            }
            //else
            //{
            //}
        }
    }
    public int GetValue()
    {
        return value;
    }
    public void SetValue(int value)
    {
        this.value = value;
    }
    public void SetIndexI(int i)
    {
        this.indexI = i;
    }
    public int GetIndexI()
    {
        return indexI;
    }
    public void SetIndexJ(int j)
    {
        this.indexJ = j;
    }
    public int GetIndexJ()
    {
        return indexJ;
    }
    public void SetLiberties(int liberties)
    {
        this.liberties = liberties;
    }
    public int GetLiberties()
    {
        return liberties;
    }
    public void UpdateLiberties()
    {
        for (int i = 0; i < gameControl.GetRow(); i++)
        {
            for (int j = 0; j < gameControl.GetCol(); j++)
            {
                if (gameControl.GetMatrix()[i, j].GetComponent<Stone>().GetValue() != -1)
                {
                    gameControl.GetMatrix()[i, j].GetComponent<Stone>().SetLiberties(Liberties(i, j));
                }
            }
        }
    }
    public int Liberties(int i, int j)
    {
        int liberties = 0;
        int left = j - 1;
        int right = j + 1;
        int top = i - 1;
        int bottom = i + 1;
        if (left >= 0)
        {
            if (gameControl.GetMatrix()[i, left].GetComponent<Stone>().GetValue() == -1)
            {
                liberties++;
            }
        }
        if (right < gameControl.GetCol())
        {
            if (gameControl.GetMatrix()[i, right].GetComponent<Stone>().GetValue() == -1)
            {
                liberties++;
            }
        }
        if (top >= 0)
        {
            if (gameControl.GetMatrix()[top, j].GetComponent<Stone>().GetValue() == -1)
            {
                liberties++;
            }
        }
        if (bottom < gameControl.GetRow())
        {
            if (gameControl.GetMatrix()[bottom, j].GetComponent<Stone>().GetValue() == -1)
            {
                liberties++;
            }
        }
        return liberties;
    }
    public bool CanPutStones(int val)
    {
        bool canPutStones = true;
        gameControl.search(val);
        return canPutStones;
    }
    public bool IsSingleStone()
    {
        bool isSingleStone = true;
        int left = indexJ - 1;
        int right = indexJ + 1;
        int top = indexI - 1;
        int bottom = indexI + 1;
        if (left >= 0)
        {
            if (gameControl.GetMatrix()[indexI, left].GetComponent<Stone>().GetValue() == value)
            {
                isSingleStone = false;
            }
        }
        if (right < gameControl.GetCol())
        {
            if (gameControl.GetMatrix()[indexI, right].GetComponent<Stone>().GetValue() == value)
            {
                isSingleStone = false;
            }
        }
        if (top >= 0)
        {
            if (gameControl.GetMatrix()[top, indexJ].GetComponent<Stone>().GetValue() == value)
            {
                isSingleStone = false;
            }
        }
        if (bottom < gameControl.GetRow())
        {
            if (gameControl.GetMatrix()[bottom, indexJ].GetComponent<Stone>().GetValue() == value)
            {
                isSingleStone = false;
            }
        }
        return isSingleStone;
    }
    public bool IsAnEye(int val)
    {
        bool isAnEye = false;
        numberOfStonesArround = 0;
        int left = indexJ - 1;
        int right = indexJ + 1;
        int top = indexI - 1;
        int bottom = indexI + 1;
        if (left >= 0)
        {
            if (gameControl.GetMatrix()[indexI, left].GetComponent<Stone>().GetValue() == val && value == -1)
            {
                numberOfStonesArround++;
            }
        }
        if (right < gameControl.GetCol())
        {
            if (gameControl.GetMatrix()[indexI, right].GetComponent<Stone>().GetValue() == val && value == -1)
            {
                numberOfStonesArround++;
            }
        }
        if (top >= 0)
        {
            if (gameControl.GetMatrix()[top, indexJ].GetComponent<Stone>().GetValue() == val && value == -1)
            {
                numberOfStonesArround++;
            }
        }
        if (bottom < gameControl.GetRow())
        {
            if (gameControl.GetMatrix()[bottom, indexJ].GetComponent<Stone>().GetValue() == val && value == -1)
            {
                numberOfStonesArround++;
            }
        }
        if((indexI == 0 || indexI == gameControl.GetRow() - 1) && (indexJ == 0 || indexJ == gameControl.GetCol() - 1) && numberOfStonesArround == 2)
        {
            isAnEye = true;
        }
        if((indexI == 0 || indexJ == 0 || indexI == gameControl.GetRow() - 1 || indexJ == gameControl.GetCol() - 1) && numberOfStonesArround == 3)
        {
            isAnEye = true;
        }
        else
        {
            if (numberOfStonesArround == 4) isAnEye = true;
        }
        return isAnEye;
    }
    public bool IsCommonTerritory()
    {
        bool isCommonTerritory = false, hasBlackStone = false, hasWhiteStone = false;
        int left = indexJ - 1;
        int right = indexJ + 1;
        int top = indexI - 1;
        int bottom = indexI + 1;
        if (left >= 0)
        {
            if ((gameControl.GetMatrix()[indexI, left].GetComponent<Stone>().GetValue() == 0 ||
                gameControl.GetMatrix()[indexI, left].GetComponent<Stone>().GetValue() == 1) 
                && value == -1)
            {
                if (gameControl.GetMatrix()[indexI, left].GetComponent<Stone>().GetValue() == 0) {
                    hasBlackStone = true;
                }
                if(gameControl.GetMatrix()[indexI, left].GetComponent<Stone>().GetValue() == 1)
                {
                    hasWhiteStone = true;
                }
            }
        }
        if (right < gameControl.GetCol())
        {
            if ((gameControl.GetMatrix()[indexI, right].GetComponent<Stone>().GetValue() == 0 ||
                gameControl.GetMatrix()[indexI, right].GetComponent<Stone>().GetValue() == 1) && value == -1)
            {
                if (gameControl.GetMatrix()[indexI, right].GetComponent<Stone>().GetValue() == 0)
                {
                    hasBlackStone = true;
                }
                if (gameControl.GetMatrix()[indexI, right].GetComponent<Stone>().GetValue() == 1)
                {
                    hasWhiteStone = true;
                }
            }
        }
        if (top >= 0)
        {
            if ((gameControl.GetMatrix()[top, indexJ].GetComponent<Stone>().GetValue() == 0 ||
                gameControl.GetMatrix()[top, indexJ].GetComponent<Stone>().GetValue() == 1) && value == -1)
            {
                if (gameControl.GetMatrix()[top, indexJ].GetComponent<Stone>().GetValue() == 0)
                {
                    hasBlackStone = true;
                }
                if (gameControl.GetMatrix()[top, indexJ].GetComponent<Stone>().GetValue() == 1)
                {
                    hasWhiteStone = true;
                }
            }
        }
        if (bottom < gameControl.GetRow())
        {
            if ((gameControl.GetMatrix()[bottom, indexJ].GetComponent<Stone>().GetValue() == 0 ||
                gameControl.GetMatrix()[bottom, indexJ].GetComponent<Stone>().GetValue() == 1) && value == -1)
            {
                if (gameControl.GetMatrix()[bottom, indexJ].GetComponent<Stone>().GetValue() == 0)
                {
                    hasBlackStone = true;
                }
                if (gameControl.GetMatrix()[bottom, indexJ].GetComponent<Stone>().GetValue() == 1)
                {
                    hasWhiteStone = true;
                }
            }
        }
        if (hasBlackStone && hasWhiteStone) isCommonTerritory = true;
        return isCommonTerritory;
    }
    public bool HasNextTo(int val)
    {
        int left = indexJ - 1;
        int right = indexJ + 1;
        int top = indexI - 1;
        int bottom = indexI + 1;
        if (left >= 0)
        {
            if (gameControl.GetMatrix()[indexI, left].GetComponent<Stone>().GetValue() == val && value == -1)
            {
                return true;
            }
        }
        if (right < gameControl.GetCol())
        {
            if (gameControl.GetMatrix()[indexI, right].GetComponent<Stone>().GetValue() == val && value == -1)
            {
                return true;
            }
        }
        if (top >= 0)
        {
            if (gameControl.GetMatrix()[top, indexJ].GetComponent<Stone>().GetValue() == val && value == -1)
            {
                return true;
            }
        }
        if (bottom < gameControl.GetRow())
        {
            if (gameControl.GetMatrix()[bottom, indexJ].GetComponent<Stone>().GetValue() == val && value == -1)
            {
                return true;
            }
        }
        return false;
    }
    public bool HasNextToCommonTerritory()
    {
        int left = indexJ - 1;
        int right = indexJ + 1;
        int top = indexI - 1;
        int bottom = indexI + 1;
        if (left >= 0)
        {
            if (gameControl.GetMatrix()[indexI, left].GetComponent<Stone>().IsCommonTerritory())
            {
                return true;
            }
        }
        if (right < gameControl.GetCol())
        {
            if (gameControl.GetMatrix()[indexI, right].GetComponent<Stone>().IsCommonTerritory())
            {
                return true;
            }
        }
        if (top >= 0)
        {
            if (gameControl.GetMatrix()[top, indexJ].GetComponent<Stone>().IsCommonTerritory())
            {
                return true;
            }
        }
        if (bottom < gameControl.GetRow())
        {
            if (gameControl.GetMatrix()[bottom, indexJ].GetComponent<Stone>().IsCommonTerritory())
            {
                return true;
            }
        }
        return false;
    }
    public void SetDefault()
    {
        sprite.sprite = null;
        liberties = 4;
        value = -1;
    }
    public void SetBlackStone()
    {
        sprite.sprite = blackSprite;
        value = 0;
    }
    public void SetWhiteStone()
    {
        sprite.sprite = whiteSprite;
        value = 1;
    }
    public void SetBlackTerritory()
    {
        sprite.sprite = blackTerritory;
        territoryValue = 0;
    }
    public void SetWhiteTerritory()
    {
        sprite.sprite = whiteTerritory;
        territoryValue = 1;
    }
    public int GetNumberOfStonesArround()
    {
        return numberOfStonesArround;
    }
    public int GetTerritoryValue()
    {
        return territoryValue;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaBeta : MonoBehaviour
{

    public class MoveResult
    {
        public int Value { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }

    public static int[,] board = {
        {1, 2, 1, 2, 1},
        {2, 1, 2, 1, 2},
        {1, 2, 1, 2, 1},
        {2, 1, 2, 1, 2},
        {1, 2, 1, 2, 0} // 0 đại diện cho ô trống
        };

    public static int depth = 3; // Độ sâu tối đa của thuật toán Alpha-Beta

    public static void PMain()
    {
        // Kiểm tra hàm AlphaBeta()
        MoveResult result = AlphaBetaa();
        Debug.Log("Best Value: " + result.Value);
        Debug.Log("Best Move: Row = " + result.Row + ", Column = " + result.Column);
    }

    public static MoveResult AlphaBetaa()
    {
        int alpha = int.MinValue;
        int beta = int.MaxValue;

        MoveResult bestMove = MaxValue(alpha, beta, depth);

        return bestMove;
    }

    static MoveResult MaxValue(int alpha, int beta, int currentDepth)
    {
        int v = int.MinValue;
        MoveResult bestMove = new MoveResult();

        if (IsTerminal() || currentDepth == 0)
        {
            bestMove.Value = Evaluate();
            return bestMove;
        }

        foreach (var child in GenerateMoves())
        {
            MoveResult childResult = MinValue(alpha, beta, currentDepth - 1);

            if (childResult.Value > v)
            {
                v = childResult.Value;
                bestMove = new MoveResult { Value = v, Row = GetRow(child), Column = GetColumn(child) };
            }

            alpha = Math.Max(alpha, v);

            if (alpha >= beta)
                break;
        }

        return bestMove;
    }

    static MoveResult MinValue(int alpha, int beta, int currentDepth)
    {
        int v = int.MaxValue;
        MoveResult bestMove = new MoveResult();

        if (IsTerminal() || currentDepth == 0)
        {
            bestMove.Value = Evaluate();
            return bestMove;
        }

        foreach (var child in GenerateMoves())
        {
            MoveResult childResult = MaxValue(alpha, beta, currentDepth - 1);

            if (childResult.Value < v)
            {
                v = childResult.Value;
                bestMove = new MoveResult { Value = v, Row = GetRow(child), Column = GetColumn(child) };
            }

            beta = Math.Min(beta, v);

            if (alpha >= beta)
                break;
        }

        return bestMove;
    }

    static void PrintBoard(int[,] b)
    {
        int rows = b.GetLength(0);
        int cols = b.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Debug.Log(b[i, j] + " ");
            }
            Debug.Log("");
        }
        Debug.Log("");
    }

    static bool IsTerminal()
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (board[i, j] == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    static int Evaluate()
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        int blackScore = 0;
        int whiteScore = 0;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (board[i, j] == 1)
                {
                    blackScore++;
                }
                else if (board[i, j] == 2)
                {
                    whiteScore++;
                }
            }
        }

        return blackScore - whiteScore;
    }

    static List<int[,]> GenerateMoves()
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        List<int[,]> moves = new List<int[,]>();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (board[i, j] == 0)
                {
                    int[,] newBoard = (int[,])board.Clone();
                    newBoard[i, j] = 2;
                    moves.Add(newBoard);
                }
            }
        }

        return moves;
    }

    static int GetRow(int[,] board)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (board[i, j] == 2)
                {
                    return i;
                }
            }
        }

        return -1; // Không tìm thấy quân cờ
    }

    static int GetColumn(int[,] board)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (board[i, j] == 2)
                {
                    return j;
                }
            }
        }

        return -1; // Không tìm thấy quân cờ
    }
}


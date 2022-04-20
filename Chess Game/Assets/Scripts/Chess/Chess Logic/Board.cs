using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess Logic/Board")]
public class Board : ScriptableObject
{
    public Vector2Int boardSize { get; private set; }

    Space[,] spaces;
    [HideInInspector] public List<Piece> pieceList = new List<Piece>();

    public Board(Vector2Int size)
    {
        boardSize = size;
        spaces = new Space[size.x, size.y];
        for(int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                spaces[x, y] = new Space();
            }
        }
    }

    public Space GetSpace(int x, int y) => GetSpace(new Vector2Int(x, y));
    public Space GetSpace(Vector2Int pos)
    {
        if (!IsPosOnBoard(pos)) 
            return null;
        return spaces[pos.x, pos.y];
    }

    public bool IsPosOnBoard(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < boardSize.x && pos.y < boardSize.y;
    }

    public Vector2Int GetPieceForwardDir(Piece piece)
    {
        switch (piece.teamNumber)
        {
            case 1:
                return new Vector2Int(0, 1);
            case 2:
                return new Vector2Int(0, -1);
        };
        return new Vector2Int();
    }

    public void Clear()
    {
        pieceList.Clear();
        for (int x = 0; x < boardSize.x; x++)
        {
            for (int y = 0; y < boardSize.y; y++)
            {
                spaces[x, y] = new Space();
            }
        }
    }
}

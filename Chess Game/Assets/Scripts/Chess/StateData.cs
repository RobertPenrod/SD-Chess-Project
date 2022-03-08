using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateData
{
    public List<PieceData> pieceData = new List<PieceData>();
    public int turnIndex = 1;
    public int turnCount = 1;
    public MoveData lastMove;
}

public class PieceData
{
    public int mapIndex;
    public int team;
    public int boardIndex;
    public Vector2Int position;
    public bool hasMoved;

    public PieceData() { }

    public PieceData(int mapIndex, int team, Vector2Int position)
    {
        this.mapIndex = mapIndex;
        this.team = team;
        this.position = position;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateData
{
    public List<PieceData> pieceData = new List<PieceData>();
    public int turnIndex = 1;
    public int turnCount = 1;
    public MoveData lastMove;
    public List<EnPassantData> enPassantDataList = new List<EnPassantData>();
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

public class EnPassantData
{
    public Vector2Int capturePos;
    public Piece enPassantablePiece;
    public Vector2Int piecePos;

    public EnPassantData(Vector2Int capturePos, Piece enPassantablePiece)
    {
        this.capturePos = capturePos;
        this.enPassantablePiece = enPassantablePiece;
        this.piecePos = enPassantablePiece.currentPos;
    }

    public EnPassantData(EnPassantData otherData)
    {
        this.capturePos = otherData.capturePos;
        this.enPassantablePiece = otherData.enPassantablePiece;
        this.piecePos = otherData.piecePos;
    }
}
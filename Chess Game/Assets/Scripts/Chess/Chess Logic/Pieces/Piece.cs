using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess Logic/Piece")]
public class Piece : ScriptableObject
{
    public Sprite icon;
    public bool isRoyal;
    public Vector2Int currentPos;
    public int teamNumber;
    public int pointValue;
    public MoveBehavior moveBehavior;
    public bool hasMoved { get; private set; }
    public bool isImmuneToAtomicCapture;
    public bool isPromotable;
    public Piece pieceToPromoteTo;

    /// <summary>
    /// Whether or not this piece can castle as a standard king would.
    /// </summary>
    public bool canCastle;

    /// <summary>
    /// The piece that slides to the other side of the castling piece (the Rook using standard pieces).
    /// </summary>
    public bool castleWall;

    [HideInInspector] public Board board;
    [HideInInspector] public ChessGame chessGame;

    public Action OnPromotion_Event;

    public void Promote()
    {
        icon = pieceToPromoteTo.icon;
        moveBehavior = pieceToPromoteTo.moveBehavior;
        OnPromotion_Event?.Invoke();
    }

    public Vector2Int forward => board.GetPieceForwardDir(this);

    public int mapIndex;

    public bool IsOnSameTeam(Piece otherPiece)
    {
        return teamNumber == otherPiece.teamNumber;
    }

    public bool IsOnSameTeam(int otherTeamNum)
    {
        return teamNumber == otherTeamNum;
    }

    public void AddToBoard(Board newBoard, Vector2Int pos)
    {
        RemoveFromBoard(); // remove from previous board if applicable.

        board = newBoard;
        Space space = board.GetSpace(pos);
        if(space == null)
        {
            Debug.LogError("AddToBoard pos (" + pos + ") not in range");
            board = null;
            return;
        }

        currentPos = pos;
        space.piece = this;
        board.pieceList.Add(this);
    }

    public void RemoveFromBoard()
    {
        if (board == null) return;

        Space space = board.GetSpace(currentPos);
        board.pieceList.Remove(this);
        board = null;
        currentPos = new Vector2Int(-1, -1);

        if (space == null)
        {
            Debug.LogError("RemoveFromBoard pos (" + currentPos + ") not in range");
            return;
        }

        space.piece = null;
    }

    public bool MovePiece(int x, int y) => MovePiece(new Vector2Int(x, y));
    public bool MovePiece(Vector2Int newPos)
    {
        Space targetSpace = board?.GetSpace(newPos);
        if (targetSpace == null) { return false; }

        board.GetSpace(currentPos).piece = null;

        if (targetSpace.piece != null)
        {
            chessGame.CapturePiece(targetSpace.piece, this);
        }

        if (board == null)
            return true;

        targetSpace.piece = this;
        currentPos = newPos;

        hasMoved = true;
        return true;
    }

    public List<MoveData> GetMoves()
    {
        return moveBehavior.GetMoves(this);
    }

    public List<Vector2Int> GetThreatMap()
    {
        List<MoveData> threatmapMoves = moveBehavior.GetThreatMapMoves(this);
        List<Vector2Int> threatMap = new List<Vector2Int>();
        threatmapMoves.ForEach(x => threatMap.Add(x.dest));
        return threatMap;
    }
}

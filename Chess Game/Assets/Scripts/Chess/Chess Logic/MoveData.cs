using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveData
{
    public Vector2Int start;
    public Vector2Int dest;
    public Piece piece;

    public Action OnMoveMade_Event;

    public MoveData(Piece p, Vector2Int start, Vector2Int dest)
    {
        this.piece = p;
        this.start = start;
        this.dest = dest;
    }

    public MoveData GetClone()
    {
        return new MoveData(piece, start, dest);
    }

    public bool IsCapture(out Piece capturedPiece)
    {
        capturedPiece = null;
        Space destSpace = piece.board.GetSpace(dest);
        if (destSpace == null) return false;
        Piece destPiece = destSpace.piece;
        if (destPiece == null) return false;

        capturedPiece = destPiece;
        return destPiece.teamNumber != piece.teamNumber;
    }
}

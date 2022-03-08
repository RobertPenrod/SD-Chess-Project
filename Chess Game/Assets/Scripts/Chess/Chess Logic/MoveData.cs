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
}

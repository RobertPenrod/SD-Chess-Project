using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveBehavior : ScriptableObject
{
    public bool firstMoveOnly = false;
    public MoveType moveType = MoveType.MoveAndCapture;

    public enum MoveType
    {
        Move,
        Capture,
        MoveAndCapture
    }

    public abstract List<Vector2Int> GetMoves(Piece piece, Vector2Int? previousPos = null);
}

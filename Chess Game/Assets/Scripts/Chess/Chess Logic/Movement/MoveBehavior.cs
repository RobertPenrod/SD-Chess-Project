using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveBehavior : ScriptableObject
{
    public bool firstMoveOnly = false;
    public MoveType moveType = MoveType.MoveAndCapture;

    bool canCapture => moveType == MoveType.Capture || moveType == MoveType.MoveAndCapture;
    bool canMove => moveType == MoveType.Move || moveType == MoveType.MoveAndCapture;

    public enum MoveType
    {
        Move,
        Capture,
        MoveAndCapture
    }

    public List<Vector2Int> GetMoves(Piece piece, Vector2Int? previousPos = null)
    {
        if(firstMoveOnly && piece.hasMoved)
        {
            // return empty list since piece has already moved.
            return new List<Vector2Int>();
        }

        Board board = piece.board;
        List<Vector2Int> moves = GetMoves_Abstract(piece, previousPos);
        moves = FilterMoveType(moves, piece);

        return moves;
    }
    protected abstract List<Vector2Int> GetMoves_Abstract(Piece piece, Vector2Int? previousPos = null);

    List<Vector2Int> FilterMoveType(List<Vector2Int> moves, Piece piece)
    {
        Board board = piece.board;
        for(int i = 0; i < moves.Count; i++)
        {
            Vector2Int movePos = moves[i];
            Space moveSpace = board.GetSpace(movePos);
            bool spaceHasPiece = moveSpace.piece != null;

            if (spaceHasPiece)
            {
                bool sameTeam = piece.IsOnSameTeam(moveSpace.piece);
                if(sameTeam || !canCapture)
                {
                    // remove move since we cannot move into a piece on our team
                    moves.RemoveAt(i);
                    i--;
                    continue;
                }
            }
            else if(!canMove)
            {
                // remove current move since it is not a capture
                moves.RemoveAt(i);
                i--;
                continue;
            }
        }
        return moves;
    }

    // Finish this
    protected Vector2Int PieceToBoardSpace(Piece piece, Vector2Int pos)
    {
        Board board = piece.board;
        Vector2Int forwardDir = board.GetPieceForwardDir(piece);
        return pos;
    }
}

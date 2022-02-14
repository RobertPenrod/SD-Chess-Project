using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveBehavior : ScriptableObject
{
    [Header("Parameters")]
    public bool firstMoveOnly = false;
    public MoveType moveType = MoveType.MoveAndCapture;

    [Header("En Passant")]
    public bool canTakeEnPassant = false;
    public bool isEnPassantable = false;

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
        moves = RemoveDuplicateMoves(moves, piece);

        return moves;
    }
    protected abstract List<Vector2Int> GetMoves_Abstract(Piece piece, Vector2Int? previousPos = null);

    List<Vector2Int> RemoveDuplicateMoves(List<Vector2Int> moves, Piece piece)
    {
        List<Vector2Int> finalMoves = new List<Vector2Int>();
        moves.ForEach(x =>
        {
            if(!finalMoves.Contains(x))
            {
                finalMoves.Add(x);
            }
        });
        return finalMoves;
    }
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

    protected Vector2Int PieceToBoardSpace(Piece piece, Vector2Int pos)
    {
        Board board = piece.board;
        Vector2Int forwardDir = board.GetPieceForwardDir(piece);
        Vector2Int pieceRelativePos = pos - piece.currentPos;
        
        if(forwardDir == Vector2Int.up)
        {
            // Do nothing
        }
        else if(forwardDir == Vector2Int.right)
        {
            int newX = pieceRelativePos.y;
            int newY = -pieceRelativePos.x;
            pieceRelativePos = new Vector2Int(newX, newY);
        } 
        else if(forwardDir == Vector2Int.down)
        {
            pieceRelativePos.y *= -1;
            pieceRelativePos.x *= -1;
        } 
        else if(forwardDir == Vector2Int.left)
        {
            int newX = -pieceRelativePos.y;
            int newY = pieceRelativePos.x;
            pieceRelativePos = new Vector2Int(newX, newY);
        }

        Vector2Int boardPos = piece.currentPos + pieceRelativePos;
        return boardPos;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess Logic/Move Behavior/Leaps")]
public class MB_Leaps : MoveBehavior
{ 
    public List<Vector2Int> leapOffset;

    protected override List<Vector2Int> GetMoves_Abstract(Piece piece, Vector2Int? previousPos = null)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        Vector2Int pos = piece.currentPos;
        if (previousPos != null)
            pos = previousPos.Value;
        Board board = piece.board;

        foreach (Vector2Int o in leapOffset)
        {
            Vector2Int pieceSpaceLeapPos = pos + o;
            Vector2Int boardPos = PieceToBoardSpace(piece, pieceSpaceLeapPos);

            if (!board.IsPosOnBoard(boardPos))
                continue;

            Space space = board.GetSpace(boardPos);
            if(space.piece != null)
            {
                if(piece.IsOnSameTeam(space.piece))
                {
                    continue;
                }
            }

            moves.Add(boardPos);
        }
        
        return moves;
    }
}

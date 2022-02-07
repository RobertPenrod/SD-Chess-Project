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
            Vector2Int leapPos = pos + o;

            if (!board.IsPosOnBoard(leapPos))
                continue;

            Space space = board.GetSpace(leapPos);
            if(space.piece != null)
            {
                if(piece.IsOnSameTeam(space.piece))
                {
                    continue;
                }
            }

            moves.Add(leapPos);
        }
        
        return moves;
    }
}

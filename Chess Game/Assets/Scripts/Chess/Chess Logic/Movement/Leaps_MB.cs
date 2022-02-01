using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess Logic/Move Behavior/Leaps")]
public class Leaps_MB : MoveBehavior
{ 
    public List<Vector2Int> leapOffset;

    public override List<Vector2Int> GetMoves(Piece piece, Vector2Int? previousPos = null)
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
            if(space.pieceList.Count > 0)
            {
                if(piece.IsOnSameTeam(space.pieceList[0]))
                {
                    continue;
                }
            }

            moves.Add(leapPos);
        }
        
        return moves;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess Logic/Move Behavior/MB_List")]
public class MB_List : MoveBehavior
{
    public List<MoveBehavior> moveBehaviorList;

    protected override List<MoveData> GetMoves_Abstract(Piece piece, Vector2Int? previousPos = null, bool isThreatMap = false)
    {
        List<MoveData> moves = new List<MoveData>();
        foreach(MoveBehavior mb in moveBehaviorList)
        {
            if (isThreatMap)
            {
                moves.AddRange(mb.GetThreatMapMoves(piece));
            }
            else
            {
                moves.AddRange(mb.GetMoves(piece, previousPos, true));
            }
        }
        return moves;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess Logic/Move Behavior/MB_List")]
public class MB_List : MoveBehavior
{
    public List<MoveBehavior> moveBehaviorList;

    public override List<Vector2Int> GetMoves(Piece piece, Vector2Int? previousPos = null)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        foreach(MoveBehavior mb in moveBehaviorList)
        {
            moves.AddRange(mb.GetMoves(piece, previousPos));
        }
        return moves;
    }
}

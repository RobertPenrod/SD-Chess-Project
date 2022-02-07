using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess Logic/Move Behavior/Slide")]
public class MB_Slide : MoveBehavior
{
    public Vector2Int dir;
    public int range = 0;

    protected override List<Vector2Int> GetMoves_Abstract(Piece piece, Vector2Int? previousPos = null)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        Vector2Int pos = piece.currentPos;
        if (previousPos != null)
            pos = previousPos.Value;
        Board board = piece.board;

        bool moveDone = false;
        int currentRange = 0;
        while(!moveDone)
        {
            // Check if maximum range reached
            currentRange++;
            if (range > 0 && currentRange > range)
            {
                break;
            }

            pos += dir;

            // Is the new position valid,
            // are we done moving?
            bool isOnBoard = board.IsPosOnBoard(pos);
            if (!isOnBoard)
                break;
            Space space = board.GetSpace(pos);
            bool spaceHasPiece = space.piece != null;
            if(spaceHasPiece)
            {
                if(piece.IsOnSameTeam(space.piece))
                {
                    // if same team, move invalid and done
                    break;
                }
                else
                {
                    // of different team, can capture, move is done
                    moveDone = false;
                }
            }

            // if valid
            // add new pos to moves
            moves.Add(pos);
        }

        return moves;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess Logic/Move Behavior/Slide")]
public class MB_Slide : MoveBehavior
{
    public Vector2Int dir;
    public int range = 0;

    protected override List<MoveData> GetMoves_Abstract(Piece piece, Vector2Int? previousPos = null, bool isThreatMap = false)
    {
        List<MoveData> moves = new List<MoveData>();

        Vector2Int pieceSpacePos = piece.currentPos;
        if (previousPos != null)
            pieceSpacePos = previousPos.Value;
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

            pieceSpacePos += dir;
            Vector2Int boardPos = PieceToBoardSpace(piece, pieceSpacePos);

            // Is the new position valid,
            // are we done moving?
            bool isOnBoard = board.IsPosOnBoard(boardPos);
            if (!isOnBoard)
                break;
            Space space = board.GetSpace(boardPos);
            bool spaceHasPiece = space.piece != null;
            if(spaceHasPiece)
            {
                if(piece.IsOnSameTeam(space.piece))
                {
                    // if same team, move pos invalid and done
                    break;
                }
                else
                {
                    // of different team, can capture, move is done
                    moveDone = true;
                }
            }

            // if valid
            // add new pos to moves
            MoveData moveData = new MoveData(piece, piece.currentPos, boardPos);
            moves.Add(moveData);
        }

        return moves;
    }
}

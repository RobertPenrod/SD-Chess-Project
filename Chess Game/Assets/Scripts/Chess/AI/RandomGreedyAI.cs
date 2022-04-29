using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGreedyAI : ChessAI
{
    public override MoveData GetMove(ChessGame chessGame)
    {
        List<Piece> pieceList = chessGame.teamInfo[teamNumber].GetPieceList();
        List<MoveData> moveList = new List<MoveData>();
        pieceList.ForEach(piece => moveList.AddRange(piece.GetMoves()));

        // look for capture move
        int highestValue = int.MinValue;
        MoveData bestMove = moveList[Random.Range(0, moveList.Count)]; // random move
        int captureCount = 0;
        foreach (MoveData moveData in moveList)
        {
            if (moveData.IsCapture(out Piece capturedPiece))
            {
                captureCount++;
                int pointValue = capturedPiece.pointValue;
                if (pointValue > highestValue)
                {
                    bestMove = moveData;
                }
            }
        }
        Debug.Log(captureCount);

        return bestMove;
    }
}

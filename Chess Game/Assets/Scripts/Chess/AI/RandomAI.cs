using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAI : ChessAI
{
    public override MoveData GetMove(ChessGame chessGame)
    {
        List<Piece> pieceList = chessGame.teamInfo[teamNumber].GetPieceList();
        for(int maxIter = 1000; maxIter > 0; maxIter--)
        {
            Piece randomPiece = pieceList[Random.Range(0, pieceList.Count)];
            List<MoveData> randomMoveList = randomPiece.GetMoves();
            if (randomMoveList.Count == 0) continue;
            MoveData randomMove = randomMoveList[Random.Range(0, randomMoveList.Count)];
        }

        return null;
    }
}

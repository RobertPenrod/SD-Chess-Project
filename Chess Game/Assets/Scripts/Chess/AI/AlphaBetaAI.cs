using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaBetaAI : ChessAI
{
    public ChessGame curGame;

    public struct Value
    {
        public int numValue;
        public MoveData bestMove;
    }

    // Returns value of given piece using point system for type's variant. 
    // Right now, the parameter is set at 'conventional' by default but that will be removed once selecting variants is supported.
    // Might also do separate methods for variants instead, just depends.
    public int getValue(Piece move, string type="conventional")
    {
        int pointValue = 0;
        int enemyColor = 1;
        if (move.teamNumber == 1) enemyColor = 2;
        // List of pieces on the enemy team.
        List<Piece> EnemyPieces = curGame.teamInfo[enemyColor].GetPieceList();
        // Checking if move would be capturable by enemy, and weighting it by piece value.
        if (curGame.teamInfo[teamNumber].threatMap.Contains(move.currentPos))
        {
            pointValue += -1 * move.pointValue;
        }

        // Checking if move would capture enemy piece, and weighting it by that enemy's piece value.
        for (int i = 0; i < EnemyPieces.Count; i++)
        {
            // If a current enemy piece's position is where the new move would be, add that to value.
            if (EnemyPieces[i].currentPos == move.currentPos)
            {
                // Add value of captured piece to pointValue.
                pointValue += EnemyPieces[i].pointValue;
                break;
            }
        }

        return pointValue;
    }

    // Negamax with alpha-beta pruning, taken from pseudocode in "Memory versus Search in Games" by Dennis Breuker.
    public Value alphaBeta(Piece piece, int depth, int alpha, int beta, int color)
    {
        List<MoveData> childMoves = piece.GetMoves();
        int enemyColor = 1;
        if (color == 1) enemyColor = 2;
        Value value = new Value();
        value.numValue = -10000;
        if (depth == 0 || childMoves.Count == 0)
        {
            value.numValue = color * getValue(piece);
            return value;
        }

        // Can add move ordering here to make search more efficient, but want to get base AI implemented/working first.

        // In pseudocode, value is initially -inf so I just used a very low number. 
        
        value.bestMove = childMoves[0];
        for (int i = 0; i < childMoves.Count; i++)
        {
            Value curValue = alphaBeta(childMoves[i].piece, depth - 1, -beta, -alpha, enemyColor);
            value.numValue = -(curValue.numValue);
            alpha = Mathf.Max(value.numValue, alpha);
            if (alpha >= beta) break;
        }

        return value;
    }

    // Returns highest value piece. 
    public override MoveData GetMove(ChessGame chessGame)
    {
        curGame = chessGame;
        List<Piece> pieceList = chessGame.teamInfo[teamNumber].GetPieceList();
        List<Value> maxValues = new List<Value>();
        for (int i = 0; i < pieceList.Count; i++)
        {
            maxValues[i] = alphaBeta(pieceList[i], 5, -100000, -100000, 1);
        }

        int maxIndex = 0;
        // I had issues using Math max method in unity so ended up just doing a loop...
        for (int i = 1; i < pieceList.Count; i++)
        {
            if (maxValues[i].numValue > maxValues[maxIndex].numValue) maxIndex = i;
        }
        return maxValues[maxIndex].bestMove;
    }
}

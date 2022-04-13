using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaBetaAI : ChessAI
{
    public ChessGame curGame;
    public SearchTree gameTree;

    public struct Value
    {
        public int numValue;
        public MoveData bestMove;
    }

    public Value alphaBeta(SearchNode curPosition, int alpha, int beta, int depth)
    {
        // Change this for team value.
        Value value = new Value();
        if (depth == 0) {
            Value vp = new Value();
            vp.bestMove = curPosition.currMove;
            vp.numValue = curPosition.getValue();
            return value;
        }
 
        value.numValue = -10000;
        List<SearchNode> curMoves = curPosition.getChildren();
        for (int i = 0; i < curMoves.Count; i++)
        {
            Value cur = alphaBeta(curMoves[i], -beta, -alpha, depth - 1);
            if (-cur.numValue > value.numValue) {
                value.numValue = -cur.numValue;
                value.bestMove = cur.bestMove;
            }
            if (value.numValue > alpha) { 
                alpha = value.numValue;
            }
            if (alpha >= beta) break;
        }
        return value;
    }

    // AI team number is 2.
    public override MoveData GetMove(ChessGame chessGame)
    {
        gameTree = new SearchTree(chessGame.GetState(), 20);
        // Call alpha-beta. Return move from that algorithm. 
        return alphaBeta(gameTree.getRoot(), -10000, -10000, 20).bestMove;
    }
}

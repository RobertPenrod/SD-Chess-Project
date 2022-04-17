using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaBetaAI : ChessAI
{
    public SearchTree gameTree = null;

    public struct Value
    {
        public int numValue;
        public MoveData bestMove;
    }
    
    public Value alphaBeta(SearchNode curPosition, int alpha, int beta, int depth, int color)
    {
        // Change this for team value.
        Value value = new Value();
        List<SearchNode> curMoves = curPosition.getChildren();
        if (depth == 0 || curMoves.Count == 0) {
            Value vp = new Value();
            vp.bestMove = curPosition.currMove;
            vp.numValue = color * curPosition.getValue();
            return vp;
        }
        value.numValue = -10000;
        for (int i = 0; i < curMoves.Count; i++)
        {
            Value cur = alphaBeta(curMoves[i], -beta, -alpha, depth - 1, -color);
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

    public override MoveData GetMove(ChessGame chessGame)
    {
        // Start game position.
       // if (gameTree == null)
        //{
            gameTree = new SearchTree(chessGame, 2);
        //}

        // If we aren't at the start position, we can just search for the new game, make that the parent, 
        // and generate one new level instead of remaking the entire tree every time. 
       // else
       // {
        //    gameTree.newParent(chessGame);
        //}

        // Call alpha-beta. Return move from that algorithm. 
        return alphaBeta(gameTree.getRoot(), -10000, +10000, 2, 1).bestMove;
    }
}

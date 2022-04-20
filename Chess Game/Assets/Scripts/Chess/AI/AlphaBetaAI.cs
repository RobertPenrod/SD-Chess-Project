using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public struct Value
{
    public MoveData bestMove;
    public int numValue;
}



public class AlphaBetaAI : ChessAI
{
    public int originalDepth;

    // Generating all moves in a game.
    public List<MoveData> generateMoves(ChessGame chessGame, int teamPlayer, int depth)
    {
        Debug.Log("new call");
        List<MoveData> ourMoves = new List<MoveData>();
        /*
        List<Piece> pieceList = chessGame.teamInfo[teamNumber].GetPieceList();
        for (int i = 0; i < pieceList.Count; i++)
        {
            ourMoves.AddRange(pieceList[i].GetMoves());
        }*/
        /*
        List<Piece> pieceList = chessGame.GetEnemyPieces(teamPlayer);
        for (int i = 0; i < pieceList.Count; i++)
        { 
            if (pieceList[i] == null) continue;
            ourMoves.AddRange(pieceList[i].GetMoves());
        }
        */
        List<Piece> pieceList = chessGame.GetAllPieces();
        for (int i = 0; i < pieceList.Count; i++) {
            if (pieceList[i].teamNumber != teamPlayer) ourMoves.AddRange(pieceList[i].GetMoves());
            else continue;
        }
        return ourMoves;
    }

    // Sorts moves by their value (in descending order, i.e. better moves are evaluated first).
    public List<MoveData> orderMoves(List<MoveData> ourMoves)
    {
        return ourMoves.OrderByDescending(x => eval(x.piece.chessGame)).ToList();
    }


    // Team numbers in game are 1 and 2, but negamax uses -color for ease of calculating values for each side, so I use this method. 
    public int converter(int color)
    {
        if (color == 1) return 2;
        else return 1;
    }
    
    // Evaluation function that considers # of pieces for each side and mobility.
    public int eval(ChessGame parentGame)
    {
        int score = 0;
        int whiteWeight = 0;
        int blackWeight = 0;
        int whiteMobility = 0;
        int blackMobility = 0;
        List<Piece> pieceList = parentGame.GetAllPieces();

        for (int i = 0; i < pieceList.Count; i++)
        {
            if (pieceList[i].teamNumber == 2)
            {
                // If pieceList is null then it's captured.
                if (pieceList[i] == null) whiteWeight -= pieceList[i].pointValue * 5;
                // Checking if piece is in danger.
                else {
                    if (parentGame.teamInfo[1].threatMap.Contains(pieceList[i].currentPos)) blackWeight += pieceList[i].pointValue * 4;
                    whiteWeight += pieceList[i].pointValue;
                    whiteMobility += pieceList[i].GetMoves().Count;
                }
            }
            else { 
                // If pieceList is null then it's captured.
                if (pieceList[i] == null) blackWeight -= pieceList[i].pointValue * 5;
                // Checking if piece is in danger.
                else {
                    if (parentGame.teamInfo[2].threatMap.Contains(pieceList[i].currentPos)) whiteWeight += pieceList[i].pointValue * 4; 
                    blackWeight += pieceList[i].pointValue; 
                    blackMobility += pieceList[i].GetMoves().Count;
                }
            }
        }

        score = (whiteWeight - blackWeight) + (whiteMobility - blackMobility);


        return score;
    }

    /*
    public Value alphaBeta(ChessGame parentGame, MoveData curMove, int depth, int alpha, int beta, int color)
    {
        if (depth == 0 || curMove.piece.GetMoves().Count == 0)
        {
            Value vp = new Value();
            vp.numValue = color * eval(parentGame); 
            vp.bestMove = curMove;
            return vp;
        }

        ChessGame dummyGame = parentGame.CreateSimulatedCloneGame();
        dummyGame.MakeMove(curMove.start, curMove.dest);
        List<MoveData> ourMoves = generateMoves(dummyGame, converter(color), depth); 
        ourMoves = orderMoves(ourMoves);
     

        Value value = new Value();
        value.numValue = -10000;
        value.bestMove = curMove;

        for (int i = 0; i < ourMoves.Count; i++)
        {
            Value newValue = alphaBeta(dummyGame, ourMoves[i], depth - 1, -beta, -alpha, -color);
            if (-newValue.numValue > value.numValue)
            {
                value.numValue = -newValue.numValue;
                if (depth == originalDepth) value.bestMove = newValue.bestMove;
            }

            if (value.numValue > alpha) alpha = value.numValue;

            if (alpha >= beta) break;
        }
        return value;
    }*/

    public int alphaBeta(ChessGame parentGame, MoveData curMove, int depth, int alpha, int beta, int color) {
        if (depth == 0 || curMove.piece.GetMoves().Count == 0)
        {
            return color * eval(parentGame);
        }

        ChessGame dummyGame = parentGame.CreateSimulatedCloneGame();
        dummyGame.MakeMove(curMove.start, curMove.dest);
        List<MoveData> ourMoves = generateMoves(dummyGame, converter(color), depth); 
        ourMoves = orderMoves(ourMoves);
     

        int value = -10000;

        for (int i = 0; i < ourMoves.Count; i++)
        {
            int newValue = alphaBeta(dummyGame, ourMoves[i], depth - 1, -beta, -alpha, -color);
            if (-newValue > value)
            {
                value = -newValue;
            }

            if (value > alpha) alpha = value;

            if (alpha >= beta) break;
        }

        return value;
    }
    

    public override MoveData GetMove(ChessGame chessGame)
    {
        List<MoveData> moveList = generateMoves(chessGame, 2, 2); // If we end up giving the user the option to choose color this line needs to be changed/pass in user color as parameter. 
        /*
        List<Value> valueList = new List<Value>();
        originalDepth = 2;
        
        for (int i = 0; i < moveList.Count; i++)
        {
            valueList.Add(alphaBeta(chessGame, moveList[i], 2, -10000, 10000, 1));
        }

        MoveData bestMove = valueList[0].bestMove;
        
        for (int i = 1; i < valueList.Count; i++)
        {
            if (valueList[i - 1].numValue < valueList[i].numValue) bestMove = valueList[i].bestMove;
        }

        Debug.Log("returned start: " + bestMove.start + " dest: " + bestMove.dest);*/
        List<int> valueList = new List<int>();
        originalDepth = 2;
        
        for (int i = 0; i < moveList.Count; i++)
        {
            valueList.Add(alphaBeta(chessGame, moveList[i], 2, -10000, 10000, 1));
        }

        MoveData bestMove = moveList[0];
        
        for (int i = 1; i < valueList.Count; i++)
        {
            if (valueList[i - 1] < valueList[i]) bestMove = moveList[i];
        }

        Debug.Log("returned start: " + bestMove.start + " dest: " + bestMove.dest);
        return bestMove;
    }
}

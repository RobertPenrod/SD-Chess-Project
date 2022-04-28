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

    // Piece tables defined by Tomasz Michniewski.
    int[,] pawnTable = new int[8, 8] {  {0, 0, 0, 0, 0, 0, 0, 0}, 
                                        {50, 50, 50, 50, 50, 50, 50, 50}, 
                                        {10, 10, 20, 30, 30, 20, 10, 10}, 
                                        {5, 5, 10, 25, 25, 10, 5, 5}, 
                                        {0, 0, 0, 20, 20, 0, 0, 0}, 
                                        {5, -5, -10, 0, 0, -10, -5, 5}, 
                                        {5, 10, 10, -20, -20, 10, 10, 5}, 
                                        {0, 0, 0, 0, 0, 0, 0, 0}};
    
    int[,] knightTable = new int[8, 8] {{-50, -40, -30, -30, -30, -30, -40, -50}, 
                                        {-40, -20, 0, 0, 0, 0, -20, -40}, 
                                        {-30, 0, 10, 15, 15, 10, 0, -30}, 
                                        {-30, 5, 15, 20, 20, 15, 5, -30}, 
                                        {-30, 0, 15, 20, 20, 15, 0, -30}, 
                                        {-30, 5, 10, 15, 15, 10, 5, -30}, 
                                        {-40, -20, 0, 5, 5, 0, -20, -40}, 
                                        {-50, -40, -30, -30, -30, -30, -40, -50}};
    
    int[,] bishopTable = new int[8, 8] {{-20, -10, -10, -10, -10, -10, -10, -20}, 
                                        {-10, 0, 0, 0, 0, 0, 0, -10}, 
                                        {-10, 0, 5, 10, 10, 5, 0, -10}, 
                                        {-10, 5, 5, 10, 10, 5, 5, -10}, 
                                        {-10, 0, 10, 10, 10, 10, 0, -10}, 
                                        {-10, 10, 10, 10, 10, 10, 10, -10}, 
                                        {-10, 5, 0, 0, 0, 0, 5, -10}, 
                                        {-20, -10, -10, -10, -10, -10, -10, -20}};

    int[,] rookTable = new int[8, 8] {  {0, 0, 0, 0, 0, 0, 0, 0}, 
                                        {5, 10, 10, 10, 10, 10, 10, 5}, 
                                        {-5, 0, 0, 0, 0, 0, 0, -5}, 
                                        {-5, 0, 0, 0, 0, 0, 0, -5}, 
                                        {-5, 0, 0, 0, 0, 0, 0, -5}, 
                                        {-5, 0, 0, 0, 0, 0, 0, -5}, 
                                        {-5, 0, 0, 0, 0, 0, 0, -5}, 
                                        {0, 0, 0, 5, 5, 0, 0, 0}};

    int[,] queenTable = new int[8, 8] { {-20, -10, -10, -5, -5, -10, -10, -20}, 
                                        {-10, 0, 0, 0, 0, 0, 0, -10}, 
                                        {-10, 0, 5, 5, 5, 5, 0, -10}, 
                                        {-5, 0, 5, 5, 5, 5, 0, -5}, 
                                        {0, 0, 5, 5, 5, 5, 0, -5}, 
                                        {-10, 5, 5, 5, 5, 5, 0, -10}, 
                                        {-10, 0, 5, 0, 0, 0, 0, -10}, 
                                        {-20, -10, -10, -5, -5, -10, -10, -20}};

    int[,] kingTable = new int[8, 8] {  {-30, -40, -40, -50, -50, -40, -40, -30}, 
                                        {-30, -40, -40, -50, -50, -40, -40, -30}, 
                                        {-30, -40, -40, -50, -50, -40, -40, -30}, 
                                        {-30, -40, -40, -50, -50, -40, -40, -30}, 
                                        {-20, -30, -30, -40, -40, -30, -30, -20}, 
                                        {-10, -20, -20, -20, -20, -20, -20, -10}, 
                                        {20, 30, 0, 0, 0, 0, 20, 30}, 
                                        {20, 30, 10, 0, 0, 10, 30, 20}};

    // Generating all moves in a game.
    public List<MoveData> generateMoves(ChessGame chessGame, int teamPlayer)
    {
        List<MoveData> ourMoves = new List<MoveData>();
        Debug.Log("p");
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
        Debug.Log("pieces: " + pieceList.Count);
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
    public int ColorToTeam(int color)
    {
        if (color == 1) return 2;
        else return 1;
    }

    public int TeamToColor(int teamNum)
    {
        if (teamNum == 2) return 1;
        else return -1;
    }
    
    /*
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
                if (pieceList[i] == null) whiteWeight -= pieceList[i].pointValue * 100;
                // Checking if piece is in danger.
                else {
                    if (parentGame.teamInfo[1].threatMap.Contains(pieceList[i].currentPos)) whiteWeight -= pieceList[i].pointValue * 4;
                    whiteWeight += pieceList[i].pointValue;
                    whiteMobility += pieceList[i].GetMoves().Count;
                }
            }
            else { 
                // If pieceList is null then it's captured.
                if (pieceList[i] == null) blackWeight -= pieceList[i].pointValue * 100;
                // Checking if piece is in danger.
                else {
                    if (parentGame.teamInfo[2].threatMap.Contains(pieceList[i].currentPos)) blackWeight -= pieceList[i].pointValue * 4;
                    blackWeight += pieceList[i].pointValue; 
                    blackMobility += pieceList[i].GetMoves().Count;
                }
            }
        }
        score = (whiteWeight - blackWeight) + (whiteMobility - blackMobility);
        return score;
    }
    */

    public int eval(ChessGame parentGame) 
    {
        int score = 0;
        List<Piece> pieceList = parentGame.GetAllPieces();
        int whiteWeight = 0;
        int blackWeight = 0;
        int whiteMobility = 0;
        int blackMobility = 0;

        for (int i = 0; i < pieceList.Count; i++) {
            if (pieceList[i].teamNumber == 2) { // white
                if (pieceList[i].currentPos.x == -1 || pieceList[i].currentPos.y == -1) whiteWeight -= pieceList[i].pointValue * 1000;
                else {
                switch (pieceList[i].pointValue) {
                    case 1: // pawn
                        whiteWeight += pawnTable[(int)pieceList[i].currentPos.x, (int)pieceList[i].currentPos.y];
                        break;
                    case 3: // knight
                        whiteWeight += knightTable[(int)pieceList[i].currentPos.x, (int)pieceList[i].currentPos.y];
                        break;
                    case 4: // bishop
                        whiteWeight += bishopTable[(int)pieceList[i].currentPos.x, (int)pieceList[i].currentPos.y];
                        break;
                    case 5: // rook
                        whiteWeight += rookTable[(int)pieceList[i].currentPos.x, (int)pieceList[i].currentPos.y];
                        break;
                    case 9: // queen
                        whiteWeight += queenTable[(int)pieceList[i].currentPos.x, (int)pieceList[i].currentPos.y];
                        break;
                    case 10: // king
                        whiteWeight += kingTable[(int)pieceList[i].currentPos.x, (int)pieceList[i].currentPos.y];
                        break;
                }
                }
                //whiteMobility += pieceList[i].GetMoves().Count;
            }

            else { // black
                    if (pieceList[i].currentPos.x == -1 || pieceList[i].currentPos.y == -1) blackWeight -= pieceList[i].pointValue * 1000;
                    else {
                    switch (pieceList[i].pointValue) {
                        case 1: // pawn
                            blackWeight += pawnTable[Mathf.Abs((int)pieceList[i].currentPos.x - 7), Mathf.Abs((int)pieceList[i].currentPos.y - 7)];
                            break;
                        case 3: // knight
                            blackWeight += knightTable[Mathf.Abs((int)pieceList[i].currentPos.x - 7), Mathf.Abs((int)pieceList[i].currentPos.y - 7)];
                            break;
                        case 4: // bishop
                            blackWeight += bishopTable[Mathf.Abs((int)pieceList[i].currentPos.x - 7), Mathf.Abs((int)pieceList[i].currentPos.y - 7)];
                            break;
                        case 5: // rook
                            blackWeight += rookTable[Mathf.Abs((int)pieceList[i].currentPos.x - 7), Mathf.Abs((int)pieceList[i].currentPos.y - 7)];
                            break;
                        case 9: // queen
                            blackWeight += queenTable[Mathf.Abs((int)pieceList[i].currentPos.x - 7), Mathf.Abs((int)pieceList[i].currentPos.y - 7)];
                            break;
                        case 10: // king
                            blackWeight += kingTable[Mathf.Abs((int)pieceList[i].currentPos.x - 7), Mathf.Abs((int)pieceList[i].currentPos.y - 7)];
                            break;
                }}
                //blackMobility += pieceList[i].GetMoves().Count;
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
        List<MoveData> ourMoves = generateMoves(dummyGame, ColorToTeam(color)); 
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
        List<MoveData> moveList = generateMoves(chessGame, teamNumber); // If we end up giving the user the option to choose color this line needs to be changed/pass in user color as parameter. 
        Debug.Log(moveList.Count);
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
        
        for (int i = 0; i < moveList.Count; i++)
        {
            valueList.Add(alphaBeta(chessGame, moveList[i], 2, -10000, 10000, 1));
        }
        
        MoveData bestMove = moveList[0];
        
        for (int i = 1; i < valueList.Count; i++)
        {
            if (valueList[i - 1] < valueList[i]) bestMove = moveList[i];
        }
        return bestMove;
    }
}

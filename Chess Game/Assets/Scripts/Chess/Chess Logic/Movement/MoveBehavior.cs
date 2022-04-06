using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveBehavior : ScriptableObject
{
    [Header("Parameters")]
    public bool firstMoveOnly = false;
    public MoveType moveType = MoveType.MoveAndCapture;

    [Header("En Passant")]
    public bool canTakeEnPassant = false;
    public bool isEnPassantable = false;

    bool canCapture => moveType == MoveType.Capture || moveType == MoveType.MoveAndCapture;
    bool canMove => moveType == MoveType.Move || moveType == MoveType.MoveAndCapture;

    public enum MoveType
    {
        Move,
        Capture,
        MoveAndCapture
    }

    public List<MoveData> GetMoves(Piece piece, Vector2Int? previousPos = null)
    {
        bool isSimulation = piece.chessGame.IsSimulation;
        bool removeCheckMoves = !isSimulation;
        bool addCastlingMoves = !isSimulation;

        bool canGetMove = piece.board != null && (!firstMoveOnly || !piece.hasMoved);
        if (!canGetMove)
        {
            return new List<MoveData>();
        }

        Board board = piece.board;
        List<MoveData> moves = GetMoves_Abstract(piece, previousPos);

        if(piece.canCastle && addCastlingMoves)
        {
            //moves.AddRange(GetCastlingMoves(piece));
        }

        moves = FilterMoveAndCapture(moves, piece);
        moves = RemoveDuplicateMoves(moves, piece);

        if (removeCheckMoves)
        {
            moves = RemoveCheckMoves(moves, piece);
        }

        // Test
        if(isEnPassantable)
        {
            moves.ForEach(x => x.OnMoveMade_Event += () =>
            {
                // Drop En Passant Zone behind piece,
                // Zone is cleared when player owning piece takes their next turn.
                Vector2Int enPassantPos = piece.currentPos - piece.board.GetPieceForwardDir(piece);
                EnPassantData enPassantData = new EnPassantData(enPassantPos, piece);
                piece.chessGame.enPassantDataList.Add(enPassantData);
            });
        }

        return moves;
    }
    protected abstract List<MoveData> GetMoves_Abstract(Piece piece, Vector2Int? previousPos = null, bool isThreatMap = false);

    List<MoveData> GetCastlingMoves(Piece piece)
    {
        Debug.Log("Get Castling Moves");
        List<MoveData> castlingMoveList = new List<MoveData>();
        if (piece.hasMoved) return castlingMoveList;

        Piece leftPiece = PieceCast(piece, Vector2Int.left);
        Piece upPiece = PieceCast(piece, Vector2Int.up);
        Piece rightPiece = PieceCast(piece, Vector2Int.right);
        Piece downPiece = PieceCast(piece, Vector2Int.down);
        List<Piece> foundPieceList = new List<Piece> { leftPiece, upPiece, rightPiece, downPiece };
        List<Vector2Int> pieceDir = new List<Vector2Int> { Vector2Int.left, Vector2Int.up, Vector2Int.right, Vector2Int.down };
        for(int i = 0; i < foundPieceList.Count; i++)
        {
            Piece castleWallPiece = foundPieceList[i];
            if (castleWallPiece == null) continue;

            if (Vector2Int.Distance(castleWallPiece.currentPos, piece.currentPos) < 2f) continue;

            bool isFoundPieceValidForCastle = castleWallPiece.castleWall && !castleWallPiece.hasMoved;
            if (!isFoundPieceValidForCastle) continue;

            // Found piece is valid for castle
            // we know its direction.
            // we need to check if we can move "piece" 2 spaces towards the castle wall,
            // making sure that the player who owns this piece is not in check when they do these 2 moves.
            Vector2Int dir = pieceDir[i];
            Vector2Int dest1 = piece.currentPos + dir;
            Vector2Int dest2 = dest1 + dir;

            //ChessGame testGame = new ChessGame(piece.chessGame);
            //testGame.LoadState(piece.chessGame.GetState());

            ChessGame mainGame = piece.chessGame;
            ChessGame testGame = new ChessGame(mainGame.gameBoardList[0].boardSize, mainGame.playerCount);
            testGame.IsSimulation = true;
            testGame.pieceMap = mainGame.pieceMap;
            testGame.LoadState(mainGame.GetState());

            Debug.Log(foundPieceList[i].name + " " + dir);
            Debug.Log("Testing Move " + piece.currentPos + " to " + dest1);
            bool moveSuccesful_1 = testGame.MakeMove(piece.currentPos, dest1, doEndTurn: false);
            if (!moveSuccesful_1) continue;
            testGame.UpdateGameInfo();
            if (testGame.GetPiecesTeamInfo(piece).isInCheck) continue;

            Debug.Log("Move 1 Valid");
            bool moveSuccesful_2 = testGame.MakeMove(dest1, dest2, doEndTurn: false);
            if (!moveSuccesful_2) continue;
            testGame.UpdateGameInfo();
            if (testGame.GetPiecesTeamInfo(piece).isInCheck) continue;

            Debug.Log("Move 2 Valid");
            Debug.Log("Testing Move " + castleWallPiece.currentPos + " to " + dest1);
            bool moveSuccesful_3 = testGame.ForceMove(castleWallPiece.currentPos, dest1);
            if (!moveSuccesful_3) continue;
            testGame.UpdateGameInfo();
            if (testGame.GetPiecesTeamInfo(piece).isInCheck) continue;

            Debug.Log("Move 3 Valid");

            // At this point, the castle move is valid
            Vector2Int castlePos = piece.currentPos += dir * 2;
            MoveData castleMove = new MoveData(piece, piece.currentPos, castlePos);
            castleMove.OnMoveMade_Event += () =>
            {
                piece.chessGame.MakeMove(castleWallPiece.currentPos, piece.currentPos + dir);
            };
            castlingMoveList.Add(castleMove);
        }
        Debug.Log("Found " + castlingMoveList.Count + " Castle Moves");
        return castlingMoveList;
    }

    Piece PieceCast(Piece startingPiece, Vector2Int dir)
    {
        Vector2Int startPos = startingPiece.currentPos;
        for(Vector2Int searchPos = startPos + dir; startingPiece.board.IsPosOnBoard(searchPos); searchPos += dir)
        {
            Space searchSpace = startingPiece.board.GetSpace(searchPos);
            Piece foundPiece = searchSpace.piece;
            if(foundPiece != null)
            {
                return foundPiece;
            }
        }
        return null;
    }

    public List<MoveData> GetThreatMapMoves(Piece piece)
    {
        bool canGetThreatMap = piece.board != null && canCapture && (!firstMoveOnly || !piece.hasMoved);
        if (!canGetThreatMap)
        {
            return new List<MoveData>();
        }

        // Return move without filters to get all squares under attack by this piece.
        return GetMoves_Abstract(piece, isThreatMap: true);
    }

    List<MoveData> RemoveCheckMoves(List<MoveData> moveList, Piece piece)
    {
        List<MoveData> finalMoves = new List<MoveData>();

        int teamNum = piece.teamNumber;
        ChessGame mainGame = piece.chessGame;

        ChessGame testGame = piece.chessGame.CreateSimulatedCloneGame();

        ChessGameManager gameManager = MonoBehaviour.FindObjectOfType<ChessGameManager>();
        gameManager.checkTestStateList.Clear();

        foreach (MoveData move in moveList)
        {
            Vector2Int dest = move.dest;
            testGame.LoadState(mainGame.GetState());
            bool moveSuccesful = testGame.MakeMove(piece.currentPos, dest);

            // Testing
            StateData testState = testGame.GetState();
            gameManager.checkTestStateList.Add(testState);

            bool inCheck = testGame.teamInfo[teamNum].isInCheck;

            int stateNum = moveList.IndexOf(move);
            //Debug.Log("State " + stateNum + ", team 2 in check: " + testGame.teamInfo[2].isInCheck + ", team " + teamNum + ": " + inCheck);

            if (moveSuccesful && !inCheck)
            {
                finalMoves.Add(move);
            }
        }

        //gameManager.ShowTestStates();

        return finalMoves;
    }

    List<MoveData> RemoveDuplicateMoves(List<MoveData> moves, Piece piece)
    {
        List<MoveData> finalMoves = new List<MoveData>();
        moves.ForEach(x =>
        {
            if(!finalMoves.Contains(x))
            {
                finalMoves.Add(x);
            }
        });
        return finalMoves;
    }
    List<MoveData> FilterMoveAndCapture(List<MoveData> moves, Piece piece)
    {
        Board board = piece.board;
        for(int i = 0; i < moves.Count; i++)
        {
            Vector2Int movePos = moves[i].dest;
            Space moveSpace = board.GetSpace(movePos);
            bool spaceHasPiece = moveSpace.piece != null;

            if (spaceHasPiece)
            {
                bool sameTeam = piece.IsOnSameTeam(moveSpace.piece);
                if(sameTeam || !canCapture)
                {
                    // remove move since we cannot move into a piece on our team
                    moves.RemoveAt(i);
                    i--;
                    continue;
                }
            }
            else // Space doesnt have a piece
            {
                if (!canMove) // piece can't move unless it has a capture
                {
                    // Check for enpassant data

                    bool moveValid = false;
                    if(canTakeEnPassant && canCapture)
                    {
                        List<EnPassantData> enPassantList = piece.chessGame.FindEnPassants(movePos, piece);
                        if(enPassantList.Count > 0)
                        {
                            moveValid = true;
                            moves[i].OnMoveMade_Event += () =>
                            {
                                enPassantList.ForEach(x =>
                                {
                                    piece.chessGame.CapturePiece(x.piecePos);
                                });
                            };
                        }
                    }

                    if (!moveValid)
                    {
                        // remove current move since it is not a capture
                        moves.RemoveAt(i);
                        i--;
                        continue;
                    }
                }
            }
        }
        return moves;
    }

    protected Vector2Int PieceToBoardSpace(Piece piece, Vector2Int pos)
    {
        Board board = piece.board;
        Vector2Int forwardDir = board.GetPieceForwardDir(piece);
        Vector2Int pieceRelativePos = pos - piece.currentPos;
        
        if(forwardDir == Vector2Int.up)
        {
            // Do nothing
        }
        else if(forwardDir == Vector2Int.right)
        {
            int newX = pieceRelativePos.y;
            int newY = -pieceRelativePos.x;
            pieceRelativePos = new Vector2Int(newX, newY);
        } 
        else if(forwardDir == Vector2Int.down)
        {
            pieceRelativePos.y *= -1;
            pieceRelativePos.x *= -1;
        } 
        else if(forwardDir == Vector2Int.left)
        {
            int newX = -pieceRelativePos.y;
            int newY = pieceRelativePos.x;
            pieceRelativePos = new Vector2Int(newX, newY);
        }

        Vector2Int boardPos = piece.currentPos + pieceRelativePos;
        return boardPos;
    }
}

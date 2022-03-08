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

    public List<MoveData> GetMoves(Piece piece, Vector2Int? previousPos = null, bool removeCheckMoves = false)
    {
        bool canGetMove = piece.board != null && (!firstMoveOnly || !piece.hasMoved);
        if (!canGetMove)
        {
            return new List<MoveData>();
        }

        Board board = piece.board;
        List<MoveData> moves = GetMoves_Abstract(piece, previousPos);

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
            });
        }

        return moves;
    }
    protected abstract List<MoveData> GetMoves_Abstract(Piece piece, Vector2Int? previousPos = null, bool isThreatMap = false);

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

        ChessGame testGame = new ChessGame(mainGame.gameBoardList[0].boardSize, mainGame.playerCount);
        testGame.pieceMap = mainGame.pieceMap;

        ChessGameManager gameManager = MonoBehaviour.FindObjectOfType<ChessGameManager>();
        gameManager.checkTestStateList.Clear();

        foreach (MoveData move in moveList)
        {
            Vector2Int dest = move.dest;
            testGame.LoadState(mainGame.GetState());
            bool moveSuccesful = testGame.MakeMove(piece.currentPos, dest, isSimulation : true);

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
            else if(!canMove)
            {
                // remove current move since it is not a capture
                moves.RemoveAt(i);
                i--;
                continue;
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

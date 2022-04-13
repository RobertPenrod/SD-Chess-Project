using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessGame 
{
    public class TeamInfo
    {
        public bool isInCheck;
        List<Piece> pieceList;
        List<Piece> royalPieceList;
        public List<Vector2Int> threatMap;

        public TeamInfo()
        {
            isInCheck = false;
            pieceList = new List<Piece>();
            royalPieceList = new List<Piece>();
            threatMap = new List<Vector2Int>();
        }

        public void Clear()
        {
            isInCheck = false;
            pieceList.Clear();
            royalPieceList.Clear();
            threatMap.Clear();
        }

        public List<Piece> GetPieceList() => pieceList;
        public void AddPiece(Piece p)
        {
            pieceList.Add(p);
            if(p.isRoyal)
            {
                royalPieceList.Add(p);
            }
        }

        public void CalculateThreatMap()
        {
            threatMap.Clear();
            foreach(Piece p in pieceList)
            {
                List<Vector2Int> moves = p.GetThreatMap();
                threatMap.AddRange(moves);
            }

            // remove duplicate moves
            for(int i = 0; i < threatMap.Count; i++)
            {
                Vector2Int currentMove = threatMap[i];
                threatMap.RemoveAll((Vector2Int v) =>
                {
                    return v.x == currentMove.x && v.y == currentMove.y;
                });
                threatMap.Add(currentMove);
            }
        }

        public void UpdateInCheck(TeamInfo[] teams)
        {
            isInCheck = false;
            foreach(Piece p in royalPieceList)
            {
                for(int i = 0; i < teams.Length; i++)
                {
                    TeamInfo otherTeam = teams[i];
                    if (otherTeam == this) continue;

                    bool pieceInCheck = otherTeam.threatMap.Contains(p.currentPos);
                    if(pieceInCheck)
                    {
                        //Debug.Log("IN CHECK!");
                        isInCheck = true;
                    }
                }
            }
        }
    }

    public List<Board> gameBoardList = new List<Board>();
    public List<Piece> pieceMap = new List<Piece>();
    List<Piece> pieceList = new List<Piece>();
    public TeamInfo[] teamInfo;

    public int playerCount { get; private set; }
    public int turnIndex { get; private set; }
    public int turnCount { get; private set; }
    MoveData lastMove;

    // Events
    public Action OnEndTurn_Event;

    public ChessGame(Vector2Int? boardSize = null, int playerCount = 2)
    {
        Vector2Int initBoardSize = (Vector2Int)(boardSize == null ? new Vector2Int(8, 8) : boardSize);
        Board newBoard = new Board(initBoardSize);
        gameBoardList.Add(newBoard);

        this.playerCount = playerCount;
        turnIndex = 1;
        turnCount = 1;
        InitTeamInfo();
        UpdateGameInfo();
    }

    #region State
    public StateData GetState()
    {
        StateData stateData = new StateData();
        stateData.turnIndex = turnIndex;
        stateData.turnCount = turnCount;
        stateData.lastMove = lastMove?.GetClone();
        stateData.BoardValuePlayerOne = GetBoardValue(1);
        stateData.BoardValuePlayerTwo = GetBoardValue(2);

        foreach (Piece p in pieceList)
        {
            // Construct piece data
            PieceData pieceData = new PieceData();
            int boardIndex = p.board == null ? -1 : gameBoardList.IndexOf(p.board);
            pieceData.mapIndex = p.mapIndex;
            pieceData.team = p.teamNumber;
            pieceData.boardIndex = boardIndex;
            pieceData.position = p.currentPos;
            pieceData.hasMoved = p.hasMoved;

            // Add pieceData to stateData
            stateData.pieceData.Add(pieceData);
        }
        return stateData;
    }

    public void LoadState(StateData stateData)
    {
        ClearState();

        turnIndex = stateData.turnIndex;
        turnCount = stateData.turnCount;
        lastMove = stateData.lastMove?.GetClone();

        foreach(PieceData pData in stateData.pieceData)
        {
            CreatePieceFromPieceData(pData);
        }

        UpdateGameInfo();
    }

    void CreatePieceFromPieceData(PieceData pieceData)
    {
        if (pieceData == null || pieceData.boardIndex != 0) return;

        Piece basePiece = pieceMap[pieceData.mapIndex];
        Piece newPiece = MonoBehaviour.Instantiate(basePiece);
        newPiece.AddToBoard(gameBoardList[pieceData.boardIndex], pieceData.position);
        newPiece.teamNumber = pieceData.team;
        newPiece.mapIndex = pieceData.mapIndex;
        AddPiece(newPiece);
    }

    void ClearState()
    {
        pieceList.Clear();
        turnIndex = 1;
        turnCount = 1;
        gameBoardList.ForEach(x => x.Clear());
        foreach(TeamInfo tInfo in teamInfo)
        {
            tInfo.Clear();
        }
    }
    #endregion


    void InitTeamInfo()
    {
        teamInfo = new TeamInfo[playerCount+1];
        for(int i = 0; i < teamInfo.Length; i++)
        {
            teamInfo[i] = new TeamInfo();
        }
    }

    public void AddPiece(Piece p)
    {
        pieceList.Add(p);
        teamInfo[p.teamNumber].AddPiece(p);
        p.chessGame = this;
    }

    public void EndTurn()
    {
        UpdateGameInfo();
        StartNextTurn();
        OnEndTurn_Event?.Invoke();
    }

    void UpdateGameInfo()
    {
        UpdateAllThreatMaps();
        UpdatePiecesInCheck();
    }

    void StartNextTurn()
    {
        turnCount++;
        turnIndex = ((turnIndex + 1) % (playerCount+1));
        if (turnIndex == 0) turnIndex++;
    }

    void UpdateAllThreatMaps()
    {
        for(int i = 0; i < teamInfo.Length; i++)
        {
            teamInfo[i].CalculateThreatMap();
        }
    }

    void UpdatePiecesInCheck()
    {
        for(int i = 0; i < teamInfo.Length; i++)
        {
            teamInfo[i].UpdateInCheck(teamInfo);
        }
    }

    public List<Piece> GetAllPieces()
    {
        return pieceList;
    }

    public List<Piece> GetEnemyPieces(int teamIndex)
    {
        List<Piece> totalPieces = new List<Piece>();
        for(int i = 0; i < teamInfo.Length; i++)
        {
            if (i == teamIndex) continue;
            totalPieces.AddRange(teamInfo[i].GetPieceList());
        }
        return totalPieces;
    }

    // Returns value of player's team pieces - enemy's team pieces. 
    // Higher value is good for player.
    public int GetBoardValue(int teamIndex)
    {
        int boardValue = 0;
        List<Piece> totalPieces = GetAllPieces();
        for (int i = 0; i < totalPieces.Count; i++)
        {
            if (totalPieces[i].teamNumber == teamIndex) boardValue += totalPieces[i].pointValue;
            else boardValue -= totalPieces[i].pointValue;

        }
        return boardValue;
    }

    public bool MakeMove(Vector2Int start, Vector2Int dest, int boardNum = 0, bool isSimulation = false)
    {
        Board moveBoard = gameBoardList[boardNum];
        Piece movePiece = moveBoard.GetSpace(start)?.piece;
        if (movePiece == null) return false;

        List<MoveData> moveDataList = movePiece.GetMoves(isSimulation : isSimulation);
        MoveData moveMade = null;
        foreach (MoveData move in moveDataList)
        {
            if (move.dest == dest)
            {
                moveMade = move;
                break;
            }
        }
        if (moveMade == null)
        {
            Debug.LogWarning("Invalid Move from: " + start + " to " + dest);
            return false;
        }

        bool succesfulMove = movePiece.MovePiece(dest.x, dest.y);

        if(succesfulMove)
        {
            moveMade.OnMoveMade_Event?.Invoke();
            lastMove = new MoveData(movePiece, start, dest);
        }

        EndTurn();
        return succesfulMove;
    }

    public ChessGame CreateSimulatedCloneGame()
    {
        ChessGame testGame = new ChessGame(gameBoardList[0].boardSize, playerCount);
        testGame.pieceMap = pieceMap;
        testGame.LoadState(GetState());
        return testGame;
    }
}

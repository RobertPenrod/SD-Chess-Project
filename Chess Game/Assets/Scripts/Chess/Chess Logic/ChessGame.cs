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
        public bool HasLost;

        public TeamInfo()
        {
            isInCheck = false;
            pieceList = new List<Piece>();
            royalPieceList = new List<Piece>();
            threatMap = new List<Vector2Int>();
            HasLost = false;
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

            // Variant settings
            if (pieceList.Count > 0)
            {
                ChessGame chessGame = pieceList[0]?.chessGame;
                int teamNum = pieceList[0].teamNumber;
                if (chessGame.OnCalculateThreatMap_Event != null)
                {
                    threatMap = chessGame.OnCalculateThreatMap_Event.Invoke(new CalculateThreatMapEventData(teamNum, threatMap));
                }
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

                    if (otherTeam.threatMap == null) continue;

                    bool pieceInCheck = otherTeam.threatMap.Contains(p.currentPos);
                    if(pieceInCheck)
                    {
                        //Debug.Log("IN CHECK!");
                        isInCheck = true;
                    }
                }
            }
        }

        public List<MoveData> GetMoves()
        {
            List<MoveData> moveList = new List<MoveData>();
            foreach (Piece p in pieceList)
            {
                moveList.AddRange(p.GetMoves());
            }
            return moveList;
        }

        public int GetUncapturedRoyalCount()
        {
            int count = 0;
            foreach(Piece p in royalPieceList)
            {
                if (p.board != null)
                    count++;
            }
            return count;
        }

        public int GetUncapturedPieceCount()
        {
            int count = 0;
            foreach (Piece p in pieceList)
            {
                if (p.board != null)
                    count++;
            }
            return count;
        }
    }

    public MatchSettings MatchSettings;

    public List<Board> gameBoardList = new List<Board>();
    public List<Piece> pieceMap = new List<Piece>();
    List<Piece> pieceList = new List<Piece>();
    public TeamInfo[] teamInfo;

    public bool IsGameOver { get; private set; }

    public int playerCount { get; private set; }
    public int turnIndex { get; private set; }
    public int turnCount { get; private set; }

    public bool IsSimulation = false;

    MoveData lastMove;

    public List<EnPassantData> enPassantDataList = new List<EnPassantData>();

    // Events
    public Action OnEndTurn_Event;
    public Action<CaptureEventData> OnCapture_Event;
    public Func<CalculateThreatMapEventData, List<Vector2Int>> OnCalculateThreatMap_Event;

    public class CaptureEventData
    {
        public Vector2Int CapturePos;
        public Piece PieceToCapture;
        public Piece AttackingPiece;

        public CaptureEventData(Piece attackingPiece, Piece pieceToCapture, Vector2Int capturePos)
        {
            CapturePos = capturePos;
            PieceToCapture = pieceToCapture;
            AttackingPiece = attackingPiece;
        }
    };

    public class CalculateThreatMapEventData
    {
        public int TeamNumber;
        public List<Vector2Int> ThreatMap;

        public CalculateThreatMapEventData(int teamNumber, List<Vector2Int> threatMap) 
        {
            TeamNumber = teamNumber;
            ThreatMap = threatMap;
        }
    }

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

    public ChessGame(ChessGame chessGame)
    {
        Vector2Int initBoardSize = chessGame.gameBoardList[0].boardSize;
        Board newBoard = new Board(initBoardSize);
        gameBoardList.Add(newBoard);

        pieceMap.AddRange(chessGame.pieceMap);

        this.playerCount = chessGame.playerCount;
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

        // Piece Data
        foreach(Piece p in pieceList)
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

        // En Passant Data
        foreach(EnPassantData enPassantData in enPassantDataList)
        {
            stateData.enPassantDataList.Add(new EnPassantData(enPassantData));
        }

        return stateData;
    }

    public void LoadState(StateData stateData)
    {
        ClearState();

        turnIndex = stateData.turnIndex;
        turnCount = stateData.turnCount;
        lastMove = stateData.lastMove?.GetClone();

        // Load Piece Data
        foreach(PieceData pData in stateData.pieceData)
        {
            CreatePieceFromPieceData(pData);
        }

        // Load En Passant Data
        enPassantDataList.Clear();
        foreach(EnPassantData loadedEnPassantData in stateData.enPassantDataList)
        {
            enPassantDataList.Add(new EnPassantData(loadedEnPassantData));
        }

        UpdateGameInfo();
    }

    void CreatePieceFromPieceData(PieceData pieceData)
    {
        if (pieceData == null || pieceData.boardIndex != 0) return;

        Piece basePiece = pieceMap[pieceData.mapIndex];
        Piece newPiece = MonoBehaviour.Instantiate(basePiece);
        //Debug.Log("Instantiate Piece");
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


    public TeamInfo GetPiecesTeamInfo(Piece p)
    {
        return teamInfo[p.teamNumber];
    }

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
        CheckForPromotion();
        UpdateGameInfo();
        StartNextTurn();
        CheckWinCondition();
        CleanEnPassantData();
        OnEndTurn_Event?.Invoke();
    }

    void CheckForPromotion()
    {
        if (lastMove == null) return;
        if (!lastMove.piece.isPromotable) return;
        if (lastMove.piece.board == null) return;
        Vector2Int boardCheckPos = lastMove.piece.board.GetPieceForwardDir(lastMove.piece) + lastMove.piece.currentPos;
        if (lastMove.piece.board.IsPosOnBoard(boardCheckPos)) return;

        lastMove.piece.Promote();
    }

    void CleanEnPassantData()
    {
        for(int i = 0; i < enPassantDataList.Count; i++)
        {
            EnPassantData data = enPassantDataList[i];
            if(data.enPassantablePiece.teamNumber == turnIndex)
            {
                enPassantDataList.RemoveAt(i);
                i--;
                continue;
            }
        }
    }

    public void UpdateGameInfo()
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

    public bool MakeMove(Vector2Int start, Vector2Int dest, int boardNum = 0, bool doEndTurn = true)
    {
        Board moveBoard = gameBoardList[boardNum];
        Piece movePiece = moveBoard.GetSpace(start)?.piece;
        if (movePiece == null)
        {
            Debug.Log("Make Move Error: movePiece is null");
            return false;
        }

        List<MoveData> moveDataList = movePiece.GetMoves();
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
            Debug.Log("Invalid Move from: " + start + " to " + dest);
            return false;
        }

        bool succesfulMove = movePiece.MovePiece(dest.x, dest.y);

        if(succesfulMove)
        {
            moveMade.OnMoveMade_Event?.Invoke();
            lastMove = new MoveData(movePiece, start, dest);
        }
        else
        {
            Debug.Log("Move not succesful");
        }

        if (doEndTurn)
        {
            EndTurn();
        }
        return succesfulMove;
    }

    public bool ForceMove(Vector2Int start, Vector2Int dest, int boardNum = 0)
    {
        Board board = gameBoardList[boardNum];
        Space space = board.GetSpace(start);
        if (space == null) return false;
        Piece movePiece = space.piece;
        if (movePiece == null) return false;
        return movePiece.MovePiece(dest);
    }

    public List<EnPassantData> FindEnPassants(Vector2Int pos, Piece passantingPiece)
    {
        List<EnPassantData> foundEnPassants = new List<EnPassantData>();
        foreach (EnPassantData enPassantData in enPassantDataList)
        {
            if (enPassantData.capturePos == pos && !passantingPiece.IsOnSameTeam(enPassantData.enPassantablePiece))
            {
                foundEnPassants.Add(enPassantData);
            }
        }
        return foundEnPassants;
    }

    public void CapturePiece(Piece p, Piece attackingPiece)
    {
        OnCapture_Event?.Invoke(new CaptureEventData(attackingPiece, p, p.currentPos));
        p.RemoveFromBoard();
    }

    public void CapturePiece(Vector2Int piecePos, Piece attackingPiece, int boardIndex = 0)
    {
        Piece pieceToCapture = gameBoardList[boardIndex].GetSpace(piecePos).piece;
        if (pieceToCapture != null)
        {
            CapturePiece(pieceToCapture, attackingPiece);
        }
        else
        {
            Debug.Log("Tried to capture piece at " + piecePos + " but no piece found");
        }
    }
    
    public ChessGame CreateSimulatedCloneGame()
    {
        //ChessGame testGame = new ChessGame(gameBoardList[0].boardSize, playerCount);
        ChessGame testGame = new ChessGame();
        testGame.InitializeFromMatchSettings(MatchSettings);

        testGame.IsSimulation = true;
        testGame.pieceMap = pieceMap;
        testGame.LoadState(GetState());
        return testGame;
    }

    public void InitializeFromMatchSettings(MatchSettings matchSettings)
    {
        MatchSettings = matchSettings;

        // Atomic Captures
        if (MatchSettings.AtomicCaptures)
        {
            AtomicCapturesRule atomicCapturesRule = new AtomicCapturesRule();
            atomicCapturesRule.BindGame(this);
        }
    }

    void CheckWinCondition()
    {
        MatchSettings.GoalType goal = MatchSettings.Goal;
        TeamInfo teamStartingTurn = teamInfo[turnIndex];

        if(goal == MatchSettings.GoalType.Checkmate)
        {
            if(teamStartingTurn.GetMoves().Count == 0)
            {
                // team turnIndex loses
                teamStartingTurn.HasLost = true;
            }
        }
        else if(goal == MatchSettings.GoalType.CaptureKings)
        {
            if(teamStartingTurn.GetUncapturedRoyalCount() == 0)
            {
                teamStartingTurn.HasLost = true;
            }
        }
        else if(goal == MatchSettings.GoalType.CaptureAllPieces)
        {
            if(teamStartingTurn.GetUncapturedPieceCount() == 0)
            {
                teamStartingTurn.HasLost = true;
            }
        }

        List<int> remainingTeamNums = new List<int>();
        for(int i = 1; i < teamInfo.Length; i++)
        {
            if(!teamInfo[i].HasLost)
            {
                remainingTeamNums.Add(i);
            }
        }

        if(remainingTeamNums.Count == 1)
        {
            IsGameOver = true;
            if (!IsSimulation)
            {
                Debug.Log("Team " + remainingTeamNums[0] + " Wins!");
                GameObject.FindObjectOfType<GameOverMenu>().GameOver(remainingTeamNums[0]);
            }
        }
    }
}

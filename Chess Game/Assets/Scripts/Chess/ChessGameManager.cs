using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessGameManager : MonoBehaviour
{
    public Transform PieceHolder;
    public BoardGO BoardGO;

    [Header("Pieces")]
    public GameObject piecePrefab;
    public Piece pawn;
    public Piece rook;
    public Piece bishop;
    public Piece knight;
    public Piece king;
    public Piece queen;

    public ChessGame ChessGame { get; private set; }

    MatchSettingsGO _matchSettingsGO;
    MatchSettings _matchSettings;

    private void Awake()
    {
        FindMatchSettings();
        InitializeGame();
    }

    void FindMatchSettings()
    {
        _matchSettingsGO = FindObjectOfType<MatchSettingsGO>();
        if (_matchSettingsGO == null)
        {
            Debug.LogWarning("Local Game Started but no Match Settings Found.");
            _matchSettings = new MatchSettings();
        }
        else
        {
            _matchSettings = _matchSettingsGO.MatchSettings;
        }
    }

    void InitializeGame()
    {
        ChessGame = new ChessGame();
        BoardGO.BindBoard(ChessGame.gameBoardList[0]);
        CenterCamera();

        // Initialize Default gameState
        StateData initData = CreateDefaultInitStateData();
        ChessGame.LoadState(initData);
        ChessGame.LoadState(ChessGame.GetState());
        CreateGraphicalPiecesForGameData(ChessGame, PieceHolder);

        ChessGame.InitializeFromMatchSettings(_matchSettings);

        InitializePlayers();
    }

    void InitializePlayers()
    {
        ChessInputSystem inputSystem = FindObjectOfType<ChessInputSystem>();

        if (!SetupAI(_matchSettings.PlayerType1, 1))
        {
            inputSystem.selectableTeamList.Add(1);
        }

        if(!SetupAI(_matchSettings.PlayerType2, 2))
        {
            inputSystem.selectableTeamList.Add(2);
        }
    }

    bool SetupAI(MatchSettings.PlayerType playerType, int teamNum)
    {
        GameObject aiObject = null;
        ChessAI playerAI = null;
        if (playerType == MatchSettings.PlayerType.AI_AlphaBeta)
        {
            aiObject = new GameObject();
            aiObject.name = "AI_AlphaBeta_" + teamNum;
            playerAI = aiObject.AddComponent<AlphaBetaAI>();
        }
        else if (playerType == MatchSettings.PlayerType.AI_Random)
        {
            aiObject = new GameObject();
            aiObject.name = "AI_Random_" + teamNum;
            playerAI = aiObject.AddComponent<RandomAI>();
        }
        else if(playerType == MatchSettings.PlayerType.AI_RandomGreedy)
        {
            aiObject = new GameObject();
            aiObject.name = "AI_RandomGreedy" + teamNum;
            playerAI = aiObject.AddComponent<RandomGreedyAI>();
        }

        if (playerAI != null)
        {
            playerAI.Setup(this, teamNum);
            return true;
        }
        return false;
    }


    void CenterCamera()
    {
        Vector2Int boardSize = ChessGame.gameBoardList[0].boardSize;
        Vector3 boardOffset = (Vector3)((Vector2)boardSize * 0.5f);
        Vector3 depthOffset = Vector3.forward * Camera.main.transform.position.z;
        Vector3 tileOffset = -(Vector3)Vector2.one * 0.5f;
        Camera.main.transform.position = boardOffset + tileOffset + depthOffset;
    }

    StateData CreateDefaultInitStateData()
    {
        // Create Default Piecemap
        List<Piece> pieceMap = new List<Piece>();
        pieceMap.AddRange(new Piece[]
        {
            pawn,
            rook,
            knight,
            bishop,
            queen,
            king,
        });

        // Should probably load piecemap into game somewhere else..
        ChessGame.pieceMap = pieceMap;

        StateData stateData = new StateData();
        stateData.pieceData.Add(new PieceData(1, 1, new Vector2Int(0, 0)));
        stateData.pieceData.Add(new PieceData(2, 1, new Vector2Int(1, 0)));
        stateData.pieceData.Add(new PieceData(3, 1, new Vector2Int(2, 0)));
        stateData.pieceData.Add(new PieceData(4, 1, new Vector2Int(3, 0)));
        stateData.pieceData.Add(new PieceData(5, 1, new Vector2Int(4, 0)));
        stateData.pieceData.Add(new PieceData(3, 1, new Vector2Int(5, 0)));
        stateData.pieceData.Add(new PieceData(2, 1, new Vector2Int(6, 0)));
        stateData.pieceData.Add(new PieceData(1, 1, new Vector2Int(7, 0)));

        stateData.pieceData.Add(new PieceData(0, 1, new Vector2Int(0, 1)));
        stateData.pieceData.Add(new PieceData(0, 1, new Vector2Int(1, 1)));
        stateData.pieceData.Add(new PieceData(0, 1, new Vector2Int(2, 1)));
        stateData.pieceData.Add(new PieceData(0, 1, new Vector2Int(3, 1)));
        stateData.pieceData.Add(new PieceData(0, 1, new Vector2Int(4, 1)));
        stateData.pieceData.Add(new PieceData(0, 1, new Vector2Int(5, 1)));
        stateData.pieceData.Add(new PieceData(0, 1, new Vector2Int(6, 1)));
        stateData.pieceData.Add(new PieceData(0, 1, new Vector2Int(7, 1)));

        stateData.pieceData.Add(new PieceData(1, 2, new Vector2Int(0, 7)));
        stateData.pieceData.Add(new PieceData(2, 2, new Vector2Int(1, 7)));
        stateData.pieceData.Add(new PieceData(3, 2, new Vector2Int(2, 7)));
        stateData.pieceData.Add(new PieceData(4, 2, new Vector2Int(3, 7)));
        stateData.pieceData.Add(new PieceData(5, 2, new Vector2Int(4, 7)));
        stateData.pieceData.Add(new PieceData(3, 2, new Vector2Int(5, 7)));
        stateData.pieceData.Add(new PieceData(2, 2, new Vector2Int(6, 7)));
        stateData.pieceData.Add(new PieceData(1, 2, new Vector2Int(7, 7)));

        stateData.pieceData.Add(new PieceData(0, 2, new Vector2Int(0, 6)));
        stateData.pieceData.Add(new PieceData(0, 2, new Vector2Int(1, 6)));
        stateData.pieceData.Add(new PieceData(0, 2, new Vector2Int(2, 6)));
        stateData.pieceData.Add(new PieceData(0, 2, new Vector2Int(3, 6)));
        stateData.pieceData.Add(new PieceData(0, 2, new Vector2Int(4, 6)));
        stateData.pieceData.Add(new PieceData(0, 2, new Vector2Int(5, 6)));
        stateData.pieceData.Add(new PieceData(0, 2, new Vector2Int(6, 6)));
        stateData.pieceData.Add(new PieceData(0, 2, new Vector2Int(7, 6)));
        return stateData;
    }

    void CreateGraphicalPiecesForGameData(ChessGame game, Transform holder)
    {
        // Delete old graphics if any
        foreach (Transform t in holder)
        {
            Destroy(t.gameObject);
        }

        // Create new graphics
        foreach (Piece p in game.GetAllPieces())
        {
            GameObject newGO = Instantiate(piecePrefab);
            PieceGO pieceGO = newGO.GetComponent<PieceGO>();
            pieceGO.BindPiece(p);

            pieceGO.transform.SetParent(holder);
        }
    }
}

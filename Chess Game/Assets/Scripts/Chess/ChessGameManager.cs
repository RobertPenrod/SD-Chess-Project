using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessGameManager : MonoBehaviour
{
    public Transform pieceHolder;
    public BoardGO boardGO;

    [Header("Pieces")]
    public GameObject piecePrefab;
    public Piece pawn;
    public Piece rook;
    public Piece bishop;
    public Piece knight;
    public Piece king;
    public Piece queen;

    public ChessGame chessGame { get; private set; }

    private void OnDrawGizmos()
    {
        if (chessGame == null || chessGame.teamInfo == null) return;

        Gizmos.color = Color.red;
        List<Vector2Int> threatMap = chessGame.teamInfo[1].threatMap;
        foreach(Vector2Int v in threatMap)
        {
            Vector3 pos = new Vector3(v.x, v.y, 0);
            Gizmos.DrawWireSphere(pos, 0.25f);
        }
    }

    private void Awake()
    {
        chessGame = new ChessGame();
        boardGO.BindBoard(chessGame.gameBoardList[0]);
        CenterCamera();

        // Initialize Default gameState
        StateData initData = CreateDefaultInitStateData();
        chessGame.LoadState(initData);
        chessGame.LoadState(chessGame.GetState());
        CreateGraphicalPiecesForGameData(chessGame, pieceHolder);
    }

    void CenterCamera()
    {
        Vector2Int boardSize = chessGame.gameBoardList[0].boardSize;
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
        chessGame.pieceMap = pieceMap;

        StateData stateData = new StateData();
        stateData.pieceData.Add(new PieceData(1, 1, new Vector2Int(0, 0)));
        //stateData.pieceData.Add(new PieceData(2, 1, new Vector2Int(1, 0)));
        //stateData.pieceData.Add(new PieceData(3, 1, new Vector2Int(2, 0)));
        stateData.pieceData.Add(new PieceData(4, 1, new Vector2Int(3, 0)));
        stateData.pieceData.Add(new PieceData(5, 1, new Vector2Int(4, 0)));
        // stateData.pieceData.Add(new PieceData(3, 1, new Vector2Int(5, 0)));
        //stateData.pieceData.Add(new PieceData(2, 1, new Vector2Int(6, 0)));
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
        //stateData.pieceData.Add(new PieceData(2, 2, new Vector2Int(1, 7)));
        //stateData.pieceData.Add(new PieceData(3, 2, new Vector2Int(2, 7)));
        stateData.pieceData.Add(new PieceData(4, 2, new Vector2Int(3, 7)));
        stateData.pieceData.Add(new PieceData(5, 2, new Vector2Int(4, 7)));
        //stateData.pieceData.Add(new PieceData(3, 2, new Vector2Int(5, 7)));
        //stateData.pieceData.Add(new PieceData(2, 2, new Vector2Int(6, 7)));
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
        foreach(Transform t in holder)
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




    // Check testing
    public Transform testStateHolder;
    public List<StateData> checkTestStateList = new List<StateData>();
    public void ShowTestStates()
    {
        // Clear testStateHolder
        foreach(Transform t in testStateHolder)
        {
            Destroy(t.gameObject);
        }
        Debug.Log("----TEST----");

        int stateNum = 0;
        foreach (StateData testState in checkTestStateList)
        {
            ChessGame testGame = new ChessGame(chessGame.gameBoardList[0].boardSize, chessGame.playerCount);
            testGame.pieceMap = chessGame.pieceMap;
            testGame.LoadState(testState);

            Transform holder = new GameObject().transform;
            holder.SetParent(testStateHolder);
            holder.name = "State " + stateNum;
            float x = (stateNum + 1f) * 9f;
            holder.position = new Vector3(x, 0, 0);

            Debug.Log("State " + stateNum + ", team 2 in check: " + testGame.teamInfo[2].isInCheck);
            CreateGraphicalPiecesForGameData(testGame, holder);
            stateNum++;
        }
    }
}

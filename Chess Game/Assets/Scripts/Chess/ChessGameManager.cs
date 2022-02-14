using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessGameManager : MonoBehaviour
{
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

    private void Awake()
    {
        chessGame = new ChessGame();
        boardGO.BindBoard(chessGame.board);
        CenterCamera();

        InitPieces();
        BindGameObjectPiecesToData();
    }

    void CenterCamera()
    {
        Vector2Int boardSize = chessGame.board.boardSize;
        Vector3 boardOffset = (Vector3)((Vector2)boardSize * 0.5f);
        Vector3 depthOffset = Vector3.forward * Camera.main.transform.position.z;
        Vector3 tileOffset = -(Vector3)Vector2.one * 0.5f;
        Camera.main.transform.position = boardOffset + tileOffset + depthOffset;
    }

    void InitPieces()
    {
        // Player 1
        CreatePiece(rook, 0, 0, 1);
        CreatePiece(knight, 1, 0, 1);
        CreatePiece(bishop, 2, 0, 1);
        CreatePiece(queen, 3, 0, 1);
        CreatePiece(king, 4, 0, 1);
        CreatePiece(bishop, 5, 0, 1);
        CreatePiece(knight, 6, 0, 1);
        CreatePiece(rook, 7, 0, 1);

        CreatePiece(pawn, 0, 1, 1);
        CreatePiece(pawn, 1, 1, 1);
        CreatePiece(pawn, 2, 1, 1);
        CreatePiece(pawn, 3, 1, 1);
        CreatePiece(pawn, 4, 1, 1);
        CreatePiece(pawn, 5, 1, 1);
        CreatePiece(pawn, 6, 1, 1);
        CreatePiece(pawn, 7, 1, 1);

        // Player 2
        CreatePiece(rook, 0, 7, 2);
        CreatePiece(knight, 1, 7, 2);
        CreatePiece(bishop, 2, 7, 2);
        CreatePiece(queen, 3, 7, 2);
        CreatePiece(king, 4, 7, 2);
        CreatePiece(bishop, 5, 7, 2);
        CreatePiece(knight, 6, 7, 2);
        CreatePiece(rook, 7, 7, 2);

        CreatePiece(pawn, 0, 6, 2);
        CreatePiece(pawn, 1, 6, 2);
        CreatePiece(pawn, 2, 6, 2);
        CreatePiece(pawn, 3, 6, 2);
        CreatePiece(pawn, 4, 6, 2);
        CreatePiece(pawn, 5, 6, 2);
        CreatePiece(pawn, 6, 6, 2);
        CreatePiece(pawn, 7, 6, 2);
    }

    Piece CreatePiece(Piece pieceBase, int x, int y, int teamNumber) => CreatePiece(pieceBase, new Vector2Int(x, y), teamNumber);
    Piece CreatePiece(Piece pieceBase, Vector2Int? pos = null, int teamNumber = 0)
    {
        Piece newPiece = Instantiate(pieceBase);
        if (pos != null)
            newPiece.AddToBoard(chessGame.board, pos.Value);
        newPiece.teamNumber = teamNumber;

        chessGame.pieceList.Add(newPiece);
        return newPiece;
    }

    void BindGameObjectPiecesToData()
    {
        foreach (Piece p in chessGame.pieceList)
        {
            GameObject newGO = Instantiate(piecePrefab);
            PieceGO pieceGO = newGO.GetComponent<PieceGO>();
            pieceGO.BindPiece(p);

            pieceGO.transform.SetParent(transform);
        }
    }
}

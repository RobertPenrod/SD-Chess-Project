using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessGame : MonoBehaviour
{
    public BoardGO boardGO;
    Board board;
    
    [Header("Testing")]
    public GameObject piecePrefab;
    public Piece testPiece1;
    public Piece testPiece2;

    List<Piece> pieceList = new List<Piece>();

    private void OnDrawGizmos()
    {
        if (pieceList == null || pieceList.Count <= 0) return;

        for (int i = 0; i < pieceList.Count; i++)
        {
            Color c = Color.HSVToRGB((i / 5f) % 1, 1f, 1f);
            Gizmos.color = new Color(c.r, c.g, c.b, 0.25f);
            
            List<Vector2Int> moves = pieceList[i].GetMoves();
            foreach (Vector2Int p in moves)
            {
                Vector3 pos = new Vector3(p.x, p.y, 0);
                Gizmos.DrawSphere(pos, 0.2f);
            }
        }
    }

    private void Awake()
    {
        board = new Board(new Vector2Int(25, 15));
        boardGO.BindBoard(board);

        // Create Piece Data
        Piece newPiece = CreatePiece(testPiece1);
        newPiece.AddToBoard(board, new Vector2Int(4, 8));
        newPiece.teamNumber = 2;

        newPiece = CreatePiece(testPiece2);
        newPiece.AddToBoard(board, new Vector2Int(4, 0));
        newPiece.teamNumber = 1;

        BindGameObjectPiecesToData();

        ExtensionMethods.RepeatingInvoke(1f, 1.5f, 0, this, () =>
        {
            Piece randomPiece = ExtensionMethods.RandomListItem(pieceList);
            List<Vector2Int> moveList = randomPiece.GetMoves();
            if (moveList.Count > 0)
            {
                Vector2Int randomMove = ExtensionMethods.RandomListItem(moveList);
                randomPiece.MovePiece(randomMove);
            }
        });
    }

    Piece CreatePiece(Piece pieceBase)
    {
        Piece newPiece = Instantiate(pieceBase);
        pieceList.Add(newPiece);
        return newPiece;
    }

    void BindGameObjectPiecesToData()
    {
        foreach(Piece p in pieceList)
        {
            GameObject newGO = Instantiate(piecePrefab);
            PieceGO pieceGO = newGO.GetComponent<PieceGO>();
            pieceGO.BindPiece(p);

            pieceGO.transform.SetParent(transform);
        }
    }
}

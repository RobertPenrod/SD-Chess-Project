using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessInputSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject selectedGFX;
    [SerializeField] GameObject moveIconPrefab;

    PieceGO selectedPieceUI;

    List<GameObject> moveIconList = new List<GameObject>();

    bool pieceMoved = false;

    ChessGameManager gameManager;

    List<Vector2Int> possibleMovePositions = new List<Vector2Int>();

    private void Awake()
    {
        gameManager = FindObjectOfType<ChessGameManager>();
    }

    private void Update()
    {
        pieceMoved = false;
        HandlePieceMovement();
        if (!pieceMoved)
        {
            HandlePieceSelection();
        }
    }

    void HandlePieceMovement()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (selectedPieceUI == null) return;
        if (selectedPieceUI.piece.teamNumber != gameManager.chessGame.turnIndex) return;

        Vector2Int mouseBoardPos = GetMouseBoardPos();
        if(possibleMovePositions.Contains(mouseBoardPos))
        {
            gameManager.chessGame.MakeMove(selectedPieceUI.piece.currentPos, mouseBoardPos);
            pieceMoved = true;
            SelectPiece(null);
        }
    }

    void HandlePieceSelection()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        PieceGO newPiece = GetMoseOverPiece();
        if (newPiece == null || newPiece == selectedPieceUI || newPiece.piece.teamNumber != gameManager.chessGame.turnIndex)
        {
            SelectPiece(null);
            return;
        }
        SelectPiece(newPiece);
    }

    void SelectPiece(PieceGO pieceToSelect)
    {
        selectedPieceUI = pieceToSelect;
        
        // Update GFX
        selectedGFX.SetActive(selectedPieceUI != null);
        if (selectedPieceUI != null)
        {
            Vector2Int pieceBoardPos = selectedPieceUI.piece.currentPos;
            Vector3 selectedGFXPos = new Vector3(pieceBoardPos.x, pieceBoardPos.y, selectedGFX.transform.position.z);
            selectedGFX.transform.position = selectedGFXPos;

            // Show move preview stuff
            List<MoveData> moves = selectedPieceUI.piece.GetMoves();
            ShowMovePreview(moves);

            possibleMovePositions.Clear();
            moves.ForEach(x => possibleMovePositions.Add(x.dest));
        }
        else
        {
            ClearMovePreview();
            possibleMovePositions.Clear();
        }
    }

    void ClearMovePreview()
    {
        while(moveIconList.Count > 0)
        {
            Destroy(moveIconList[0]);
            moveIconList.RemoveAt(0);
        }
    }

    void ShowMovePreview(List<MoveData> moves)
    {
        ClearMovePreview();
        foreach(MoveData move in moves)
        {
            Vector3 pos = new Vector3(move.dest.x, move.dest.y, moveIconPrefab.transform.position.z);
            GameObject newMoveIcon = Instantiate(moveIconPrefab, pos, Quaternion.identity, this.transform);
            moveIconList.Add(newMoveIcon);
        }
    }

    PieceGO GetMoseOverPiece()
    {
        Vector2 mousePos = GetMousePos();
        Collider2D overlapCollider = Physics2D.OverlapPoint(mousePos);
        PieceGO pieceUI = overlapCollider?.gameObject.GetComponent<PieceGO>();
        return pieceUI;
    }

    Vector2 GetMousePos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition); 
    }

    Vector2Int GetMouseBoardPos()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int mouseXInt = (int)(mousePos.x + 0.5f);
        int mouseYInt = (int)(mousePos.y + 0.5f);
        Vector2Int mousePosInt = new Vector2Int(mouseXInt, mouseYInt);
        return mousePosInt;
    }
}

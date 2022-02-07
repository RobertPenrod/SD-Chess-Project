using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess Logic/Piece")]
public class Piece : ScriptableObject
{
    public Sprite icon;
    public bool isRoyal;
    public Vector2Int currentPos;
    public int teamNumber;
    public MoveBehavior moveBehavior;
    public bool hasMoved { get; private set; }

    [HideInInspector] public Board board;

    public bool IsOnSameTeam(Piece otherPiece)
    {
        return teamNumber == otherPiece.teamNumber;
    }

    public void AddToBoard(Board newBoard, Vector2Int pos)
    {
        RemoveFromBoard(); // remove from previous board if applicable.

        board = newBoard;
        Space space = board.GetSpace(pos);
        if(space == null)
        {
            Debug.LogError("AddToBoard pos (" + pos + ") not in range");
            board = null;
            return;
        }

        currentPos = pos;
        space.piece = this;
        board.pieceList.Add(this);
    }

    public void RemoveFromBoard()
    {
        if (board == null) return;
        
        Space space = board.GetSpace(currentPos);
        if (space == null)
        {
            Debug.LogError("RemoveFromBoard pos (" + currentPos + ") not in range");
            return;
        }

        space.piece = null;
        board.pieceList.Remove(this);
        board = null;
        currentPos = new Vector2Int(-1, -1);
    }

    public bool MovePiece(int x, int y) => MovePiece(new Vector2Int(x, y));
    public bool MovePiece(Vector2Int newPos)
    {
        Space targetSpace = board?.GetSpace(newPos);
        if (targetSpace == null) { return false; }

        board.GetSpace(currentPos).piece = null;
        targetSpace.piece = this;
        currentPos = newPos;

        hasMoved = true;
        return true;
    }

    public List<Vector2Int> GetMoves()
    {
        return moveBehavior.GetMoves(this);
    }
}

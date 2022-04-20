using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardTiler))]
public class BoardGO : MonoBehaviour
{
    Board board;
    BoardTiler boardTiler;

    public void BindBoard(Board newBoard)
    {
        GetCompReferences();
        board = newBoard;
        boardTiler.TileCount = board.boardSize;
        boardTiler.InitBoard();
    }

    void GetCompReferences()
    {
        boardTiler = GetComponent<BoardTiler>();
    }
}

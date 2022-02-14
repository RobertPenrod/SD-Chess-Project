using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessGame
{
    public Board board;
    public List<Piece> pieceList = new List<Piece>();

    public int playerCount { get; private set; }
    public int turnIndex { get; private set; }
    public int turnCount { get; private set; }

    public ChessGame()
    {
        board = new Board(new Vector2Int(8,8));
        playerCount = 2;
        turnIndex = 1;
        turnCount = 1;
    }

    public void EndTurn()
    {
        StartNextTurn();
    }

    void StartNextTurn()
    {
        turnCount++;
        turnIndex = ((turnIndex + 1) % (playerCount+1));
        if (turnIndex == 0) turnIndex++;
    }
}

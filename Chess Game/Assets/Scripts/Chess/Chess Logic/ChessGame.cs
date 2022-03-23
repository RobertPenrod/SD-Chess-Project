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
    public int currentTurn { get; private set; }

    public ChessGame()
    {
        board = new Board(new Vector2Int(8,8));
        playerCount = 2;
        turnIndex = 1;
        turnCount = 1;
        currentPla = "w";
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

    public StateToFEN (Board board)
    {
        Space[,] spaces = board.spaces;
        string final = "";
        string currentFEN = "";
        int spacesWithoutPiece = 0;

        //portion for game board state
        for (int i = 0; i < spaces.Length; i++) {
            for (int j = 0; j < spaces[i].Length; j++) {
                Space current = spaces[i][j];
                if (current.piece != null) {
                    if (spacesWithoutPiece != 0) {
                        final += spacesWithoutPiece;
                        spacesWithoutPiece = 0;
                    }

                    string newFEN = current.fenChar;

                    // is 0 black and 1 white? thats the assumption i went with here
                    (current.teamNumber == 0) ? final += newFEN : final += newFEN.ToUpper();
                } else {
                    spacesWithoutPiece++;
                }
            }
            if (i != spaces.length) {
                final += "/";
            }
        }
        if (spacesWithoutPiece != 0) {
            final += spacesWithoutPiece;
        }

        //portion of who moves next
        if (turnCount % 2 == 0) {
            final += " w";
        } else {
            final += " b";
        }

        // portion of castling WIP
        final += (" " + "-");

        // portion of en passant WIP
        final += (" " + "-");

        // portion of half moves WIP
        final += (" " + "0");

        // portion of turns starting at 1
        final += (" " + (turnCount / 2).Ceiling());

        return final;
    }   
}

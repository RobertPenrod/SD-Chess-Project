using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChessAI : MonoBehaviour
{
    [SerializeField] ChessGameManager gameToPlay;
    [SerializeField] protected int teamNumber = 2;

    public abstract MoveData GetMove(ChessGame chessGame);

    private void Start()
    {
        // Wait a moment for the game to load before trying to make a move.
        ExtensionMethods.DelayedInvoke(2.5f, this, () =>
        {
            if (gameToPlay == null) return;

            // Is it this AI's move first?
            if(IsTurn(gameToPlay.chessGame))
            {
                MakeMove(gameToPlay.chessGame);
            }

            // Evertime a turn is ended check to see if it is this AI's turn.
            gameToPlay.chessGame.OnEndTurn_Event += () =>
            {
                if (!IsTurn(gameToPlay.chessGame)) return;

                // Add a delay so the AI does not play instantly
                ExtensionMethods.DelayedInvoke(1f, this, () =>
                {
                    MakeMove(gameToPlay.chessGame);
                });
            };
        });
    }

    bool IsTurn(ChessGame chessGame)
    {
        return chessGame.turnIndex == teamNumber;
    }

    void MakeMove(ChessGame chessGame)
    {
        MoveData moveToMake = GetMove(chessGame);
        if (moveToMake == null)
        {
            Debug.LogError("AI did not get a valid move");
            return;
        }

        chessGame.MakeMove(moveToMake.start, moveToMake.dest);
    }
}

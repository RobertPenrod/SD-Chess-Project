using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomicCapturesRule : ChessVariantRule
{
    ChessGame _chessGame;

    public override void BindGame(ChessGame chessGame)
    {
        _chessGame = chessGame;
        chessGame.OnCapture_Event += AtomicCaptureEvent;
        chessGame.OnCalculateThreatMap_Event += OnCalculateThreatMap;
    }

    void AtomicCaptureEvent(ChessGame.CaptureEventData eventData)
    {
        if (eventData.AttackingPiece == null) return;

        Debug.Log("AtomicCapture Rule Trigger @ " + eventData.CapturePos);

        Vector2Int capturePos = eventData.CapturePos;
        int attackingTeam = eventData.AttackingPiece.teamNumber;
        Board board = eventData.AttackingPiece.board;

        // Search in explosion Radius
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int searchPos = capturePos + new Vector2Int(x, y);


                Space space = board.GetSpace(searchPos);
                if (space == null)
                    continue; // Space isn't on board, continue.

                Piece foundPiece = space.piece;
                if (foundPiece == null)
                    continue; // No piece found, nothing to do.

                /*
                bool isFoundPieceEnemy = !foundPiece.IsOnSameTeam(eventData.AttackingPiece);
                if (!isFoundPieceEnemy)
                    continue; // Found piece is on same team, do nothing
                */

                if (foundPiece.isImmuneToAtomicCapture) continue;

                eventData.AttackingPiece.chessGame.CapturePiece(foundPiece, null);
            }
        }

        eventData.AttackingPiece.RemoveFromBoard();
    }

    List<Vector2Int> OnCalculateThreatMap(ChessGame.CalculateThreatMapEventData eventData)
    {
        // Add Explosions to the threat map
        List<Vector2Int> newThreatMap = new List<Vector2Int>();
        int teamNum = eventData.TeamNumber;
        List<Vector2Int> threatMap = eventData.ThreatMap;
        Board board = _chessGame.gameBoardList[0];

        foreach(Vector2Int threatPos in threatMap)
        {
            newThreatMap.Add(threatPos);

            Space space = board.GetSpace(threatPos);
            if (space == null)
                continue;

            Piece piece = space.piece;
            if (piece == null)
                continue;

            bool pieceIsEnemy = !piece.IsOnSameTeam(teamNum);
            if (!pieceIsEnemy)
                continue;

            // Found piece in threat map that is an enemy, add explosion threat around found enemy piece.
            for(int x = -1; x <= 1; x++)
            {
                for(int y = -1; y <= 1; y++)
                {
                    Vector2Int explosionThreatPos = threatPos + new Vector2Int(x, y);
                    if(board.IsPosOnBoard(explosionThreatPos) && !newThreatMap.Contains(explosionThreatPos) && !threatMap.Contains(explosionThreatPos))
                    {
                        newThreatMap.Add(explosionThreatPos);
                    }
                }
            }
        }

        return newThreatMap;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MatchSettings
{ 
    public enum GoalType { Checkmate, CaptureKings, CaptureAllPieces};
    public enum PlayerType { Human, AI_AlphaBeta, AI_Random, AI_RandomGreedy};

    public GoalType Goal = GoalType.Checkmate;
    public bool AtomicCaptures = false;
    public bool AliceChess = false;
    public PlayerType PlayerType1;
    public PlayerType PlayerType2;

    // Board settings
    // size, custom shape, ect..

    // Layout/Piece settings
    // pieces, starting positions, piecemap, ect..
}

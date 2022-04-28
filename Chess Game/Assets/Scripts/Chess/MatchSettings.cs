using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MatchSettings
{ 
    public enum GoalType { Checkmate, CaptureKings, CaptureAllPieces};

    public GoalType Goal = GoalType.Checkmate;
    public bool AtomicCaptures = false;
    public bool AliceChess = false;

    // Board settings
    // size, custom shape, ect..

    // Layout/Piece settings
    // pieces, starting positions, piecemap, ect..
}

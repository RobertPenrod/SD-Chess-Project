using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSettings
{
    public enum GoalType { Checkmate, Regicide, CaptureAllPieces};

    public GoalType goal = GoalType.Checkmate;
    public bool atomicCaptures = false;
    public bool aliceChess = false;

    // Board settings
    // size, custom shape, ect..

    // Layout/Piece settings
    // pieces, starting positions, piecemap, ect..
}

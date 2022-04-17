using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SearchNode 
{
    public StateData chessGame;
    public List<SearchNode> children = new List<SearchNode>();
    public MoveData currMove;

    public SearchNode(StateData chessGame, MoveData currMove)
    {
        this.chessGame = chessGame;
        this.currMove = currMove;
    }

    public int getValue()
    {
        if (this.currMove.piece.teamNumber == 1) return chessGame.BoardValuePlayerOne;
        else return chessGame.BoardValuePlayerTwo;
    }

    public List<SearchNode> getChildren()
    {
        return children;
    }

    public void AddChildren(SearchNode child)
    {
        children.Add(child);
    }
}

/*  
 *  Search tree our AI uses to search. Each node is a chess game, and its children are 
 *  chess games with possible moves of the opponent.
 */
public class SearchTree : MonoBehaviour
{
    ChessGame mainGame;
    SearchNode root;
    int depth;
    int i;

    public SearchTree(ChessGame rootGame, int depth)
    {
        this.mainGame = rootGame;
        this.root = new SearchNode(rootGame.GetState(), rootGame.GetState().lastMove);
        this.depth = depth;
        populate(mainGame, root, 0, opposite(root.chessGame.turnIndex));
    }

    public int opposite(int curPlayer)
    {
        if (curPlayer == 2 || curPlayer == null) return 1;
        else return 2;
    }

    public SearchNode getRoot()
    {
        return this.root;
    }

    /* Building the entire tree for even a low depth is way too much memory so the approach is:
     * If less than four moves possible, just make all of them children. Otherwise,
     * sort the children then call populate. 
    */ 
    public void populate(ChessGame parentGame, SearchNode node, int curDepth, int curPlayer)
    {
        if (curDepth > this.depth) return;

        // Loading in dummy chess game to get info from. 
        ChessGame dummyGame = parentGame.CreateSimulatedCloneGame();
        dummyGame.LoadState(node.chessGame);

        List<Piece> curPieces = dummyGame.GetEnemyPieces(curPlayer);
        for (int i = 0; i < curPieces.Count; i++)
        {
            List<MoveData> pieceMoves = curPieces[i].GetMoves();
            for (int j = 0; j < pieceMoves.Count; j++)
            {
                ChessGame placeHolder = dummyGame.CreateSimulatedCloneGame();
                placeHolder.MakeMove(pieceMoves[j].start, pieceMoves[j].dest);
                SearchNode searchNode = new SearchNode(placeHolder.GetState(), pieceMoves[j]);
                node.AddChildren(searchNode);
                populate(placeHolder, searchNode, curDepth + 1, opposite(curPlayer));
            }
        }
    }
}

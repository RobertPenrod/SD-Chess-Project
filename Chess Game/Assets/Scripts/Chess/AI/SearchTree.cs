using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SearchNode
{
    public StateData chessGame;
    public List<SearchNode> children;
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
}

/*  
 *  Search tree our AI uses to search. Each node is a chess game, and its children are 
 *  chess games with possible moves of the opponent.
 */
public class SearchTree : MonoBehaviour
{
    SearchNode root;
    int depth;

    public SearchTree(StateData rootGame, int depth)
    {
        this.root.chessGame = rootGame;
        this.root.children = null;
        this.depth = depth;
        populate(root, 0, opposite(rootGame.lastMove.piece.teamNumber));
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

    /*
     * Starting with current game and until depth is reached, 
     * turn enemy pieces' moves into children of current node.
     * Then recursively call method on each child.
     */
    public void populate(SearchNode node, int curDepth, int curPlayer)
    {
        if (curDepth == this.depth) return;

        // Loading in dummy chess game to get info from. 
        ChessGame dummyGame = new ChessGame();
        dummyGame.LoadState(node.chessGame);

        // Get enemy's pieces/moves and make them into nodes, children of node. 
        List<SearchNode> children = null;
        List<Piece> curPieces = dummyGame.GetEnemyPieces(curPlayer);
        for (int i = 0; i < curPieces.Count; i++)
        {
            List<MoveData> pieceMoves = curPieces[i].GetMoves();
            for (int j = 0; j < pieceMoves.Count; j++)
            {
                ChessGame placeHolder = dummyGame.CreateSimulatedCloneGame();
                placeHolder.MakeMove(pieceMoves[j].start, pieceMoves[j].dest);
                SearchNode searchNode = new SearchNode(placeHolder.GetState(), pieceMoves[j]);
                children.Add(searchNode);
                populate(searchNode, curDepth + 1, opposite(curPlayer));
            }
        }
    }

    /*  
     *  When a move is made (and we progress from the previous chess game),
     *  that move's node becomes the new root. 
     */
    public void newParent(MoveData newParent)
    {
        for (int i = 0; i < root.children.Count; i++)
        {
            if (newParent == root.children[i].currMove)
            {
                root = root.children[i];
                return;
            }
        }
    }
}

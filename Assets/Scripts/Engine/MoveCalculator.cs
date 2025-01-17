using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCalculator
{
    private Evaluation evaluator;
    private ChessBoard board;
    private Game.Side side;


    public MoveCalculator(Evaluation evaluator, ChessBoard board)
    {
        this.evaluator = evaluator;
        this.board = board;
    }

    /*
     *  Upper and Lower Bound Search (called Alpha and Beta.) 
     *  We maintain a lower bound because if a move is too bad we don't consider it. 
     *  But we also have to maintain an upper bound because if a move at depth 3 or higher leads 
     *  to a continuation that is too good, the other player won't allow it, because there was a 
     *  better move higher up on the game tree that he could have played to avoid this situation. 
     *  One player's lower bound is the other player's upper bound. 
     *  
     *  The algorithm is heavily dependent upon the order in which moves are searched.  
     *  If you consistently search the worst move first, a beta cutoff is never achieved
     *  It can be improved by introducing Move-Ordering Priority in the search(ie prioritizing looking at captures)
     *  
     *  Game side will be represented by boolean to quickly reverse it
     */
    public Move AlphaBetaSearch(int depth, int alpha, int beta, bool side)
    {
        if (depth == 0) return Evaluate();

        List<Move> moves = new List<Move>();

        if (side) moves = board.GetPossibleMovesWhite();
        else if(!side) moves = board.GetPossibleMovesBlack();

        //Check for game over status?

        foreach (Move m in moves)
        {
            //board.MakeMove(m);
            Move omega = AlphaBetaSearch(depth - 1, -beta, -alpha, !side);
            //board.UnmakeMove(m);

            if (omega >= beta) return beta;

            alpha = Math.Max(alpha, omega);

        }

        return alpha;
    }

    private int Evaluate()
    {

    }

    public Move GetRandomMove(List<Move> moves)
    {
        //For Testing, select a random move
        System.Random r = new System.Random();
        int num = r.Next(0, moves.Count);

        for(int i=0;i < moves.Count; i++)
        {
            if (i == num) return moves[i];
        }

        return new Move();
    }
}

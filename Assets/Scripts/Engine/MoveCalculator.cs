using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCalculator
{
    private Evaluation evaluator;
    private BitBoard board;
    private Game.Side side;

    private List<Move> line;

    public Move GetMove()
    {
        //GetLine();
        return line[line.Count - 1];
    }

    public List<Move> GetLine()
    {
        Debug.Log("Reading Engine Line...");
        for(int i = 0; i < line.Count; i++)
        {
            Debug.Log("Move:" + i + ": " + line[i].ToString());
        }
        return line;
    }

    //Make sure to reset the line before using alpha beta
    public void NewLine()
    {
        line = new List<Move>();
        line.Add(new Move());
        line.Add(new Move());
     //   line.Add(new Move());
      //  line.Add(new Move());

    }

    public MoveCalculator(Evaluation evaluator, BitBoard board)
    {
        this.line = new List<Move>();
        this.evaluator = evaluator;
        this.board = board;
    }

    /*
     *  Upper and Lower Bound Search (called Alpha and Beta.) 
     *  The idea is that two scores are passed around in the search.  
     *  The first one is alpha, which is the best score that can be forced by some means. 
     *  Anything worth less than this is of no use, because there is a strategy that 
     *  is known to result in a score of alpha.  Anything less than or equal to alpha is no
     *  improvement. The second score is beta. Beta is the worst-case scenario for the opponent. 
     *  It's the worst thing that the opponent has to endure, because it's known that there is a
     *  way for the opponent to force a situation no worse than beta, from the opponent's point of view. 
     *  If the search finds something that returns a score of beta or better, it's too good, so the side 
     *  to move is not going to get a chance to use this strategy.

     *  We maintain a lower bound because if a move is too bad we don't consider it. 
     *  But we also have to maintain an upper bound because if a move at depth 3 or higher leads 
     *  to a continuation that is too good, the other player won't allow it, because there was a 
     *  better move higher up on the game tree that they could have played to avoid this situation. 
     *  One player's lower bound is the other player's upper bound. 
     *  
     *  The algorithm is heavily dependent upon the order in which moves are searched.  
     *  If you consistently search the worst move first, a beta cutoff is never achieved
     *  It can be improved by introducing Move-Ordering Priority in the search(ie prioritizing looking at captures)
     */
    public float AlphaBetaSearch(int depth, float alpha, float beta, BitBoard board, bool maximizingPlayer)
    {
        if (depth == 0) return evaluator.GetEvaluation(board);

        List<Move> moves = new List<Move>();

        if (maximizingPlayer)
        {
            float maxEval = -10000;
            moves = board.GetPossibleMovesWhite();
            foreach (Move m in moves)
            {
                BitBoard temp = board.Copy();

                temp.MakeMove(m);
                float eval = AlphaBetaSearch(depth - 1, alpha, beta, temp, false);
                maxEval = Math.Max(maxEval, eval);

                if (eval > alpha)
                {
                    alpha = eval;
                    line[depth - 1] = m;
                }
                if (beta <= alpha) break; 
            }
            return maxEval;
        }
        else
        {
            moves = board.GetPossibleMovesBlack();
            float minEval = 10000;
            foreach (Move m in moves)
            {
                BitBoard temp = board.Copy();

                temp.MakeMove(m);
                float eval = AlphaBetaSearch(depth - 1, alpha, beta, temp, true);
                minEval = Math.Min(minEval, eval);

                if (eval < beta)
                {
                    beta = eval;
                    line[depth - 1] = m;
                }
                if (beta <= alpha) break;
            }
            return minEval;
        }
    }



    //Older variant of alpha beta, could not get black to make good moves on this version
    public float ZAlphaBetaSearch(int depth, float alpha, float beta, BitBoard board, bool side)
    {
        if (depth == 0) return evaluator.GetEvaluation(board);

        //if (line == null) line = this.line; //Initialize line if this is the first alpha beta call
        List<Move> moves = new List<Move>();

        if (side) moves = board.GetPossibleMovesWhite();
        else if(!side) moves = board.GetPossibleMovesBlack();

        //Check for game over status?/ Checks
        foreach (Move m in moves)
        {
            Debug.Log(m);
        }

        foreach (Move m in moves) { 
            BitBoard temp = board.Copy();
            // Debug.Log("depth" + depth + ", move:" + m);
             //if (m.capturedPiece == ' ') continue; //Skipping castle for now
            //Debug.Log("Depth: " + depth + "Making Move.. " + m + "Eval: " + eval + "\nAlpha: " + alpha + " Beta: " + beta);
            temp.MakeMove(m);
            float eval = -AlphaBetaSearch(depth - 1, -beta, -alpha, temp, !side);
            //board.UnmakeMove(m);
            //Debug.Log("Making Move.. " + m);
            //Debug.Log("eval value of move "  + eval);

            if (eval >= beta) return beta;

            if (eval > alpha)   //   alpha = Math.Max(alpha, eval);
            {
                alpha = eval;
                line[depth - 1] = m; //Replace the line
            }

        }

        return alpha;
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

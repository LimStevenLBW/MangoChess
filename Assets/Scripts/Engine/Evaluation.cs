using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evaluation
{
    private float whiteRating;
    private float blackRating;

    public ChessBoard board;
    private List<ChessBoardFile> files;

    public Evaluation(ChessBoard board)
    {
        this.board = board;
    }

    public float GetEvaluation()
    {
        float eval = 0;
        eval += GetMaterialEval();

        return eval;
    }

    private float GetMaterialEval()
    {
        int bRooks, bKnights, bBishops, bQueens, bPawns;
            bRooks = bKnights = bBishops = bQueens = bPawns = 0;
        int wRooks, wKnights, wBishops, wQueens, wPawns;
            wRooks = wKnights = wBishops = wQueens = wPawns = 0;

        float eval;

        files = board.GetFiles();

        #region Material Count
        for (int row = 0; row < 8; row++)
        {
            for(int col = 0; col < 8; col++)
            {
                char code = files[col].GetPieceCode(row);
                switch (code)
                {
                    case ('r'):
                        bRooks += 1;
                        break;
                    case ('n'):
                        bKnights += 1;
                        break;
                    case ('b'):
                        bBishops += 1;
                        break;
                    case ('p'):
                        bPawns += 1;
                        break;
                    case ('q'):
                        bQueens += 1;
                        break;

                    case ('R'):
                        wRooks += 1;
                        break;
                    case ('N'):
                        wKnights += 1;
                        break;
                    case ('B'):
                        wBishops += 1;
                        break;
                    case ('P'):
                        wPawns += 1;
                        break;
                    case ('Q'):
                        wQueens += 1;
                        break;
                    default:
                        break;
                }
            }
        }

        int bPieceCount = bRooks + bKnights + bBishops + bQueens; //Start:  7
        int wPieceCount = wRooks + wKnights + wBishops + wQueens;
        int totalPieces = bPieceCount + wPieceCount;
        #endregion

        //Pawn Material Value
        float pMult = Math.Abs(1.7f - totalPieces * 0.1f); //0.3 -> 1.7 in pawn value when piece count gets low
        float bPawnsValue = bPawns * pMult;
        float wPawnsValue = wPawns * pMult;

        //Rook Material Value
        float bRooksValue = bRooks * 5;
        float wRooksValue = wRooks * 5;

        //Knight Material Value
        float bKnightsValue = bKnights * 3f;
            if (wPawns > 6) bKnightsValue += 0.1f; //At high pawn counts, the knight is more useful
        float wKnightsValue = wKnights * 3f;
            if (bPawns > 6) wKnightsValue += 0.1f;

        //Bishop Material Value
        float bBishopsValue = bBishops * 3f;
            if (wPawns <= 6) bBishopsValue += 0.1f; //At low pawn counts, the bishop is more useful
        float wBishopsValue = wBishops * 3f;
            if (bPawns <= 6) wBishopsValue += 0.1f;

        //Queen Material Value
        float bQueensValue = bQueens * 9;
        float wQueensValue = wQueens * 9;

        
        //Debug.Log(wPawnsValue);
        //Debug.Log(bPawnsValue);
        /*
        Debug.Log(wKnightsValue);
        Debug.Log(wBishopsValue);
        Debug.Log(wRooksValue);
        Debug.Log(wQueensValue);
        */

        eval = (wPawnsValue + wBishopsValue + wKnightsValue + wRooksValue + wQueensValue) 
            - (bPawnsValue + bBishopsValue + bKnightsValue + bRooksValue + bQueensValue);

        return eval;
    }

  

  
    
}

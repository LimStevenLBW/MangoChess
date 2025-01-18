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

    private int CountMaterial(ulong bitboard)
    {
        int count = 0;
        for (int i = 0; i < 64; i++)
        {
            if (((bitboard >> i) & 1) == 1)
            {
                count += 1;
            }
        }
        return count;
    }

    private float GetMaterialEval()
    {
        //Counters
        int bRooks, bKnights, bBishops, bQueens, bPawns, bKing;
            bRooks = bKnights = bBishops = bQueens = bPawns = bKing = 0;
        int wRooks, wKnights, wBishops, wQueens, wPawns, wKing;
            wRooks = wKnights = wBishops = wQueens = wPawns = wKing = 0;

        float eval;

        #region Material Count

        bPawns = CountMaterial(board.Blk_Pawns);
        bRooks = CountMaterial(board.Blk_Rooks);
        bKnights = CountMaterial(board.Blk_Knights);
        bBishops = CountMaterial(board.Blk_Bishops);
        bQueens = CountMaterial(board.Blk_Queens);
        bKing = CountMaterial(board.Blk_King);

        wPawns = CountMaterial(board.Wht_Pawns);
        wRooks = CountMaterial(board.Wht_Rooks);
        wKnights = CountMaterial(board.Wht_Knights);
        wBishops = CountMaterial(board.Wht_Bishops);
        wQueens = CountMaterial(board.Wht_Queens);
        wKing = CountMaterial(board.Wht_King);

        /*
        files = board.GetFiles();
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
        */

        int bPieceCount = bRooks + bKnights + bBishops + bQueens; //Start:  7
        int wPieceCount = wRooks + wKnights + wBishops + wQueens;
        int totalPieces = bPieceCount + wPieceCount;
        #endregion

        //Pawn Material Value
        float pMult = Math.Abs(1.7f - totalPieces * 0.1f); //0.3 -> 1.7 in pawn value when piece count gets low
        float bPawnsVal = bPawns * pMult;
        float wPawnsVal = wPawns * pMult;

        //Rook Material
        float bRooksVal = bRooks * 5;
        float wRooksVal = wRooks * 5;

        //Knight Material
        float bKnightsVal = bKnights * 3f;
            if (wPawns > 6) bKnightsVal += 0.1f; //At high pawn counts, the knight is more useful
        float wKnightsVal = wKnights * 3f;
            if (bPawns > 6) wKnightsVal += 0.1f;

        //Bishop Material
        float bBishopsVal = bBishops * 3f;
            if (wPawns <= 6) bBishopsVal += 0.1f; //At low pawn counts, the bishop is more useful
        float wBishopsVal = wBishops * 3f;
            if (bPawns <= 6) wBishopsVal += 0.1f;

        //Queen Material
        float bQueensVal = bQueens * 9;
        float wQueensVal = wQueens * 9;

        //King Material
        float bKingVal = bKing * 1000000;
        float wKingVal = wKing * 1000000;

        /*
        Debug.Log(wPawnsVal);
        Debug.Log(bPawnsVal);
        Debug.Log(wKnightsVal);
        Debug.Log(wBishopsVal);
        Debug.Log(wRooksVal);
        Debug.Log(wQueensVal);
        */

        eval = (wPawnsVal + wBishopsVal + wKnightsVal + wRooksVal + wQueensVal + wKingVal) 
            - (bPawnsVal + bBishopsVal + bKnightsVal + bRooksVal + bQueensVal - bKingVal);

        return eval;
    }

  

  
    
}

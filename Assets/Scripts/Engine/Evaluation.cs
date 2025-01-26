using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evaluation
{
    private float whiteRating;
    private float blackRating;
    private int bPieceCount;
    private int wPieceCount;
    private BitBoard board;
    private List<ChessBoardFile> files;
    private List<int> edge = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 15, 16, 23, 24, 31, 32, 39, 40, 47, 48, 55, 56, 57, 58, 59, 60, 61, 62, 63};

    public Evaluation(BitBoard board)
    {
        this.board = board;
    }

    public float GetEvaluation(BitBoard board)
    {
        this.board = board;
        float eval = 0;
        eval += GetMaterialEval();
        eval += GetPawnPositionEval();

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

        bPieceCount = bRooks + bKnights + bBishops + bQueens; //Start:  7
        wPieceCount = wRooks + wKnights + wBishops + wQueens;
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
            - (bPawnsVal + bBishopsVal + bKnightsVal + bRooksVal + bQueensVal + bKingVal);

        return eval;
    }

    private float GetPawnPositionEval()
    {
        float bonus = 0;
        ulong blkPawns = board.Blk_Pawns;
        ulong whtPawns = board.Wht_Pawns;

        //Pawn Position Bonus for White
        for (int i = 18; i < 56; i++) 
        {
            if (((whtPawns >> i) & 1) == 1)
            {
                if (i >= 17 && i < 21 || i == 22) bonus += 0.1f; //Defensive Center + Fianchetto
                else if (i >= 26 && i < 29) bonus += 0.2f; //Center
                else if (i >= 34 && i < 45) bonus += 0.2f; //Extended Center
                else if (i >= 40 && i < 48) bonus += 0.4f; //Third to last row
                else if (i >= 48) bonus += 0.5f; // Second to last row
            }
        }

        //Pawn Position Bonus for Black
        for (int i = 8; i < 24; i++)
        {
            if (((blkPawns >> i) & 1) == 1)
            {
                if (i >= 41 && i < 45 || i == 46) bonus -= 0.1f;  //Defensive Center
                else if (i >= 34 && i < 37) bonus -= 0.3f; //Center
                else if (i >= 26 && i < 29) bonus -= 0.2f; //Extended Center
                else if (i >= 16 && i < 24) bonus -= 0.3f; //Third to last row
                else if (i >= 8 && i < 16) bonus -= 0.5f; // Second to last row
            }
        }

        //Inactive Piece Penalty
        ulong blk_Pieces = board.Blk_Knights | board.Blk_Bishops | board.Blk_Queens;
        ulong wht_Pieces = board.Wht_Knights | board.Wht_Bishops | board.Wht_Queens;

        //Penalty for White's inactive pieces
        for (int i = 0; i < 8; i++)
        {
            if (((wht_Pieces >> i) & 1) == 1)
            {
                bonus -= 0.1f;
            }
        }

        //Penalty for Black inactive pieces
        for (int i = 56; i < 64; i++)
        {
            if (((blk_Pieces >> i) & 1) == 1)
            {
                bonus += 0.1f;
            }
        }

        //Castled Bonus
        if (board.isWhiteCastled) bonus += 0.4f;
        if (board.isBlackCastled) bonus -= 0.4f;

        //Safe King Bonus White
        for (int i = 0; i < 8; i++)
        {
            if (bPieceCount < 2) break; //King should participate if piece count is low
            if (((board.Wht_King >> i) & 1) == 1)
            {
                if (i >= 0 && i < 3) bonus += 0.2f;
                if (i >= 5) bonus += 0.2f;
            }
        }

        //Safe King Bonus Black
        for (int i = 56; i < 64; i++)
        {
            if (wPieceCount < 2) break;
            if (((board.Blk_King >> i) & 1) == 1)
            {
                if (i >= 56 && i < 59) bonus -= 0.2f;
                if (i >= 61) bonus -= 0.2f;
            }
        }

        //Fianchetto
        for (int i = 0; i < 64; i++)
        {
            if (((board.Wht_Bishops >> i) & 1) == 1)
            {
                if (i == 9 || i == 14) bonus += 0.1f;
            }
            if (((board.Blk_Bishops >> i) & 1) == 1)
            {
                if (i == 49 || i == 54) bonus -= 0.1f;
            }
        }

        //Knights on the Rim are Dim
        for (int i = 0; i < 64; i++)
        {
            if (((board.Wht_Knights >> i) & 1) == 1)
            {
                if (edge.Contains(i)) bonus -= 0.1f;
            }
            if (((board.Blk_Knights >> i) & 1) == 1)
            {
                if (edge.Contains(i)) bonus += 0.1f;
            }
        }
        return bonus;


    }


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
}


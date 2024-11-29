using System;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    //Bitboards
    ulong blk_Pawns, blk_Knights, blk_Bishops, blk_Rooks, blk_King, blk_Queens;
    ulong wht_Pawns, wht_Knights, wht_Bishops, wht_Rooks, wht_King, wht_Queens;

    static ulong A_FILE;
    static ulong H_FILE;
    static ulong RANK_1 = 72057594037927936L;
    static ulong RANK_4 = 1095216660480L;
    static ulong RANK_5 = 4278190080L;
    static ulong RANK_8 = 255L;
    static ulong WHT_CANT_CAPTURE;
    static ulong BLK_PIECES;
    static ulong EMPTY_SQUARES;

    [SerializeField] private List<ChessBoardFile> file = new List<ChessBoardFile>();

    // Start is called before the first frame update
    void Start()
    {
       // files[0] = a_File; files[1] = b_File; files[2] = c_File; files[3] = d_File;
        //files[4] = e_File; files[5] = f_File; files[6] = g_File; files[7] = h_File;
    }

    public static string GetPossibleMovesWhite(string history, ulong wht_Pawns, ulong wht_Knights, ulong wht_Bishops, ulong wht_Rooks, ulong wht_King, ulong wht_Queens, ulong blk_King, ulong blk_Pawns, ulong blk_Knights, ulong blk_Bishops, ulong blk_Rooks, ulong blk_Queens)
    {
        WHT_CANT_CAPTURE = ~(wht_Pawns | wht_Knights | wht_Bishops | wht_Rooks | wht_King | wht_Queens | blk_King);
        BLK_PIECES = blk_Pawns | blk_Knights | blk_Bishops | blk_Rooks | blk_King | blk_Queens;
        EMPTY_SQUARES = ~(blk_Pawns | blk_Knights | blk_Bishops | blk_Rooks | blk_King | blk_Queens | wht_Pawns | wht_Knights | wht_Bishops | wht_Rooks | wht_King | wht_Queens);

        string moveList = "";


        return moveList;
    }

    public static String GetWhitePawnMoves(string history, ulong wht_Pawns)
    {
        string moveList = "";
        //All pieces that a pawn can capture to the right if that piece is not on Rank8(promotion) or A File(can't right capture)
        ulong PAWN_MOVES = (wht_Pawns >> 7) & BLK_PIECES & ~RANK_8 & ~A_FILE; 
        for(int i = 0; i < 64; i++) //Iterate through the bitboard
        {
            if (((PAWN_MOVES >> i) & 1) == 1) //if the bit is active
                moveList += "" + (i / 8 + 1) + (i % 8 - 1) + (i / 8) + (i % 8); //(x1,y1,x2,y2) Formula to store the right capture destination as a move
        }
         
        PAWN_MOVES = (wht_Pawns >> 9) & BLK_PIECES & ~RANK_8 & ~H_FILE; //Left capture
        for (int i = 0; i < 64; i++) //Iterate through the bitboard, possibly optimize using trailing zeros calculation instead
        {
            if (((PAWN_MOVES >> i) & 1) == 1) 
                moveList += "" + (i / 8 + 1) + (i % 8 + 1) + (i / 8) + (i % 8); 
        }

        PAWN_MOVES = (wht_Pawns >> 8) & EMPTY_SQUARES & ~RANK_8; //Forward Move, if square is empty
        for (int i = 0; i < 64; i++)
        {
            if (((PAWN_MOVES >> i) & 1) == 1)
                moveList += "" + (i / 8 + 1) + (i % 8) + (i / 8) + (i % 8);
        }

        PAWN_MOVES = (wht_Pawns >> 8) & EMPTY_SQUARES & EMPTY_SQUARES >>8 & RANK_4; //Double Move Forward
        for (int i = 0; i < 64; i++)
        {
            if (((PAWN_MOVES >> i) & 1) == 1)
                moveList += "" + (i / 8 + 2) + (i % 8) + (i / 8) + (i % 8);
        }

        //Promotion
        PAWN_MOVES = (wht_Pawns >> 7) & BLK_PIECES & RANK_8 & ~A_FILE; //Promote by right capture
        for (int i = 0; i < 64; i++) 
        {
            if (((PAWN_MOVES >> i) & 1) == 1)
                moveList += "" + (i % 8 - 1) + (i % 8) + "QP" + (i % 8 - 1) + (i % 8) + "RP" + (i % 8 - 1) + (i % 8) + "BP" + (i % 8 - 1) + (i % 8) + "KP";

        }

        PAWN_MOVES = (wht_Pawns >> 9) & BLK_PIECES & RANK_8 & ~H_FILE; //Promote by left capture
        for (int i = 0; i < 64; i++) 
        {
            if (((PAWN_MOVES >> i) & 1) == 1)
                moveList += "" + (i % 8 + 1) + (i % 8) + "QP" + (i % 8 + 1) + (i % 8) + "RP" + (i % 8 + 1) + (i % 8) + "BP" + (i % 8 + 1) + (i % 8) + "KP";

        }

        PAWN_MOVES = (wht_Pawns >> 9) & EMPTY_SQUARES & RANK_8; //Promote by forward movement
        for (int i = 0; i < 64; i++)
        {
            if (((PAWN_MOVES >> i) & 1) == 1)
                moveList += "" + (i % 8) + (i % 8) + "QP" + (i % 8) + (i % 8) + "RP" + (i % 8) + (i % 8) + "BP" + (i % 8) + (i % 8) + "KP";

        }

        //Checking for en passant, need to know prior moves
        if (history.Length >= 4)
        {
            //Checks if the last move was a double forward movement
            if (history[history.Length - 1] == history[history.Length - 3]
                && Math.Abs(history[history.Length - 2] - history[history.Length - 4]) == 2)
            {
                PAWN_MOVES = (wht_Pawns << 1) & BLK_PIECES & RANK_5 & ~A_FILE;
               // PAWN_MOVES = PAWN_MOVES & ~(PAWN_MOVES - 1);
                for (int i = 0; i < 64; i++) //Capture Right En Passant
                {
                    if (((PAWN_MOVES >> i) & 1) == 1)
                        moveList += "" + (i % 8 - 1) + (i % 8) + " E";
                }

                PAWN_MOVES = (wht_Pawns >> 1) & BLK_PIECES & RANK_5 & ~H_FILE;
                for (int i = 0; i < 64; i++) //Capture Left En Passant
                {
                    if (((PAWN_MOVES >> i) & 1) == 1)
                        moveList += "" + (i % 8 + 1) + (i % 8) + " E";
                }
            }
        }

        return moveList;
    }

    public void ResetStartingPosition()
    {
        //"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        string startingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

        ClearBoard();

        FENtoChessBoard(startingFEN);
    }

    public void ClearBoard()
    {
        foreach (ChessBoardFile f in file)
        {
            f.ClearAllPieces();
        }
    }


    //rnbqkbnr/pppppppp/2/k
    public void FENtoChessBoard(string FEN)
    {
        ClearBoard();
        char[] fenArr = FEN.ToCharArray();
        int i = 0;
        int skip = 0;

        for (int row = 7; row >= 0; row--)
        {
            for(int col = 0; col < 8; col++)
            {
                if (skip > 1) //Note that a piece instantiation is already skipped when int skip is re-assigned
                {
                    //Debug.Log("skip count:" + skip + "last known c was:" + fenArr[i] +", i = " + i);
                    skip--;
                    continue;
                }

                char c = fenArr[i];
                i++;
                if (c == '/') {
                    c = fenArr[i];
                    i++;
                }

               // Debug.Log("col" + col + ",row" + row + ",i" + i + ",c" + c);
                if (char.IsNumber(c))
                {
                    skip = c - '0';
                }
                else
                {
                    file[col].CreatePiece(row, c);
                }
                


            }
        }

        PosToBitBoard();
    }

    public void PosToBitBoard()
    {
        int i = 0;
        for(int row = 7; row >= 0; row--)
        {
            for(int col = 0; col < 8; col++)
            {
                char c = file[col].GetPieceCode(row);

                string b = "0000000000000000000000000000000000000000000000000000000000000000";
                b = b.Substring(0, i) + '1' + b.Substring(i + 1);

                switch (c)
                {
                    case ('r'):
                        blk_Rooks += Convert.ToUInt64(b, 2);
                        break;
                    case ('n'):
                        blk_Knights += Convert.ToUInt64(b, 2);
                        break;
                    case ('b'):
                        blk_Bishops += Convert.ToUInt64(b, 2);
                        break;
                    case ('p'):
                        blk_Pawns += Convert.ToUInt64(b, 2);
                        break;
                    case ('k'):
                        blk_King += Convert.ToUInt64(b, 2);
                        break;
                    case ('q'):
                        blk_Queens += Convert.ToUInt64(b, 2);
                        break;

                    case ('R'):
                        wht_Rooks += Convert.ToUInt64(b, 2);
                        break;
                    case ('N'):
                        wht_Knights += Convert.ToUInt64(b, 2);
                        break;
                    case ('B'):
                        wht_Bishops += Convert.ToUInt64(b, 2);
                        break;
                    case ('P'):
                        wht_Pawns += Convert.ToUInt64(b, 2);
                        break;
                    case ('K'):
                        wht_King += Convert.ToUInt64(b, 2);
                        break;
                    case ('Q'):
                        blk_Queens += Convert.ToUInt64(b, 2);
                        break;
                    default:
                        break;
                }
                i++;
            }
        }

    }

    //"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public void FENtoBitBoard(string FEN)
    {
        bool isBoardFilled = false;
        int row = 7;

        char[] fenArr = FEN.ToCharArray();
        int j = 0; //FEN pointer
        int skip = 0;

        for (int i = 0; i < 1; i++)
        {
            if(skip > 0)
            {
                skip--;
                continue;
            }

            char c = fenArr[j];
            j++;
            if (c == '/') c = fenArr[j]; //If we see a / it's a new row
            if (char.IsNumber(c)) skip = c - 0;

            string b = "0000000000000000000000000000000000000000000000000000000000000000";
            b = b.Substring(0, i) + "1" + b.Substring(i+1, b.Length - 1);

            switch (c)
            {
                case ('r'): blk_Rooks += Convert.ToUInt64(b, 2);
                    break;
            }
                


        }
    }

   

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    //Bitboards
    ulong blk_Pawns, blk_Knights, blk_Bishops, blk_Rooks, blk_King, blk_Queens;
    ulong wht_Pawns, wht_Knights, wht_Bishops, wht_Rooks, wht_King, wht_Queens;

    //Edge Files
    static ulong A_FILE = 9259542123273814144;  
    //static ulong AB_FILE = 
    static ulong H_FILE = 72340172838076673; 

    //Ranks
    static ulong[] RANK_MASKS = { 0xFF, 0xFF00, 0xFF0000, 0xFF000000,
        0xFF00000000, 0xFF0000000000, 0xFF000000000000, 0xFF00000000000000 }; //Hexadecimal form ranks 1-8

                                // H                G                   F                   E
    static ulong[] FILE_MASKS = { 0x101010101010101, 0x202020202020202, 0x404040404040404, 0x808080808080808,
        0x1010101010101010, 0x2020202020202020, 0x3030303030303030, 0x4040404040404040}; ////Hexadecimal Files are reversed, H-> A

    // From top-right towards bottom left // Starting from SE corner
    static ulong[] DIAGONALS = { 0x1, 0x102, 0x10204, 0x1020408, 0x102040810, 0x10204081020, 0x1020408102040,
         0x102040810204080, 0x204081020408000, 0x408102040800000, 0x810204080000000,
         0x1020408000000000, 0x2040800000000000, 408000000000000000, 0x8000000000000000 };

    //Top left to bottom right \\ Starting from SW Corner
    static ulong[] ANTI_DIAGONALS = { 0x80, 0x8040, 0x80402010, 0x8040201008, 0x804020100804, 0x80402010080402,
        0x8040201008040201, 0x4020100804020100, 0x2010080402010000, 0x1008040201000000,
        0x804020100000000,  0x402010000000000,  0x408000000000000,  0x100000000000000 };

    static ulong RANK_8 = 255; //Verified
    static ulong RANK_5 = 4278190080; //Verified
    static ulong RANK_4 = 1095216660480; //Verified
    static ulong RANK_1 = 18374686479671623680; //Verified

    //Unset
    static ulong WHT_CANT_CAPTURE;
    static ulong WHT_PIECES;
    static ulong BLK_PIECES;
    static ulong EMPTY_SQUARES;
    static ulong OCCUPIED;

    //Valuable Areas
    static ulong CENTER = 103481868288; 
    static ulong EXTENDED_CENTER = 66229406269440; 

    [SerializeField] private List<ChessBoardFile> file = new List<ChessBoardFile>();

    // Start is called before the first frame update
    void Start()
    {
       // files[0] = a_File; files[1] = b_File; files[2] = c_File; files[3] = d_File;
        //files[4] = e_File; files[5] = f_File; files[6] = g_File; files[7] = h_File;


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

    //Retrieve the bitboard value for all pieces
    public void GetAllPieces()
    {
        BLK_PIECES = blk_Pawns | blk_Knights | blk_Bishops | blk_Rooks | blk_King | blk_Queens;
        WHT_PIECES = wht_Pawns | wht_Knights | wht_Bishops | wht_Rooks | wht_King | wht_Queens;
        OCCUPIED = BLK_PIECES | WHT_PIECES;
        Debug.Log("ulong All Pieces: " + OCCUPIED);

        Debug.Log(DecimalToBitboard(OCCUPIED));
        
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

        //string history = "";
       
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
                //b = b.Substring(0, i) + '1' + b.Substring(i + 1);
                b = b.Substring(i) + '1' + b.Substring(0, i);
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

        GetAllPieces();
        string history = "";
        string moves = GetPossibleMovesWhite(history, wht_Pawns, wht_Knights, wht_Bishops, wht_Rooks, wht_King, wht_Queens, blk_King, blk_Pawns, blk_Knights, blk_Bishops, blk_Rooks, blk_Queens);
        Debug.Log(moves);
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


    public string GetPossibleMovesWhite(string history, ulong wht_Pawns, ulong wht_Knights, ulong wht_Bishops, ulong wht_Rooks, ulong wht_King, ulong wht_Queens, ulong blk_King, ulong blk_Pawns, ulong blk_Knights, ulong blk_Bishops, ulong blk_Rooks, ulong blk_Queens)
    {
        WHT_CANT_CAPTURE = ~(wht_Pawns | wht_Knights | wht_Bishops | wht_Rooks | wht_King | wht_Queens | blk_King);
        BLK_PIECES = blk_Pawns | blk_Knights | blk_Bishops | blk_Rooks | blk_King | blk_Queens;
        WHT_PIECES = wht_Pawns | wht_Knights | wht_Bishops | wht_Rooks | wht_King | wht_Queens;
        EMPTY_SQUARES = ~(blk_Pawns | blk_Knights | blk_Bishops | blk_Rooks | blk_King | blk_Queens | wht_Pawns | wht_Knights | wht_Bishops | wht_Rooks | wht_King | wht_Queens);

        OCCUPIED = BLK_PIECES | WHT_PIECES;

        string moveList = GetWhitePawnMoves(history, wht_Pawns);

        //DrawMoves(GetHorizontalVerticalMoves(9));
        return moveList;
    }

    public string GetWhitePawnMoves(string history, ulong wht_Pawns)
    {
        string moveList = "";
        //All pieces that a pawn can capture to the right if that piece is not on Rank8(promotion) or A File(can't right capture)
        ulong PAWN_MOVES = (wht_Pawns >> 7) & BLK_PIECES & ~RANK_8 & ~A_FILE;
        for (int i = 0; i < 64; i++) //Iterate through the bitboard
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

        PAWN_MOVES = (wht_Pawns >> 8) & EMPTY_SQUARES & ~RANK_8; //Forward Moves, if square is empty

        DrawBitboard(PAWN_MOVES);
        for (int i = 0; i < 64; i++)
        {
            if (((PAWN_MOVES >> i) & 1) == 1)
            {
                Debug.Log("detected" + i);
                //Debug.Log("P" + i);
                //moveList += " / " + (i / 8 + 1) + (i % 8) + (i / 8) + (i % 8);
            }
               
        }

        /*
        PAWN_MOVES = (wht_Pawns >> 8) & EMPTY_SQUARES & EMPTY_SQUARES >> 8 & RANK_4; //Double Move Forward
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
         */
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

    public static ulong ReverseBits(ulong x)
    {
        string num = "" + x;
        char[] cNum = num.ToCharArray();
        Array.Reverse(cNum);
        num = new string(cNum);
        return Convert.ToUInt64(num);
    }

    public static ulong GetHorizontalVerticalMoves(int pos)
    {
        ulong binaryS = (ulong) 1 << pos;
        ulong horizontalMoves = (OCCUPIED - 2 * binaryS) ^ ReverseBits(ReverseBits(OCCUPIED) - 2 * ReverseBits(binaryS));
        ulong verticalMoves = ((OCCUPIED & FILE_MASKS[pos % 8]) - (2 * binaryS)) & ReverseBits(ReverseBits(OCCUPIED & FILE_MASKS[pos % 8]) - (2 * ReverseBits(binaryS)));
        return (horizontalMoves & RANK_MASKS[pos] | verticalMoves & RANK_MASKS[pos]);
    }

    public static ulong GetDiagonalMoves(int pos)
    {
        ulong binaryS = (ulong)1 << pos;
        ulong diagonalMoves = ((OCCUPIED & DIAGONALS[pos / 8 + pos % 8]) - (2 * binaryS) ^ ReverseBits(ReverseBits(OCCUPIED & DIAGONALS[(pos / 8) + (pos % 8)])) - (2 * ReverseBits(binaryS)));
        ulong antiDiagonalMoves = ((OCCUPIED & ANTI_DIAGONALS[(pos / 8) + 7 - (pos % 8)]) - (2 * ReverseBits(binaryS)) ^ ReverseBits(OCCUPIED & ANTI_DIAGONALS[(pos / 8) + 7 - (pos % 8)]) - (2 * ReverseBits(binaryS)));
        return (diagonalMoves & DIAGONALS[(pos / 8) + (pos % 8)] | antiDiagonalMoves & ANTI_DIAGONALS[(pos / 8) + 7 - (pos % 8)]);
    }

    public string DecimalToBitboard(ulong num)
    {
        string bitboard = Convert.ToString((long)num, 2); //Casting a ulong to long doesn't change the bit pattern at all.
        //Debug.Log(bitboard); 

        int l = 64 - bitboard.Length;

        string allPieces = "";
        for (int i = 0; i < l; i++)
        {
            allPieces += '0';
        }
        allPieces += bitboard;

        return allPieces;
    }

    public void DrawBitboard(ulong num)
    {
        string res = DecimalToBitboard(num);
        DrawBitboard(res);
    }

    public void DrawBitboard(string bitboard)
    {
        //string bitboard = DecimalToBitboard(num);
        int i = 63;
        char[] cNum = bitboard.ToCharArray();

        for (int row = 7; row >= 0; row--)
        {
            for (int col = 0; col < 8; col++)
            {

                int c = cNum[i] - '0';
                i--;
                if (c == 1)
                {
                    file[col].Highlight(row);
                }

            }
        }

    }

    public List<ChessBoardFile> GetFiles()
    {
        return file;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

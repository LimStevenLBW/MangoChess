using System;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;

public class BitBoard
{
    //Castling Rights
    public bool isWhiteCastled = false;
    public bool isBlackCastled = false;
    private bool whtCanQueenSideCastle = true;
    private bool whtCanKingSideCastle = true;
    private bool blkCanQueenSideCastle = true;
    private bool blkCanKingSideCastle = true;

    static ulong bKingRook = 9223372036854775808;
    static ulong bQueenRook = 72057594037927936;
    static ulong wKingRook = 128;
    static ulong wQueenRook = 1;

    public string initialFEN = "";

    //Edge Files
    static ulong A_FILE = 72340172838076673;  //Verified
    static ulong B_FILE = 144680345676153346; //Verified
    static ulong G_FILE = 4629771061636907072; //Verified
    static ulong H_FILE = 9259542123273814144; //Verified

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

    static ulong RANK_1 = 255; 
    static ulong RANK_5 = 1095216660480; 
    static ulong RANK_4 = 4278190080; 
    static ulong RANK_8 = 18374686479671623680; 

    //Unset
    static ulong WHT_CANT_CAPTURE;
    static ulong WHT_PIECES;
    static ulong BLK_PIECES;
    static ulong EMPTY_SQUARES;
    static ulong OCCUPIED;

    //Valuable Areas
    static ulong CENTER = 103481868288; 
    static ulong EXTENDED_CENTER = 66229406269440;

    //Stores the moves a selected piece can make
    ulong AVAILABLE_MOVES;

    private List<ChessBoardFile> files;

    private ulong blk_Pawns, blk_Knights, blk_Bishops, blk_Rooks, blk_King, blk_Queens;
    private ulong wht_Pawns, wht_Knights, wht_Bishops, wht_Rooks, wht_King, wht_Queens;

    #region Bitboard Properties
    public ulong Blk_Pawns { get => blk_Pawns; private set => blk_Pawns = value; }
    public ulong Blk_Knights { get => blk_Knights; private set => blk_Knights = value; }
    public ulong Blk_Bishops { get => blk_Bishops; private set => blk_Bishops = value; }
    public ulong Blk_Rooks { get => blk_Rooks; private set => blk_Rooks = value; }
    public ulong Blk_King { get => blk_King; private set => blk_King = value; }
    public ulong Blk_Queens { get => blk_Queens; private set => blk_Queens = value; }

    public ulong Wht_Pawns { get => wht_Pawns; private set => wht_Pawns = value; }
    public ulong Wht_Knights { get => wht_Knights; private set => wht_Knights = value; }
    public ulong Wht_Bishops { get => wht_Bishops; private set => wht_Bishops = value; }
    public ulong Wht_Rooks { get => wht_Rooks; private set => wht_Rooks = value; }
    public ulong Wht_King { get => wht_King; private set => wht_King = value; }
    public ulong Wht_Queens { get => wht_Queens; private set => wht_Queens = value; }
    #endregion
    public BitBoard() { }
   
    public BitBoard(string FEN, List<ChessBoardFile> files)
    {
        if (FEN != "") initialFEN = FEN;
        this.files = files;

        //Set the IDs for the squares
        int i = 0;
        for (int row = 0; row <= 7; row++)
        {
            for (int col = 0; col <= 7; col++)
            {
                Square square = files[col].GetSquare(row);
                square.SetID(i);
                i++;
            }
            // files[0] = a_File; files[1] = b_File; files[2] = c_File; files[3] = d_File;
            //files[4] = e_File; files[5] = f_File; files[6] = g_File; files[7] = h_File;
        }

        ResetStartingPosition();
    }

    public void ResetStartingPosition()
    {
        Game.Instance.ResetGameStatus();

        //Reset Castling Rights
        isWhiteCastled = false;
        isBlackCastled = false;
        whtCanQueenSideCastle = true;
        whtCanKingSideCastle = true;
        blkCanQueenSideCastle = true;
        blkCanKingSideCastle = true;

        string defaultFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
        if (initialFEN != "") defaultFEN = initialFEN;
        //"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        
        ClearBoard();

        FENtoChessBoard(defaultFEN);
    }

    public BitBoard Copy()
    {
        BitBoard copy = new BitBoard();
        copy.blk_Pawns = blk_Pawns;
        copy.blk_Knights = blk_Knights;
        copy.blk_Bishops = blk_Bishops;
        copy.blk_Rooks = blk_Rooks;
        copy.blk_Queens = blk_Queens;
        copy.blk_King = blk_King;

        copy.wht_Pawns = wht_Pawns;
        copy.wht_Knights = wht_Knights;
        copy.wht_Bishops = wht_Bishops;
        copy.wht_Rooks = wht_Rooks;
        copy.wht_Queens = wht_Queens;
        copy.wht_King = wht_King;

        copy.CopyRules(isWhiteCastled, isBlackCastled, whtCanQueenSideCastle, whtCanKingSideCastle, blkCanQueenSideCastle, blkCanKingSideCastle);
        return copy;
    }

    public void CopyRules(bool isWhiteCastled, bool isBlackCastled, bool wQCastle, bool wKCastle, bool bQCastle, bool bKCastle){
        this.isWhiteCastled = isWhiteCastled;
        this.isBlackCastled = isBlackCastled;
        whtCanQueenSideCastle = wQCastle;
        whtCanKingSideCastle = wKCastle;
        blkCanQueenSideCastle = bQCastle;
        blkCanKingSideCastle = bKCastle;
    }

    public void ClearBoard()
    {
        foreach (ChessBoardFile f in files)
        {
            f.ClearAllPieces();
        }
    }

    /*
     * On piece selection, reveal where this piece can move
     */
    public void ShowMovementOptions(Piece piece)
    {
        List<Move> moveList = new List<Move>();
        string b = "0000000000000000000000000000000000000000000000000000000000000000";
        int i = piece.GetSquare().GetID();
        b = b.Substring(i) + '1' + b.Substring(0, i);
        char code = piece.GetCode();

        switch (code)
        {
            case ('r'):
                moveList = GetRookMoves("", Convert.ToUInt64(b, 2), code);
                break;
            case ('n'):
                moveList = GetKnightMoves("", Convert.ToUInt64(b, 2), code);
                break;
            case ('b'):
                moveList = GetBishopMoves("", Convert.ToUInt64(b, 2), code);
                break;
            case ('p'):
                moveList = GetBlackPawnMoves("", Convert.ToUInt64(b, 2), code);
                break;
            case ('k'):
                moveList = GetKingMoves("", Convert.ToUInt64(b, 2), code);
                break;
            case ('q'):
                moveList = GetRookMoves("", Convert.ToUInt64(b, 2), code);
                moveList.AddRange(GetBishopMoves("", Convert.ToUInt64(b, 2), code));
                break;

            case ('R'):
                moveList = GetRookMoves("", Convert.ToUInt64(b, 2), code);
                break;
            case ('N'):
                moveList = GetKnightMoves("", Convert.ToUInt64(b, 2), code);
                break;
            case ('B'):
                moveList = GetBishopMoves("", Convert.ToUInt64(b, 2), code);
                break;
            case ('P'):
                moveList = GetWhitePawnMoves("", Convert.ToUInt64(b, 2), code);
                break;
            case ('K'):
                moveList = GetKingMoves("", Convert.ToUInt64(b, 2), code);
                break;
            case ('Q'):
                moveList = GetRookMoves("", Convert.ToUInt64(b, 2), code);
                moveList.AddRange(GetBishopMoves("", Convert.ToUInt64(b, 2), code));
                break;
            default:
                break;
        }

        //Display all available moves for selected piece
        /*
        Debug.Log("Available Moves:");
        foreach (Move move in moveList)
        {
            Debug.Log(move.ToString());
        }
        */
        DrawBitboard(AVAILABLE_MOVES); //Show available moves
        AVAILABLE_MOVES = 0; //Reset available moves
    }

    /*
     * Used by the engine, collect all possible moves it can make
     *  //string history, ulong wht_Pawns, ulong wht_Knights, ulong wht_Bishops, ulong wht_Rooks, ulong wht_King, ulong wht_Queens, ulong blk_King, ulong blk_Pawns, ulong blk_Knights, ulong blk_Bishops, ulong blk_Rooks, ulong blk_Queens
     */
    public List<Move> GetPossibleMovesWhite()
    {
        //CheckForWhtRookChanges();
        List<Move> moveList = GetWhitePawnMoves("", wht_Pawns, 'P');
        moveList.AddRange(GetKnightMoves("", wht_Knights, 'N'));
        moveList.AddRange(GetBishopMoves("", wht_Bishops, 'B'));
        moveList.AddRange(GetRookMoves("", wht_Queens, 'Q'));
        moveList.AddRange(GetBishopMoves("", wht_Queens, 'Q'));
        moveList.AddRange(GetRookMoves("", wht_Rooks, 'R'));
        moveList.AddRange(GetKingMoves("", wht_King, 'K'));

        AVAILABLE_MOVES = 0; //Reset available moves
        return moveList;
    }

    /*
     * string history, ulong blk_Pawns, ulong blk_Knights, ulong blk_Bishops, ulong blk_Rooks, ulong blk_King, ulong blk_Queens, ulong blk_King, ulong blk_Pawns, ulong blk_Knights, ulong blk_Bishops, ulong blk_Rooks, ulong blk_Queens
     */
    public List<Move> GetPossibleMovesBlack()
    {
        //CheckForBlkRookChanges();
        List<Move> moveList = GetBlackPawnMoves("", Blk_Pawns, 'p');
        moveList.AddRange(GetKnightMoves("", blk_Knights, 'n'));
        moveList.AddRange(GetBishopMoves("", blk_Bishops, 'b'));
        moveList.AddRange(GetRookMoves("", blk_Queens, 'q'));
        moveList.AddRange(GetBishopMoves("", blk_Queens, 'q'));
        moveList.AddRange(GetRookMoves("", blk_Rooks, 'r'));
        moveList.AddRange(GetKingMoves("", blk_King, 'k'));


        AVAILABLE_MOVES = 0; //Reset available moves
        return moveList;
    }

    public bool IsKingChecked(Game.Side side)
    {
        if(side == Game.Side.White)
        {
            List<Move> moves = GetPossibleMovesBlack();
            foreach(Move m in moves)
            {
                if (m.capturedPiece == 'K') return true;
            }
        }

        if (side == Game.Side.Black)
        {
            List<Move> moves = GetPossibleMovesWhite();
            foreach (Move m in moves)
            {
                if (m.capturedPiece == 'k') return true;
            }
        }

        return false;
    }

    //Retrieve the bitboard value for all pieces
    public void UpdateBitBoards()
    {
        WHT_CANT_CAPTURE = ~(wht_Pawns | wht_Knights | wht_Bishops | wht_Rooks | wht_King | wht_Queens | blk_King);
        BLK_PIECES = blk_Pawns | blk_Knights | blk_Bishops | blk_Rooks | blk_King | blk_Queens;
        WHT_PIECES = wht_Pawns | wht_Knights | wht_Bishops | wht_Rooks | wht_King | wht_Queens;
        EMPTY_SQUARES = ~(blk_Pawns | blk_Knights | blk_Bishops | blk_Rooks | blk_King | blk_Queens | wht_Pawns | wht_Knights | wht_Bishops | wht_Rooks | wht_King | wht_Queens);

        OCCUPIED = BLK_PIECES | WHT_PIECES;

        //Debug.Log("BITBOARD OCCUPIED SQUARES: " + OCCUPIED);
        //Debug.Log(DecimalToBitboard(OCCUPIED));

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
                    Square square = files[col].GetSquare(row);
                    square.CreatePiece(c);
                }
            }
        }

        PosToBitBoard();

        //string history = "";
       
    }
     
    public void PosToBitBoard()
    {
        blk_Pawns = blk_Knights = blk_Bishops = blk_Rooks = blk_King = blk_Queens = 0;
        wht_Pawns = wht_Knights = wht_Bishops = wht_Rooks = wht_King = wht_Queens = 0;

        int i = 0;
        for(int row = 0; row < 8; row++)
        {
            for(int col = 0; col < 8; col++)
            {
                char c = files[col].GetPieceCode(row);

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
                        wht_Queens += Convert.ToUInt64(b, 2);
                        break;
                    default:
                        break;
                }
                i++;
            }
        }

        UpdateBitBoards();
        //string history = "";
        //string moves = GetPossibleMovesWhite(history, wht_Pawns, wht_Knights, wht_Bishops, wht_Rooks, wht_King, wht_Queens, blk_King, blk_Pawns, blk_Knights, blk_Bishops, blk_Rooks, blk_Queens);
        //Debug.Log("All Possible Moves White: " + moves);
    }

    //"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public void FENtoBitBoard(string FEN)
    {
        //bool isBoardFilled = false;
        //int row = 7;

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

    /*
        * WHITE PAWN, Enpassant and promotion incomplete
        */
    public List<Move> GetWhitePawnMoves(string history, ulong wht_Pawns, char code)
{
    List<Move> moveList = new List<Move>();

    //RIGHT CAPTURE
    //All pieces that a pawn can capture to the right if that piece is not on Rank8(promotion) or A File(can't right capture)
    ulong PAWN_MOVES = (wht_Pawns << 9) & BLK_PIECES & ~A_FILE; //& ~RANK_8
    AVAILABLE_MOVES += PAWN_MOVES;
    for (int i = 0; i < 64; i++) //Iterate through the bitboard
    {
        if (((PAWN_MOVES >> i) & 1) == 1) //if the bit is active
        {
            bool isPromotion = false;
            if (i >= 0 && i < 8) isPromotion = true;
            char c = GetCapturedPieceCode(i);
            Move m = new Move(i - 9, i, code, false, false, isPromotion, capturedPiece: c);
            moveList.Add(m);
        }
    }

    //LEFT CAPTURE
    PAWN_MOVES = (wht_Pawns << 7) & BLK_PIECES & ~H_FILE;
    AVAILABLE_MOVES += PAWN_MOVES;
    for (int i = 0; i < 64; i++) //Iterate through the bitboard, possibly optimize using trailing zeros calculation instead
    {
        if (((PAWN_MOVES >> i) & 1) == 1) // moveList += "" + (i / 8 + 1) + (i % 8 + 1) + (i / 8) + (i % 8);
        {
            bool isPromotion = false;
            if (i >= 0 && i < 8) isPromotion = true;
            char c = GetCapturedPieceCode(i);
            Move m = new Move(i - 7, i, code, false, false, isPromotion, capturedPiece: c);
            moveList.Add(m);   
        }
    }

    //FORWARD MOVE ONCE
    //----- Forward Moves, if square is empty -----
    PAWN_MOVES = (wht_Pawns << 8) & EMPTY_SQUARES;
    AVAILABLE_MOVES += PAWN_MOVES;
    for (int i = 0; i < 64; i++)
    {
        if (((PAWN_MOVES >> i) & 1) == 1)
        {
            bool isPromotion = false;
            if (i >= 0 && i < 8) isPromotion = true;
            Move m = new Move(i - 8, i, code, false, false, isPromotion);
            moveList.Add(m);
            //moveList += " / " + (i / 8 + 1) + " / " + (i % 8) + " / " + (i / 8) + " / " + (i % 8);
        }
    }

    //----- Double Move Forward -----
    PAWN_MOVES = (wht_Pawns << 16) & EMPTY_SQUARES & (EMPTY_SQUARES << 8) & RANK_4;
    AVAILABLE_MOVES += PAWN_MOVES;
    // DrawBitboard(PAWN_MOVES);
    for (int i = 0; i < 64; i++)
    {
        if (((PAWN_MOVES >> i) & 1) == 1)
        {
            int start = (i - 16);
            int end = (i);
            Move m = new Move(start, end, code, false, false, false);
            moveList.Add(m);
        }      
    }
         
    /*
        
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
    } */

    return moveList;
}

    /*
     * BLACK PAWN, Enpassant and promotion incomplete
     */
    public List<Move> GetBlackPawnMoves(string history, ulong blk_Pawns, char code)
    {
        List<Move> moveList = new List<Move>();

        //RIGHT CAPTURE
        ulong PAWN_MOVES = (blk_Pawns >> 9) & WHT_PIECES & ~H_FILE;
        AVAILABLE_MOVES += PAWN_MOVES;
        for (int i = 0; i < 64; i++) //Iterate through the bitboard
        {
            if (((PAWN_MOVES >> i) & 1) == 1)
            {
                bool isPromotion = false;
                if (i >= 0 && i < 8) isPromotion = true;
                char c = GetCapturedPieceCode(i);
                Move m = new Move(i + 9, i, code, false, false, isPromotion, capturedPiece: c);
                moveList.Add(m);
            }
        }

        //LEFT CAPTURE
        PAWN_MOVES = (blk_Pawns >> 7) & WHT_PIECES & ~A_FILE;
        AVAILABLE_MOVES += PAWN_MOVES;
        for (int i = 0; i < 64; i++) 
        {
            if (((PAWN_MOVES >> i) & 1) == 1)         
            {
                bool isPromotion = false;
                if (i >= 0 && i < 8) isPromotion = true;
                char c = GetCapturedPieceCode(i);
                Move m = new Move(i+7, i, code, false, false, isPromotion, capturedPiece: c);
                moveList.Add(m);
            }

        }

        //FORWARD MOVE ONCE
        PAWN_MOVES = (blk_Pawns >> 8) & EMPTY_SQUARES; //& ~RANK_1;
        AVAILABLE_MOVES += PAWN_MOVES;
        for (int i = 0; i < 64; i++)
        {
            if (((PAWN_MOVES >> i) & 1) == 1)
            {
                bool isPromotion = false;
                if (i >= 0 && i < 8) isPromotion = true;
                Move m = new Move(i+8, i, code, false, false, isPromotion);
                moveList.Add(m);
            }
        }

        //----- Double Move Forward -----
        PAWN_MOVES = (blk_Pawns >> 16) & EMPTY_SQUARES & (EMPTY_SQUARES >> 8) & RANK_5;
        AVAILABLE_MOVES += PAWN_MOVES;
        for (int i = 0; i < 64; i++)
        {
            if (((PAWN_MOVES >> i) & 1) == 1)
            {
                Move m = new Move(i+16, i, code, false, false, false);
                moveList.Add(m);
            }
        }

        /*
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
        } */

        return moveList;
    }

    /*
     * BLACK OR WHITE ROOK, only works for one piece
     */
    public List<Move> GetRookMoves(string history, ulong rook, char code)
    {
        ulong CAPTURABLE;
        if(char.IsUpper(code)) CAPTURABLE = BLK_PIECES;
        else CAPTURABLE = WHT_PIECES;

        List<Move> moveList = new List<Move>();

        #region FORWARD MOVES (Light-Side Oriented)
        ulong nextMove = rook;
        int offset = 8;
        bool isCapture = false;

        while (nextMove != 0 && !isCapture)
        {
            if ((nextMove << 8 & CAPTURABLE) != 0)
            {
                nextMove = nextMove << 8 & CAPTURABLE;
                isCapture = true;
            }
            else if ((nextMove << 8 & EMPTY_SQUARES) != 0) nextMove = nextMove << 8 & EMPTY_SQUARES;
            else break;

            AVAILABLE_MOVES += nextMove;
            
            for (int j = 0; j < 64; j++) //Iterate through the bitboard
            {
                if (((nextMove >> j) & 1) == 1)
                {
                    char c = GetCapturedPieceCode(j);
                    Move m = new Move(j - offset, j, code, false, false, false, capturedPiece: c);
                    moveList.Add(m);    
                }
            }

            offset += 8;
        }
        #endregion

        #region BACKWARDS MOVES (Light-Side Oriented)
        nextMove = rook;
        offset = 8;
        isCapture = false;

        while (nextMove != 0 && !isCapture)
        {
            if ((nextMove >> 8 & CAPTURABLE) != 0)
            {
                nextMove = nextMove >> 8 & CAPTURABLE;
                isCapture = true;
            }
            else if ((nextMove >> 8 & EMPTY_SQUARES) != 0) nextMove = nextMove >> 8 & EMPTY_SQUARES;
            else break;

            AVAILABLE_MOVES += nextMove;
            for (int j = 0; j < 64; j++) //Iterate through the bitboard
                {
                    if (((nextMove >> j) & 1) == 1)
                    {
                        int start = j + offset;
                        int end = j;

                        char c = GetCapturedPieceCode(j);
                        Move m = new Move(start, end, code, false, false, false, capturedPiece: c);
                        moveList.Add(m);
                    }
                }
            offset += 8;
        }
        #endregion

        #region LATERAL MOVEMENT LEFT
        nextMove = rook;
        offset = 1;
        isCapture = false;

        while (nextMove != 0 && !isCapture)
        {
            if ((nextMove >> 1 & CAPTURABLE & ~H_FILE) != 0)
            {
                nextMove = nextMove >> 1 & CAPTURABLE & ~H_FILE;
                isCapture = true;
            }
            else if ((nextMove >> 1 & EMPTY_SQUARES & ~H_FILE) != 0) nextMove = nextMove >> 1 & EMPTY_SQUARES & ~H_FILE;
            else break;
            AVAILABLE_MOVES += nextMove;
            
            for (int j = 0; j < 64; j++) //Iterate through the bitboard
            {
                if (((nextMove >> j) & 1) == 1)
                {
                    int start = j + offset;
                    int end = j;

                    char c = GetCapturedPieceCode(j);
                    Move m = new Move(start, end, code, false, false, false, capturedPiece: c);
                    moveList.Add(m);
                }
            }

            offset += 1;
        }
        #endregion

        #region LATERAL MOVEMENT RIGHT
        nextMove = rook;
        offset = 1;
        isCapture = false;

        while (nextMove != 0 && !isCapture)
        {
            if ((nextMove << 1 & CAPTURABLE & ~A_FILE) != 0)
            {
                nextMove = nextMove << 1 & CAPTURABLE & ~A_FILE;
                isCapture = true;
            }
            else if ((nextMove << 1 & EMPTY_SQUARES & ~A_FILE) != 0) nextMove = (nextMove << 1) & EMPTY_SQUARES & ~A_FILE;
            else break;
            
            AVAILABLE_MOVES += nextMove;

            for (int j = 0; j < 64; j++) //Iterate through the bitboard
            {
                if (((nextMove >> j) & 1) == 1)
                {
                    int start = j - offset;
                    int end = j;

                    char c = GetCapturedPieceCode(j);
                    Move m = new Move(start, end, code, false, false, false, capturedPiece: c);
                    moveList.Add(m);
                }
            }

            offset += 1;
        }
        #endregion

        return moveList;
    }

    public List<Move> GetBishopMoves(string history, ulong bishop, char code)
    {
        ulong CAPTURABLE;
        if (char.IsUpper(code)) CAPTURABLE = BLK_PIECES;
        else CAPTURABLE = WHT_PIECES;

        List<Move> moveList = new List<Move>();

        #region TOWARDS TOP-RIGHT (Light-Side Oriented)
        ulong nextMove = bishop;
        int offset = 1;
        bool isCapture = false;

        while (nextMove != 0 && !isCapture)
        {
            if (((nextMove << 9) & CAPTURABLE & ~A_FILE) != 0)
            {
                nextMove = (nextMove << 9) & CAPTURABLE & ~A_FILE;
                isCapture = true;
            }
            else if (((nextMove << 9) & EMPTY_SQUARES & ~A_FILE) != 0) nextMove = (nextMove << 9 & EMPTY_SQUARES & ~A_FILE);
            else break;

            AVAILABLE_MOVES += nextMove;
            for (int j = 0; j < 64; j++) //Iterate through the bitboard
            {
                if (((nextMove >> j) & 1) == 1)
                {
                    int start = j - 8 * offset - offset;
                    int end = j;

                    char c = GetCapturedPieceCode(j);
                    Move m = new Move(start, end, code, false, false, false, capturedPiece: c);
                    moveList.Add(m);
                }
            }
            offset += 1;
        }
        #endregion
        
        #region TOWARDS TOP-LEFT (Light-Side Oriented)
        nextMove = bishop;
        offset = 1;
        isCapture = false;

        while (nextMove != 0 && !isCapture)
        {
            if (((nextMove << 7) & CAPTURABLE & ~H_FILE) != 0)
            {
                nextMove = (nextMove << 7) & CAPTURABLE & ~H_FILE;
                isCapture = true;
            }
            else if (((nextMove << 7) & EMPTY_SQUARES & ~H_FILE) != 0) nextMove = (nextMove << 7) & EMPTY_SQUARES & ~H_FILE;
            else break;

            AVAILABLE_MOVES += nextMove;
            for (int j = 0; j < 64; j++) //Iterate through the bitboard
            {
                if (((nextMove >> j) & 1) == 1)
                {
                    int start = j - 8 * offset + offset;
                    int end = j;

                    char c = GetCapturedPieceCode(j);
                    Move m = new Move(start, end, code, false, false, false, capturedPiece: c);
                    moveList.Add(m);
                }
            }
            offset += 1;
        }
        #endregion

       #region TOWARDS BOTTOM-RIGHT (Light-Side Oriented)
       nextMove = bishop;
       offset = 1;
       isCapture = false;

       while (nextMove != 0 && !isCapture)
       {
           if (((nextMove >> 7) & CAPTURABLE & ~A_FILE) != 0)
           {
               nextMove = (nextMove >> 7) & CAPTURABLE & ~A_FILE;
               isCapture = true;
           }
           else if (((nextMove >> 7) & EMPTY_SQUARES & ~A_FILE) != 0) nextMove = (nextMove >> 7) & EMPTY_SQUARES & ~A_FILE;
           else break;

           AVAILABLE_MOVES += nextMove;
           for (int j = 0; j < 64; j++) //Iterate through the bitboard
           {
               if (((nextMove >> j) & 1) == 1)
               {
                   int start = j + 8 * offset - offset;
                   int end = j;

                char c = GetCapturedPieceCode(j);
                Move m = new Move(start, end, code, false, false, false, capturedPiece: c);
                   moveList.Add(m);
               }
           }
           offset += 1;
       }
       #endregion


       #region TOWARDS BOTTOM-LEFT
       nextMove = bishop;
       offset = 1;
       isCapture = false;

       while (nextMove != 0 && !isCapture)
       {
           if (((nextMove >> 9) & CAPTURABLE & ~H_FILE) != 0)
           {
               nextMove = (nextMove >> 9) & CAPTURABLE & ~H_FILE;
               isCapture = true;
           }
           else if (((nextMove >> 9) & EMPTY_SQUARES & ~H_FILE) != 0) nextMove = (nextMove >> 9) & EMPTY_SQUARES & ~H_FILE;
           else break;

           AVAILABLE_MOVES += nextMove;
           for (int j = 0; j < 64; j++) //Iterate through the bitboard
           {
               if (((nextMove >> j) & 1) == 1)
               {
                   int start = j + 8 * offset + offset;
                   int end = j;

                char c = GetCapturedPieceCode(j);
                Move m = new Move(start, end, code, false, false, false, capturedPiece: c);
                   moveList.Add(m);
               }
           }
           offset += 1;
       }
       #endregion
        
        return moveList;
    }

    public List<Move> GetKnightMoves(string history, ulong knights, char code)
    {
        ulong CAPTURABLE;
        if (char.IsUpper(code)) CAPTURABLE = BLK_PIECES;
        else CAPTURABLE = WHT_PIECES;

        List<Move> moveList = new List<Move>();

        //FORWARD-RIGHT
        ulong nextMove = (knights << 17) & (EMPTY_SQUARES | CAPTURABLE) & ~A_FILE;
        AVAILABLE_MOVES += nextMove;
        for (int j = 0; j < 64; j++) //Iterate through the bitboard
        {
            if (((nextMove >> j) & 1) == 1)
            {
                int start = j - 17;
                int end = j;

                char c = GetCapturedPieceCode(j);
                Move m = new Move(start, end, code, false, false, false, capturedPiece: c);
                moveList.Add(m);
            }
        }

        //FORWARD-LEFT
        nextMove = (knights << 15) & (EMPTY_SQUARES | CAPTURABLE) & ~H_FILE;
        AVAILABLE_MOVES += nextMove;
        for (int j = 0; j < 64; j++) //Iterate through the bitboard
        {
            if (((nextMove >> j) & 1) == 1)
            {
                int start = j - 15;
                int end = j;

                char c = GetCapturedPieceCode(j);
                Move m = new Move(start, end, code, false, false, false, capturedPiece: c);
                moveList.Add(m);
            }
        }

        //LEFT-FORWARD
        nextMove = (knights << 6) & (EMPTY_SQUARES | CAPTURABLE) & ~H_FILE & ~G_FILE;
        AVAILABLE_MOVES += nextMove;
        for (int j = 0; j < 64; j++) //Iterate through the bitboard
        {
            if (((nextMove >> j) & 1) == 1)
            {
                int start = j - 6;
                int end = j;

                char c = GetCapturedPieceCode(j);
                Move m = new Move(start, end, code, false, false, false, capturedPiece: c);
                moveList.Add(m);
            }
        }

        //LEFT-BACKWARDS
        nextMove = (knights >> 10) & (EMPTY_SQUARES | CAPTURABLE) & ~H_FILE & ~G_FILE;
        AVAILABLE_MOVES += nextMove;
        for (int j = 0; j < 64; j++) //Iterate through the bitboard
        {
            if (((nextMove >> j) & 1) == 1)
            {
                int start = j + 10;
                int end = j;

                char c = GetCapturedPieceCode(j);
                Move m = new Move(start, end, code, false, false, false, capturedPiece: c);
                moveList.Add(m);
            }
        }

        //RIGHT-FORWARD
        nextMove = (knights << 10) & (EMPTY_SQUARES | CAPTURABLE) & ~A_FILE & ~B_FILE;
        AVAILABLE_MOVES += nextMove;
        for (int j = 0; j < 64; j++) //Iterate through the bitboard
        {
            if (((nextMove >> j) & 1) == 1)
            {
                int start = j - 10;
                int end = j;

                char c = GetCapturedPieceCode(j);
                Move m = new Move(start, end, code, false, false, false, capturedPiece: c);
                moveList.Add(m);
            }
        }

        //RIGHT-BACKWARDS
        nextMove = (knights >> 6) & (EMPTY_SQUARES | CAPTURABLE) & ~A_FILE & ~B_FILE;
        AVAILABLE_MOVES += nextMove;
        for (int j = 0; j < 64; j++) //Iterate through the bitboard
        {
            if (((nextMove >> j) & 1) == 1)
            {
                int start = j + 6;
                int end = j;

                char c = GetCapturedPieceCode(j);
                Move m = new Move(start, end, code, false, false, false, capturedPiece: c);
                moveList.Add(m);
            }
        }

        //BACKWARDS-LEFT
        nextMove = (knights >> 17) & (EMPTY_SQUARES | CAPTURABLE) & ~H_FILE;
        AVAILABLE_MOVES += nextMove;
        for (int j = 0; j < 64; j++) //Iterate through the bitboard
        {
            if (((nextMove >> j) & 1) == 1)
            {
                int start = j + 17;
                int end = j;

                char c = GetCapturedPieceCode(j);
                Move m = new Move(start, end, code, false, false, false, capturedPiece: c);
                moveList.Add(m);
            }
        }

        //BACKWARDS-RIGHT
        nextMove = (knights >> 15) & (EMPTY_SQUARES | CAPTURABLE) & ~A_FILE;
        AVAILABLE_MOVES += nextMove;
        for (int j = 0; j < 64; j++) //Iterate through the bitboard
        {
            if (((nextMove >> j) & 1) == 1)
            {
                int start = j + 15;
                int end = j;

                char c = GetCapturedPieceCode(j);
                Move m = new Move(start, end, code, false, false, false, capturedPiece: c);
                moveList.Add(m);
            }
        }

        return moveList;
    }

    #region King Moves and Castling
    public List<Move> GetKingMoves(string history, ulong king, char code)
    {
        ulong FRIENDLY;

        if (char.IsUpper(code)) FRIENDLY = WHT_PIECES;
        else FRIENDLY = BLK_PIECES;

        List<Move> moveList = new List<Move>();
        ulong KING_MOVES = 0;
        int start = 0;

        //Castling
        if (char.IsUpper(code)) //White
        {
            if (whtCanKingSideCastle) KING_MOVES += (king << 2) & EMPTY_SQUARES & (EMPTY_SQUARES << 1);
            if (whtCanQueenSideCastle) KING_MOVES += (king >> 2) & EMPTY_SQUARES & (EMPTY_SQUARES >> 1) & (EMPTY_SQUARES >> 1);  
        }
        else
        {
            if (blkCanKingSideCastle) KING_MOVES += (king << 2) & EMPTY_SQUARES & (EMPTY_SQUARES << 1);
            if (blkCanQueenSideCastle) KING_MOVES += (king >> 2) & EMPTY_SQUARES & (EMPTY_SQUARES >> 1) & (EMPTY_SQUARES >> 1);
        }

        for (int i = 0; i < 64; i++) 
        {
            if (((KING_MOVES >> i) & 1) == 1)
            {
                if (char.IsUpper(code)) start = 4;
                else start = 60;

                Move m = new Move(start, i, code, true, false, false);
                moveList.Add(m);
            }
        }

        //8 Way Movement Check
        KING_MOVES += (king << 8) & ~FRIENDLY; // Forward
        KING_MOVES += (king << 7) & ~FRIENDLY & ~H_FILE; //Top-Left
        KING_MOVES += (king << 9) & ~FRIENDLY & ~A_FILE; //Top-Right

        KING_MOVES += (king >> 1) & ~FRIENDLY & ~H_FILE; //Left
        KING_MOVES += (king << 1) & ~FRIENDLY & ~A_FILE; //Right

        KING_MOVES += (king >> 8) & ~FRIENDLY; //Backwards
        KING_MOVES += (king >> 7) & ~FRIENDLY & ~A_FILE; //Bottom-Right
        KING_MOVES += (king >> 9) & ~FRIENDLY & ~H_FILE; //Bottom-Left

        AVAILABLE_MOVES += KING_MOVES;

        //Find the starting location
        for (int i = 0; i < 64; i++)
        {
            if (((king >> i) & 1) == 1)
            {
                start = i;
            }
        }

        for (int i = 0; i < 64; i++) //will need a helper method for trailing zero counting
        {// int i = long.number of trailingzeros(king moves); i < 64 - ulong.numberofleadingzeros(king moves); i++
            if (((KING_MOVES >> i) & 1) == 1)
            {
                char c = GetCapturedPieceCode(i);
                Move m = new Move(start, i, code, false, false, false, capturedPiece: c);
                moveList.Add(m);
            }
        }

        return moveList;
    }

    //These castling functions were only design with player control in mind, need to rework to modify bitboard directly before the board
    //Was there a change in rook position, if so disable castling for that side
    void CheckForBlkRookChanges()
    {
        if ((blk_Rooks & bKingRook) != blk_Rooks) blkCanKingSideCastle = false;
        if ((blk_Rooks & bQueenRook) != blk_Rooks) blkCanQueenSideCastle = false;
    }

    void CheckForWhtRookChanges()
    {
        if ((wht_Rooks & wKingRook) != wht_Rooks) whtCanKingSideCastle = false;
        if ((wht_Rooks & wQueenRook) != wht_Rooks) whtCanQueenSideCastle = false;
    }

    public void HandleWhiteCastling(int kingLocation)
    {
        if (!whtCanKingSideCastle || !whtCanQueenSideCastle) return;
        whtCanKingSideCastle = false;
        whtCanQueenSideCastle = false;

        if (kingLocation == 6) //King-Side Castle
        {
            Square rookSqr = GetSquareFromIndex(7);
            Square nxtRookSqr = GetSquareFromIndex(5);
            nxtRookSqr.SetNewPiece(rookSqr.GetCurrentPiece());
            rookSqr.ClearReference();
        }
        if (kingLocation == 2)
        {
            Square rookSqr = GetSquareFromIndex(0);
            Square nxtRookSqr = GetSquareFromIndex(3);
            nxtRookSqr.SetNewPiece(rookSqr.GetCurrentPiece());
            rookSqr.ClearReference();
        }
    }
    public void HandleBlackCastling(int kingLocation)
    {
        if (!blkCanKingSideCastle || !blkCanQueenSideCastle) return;
        blkCanKingSideCastle = false;
        blkCanQueenSideCastle = false;

        if (kingLocation == 62) //King-Side Castle
        {
            Square rookSqr = GetSquareFromIndex(63);
            Square nxtRookSqr = GetSquareFromIndex(61);
            nxtRookSqr.SetNewPiece(rookSqr.GetCurrentPiece());
            rookSqr.ClearReference();
        }
        if (kingLocation == 58)
        {
            Square rookSqr = GetSquareFromIndex(56);
            Square nxtRookSqr = GetSquareFromIndex(59);
            nxtRookSqr.SetNewPiece(rookSqr.GetCurrentPiece());
            rookSqr.ClearReference();
        }
    }

    #endregion

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

    public static ulong ReverseBits(ulong x)
    {
        string num = "" + x;
        char[] cNum = num.ToCharArray();
        Array.Reverse(cNum);
        num = new string(cNum);
        return Convert.ToUInt64(num);
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

        for (int row = 0; row <= 7; row++)
        {
            for (int col = 0; col <= 7; col++)
            {

                int c = cNum[i] - '0';
                i--;
                if (c == 1)
                {
                    files[col].Highlight(row);
                }

            }
        }
    }

    public List<ChessBoardFile> GetFiles()
    {
        return files;
    }

    public char GetCapturedPieceCode(int i)
    {
        if (((blk_Pawns >> i) & 1) == 1) return 'p';
        if (((blk_Knights >> i) & 1) == 1) return 'n';
        if (((blk_Bishops >> i) & 1) == 1) return 'b';
        if (((blk_Rooks >> i) & 1) == 1) return 'r';
        if (((blk_Queens >> i) & 1) == 1) return 'q';
        if (((blk_King >> i) & 1) == 1) return 'k';

        if (((wht_Pawns >> i) & 1) == 1) return 'P';
        if (((wht_Knights >> i) & 1) == 1) return 'N';
        if (((wht_Bishops >> i) & 1) == 1) return 'B';
        if (((wht_Rooks >> i) & 1) == 1) return 'R';
        if (((wht_Queens >> i) & 1) == 1) return 'Q';
        if (((wht_King >> i) & 1) == 1) return 'K';

        //Found no captures, return whitespace
        return ' ';
    }

    public Square GetSquareFromIndex(int i)
    {
        int loc = i;
        for (int row = 0; row <= 7; row++)
        {
            for (int col = 0; col <= 7; col++)
            {
                if (loc == 0)
                {
                    return files[col].GetSquare(row);
                }

                loc--;
            }
        }

        Debug.Log("Could not locate that square at " + i);
        return null;
    }

    /*
     * Used by the engine to test different lines of play
     */
    public void MakeMove(Move m)
    {
        //  private ulong blk_Pawns, blk_Knights, blk_Bishops, blk_Rooks, blk_King, blk_Queens;
        //private ulong wht_Pawns, wht_Knights, wht_Bishops, wht_Rooks, wht_King, wht_Queens;
        if (m.isCastle)
        {
            if (char.IsUpper(m.piece)) {
                whtCanKingSideCastle = false;
                whtCanQueenSideCastle = false;
                wht_King &= ~(1ul << m.start);
                wht_King |= (1ul << m.end);
                if (m.end == 6) { //White King Side Castle
                    wht_Rooks &= ~(1ul << 7);
                    wht_Rooks |= (1ul << 5);
                }
                else if (m.end == 2){ //White Queen Side Castle
                    wht_Rooks &= ~(1ul << 0);
                    wht_Rooks |= (1ul << 3);
                }
            }
            else {
                blkCanKingSideCastle = false;
                blkCanQueenSideCastle = false;
                blk_King &= ~(1ul << m.start);
                blk_King |= (1ul << m.end);
                if (m.end == 6) { //Black King Side Castle
                    blk_Rooks &= ~(1ul << 63);
                    blk_Rooks |= (1ul << 61);
                }
                else if (m.end == 2) { //Black Queen Side Castle
                    blk_Rooks &= ~(1ul << 56);
                    blk_Rooks |= (1ul << 59);
                }
            }
        }
        else
        {   //(((blk_Pawns >> m.start) & 1) == 1) maybe not necessary, supposed to have been calculated earlier?
            switch (m.piece) //Simple moves
            {
                case 'p': blk_Pawns &= ~(1ul << m.start);
                    if (m.isPromotion) blk_Queens |= (1ul << m.end);
                    else blk_Pawns |= (1ul << m.end);
                    break;
                case 'n': blk_Knights &= ~(1ul << m.start); blk_Knights |= (1ul << m.end); break;
                case 'b': blk_Bishops &= ~(1ul << m.start); blk_Bishops |= (1ul << m.end); break;
                case 'r': blk_Rooks &= ~(1ul << m.start); blk_Rooks |= (1ul << m.end); break;
                case 'q': blk_Queens &= ~(1ul << m.start); blk_Queens |= (1ul << m.end); break;
                case 'k':
                    blk_King &= ~(1ul << m.start);
                    blk_King |= (1ul << m.end);
                    blkCanKingSideCastle = false;
                    blkCanQueenSideCastle = false;
                    break;

                case 'P': wht_Pawns &= ~(1ul << m.start);
                    if (m.isPromotion) wht_Queens |= (1ul << m.end);
                    else wht_Pawns |= (1ul << m.end);
                    break;
                case 'N': wht_Knights &= ~(1ul << m.start); wht_Knights |= (1ul << m.end); break;
                case 'B': wht_Bishops &= ~(1ul << m.start); wht_Bishops |= (1ul << m.end); break;
                case 'R': wht_Rooks &= ~(1ul << m.start); wht_Rooks |= (1ul << m.end); break;
                case 'Q': wht_Queens &= ~(1ul << m.start); wht_Queens |= (1ul << m.end); break;
                case 'K':
                    wht_King &= ~(1ul << m.start);
                    wht_King |= (1ul << m.end);
                    whtCanKingSideCastle = false;
                    whtCanQueenSideCastle = false;
                    break;
            }
        }

        switch (m.capturedPiece)
        {
            case 'p': blk_Pawns &= ~(1ul << m.end); break;
            case 'n': blk_Knights &= ~(1ul << m.end); break;
            case 'b': blk_Bishops &= ~(1ul << m.end); break;
            case 'r': blk_Rooks &= ~(1ul << m.end); break; 
            case 'q': blk_Queens &= ~(1ul << m.end); break;
            case 'k': blk_King &= ~(1ul << m.end); break;

            case 'P': wht_Pawns &= ~(1ul << m.end); break;
            case 'N': wht_Knights &= ~(1ul << m.end); break;
            case 'B': wht_Bishops &= ~(1ul << m.end); break;
            case 'R': wht_Rooks &= ~(1ul << m.end); break;
            case 'Q': wht_Queens &= ~(1ul << m.end); break;
            case 'K': wht_King &= ~(1ul << m.end); break;

            case ' ': break;
        }

        UpdateBitBoards();
    }

    //Not used anymore
    public void UnmakeMove(Move m)
    {
        if (m.isCastle)
        {
            if (char.IsUpper(m.piece)) { whtCanKingSideCastle = true; whtCanQueenSideCastle = true; }
            else { blkCanKingSideCastle = true; blkCanQueenSideCastle = true; }
        }
        else
        {  
            switch (m.piece)
            {
                case 'p': blk_Pawns &= ~(1ul << m.end); blk_Pawns |= (1ul << m.start); break;
                case 'n': blk_Knights &= ~(1ul << m.end); blk_Knights |= (1ul << m.start); break;
                case 'b': blk_Bishops &= ~(1ul << m.end); blk_Bishops |= (1ul << m.start); break;
                case 'r': blk_Rooks &= ~(1ul << m.end); blk_Rooks |= (1ul << m.start); break;
                case 'q': blk_Queens &= ~(1ul << m.end); blk_Queens |= (1ul << m.start); break;
                case 'k': blk_King &= ~(1ul << m.end); blk_King |= (1ul << m.start); break;

                case 'P': wht_Pawns &= ~(1ul << m.end); wht_Pawns |= (1ul << m.start); break;
                case 'N': wht_Knights &= ~(1ul << m.end); wht_Knights |= (1ul << m.start); break;
                case 'B': wht_Bishops &= ~(1ul << m.end); wht_Bishops |= (1ul << m.start); break;
                case 'R': wht_Rooks &= ~(1ul << m.end); wht_Rooks |= (1ul << m.start); break;
                case 'Q': wht_Queens &= ~(1ul << m.end); wht_Queens |= (1ul << m.start); break;
                case 'K': wht_King &= ~(1ul << m.end); wht_King |= (1ul << m.start); break;
            }
        }

        switch (m.capturedPiece)
        {
            case 'p': blk_Pawns |= (1ul << m.end); break;
            case 'n': blk_Knights |= (1ul << m.end); break;
            case 'b': blk_Bishops |= (1ul << m.end); break;
            case 'r': blk_Rooks |= (1ul << m.end); break;
            case 'q': blk_Queens |= (1ul << m.end); break;
            case 'k': blk_King |= (1ul << m.end); break;

            case 'P': wht_Pawns |= (1ul << m.end); break;
            case 'N': wht_Knights |= (1ul << m.end); break;
            case 'B': wht_Bishops |= (1ul << m.end); break;
            case 'R': wht_Rooks |= (1ul << m.end); break;
            case 'Q': wht_Queens |= (1ul << m.end); break;
            case 'K': wht_King |= (1ul << m.end); break;

            case ' ': break;
        }

        UpdateBitBoards();
    }

}

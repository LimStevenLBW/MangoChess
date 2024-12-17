using System;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    public string initialFEN = "";

    public struct Move
    {
        public Move(int s, int e, bool side, char piece, bool isCapture, bool passantCapture, bool isPromotion, char promotionPiece = ' ')
        {
            start = s;
            end = e;
            this.side = side;
            this.piece = piece;
            this.isCapture = isCapture;
            this.passantCapture = passantCapture;
            this.isPromotion = isPromotion;
            this.promotionPiece = promotionPiece;
        }

        public override string ToString() => $"start{start}, end{end}; {char.ToUpper(piece)}" + (isCapture ? "x" : "");


        int start;       //origin and destination squares (0 - 63)
        int end;
        bool side;           //side moving LIGHT=false or DARK=true

        char piece;          //king=0, queens=2,rooks=4,bishops=6,pawns=8
        bool isCapture;
        bool passantCapture;   //is this move a capture using en passant

        //char captured_piece; //king=0, queens=2,rooks=4,bishops=6,pawns=8        
        // bool canCastle_off[4];
        // bool isCastle[4];
        //char enPassant;         //if the pawn is pushed two places, set en passant square, along with the square the pawn is on for capture

        bool isPromotion;
        char promotionPiece;
    }

    //Bitboards
    ulong blk_Pawns, blk_Knights, blk_Bishops, blk_Rooks, blk_King, blk_Queens;
    ulong wht_Pawns, wht_Knights, wht_Bishops, wht_Rooks, wht_King, wht_Queens;

    //Edge Files
    static ulong A_FILE = 72340172838076673;  //Verified
    //static ulong AB_FILE = 
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

    [SerializeField] private List<ChessBoardFile> file = new List<ChessBoardFile>();

    // Start is called before the first frame update
    void Start()
    {
        //Set the IDs for the squares
        int i = 0;
        for (int row = 0; row <= 7; row++)
        {
            for (int col = 0; col <= 7; col++)
            {
                Square square = file[col].GetSquare(row);
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
        string defaultFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
        if (initialFEN != "") defaultFEN = initialFEN;
        //"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        
        ClearBoard();

        FENtoChessBoard(defaultFEN);
    }

    public void ClearBoard()
    {
        foreach (ChessBoardFile f in file)
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
        Char code = piece.GetCode();

        switch (code)
        {
            case ('r'):
                //blk_Rooks += Convert.ToUInt64(b, 2);
                break;
            case ('n'):
                //blk_Knights += Convert.ToUInt64(b, 2);
                break;
            case ('b'):
                //blk_Bishops += Convert.ToUInt64(b, 2);
                break;
            case ('p'):
                moveList = GetBlackPawnMoves("", Convert.ToUInt64(b, 2));
                break;
            case ('k'):
                //blk_King += Convert.ToUInt64(b, 2);
                break;
            case ('q'):
                //blk_Queens += Convert.ToUInt64(b, 2);
                break;

            case ('R'):
               // wht_Rooks += Convert.ToUInt64(b, 2);
                break;
            case ('N'):
               // wht_Knights += Convert.ToUInt64(b, 2);
                break;
            case ('B'):
               // wht_Bishops += Convert.ToUInt64(b, 2);
                break;
            case ('P'):
                moveList = GetWhitePawnMoves("", Convert.ToUInt64(b, 2));
                break;
            case ('K'):
               // wht_King += Convert.ToUInt64(b, 2);
                break;
            case ('Q'):
                moveList = GetRookMoves("", Convert.ToUInt64(b, 2), code, Game.Side.White);
                break;
            default:
                break;
        }

        foreach (Move m in moveList)
        {
            Debug.Log(m);
        }

        DrawBitboard(AVAILABLE_MOVES); //Show available moves
        AVAILABLE_MOVES = 0; //Reset available moves
    }

    //Retrieve the bitboard value for all pieces
    public void UpdateBitBoards()
    {
        WHT_CANT_CAPTURE = ~(wht_Pawns | wht_Knights | wht_Bishops | wht_Rooks | wht_King | wht_Queens | blk_King);
        BLK_PIECES = blk_Pawns | blk_Knights | blk_Bishops | blk_Rooks | blk_King | blk_Queens;
        WHT_PIECES = wht_Pawns | wht_Knights | wht_Bishops | wht_Rooks | wht_King | wht_Queens;
        EMPTY_SQUARES = ~(blk_Pawns | blk_Knights | blk_Bishops | blk_Rooks | blk_King | blk_Queens | wht_Pawns | wht_Knights | wht_Bishops | wht_Rooks | wht_King | wht_Queens);

        OCCUPIED = BLK_PIECES | WHT_PIECES;
        //Debug.Log("ulong All Pieces: " + OCCUPIED);

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
                    Square square = file[col].GetSquare(row);
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


    //string history, ulong wht_Pawns, ulong wht_Knights, ulong wht_Bishops, ulong wht_Rooks, ulong wht_King, ulong wht_Queens, ulong blk_King, ulong blk_Pawns, ulong blk_Knights, ulong blk_Bishops, ulong blk_Rooks, ulong blk_Queens
    public string GetPossibleMovesWhite()
    { 
        string moveList = "";
       // string moveList = GetWhitePawnMoves(history, wht_Pawns);

        //DrawMoves(GetHorizontalVerticalMoves(9));
        return moveList;
    }

    /*
     * WHITE PAWN, Enpassant and promotion incomplete
     */
    public List<Move> GetWhitePawnMoves(string history, ulong wht_Pawns)
    {
        List<Move> moveList = new List<Move>();

        //RIGHT CAPTURE(actually this might be a left capture, check the formula later)
        //All pieces that a pawn can capture to the right if that piece is not on Rank8(promotion) or A File(can't right capture)
        ulong PAWN_MOVES = (wht_Pawns << 9) & BLK_PIECES & ~RANK_8 & ~A_FILE;
        AVAILABLE_MOVES += PAWN_MOVES;
        for (int i = 0; i < 64; i++) //Iterate through the bitboard
        {
            if (((PAWN_MOVES >> i) & 1) == 1)
            {//if the bit is active
                int start = (i - 9);
                int end = (i);
                Move m = new Move(start, end, false, 'p', true, false, false);
                moveList.Add(m);
                //moveList += "" + (i / 8 + 1) + (i % 8 - 1) + (i / 8) + (i % 8); //(x1,y1,x2,y2) Formula to store the right capture destination as a move
            }
        }

        //LEFT CAPTURE
        PAWN_MOVES = (wht_Pawns << 7) & BLK_PIECES & ~RANK_8 & ~H_FILE; //Left capture
        AVAILABLE_MOVES += PAWN_MOVES;
        for (int i = 0; i < 64; i++) //Iterate through the bitboard, possibly optimize using trailing zeros calculation instead
        {
            if (((PAWN_MOVES >> i) & 1) == 1)             // moveList += "" + (i / 8 + 1) + (i % 8 + 1) + (i / 8) + (i % 8);
            {
                int start = (i - 7);
                int end = (i);
                Move m = new Move(start, end, false, 'p', true, false, false);
                moveList.Add(m);   
            }

        }

        //FORWARD MOVE ONCE
        //----- Forward Moves, if square is empty -----
        PAWN_MOVES = (wht_Pawns << 8) & EMPTY_SQUARES & ~RANK_8;
        AVAILABLE_MOVES += PAWN_MOVES;
        for (int i = 0; i < 64; i++)
        {
            if (((PAWN_MOVES >> i) & 1) == 1)
            {
                int start = (i - 8);
                int end = (i);
                Move m = new Move(start, end, false, 'p', false, false, false);
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
                Move m = new Move(start, end, false, 'p', false, false, false);
                moveList.Add(m);
            }      
        }
          
        foreach (Move m in moveList)
        {
            Debug.Log(m);
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
    public List<Move> GetBlackPawnMoves(string history, ulong blk_Pawns)
    {
        List<Move> moveList = new List<Move>();

        //RIGHT CAPTURE
        ulong PAWN_MOVES = (blk_Pawns >> 9) & WHT_PIECES & ~RANK_8 & ~H_FILE;
        AVAILABLE_MOVES += PAWN_MOVES;
        for (int i = 0; i < 64; i++) //Iterate through the bitboard
        {
            if (((PAWN_MOVES >> i) & 1) == 1)
            {
                int start = (i + 9);
                int end = (i);
                Move m = new Move(start, end, false, 'p', true, false, false);
                moveList.Add(m);
            }
        }

        //LEFT CAPTURE
        PAWN_MOVES = (blk_Pawns >> 7) & WHT_PIECES & ~RANK_8 & ~A_FILE;
        AVAILABLE_MOVES += PAWN_MOVES;
        for (int i = 0; i < 64; i++) 
        {
            if (((PAWN_MOVES >> i) & 1) == 1)         
            {
                int start = (i + 7);
                int end = (i);
                Move m = new Move(start, end, false, 'p', true, false, false);
                moveList.Add(m);
            }

        }

        //FORWARD MOVE ONCE
        PAWN_MOVES = (blk_Pawns >> 8) & EMPTY_SQUARES & ~RANK_1;
        AVAILABLE_MOVES += PAWN_MOVES;
        for (int i = 0; i < 64; i++)
        {
            if (((PAWN_MOVES >> i) & 1) == 1)
            {
                int start = (i + 8);
                int end = (i);
                Move m = new Move(start, end, false, 'p', false, false, false);
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
                int start = (i + 16);
                int end = (i);
                Move m = new Move(start, end, false, 'p', false, false, false);
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
     * BLACK OR WHITE ROOK
     */
    public List<Move> GetRookMoves(string history, ulong rooks, char pieceCode, Game.Side side)
    {
        ulong CAPTURABLE;

        if (side == Game.Side.Black) CAPTURABLE = WHT_PIECES;
        else CAPTURABLE = BLK_PIECES;

        List<Move> moveList = new List<Move>();

        //FORWARD MOVES (Light-Side Oriented)
        ulong moveUp = (rooks << 8) & EMPTY_SQUARES;
        int offset = 8;
        while (moveUp != 0)
        {
            AVAILABLE_MOVES += moveUp;
            for (int j = 0; j < 64; j++) //Iterate through the bitboard
                {
                    if (((moveUp >> j) & 1) == 1)
                    {
                        int start = j - offset;
                        int end = j;
                        
                        Move m = new Move(start, end, false, 'Q', false, false, false);
                        moveList.Add(m);
                    }
                }

            moveUp = (moveUp << 8) & EMPTY_SQUARES;
            offset += 8;

        }

        //BACKWARDS MOVES (Light-Side Oriented)
        moveUp = (rooks >> 8) & EMPTY_SQUARES;
        offset = 8;
        while (moveUp != 0) {
            AVAILABLE_MOVES += moveUp;
            for (int j = 0; j < 64; j++) //Iterate through the bitboard
                {
                    if (((moveUp >> j) & 1) == 1)
                    {
                        int start = j + offset;
                        int end = j;   

                        Move m = new Move(start, end, false, 'Q', false, false, false);
                        moveList.Add(m);
                    }
                }

            moveUp = (moveUp >> 8) & EMPTY_SQUARES;
            offset += 8;
        }

        //LATERAL MOVEMENT LEFT
        moveUp = (rooks >> 1) & EMPTY_SQUARES & ~H_FILE;
        offset = 1;
        while (moveUp != 0)
        {
            AVAILABLE_MOVES += moveUp;
            
            for (int j = 0; j < 64; j++) //Iterate through the bitboard
            {
                if (((moveUp >> j) & 1) == 1)
                {
                    int start = j + offset;
                    int end = j;

                    Move m = new Move(start, end, false, 'Q', false, false, false);
                    moveList.Add(m);
                }
            }
            moveUp = (moveUp >> 1) & EMPTY_SQUARES & ~H_FILE;
            offset += 1;
        }

        //LATERAL MOVEMENT RIGHT
        moveUp = (rooks << 1) & EMPTY_SQUARES & ~H_FILE;
        offset = 1;
        while (moveUp != 0)
        {
            AVAILABLE_MOVES += moveUp;

            for (int j = 0; j < 64; j++) //Iterate through the bitboard
            {
                if (((moveUp >> j) & 1) == 1)
                {
                    int start = j - offset;
                    int end = j;

                    Move m = new Move(start, end, false, 'Q', false, false, false);
                    moveList.Add(m);
                }
            }

            moveUp = (moveUp << 1) & EMPTY_SQUARES & ~A_FILE;
            offset += 1;
        }
        

        /*
        if(moveUp == 0)
        {
            //Check if we can capture a piece
            ulong captureUp = (queen >> 8) & EMPTY_SQUARES & BLK_PIECES;
            AVAILABLE_MOVES += captureUp;
        }*/
        //AVAILABLE_MOVES += moveUp;


        return moveList;
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

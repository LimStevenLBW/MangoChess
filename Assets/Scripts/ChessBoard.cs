using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    //Bitboards
    ulong blk_Pawns, blk_Knights, blk_Bishops, blk_Rooks, blk_King, blk_Queens;
    ulong wht_Pawns, wht_Knights, wht_Bishops, wht_Rooks, wht_King, wht_Queens;

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
                b = b.Substring(0, i) + c + b.Substring(i + 1, b.Length - 1);

                switch (c)
                {
                    case ('r'):
                        blk_Rooks += ulong.Parse(b);
                        break;
                    case ('n'):
                        blk_Knights += ulong.Parse(b);
                        break;
                    case ('b'):
                        blk_Bishops += ulong.Parse(b);
                        break;
                    case ('p'):
                        blk_Pawns += ulong.Parse(b);
                        break;
                    case ('k'):
                        blk_King += ulong.Parse(b);
                        break;
                    case ('q'):
                        blk_Queens += ulong.Parse(b);
                        break;

                    case ('R'):
                        wht_Rooks += ulong.Parse(b);
                        break;
                    case ('N'):
                        wht_Knights += ulong.Parse(b);
                        break;
                    case ('B'):
                        wht_Bishops += ulong.Parse(b);
                        break;
                    case ('P'):
                        wht_Pawns += ulong.Parse(b);
                        break;
                    case ('K'):
                        wht_King += ulong.Parse(b);
                        break;
                    case ('Q'):
                        blk_Queens += ulong.Parse(b);
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
                case ('r'): blk_Rooks += ulong.Parse(b);
                    break;
            }
                


        }
    }

   

    // Update is called once per frame
    void Update()
    {
        
    }
}

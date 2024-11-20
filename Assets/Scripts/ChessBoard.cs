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

    //Starting FEN
    //"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public void FENtoChessBoard(string FEN)
    {
        char[] fenArr = FEN.ToCharArray();
        int i = 0;
        int skip = 0;

        for (int row = 7; row >= 0; row--)
        {
            for(int col = 0; col < 8; col++)
            {
                if (skip > 0)
                {
                    skip--;
                    continue;
                }

                char c = fenArr[i];
                i++;
                if (char.IsNumber(c)) skip = c - 0;

                file[col].CreatePiece(row, c);
            }
        }

        /*
        for(int i=0; i < fenArr.Length; i++)
        {
            char c = fenArr[i];

            if(char.IsNumber(c)
        }
        */
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

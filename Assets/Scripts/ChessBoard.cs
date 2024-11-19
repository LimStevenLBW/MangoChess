using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    //Bitboards
    long blk_Pawns, blk_Knights, blk_Bishops, blk_Rooks, blk_King, blk_Queens;
    long wht_Pawns, wht_Knights, wht_Bishops, wht_Rooks, wht_King, wht_Queens;

    //"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public void FENtoBitBoard(string FEN)
    {
        char[] fenArr = FEN.ToCharArray();
        for(int i = 0; i <= fenArr.Length; i++)
        {

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

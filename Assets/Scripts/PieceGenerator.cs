using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceGenerator : MonoBehaviour
{
    public Piece blk_Pawn, blk_Knight, blk_Bishop, blk_Rook, blk_King, blk_Queen;
    public Piece wht_Pawn, wht_Knight, wht_Bishop, wht_Rook, wht_King, wht_Queen;

    private static PieceGenerator instance;
    public static PieceGenerator Instance { get { return instance; } }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public Piece GetPrefab(char c)
    {
        switch (c)
        {
            case ('r'): return blk_Rook;
            case ('n'): return blk_Knight;
            case ('b'): return blk_Bishop;
            case ('k'): return blk_King;
            case ('q'): return blk_Queen;
            case ('p'): return blk_Pawn;

            case ('R'): return wht_Rook;
            case ('N'): return wht_Knight;
            case ('B'): return wht_Bishop;
            case ('K'): return wht_King;
            case ('Q'): return wht_Queen;
            case ('P'): return wht_Pawn;

            default: return null;
        }
    }
}

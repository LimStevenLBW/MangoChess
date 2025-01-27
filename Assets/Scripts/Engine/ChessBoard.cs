using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    public string FEN;
    private BitBoard board;
    [SerializeField] private List<ChessBoardFile> files = new List<ChessBoardFile>();

    public BitBoard GetBitBoard()
    {
        return board;
    }

    // Start is called before the first frame update
    public void Setup()
    {
        board = new BitBoard(FEN, files);
    }

    public void Reset()
    {
        board.ResetStartingPosition();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

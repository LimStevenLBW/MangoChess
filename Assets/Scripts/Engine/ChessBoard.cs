using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    private BitBoard board;
    [SerializeField] private List<ChessBoardFile> files = new List<ChessBoardFile>();

    public BitBoard GetBitBoard()
    {
        return board;
    }

    // Start is called before the first frame update
    void Start()
    {
        board = new BitBoard(files);
        Game.Instance.InitializeData(board);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

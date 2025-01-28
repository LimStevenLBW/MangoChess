using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    public string FEN;
    private BitBoard board;
    private bool orientation;

    [SerializeField] private List<ChessBoardFile> files = new List<ChessBoardFile>();
    [SerializeField] private List<BoardTextLabel> wlabels = new List<BoardTextLabel>();
    [SerializeField] private List<BoardTextLabel> blabels = new List<BoardTextLabel>();

    public BitBoard GetBitBoard()
    {
        return board;
    }

    public void SetOrientation(Game.Side side)
    {
        if (side == Game.Side.White) orientation = true;
        else orientation = false;

        foreach(BoardTextLabel label in wlabels)
        {
            label.gameObject.SetActive(orientation);
        }
        foreach (BoardTextLabel label in blabels)
        {
            label.gameObject.SetActive(!orientation);
        }
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

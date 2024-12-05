using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoardFile : MonoBehaviour
{
    [SerializeField] List<Square> square;
    public void CreatePiece(int row, char c)
    {
        Piece piece = PieceGenerator.Instance.GetPrefab(c);
        if (piece != null) square[row].CreatePiece(piece);
    }

    public char GetPieceCode(int row)
    {
        return square[row].GetCode();
    }

    public void Highlight(int row)
    {
        square[row].Highlight();
    }

    public void ClearPieceSelection(int row)
    {
        Piece piece = square[row].GetCurrentPiece();
        if (piece != null) piece.DisableOutline();
    }

    public void ClearAllPieces()
    {
        foreach(Square s in square)
        {
            s.ClearPiece();
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

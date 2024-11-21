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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
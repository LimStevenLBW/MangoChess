using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evaluation : MonoBehaviour
{
    private float whiteRating;
    private float blackRating;

    public ChessBoard board;
    private List<ChessBoardFile> files;

    public int GetEvaluation()
    {
        int eval = 0;
        eval += GetMaterialEval();

        return eval;
    }

    private int GetMaterialEval()
    {
        int eval = 0;
        files = board.GetFiles();
        
        for (int row = 0; row < 8; row++)
        {
            for(int col = 0; col < 8; col++)
            {
                char code = files[col].GetPieceCode(row);
                switch (code)
                {
                    case ('r'):
                        
                        break;
                    case ('n'):
                       
                        break;
                    case ('b'):
                       
                        break;
                    case ('p'):
                        
                        break;
                    case ('k'):
                       
                        break;
                    case ('q'):
                        
                        break;

                    case ('R'):
                        
                        break;
                    case ('N'):
                        
                        break;
                    case ('B'):
                        
                        break;
                    case ('P'):
                        
                        break;
                    case ('K'):
                        
                        break;
                    case ('Q'):
                        
                        break;
                    default:
                        break;
                }
            }
        }
        return eval;
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

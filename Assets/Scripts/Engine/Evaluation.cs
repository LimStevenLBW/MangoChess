using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evaluation : MonoBehaviour
{
    private float whiteRating;
    private float blackRating;

    public ChessBoard board;
    private List<ChessBoardFile> files;

    public float GetEvaluation()
    {
        float eval = 0;
        eval += GetMaterialEval();

        return eval;
    }

    private float GetMaterialEval()
    {
        float eval = 0;
        bool isEndgame = false; //Bishops are valued more during the endgame

        files = board.GetFiles();
        
        for (int row = 0; row < 8; row++)
        {
            for(int col = 0; col < 8; col++)
            {
                char code = files[col].GetPieceCode(row);
                switch (code)
                {
                    case ('r'):
                        eval -= 5;
                        break;
                    case ('n'):
                        eval -= 3;
                        break;
                    case ('b'):
                        if(isEndgame) eval -= 3.1f;
                        break;
                    case ('p'):
                        eval -= 1;
                        break;
                    case ('q'):
                        eval -= 9;
                        break;

                    case ('R'):
                        eval += 5;
                        break;
                    case ('N'):
                        eval += 3;
                        break;
                    case ('B'):
                        if (isEndgame) eval += 3.1f;
                        break;
                    case ('P'):
                        eval += 1;
                        break;
                    case ('Q'):
                        eval += 9;
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

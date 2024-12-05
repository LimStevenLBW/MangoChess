using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] char pieceCode;
    protected Outline outline;
    private Game.Side sideColor;
    

    // Start is called before the first frame update
    void Awake()
    {
        if (char.IsUpper(pieceCode)){
            sideColor = Game.Side.White;
        }
        else
        {
            sideColor = Game.Side.Black;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public char GetCode()
    {
        return pieceCode;
    }

    public Game.Side GetSideColor()
    {
        return sideColor;
    }

    public void Select()
    {
        outline.enabled = true;
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }

}

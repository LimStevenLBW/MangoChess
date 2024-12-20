using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public ChessBoard board;
    public GameUI gameUI;
    public Side SideToMove = Side.White;
    public bool DEBUG_MODE;

    public enum Side
    {
        Undecided,
        White,
        Black
    }
    private Side playerSide = Side.Undecided;

    private Piece selectedPiece;

    private static Game instance;
    public static Game Instance { get { return instance; } }

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

    public bool IsStarted()
    {
        if (playerSide != Side.Undecided) return true;
        return false;
    }

    public Side GetPlayerSide()
    {
        return playerSide;
    }

    public void StartGameAsWhite()
    {
        gameUI.HideMenu();
        gameUI.ShowHud();
        playerSide = Side.White;
        gameUI.ShowPlayerToMoveLabel(playerSide);
    }

    public void StartGameAsBlack()
    {
        gameUI.HideMenu();
        gameUI.ShowHud();
        playerSide = Side.Black;
        gameUI.ShowPlayerToMoveLabel(playerSide);
    }

    public void ClearAllSelections()
    {
        for (int row = 7; row >= 0; row--)
        {
            for (int col = 0; col < 8; col++)
            {
                board.GetFiles()[col].ClearPieceSelection(row);
                board.GetFiles()[col].ClearHighlightedSquare(row);
            }
        }
    }

    public void Select(Piece piece)
    {
        ClearAllSelections();

        if(piece != null)
        {
            piece.EnableOutline();
            selectedPiece = piece;
            //Show the piece's available moves
            board.ShowMovementOptions(piece);

        }


        /*
        if (Input.GetMouseButtonDown(0) && Game.Instance.GetSelectedPiece() != currentPiece)
        {
            Game.Instance.Unhover(currentPiece);
        }
        */
    }

    public void Deselect(Piece piece)
    {

    }

    public void Hover(Piece piece)
    {
        piece.EnableOutline();
    }

    public void Unhover(Piece piece)
    {
        if(piece != selectedPiece) piece.DisableOutline();
    }

    public Piece GetSelectedPiece()
    {
        return selectedPiece;
    }

    /*
     * Makes a piece move from player input and updates the board
     */
    public void MakePlayerMove(Square destination)
    {
        if (SideToMove == Side.White) SideToMove = Side.Black;
        else if (SideToMove == Side.Black) SideToMove = Side.White;
        gameUI.ShowPlayerToMoveLabel(SideToMove);

        ClearAllSelections();
        Square formerSquare = selectedPiece.GetSquare();
        formerSquare.ClearReference();

        Piece capture = destination.GetCurrentPiece();
        if(capture == null)
        {
            Debug.Log("not a capture");
        }
        else
        {
            Debug.Log(capture);
        }

        destination.SetNewPiece(selectedPiece);
        selectedPiece.DisableOutline();
        selectedPiece = null;

        board.PosToBitBoard(); //Updates the bitboards and positions
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

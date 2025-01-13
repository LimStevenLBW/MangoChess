using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public ChessBoard board;
    public GameUI gameUI;
    public Side SideToMove = Side.White;
    public bool DEBUG_MODE;

    public MoveCalculator mCalculator;
    public GameAdvantage gameAdvantage;
    public Evaluation evaluation;

    private List<Move> moveHistory;
    public AudioSFX sfx;

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
        mCalculator = new MoveCalculator();
        moveHistory = new List<Move>();

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

    private void SwapPlayerTurn()
    {
        if (SideToMove == Side.White) SideToMove = Side.Black;
        else if (SideToMove == Side.Black) SideToMove = Side.White;
        gameUI.ShowPlayerToMoveLabel(SideToMove);

        if (playerSide != SideToMove) StartCoroutine(ComputerTakeTurn());
    }

    /*
     * Makes a piece move from player input and updates the board
     */
    public void MakePlayerMove(Square destination)
    {
        
        ClearAllSelections();
        Square formerSquare = selectedPiece.GetSquare();
        formerSquare.ClearReference();

        Piece capture = destination.GetCurrentPiece();

        if(capture == null)
        {
            sfx.PlayMoveSFX();
        }
        else
        {
            sfx.PlayCaptureSFX();
            Debug.Log(capture);
        }

        destination.SetNewPiece(selectedPiece);
        if (selectedPiece.GetCode() == 'k') board.DisableBlackCastling();
        if (selectedPiece.GetCode() == 'K') board.DisableWhiteCastling();

        selectedPiece.DisableOutline();
        selectedPiece = null;

        board.PosToBitBoard(); //Updates the bitboards and positions

        gameAdvantage.UpdateAdvantage(evaluation.GetEvaluation());

        SwapPlayerTurn();
    }

    IEnumerator ComputerTakeTurn()
    {

        List<Move> moves = board.GetPossibleMovesBlack();

        Move move = mCalculator.GetCalculation(moves);
        yield return new WaitForSeconds(0.2f);

        DoMove(move);
    }

    void DoMove(Move move)
    {
        sfx.PlayMoveSFX();

        RecordMove(move);
        Square start = board.GetSquareFromIndex(move.start);
        Square end = board.GetSquareFromIndex(move.end);
        Piece piece = start.GetCurrentPiece();
        end.SetNewPiece(piece);

        //King has moved
        if (move.piece == 'k') board.DisableBlackCastling();
        if (move.piece == 'K') board.DisableWhiteCastling();

        start.ClearReference();
        piece.DisableOutline();
        piece = null;

        board.PosToBitBoard(); //Updates the bitboards and positions

        gameAdvantage.UpdateAdvantage(evaluation.GetEvaluation());

        SwapPlayerTurn();
    }

    void RecordMove(Move move)
    {
        moveHistory.Add(move);

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

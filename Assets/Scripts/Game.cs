using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Side SideToMove = Side.White;
    public bool DEBUG_MODE;

    public GameUI gameUI;
    public MoveCalculator mCalculator;
    public GameAdvantage gameAdvantage;

    public AudioSFX sfx;

    private BitBoard board;
    private Evaluation evaluator;
    private List<Move> moveHistory;

    public enum Side
    {
        Undecided,
        White,
        Black
    }
    private Side playerSide = Side.Undecided;
    private Side computerSide = Side.Undecided;

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

    public void InitializeData(BitBoard board)
    {
        this.board = board;
        evaluator = new Evaluation(board);
        mCalculator = new MoveCalculator(evaluator, board);
        moveHistory = new List<Move>();
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
        computerSide = Side.Black;
        gameUI.ShowPlayerToMoveLabel(playerSide);
    }

    public void StartGameAsBlack()
    {
        gameUI.HideMenu();
        gameUI.ShowHud();
        playerSide = Side.Black;
        computerSide = Side.White;
        gameUI.ShowPlayerToMoveLabel(playerSide);

        ComputerTakeTurn();
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

        if (playerSide != SideToMove) ComputerTakeTurn();
    }

    /*
     * Note, should redo to make it use less redundant code, merge with computer makemove
     * Makes a piece move from player input and updates the board
     */
    public void MakePlayerMove(Square destination)
    {
        if (playerSide != SideToMove) return;

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
        }
        //Special Cases
        if (selectedPiece.GetCode() == 'k') board.HandleBlackCastling(destination.GetID());
        if (selectedPiece.GetCode() == 'K') board.HandleWhiteCastling(destination.GetID());
        
        //Pawn promote, buggy, but seems to work as long as pawn promotion isnt the first move
        if (selectedPiece.GetCode() == 'P' && destination.GetID() >= 56) destination.PromotePawn(selectedPiece);
        else if (selectedPiece.GetCode() == 'p' && destination.GetID() < 8) destination.PromotePawn(selectedPiece);
        else destination.SetNewPiece(selectedPiece);

        selectedPiece.DisableOutline();
        selectedPiece = null;

        board.PosToBitBoard(); //Updates the bitboards and positions

        gameAdvantage.UpdateAdvantage(evaluator.GetEvaluation(board));

        if (selectedPiece != null) Select(selectedPiece); //Re-select to prevent move exploit
        SwapPlayerTurn();
    }

    void ComputerTakeTurn()
    {
        bool computerSide;
        if (playerSide == Side.White) computerSide = false;
        else computerSide = true;

        mCalculator.NewLine();
        mCalculator.AlphaBetaSearch(4, -10000f, 10000f, board, computerSide);
        Move move = mCalculator.GetMove();

        //List<Move> moves = new List<Move>();
        //if (SideToMove == Side.Black) moves = board.GetPossibleMovesBlack();
        //if (SideToMove == Side.White) moves = board.GetPossibleMovesWhite();
        //Move move = mCalculator.GetRandomMove(moves);

        Debug.Log("CPU did " + move.ToString());

        StartCoroutine(ComputerCompleteMove(move));
    }

    IEnumerator ComputerCompleteMove(Move move)
    {
        System.Random r = new System.Random();
        float num = (float)(r.NextDouble() + 0.5f);
        yield return new WaitForSeconds(num);

        if (move.capturedPiece == ' ')
        {
            sfx.PlayMoveSFX();
        }
        else
        {
            sfx.PlayCaptureSFX();
        }

        RecordMove(move);

        Square start = board.GetSquareFromIndex(move.start);
        Square end = board.GetSquareFromIndex(move.end);
        Piece piece = start.GetCurrentPiece();
        

        //King has moved
        if (move.piece == 'k') board.HandleBlackCastling(move.end);
        if (move.piece == 'K') board.HandleWhiteCastling(move.end);

        //Pawn promote, buggy, but seems to work as long as pawn promotion isnt the first move
        if (piece.GetCode() == 'P' && move.end >= 56) end.PromotePawn(piece);
        else if (piece.GetCode() == 'p' && move.end < 8) end.PromotePawn(piece);
        else end.SetNewPiece(piece);

        start.ClearReference();
        piece.DisableOutline();

        board.PosToBitBoard(); //Updates the bitboards and positions

        gameAdvantage.UpdateAdvantage(evaluator.GetEvaluation(board));

        SwapPlayerTurn();
        if (selectedPiece != null) Select(selectedPiece); //Re-select to prevent move exploit

    }

    void RecordMove(Move move)
    {
        moveHistory.Add(move);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

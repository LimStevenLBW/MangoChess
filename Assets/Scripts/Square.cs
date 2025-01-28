using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Square : MonoBehaviour
{
    [SerializeField] private Material defaultMat;
    [SerializeField] private Material highlightMat;
    [SerializeField] private Piece currentPiece;
    private TextMeshPro ID_TEXT;

    private int idNumber = -1;
    private MeshRenderer mesh;
    private bool isMovementSquare = false;

    //Pieces are ignored because they are on the ignore raycast layer
    void OnMouseOver()
    { 
        //On player owned piece
        if (currentPiece != null && currentPiece.GetSideColor() == Game.Instance.GetPlayerSide() && !Game.Instance.isGameOver)
        { 
            if (Input.GetMouseButtonDown(0))
            {
                Game.Instance.Select(currentPiece);
            }
            else
            {
                //If your mouse hovers over the GameObject with the script attached, output this message
                Game.Instance.Hover(currentPiece);
            }
        }
        else if(isMovementSquare && Input.GetMouseButtonDown(0) && !Game.Instance.isGameOver)//On a highlighted square
        {
            Game.Instance.MakePlayerMove(this);
        }
        else if(Input.GetMouseButtonDown(0)) //On any other square
        {
            Game.Instance.ClearAllSelections();
        }
    }

    void OnMouseExit()
    { 
        //The mouse is no longer hovering over the GameObject so output this message each frame
        //Debug.Log("Mouse is no longer on GameObject.");

        // Start is called before the first frame update
        if (currentPiece != null && currentPiece.GetSideColor() == Game.Instance.GetPlayerSide()) {
            Game.Instance.Unhover(currentPiece);
        }
    }
   
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        if (defaultMat == null) defaultMat = mesh.material;
        else DisplayID();

        //Piece piece = Instantiate(defaultPiece, pos, Quaternion.identity);
    }

    public void SetID(int id)
    {
        idNumber = id;


        ID_TEXT = transform.GetChild(0).GetComponent<TextMeshPro>();
        if (Game.Instance.DEBUG_MODE == false) ID_TEXT.gameObject.SetActive(false);
        ID_TEXT.SetText("" + idNumber);
    }

    public int GetID()
    {
        return idNumber;
    }

    public void DisplayID()
    {
        ID_TEXT.gameObject.SetActive(true);
    }


    //Generates a new piece based on the char code
    public void CreatePiece(char c)
    {
        Piece piece = PieceGenerator.Instance.GetPrefab(c);

        if (piece == null)
        {
            Debug.Log("Create Piece function could not run due to null id");
            return;
        }

        Vector3 pos = transform.position;
        pos.y += .2f;
        currentPiece = Instantiate(piece, pos, piece.transform.rotation);
        currentPiece.SetSquare(this);
    }

    public void HighlightSquare()
    {
        mesh.material = highlightMat;
        isMovementSquare = true;
    }

    public void ClearHighlight()
    {
        mesh.material = defaultMat;
        isMovementSquare = false;
    }

    public char GetCode()
    {
        if (currentPiece == null) return ' ';
        return currentPiece.GetCode();
    }

    //Move a new piece into this square
    public void SetNewPiece(Piece piece)
    {
        Vector3 pos = transform.position;
        pos.y += .05f;
        if (currentPiece != null)
        {
            Destroy(currentPiece.gameObject);
        }

        currentPiece = piece;
        currentPiece.SetSquare(this);
        currentPiece.transform.position = pos;
    }

    //Move a new piece into this square
    public void PromotePawn(Piece piece)
    {
        Vector3 pos = transform.position;
        pos.y += .05f;
        if (currentPiece != null)
        {
            Destroy(currentPiece.gameObject);
        }



        if (char.IsUpper(piece.GetCode())) CreatePiece('Q');
        else CreatePiece('q');

        Destroy(piece.gameObject);
    }

    //Getter
    public Piece GetCurrentPiece()
    {
        return currentPiece;
    }

    //Clears the piece and destroys the gameobject
    public void ClearPiece()
    {
        if (currentPiece)
        {
            Destroy(currentPiece.gameObject);
            currentPiece = null;  
        }
    }

    //Only removes the reference
    public void ClearReference()
    {
        if (currentPiece) currentPiece = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

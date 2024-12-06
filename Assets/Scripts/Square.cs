using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    [SerializeField] private Material defaultMat;
    [SerializeField] private Material highlightMat;
    [SerializeField] private Piece currentPiece;

    private int idNumber = -1;
    private MeshRenderer mesh;

    public void SetID(int id)
    {
        idNumber = id;
    }

    public int GetID()
    {
        return idNumber;
    }

    //Pieces are ignored because they are on the ignoreraycast layer
    void OnMouseOver()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
        if (currentPiece != null && currentPiece.GetSideColor() == Game.Instance.GetPlayerSide())
        {
            if (Input.GetMouseButtonDown(0))
            {
                Game.Instance.Select(currentPiece);
            }
            else
            {
                Game.Instance.Hover(currentPiece);
            }
        }
        else if(currentPiece == null)
        {
            //Try to move a piece
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
       

        //Piece piece = Instantiate(defaultPiece, pos, Quaternion.identity);
    }


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

    public void Highlight()
    {
        mesh.material = highlightMat;
    }

    public char GetCode()
    {
        if (currentPiece == null) return ' ';
        return currentPiece.GetCode();
    }

    public Piece GetCurrentPiece()
    {
        return currentPiece;
    }

    public void ClearPiece()
    {
        if(currentPiece) Destroy(currentPiece.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

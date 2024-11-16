using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    [SerializeField] private Material defaultMat;
    [SerializeField] private Material highlightMat;
    [SerializeField] private Piece defaultPiece;
    [SerializeField] private Piece piece;

    void OnMouseOver()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
        Debug.Log("Mouse is over GameObject.");
    }

    void OnMouseExit()
    {
        //The mouse is no longer hovering over the GameObject so output this message each frame
        Debug.Log("Mouse is no longer on GameObject.");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (defaultPiece == null) return;
        Vector3 pos = transform.position;
        pos.y += 4.2f;

        Piece piece = Instantiate(defaultPiece, pos, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

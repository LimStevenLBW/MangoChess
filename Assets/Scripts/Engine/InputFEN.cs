using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputFEN : MonoBehaviour
{
    [SerializeField] private ChessBoard board;
    
    private TMP_InputField input;
    private bool isFocused;
    private string initialFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<TMP_InputField>();
        input.onSelect.AddListener((string text) => isFocused = true);
        input.onDeselect.AddListener((string text) => isFocused = false);
    }

    // Update is called once per frame
    void Update()
    {
        //Note, do not use input.isFocused, that becomes false when Return is pressed before the if-statement can run
        if (isFocused && Input.GetKeyDown(KeyCode.Return)) board.FENtoChessBoard(input.text);
    }
}

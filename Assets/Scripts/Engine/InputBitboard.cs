using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputBitboard : MonoBehaviour
{
    [SerializeField] private ChessBoard board;
    private TMP_InputField input;
    private bool isFocused;

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
        if (isFocused && Input.GetKeyDown(KeyCode.Return)) board.DrawBitboard(input.text);
    }
}

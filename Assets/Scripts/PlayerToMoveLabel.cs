using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerToMoveLabel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Image labelImage;
    public void DisplayLabel()
    {
        gameObject.SetActive(true);
    }

    public void HideLabel()
    {
        gameObject.SetActive(false);
    }

    public void UpdateLabel(Game.Side s)
    {
        if (s == Game.Side.White)
        {
            label.SetText("White to Move");
            labelImage.color = Color.white;
        }
        else if (s == Game.Side.Black)
        {
            label.SetText("Black to Move");
            labelImage.color = Color.black;
        }
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

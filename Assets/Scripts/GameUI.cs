using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public PlayerToMoveLabel playerToMoveLabel;
    public List<GameObject> canvasObjects;

    public void ShowPlayerToMoveLabel(Game.Side playerSide)
    {
        playerToMoveLabel.UpdateLabel(playerSide);
        playerToMoveLabel.DisplayLabel();
    }

    public void HideMenu()
    {
        foreach(GameObject obj in canvasObjects)
        {
            obj.SetActive(false);
        }
    }

    public void ShowMenu()
    {
        foreach (GameObject obj in canvasObjects)
        {
            obj.SetActive(true);
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

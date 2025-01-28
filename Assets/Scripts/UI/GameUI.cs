using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public PlayerToMoveLabel playerToMoveLabel;
    public List<GameObject> menuObjects;
    public List<GameObject> hudObjects;
    public Check checkText;

    public void ShowCheck()
    {
        checkText.ShowCheck();
    }
    public void ShowPlayerToMoveLabel(Game.Side playerSide)
    {
        playerToMoveLabel.UpdateLabel(playerSide);
        playerToMoveLabel.DisplayLabel();
    }

    public void HideMenu()
    {
        foreach(GameObject obj in menuObjects)
        {
            obj.SetActive(false);
        } 
    }

    public void HideHud()
    {
        foreach (GameObject obj in hudObjects)
        {
            obj.SetActive(false);
        }
    }

    public void ShowHud()
    {
        foreach (GameObject obj in hudObjects)
        {
            obj.SetActive(true);
        }
    }

    public void ShowMenu()
    {
        foreach (GameObject obj in menuObjects)
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

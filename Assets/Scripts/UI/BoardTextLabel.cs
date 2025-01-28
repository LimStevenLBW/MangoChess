using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTextLabel : MonoBehaviour
{
    Vector3 blackOrientation = new Vector3(90, 0, 0);
    Vector3 whiteOrientation = new Vector3(-90, 0, 0);

    public void SetOrientation(Game.Side side)
    {
        
        //if (side == Game.Side.White) transform.rotation = Quaternion.Euler(whiteOrientation);
        //if (side == Game.Side.Black) transform.rotation = Quaternion.Euler(blackOrientation);

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

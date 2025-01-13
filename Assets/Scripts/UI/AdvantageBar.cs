using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvantageBar : MonoBehaviour
{
    private float defaultScale = 0.5f;
    
    //A value of 0 will set this to the default scale
    //A value of 10 or more should almost max the bar
    //At 15 or higher it is maxed
    public void UpdateDisplay(float value)
    {
        Vector3 scale = transform.localScale;
        float y = defaultScale + (value * 0.05f);
        if (y > 1) y = 1;

        transform.localScale = new Vector3(scale.x, y, scale.z);
        

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

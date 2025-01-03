using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameAdvantage : MonoBehaviour
{
    [SerializeField] private AdvantageBar white;
    [SerializeField] private AdvantageBar black;
    [SerializeField] private TextMeshProUGUI evalValue;

    public void UpdateAdvantage(float evaluation)
    {
        white.UpdateDisplay(evaluation);
        black.UpdateDisplay(-evaluation);

        string evalText;
        if (evaluation >= 0) evalText = "+";
        evalText = evaluation.ToString();
        
        evalValue.SetText(evalText);
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

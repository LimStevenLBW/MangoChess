using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Check : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void ShowCheck()
    {
        text.enabled = true;
        StartCoroutine(DisableText());
    }

    IEnumerator DisableText()
    {
        yield return new WaitForSeconds(0.7f);
        text.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        text.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

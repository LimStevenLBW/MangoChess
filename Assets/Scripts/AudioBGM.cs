using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBGM : MonoBehaviour
{
    private AudioSource source;
    private bool isMuted = false;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;

        if (isMuted) source.volume = 0;
        else source.volume = 0.1f;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

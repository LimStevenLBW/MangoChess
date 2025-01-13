using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSFX : MonoBehaviour
{
    public AudioClip moveSFX;
    public AudioClip captureSFX;
    private AudioSource source;


    public void PlayMoveSFX() { source.PlayOneShot(moveSFX); }
    public void PlayCaptureSFX() { source.PlayOneShot(captureSFX); }

    // Start is called before the first frame update
    void Start()
    {
        source.GetComponent<AudioSource>();
    }


}

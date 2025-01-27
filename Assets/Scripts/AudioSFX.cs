using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSFX : MonoBehaviour
{
    public AudioClip moveSFX;
    public AudioClip captureSFX;
    public AudioClip promoteSFX;
    public AudioClip checkSFX;
    public AudioClip victorySFX;
    public AudioClip gameoverSFX;

    private AudioSource source;


    public void PlayMoveSFX() { source.PlayOneShot(moveSFX); }
    public void PlayCaptureSFX() { source.PlayOneShot(captureSFX); }

    public void PlayVictorySFX() { source.PlayOneShot(victorySFX); }
    public void PlayGameOverSFX() { source.PlayOneShot(gameoverSFX); }

    public void PlayPromoteSFX() { source.PlayOneShot(promoteSFX); }
    public void PlayCheckSFX() { source.PlayOneShot(checkSFX); }

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MusicControllerUI : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
    }

    public void PlayMusic()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}


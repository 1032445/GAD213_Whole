using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreakEmitter : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] creakClips;

    public void PlayCreak()
    {
        if (creakClips.Length == 0) return;

        audioSource.clip = creakClips[Random.Range(0, creakClips.Length)];
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.volume = Random.Range(0.5f, 1f);
        audioSource.Play();
    }
}

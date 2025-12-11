using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGroan : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip growlClip;

    public bool playOnce = true;
    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (playOnce && hasPlayed)
            return;

        hasPlayed = true;

        if (audioSource != null && growlClip != null)
            audioSource.PlayOneShot(growlClip);
    }
}

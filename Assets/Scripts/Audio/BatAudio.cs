using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatAudio : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Clips")]
    public AudioClip idleLoop;       // flapping / ambient loop
    public AudioClip screechClip;    // when spawned or aggro
    public AudioClip hitClip;        // when damaged by light
    public AudioClip deathClip;      // on death

    [Header("Settings")]
    public float idleVolume = 0.4f;
    public float screechVolume = 1f;
    public float hitVolume = 0.7f;
    public float deathVolume = 1f;

    void Start()
    {
        // Start idle loop
        if (idleLoop != null)
        {
            audioSource.clip = idleLoop;
            audioSource.loop = true;
            audioSource.volume = idleVolume;
            audioSource.Play();
        }

        // Screech on spawn
        PlayScreech();
    }

    public void PlayScreech()
    {
        if (screechClip != null)
            audioSource.PlayOneShot(screechClip, screechVolume);
    }

    public void PlayHit()
    {
        if (hitClip != null)
            audioSource.PlayOneShot(hitClip, hitVolume);
    }

    public void PlayDeath()
    {
        if (deathClip != null)
            audioSource.PlayOneShot(deathClip, deathVolume);
    }
}
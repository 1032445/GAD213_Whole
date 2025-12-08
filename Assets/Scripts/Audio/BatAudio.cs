using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatAudio : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Clips")]
    public AudioClip idleLoop;
    public AudioClip screechClip;
    public AudioClip hitClip;
    public AudioClip deathClip;

    [Header("Volumes")]
    public float idleVolume = 0.4f;
    public float screechVolume = 1f;
    public float hitVolume = 0.7f;
    public float deathVolume = 1f;

    [Header("Cooldowns")]
    public float hitCooldown = 0.12f;
    private float hitCooldownTimer = 0f;

    void Start()
    {
        audioSource.loop = false;
        audioSource.ignoreListenerVolume = false;
        audioSource.ignoreListenerPause = false;

        // start idle loop
        if (idleLoop != null)
        {
            audioSource.clip = idleLoop;
            audioSource.volume = idleVolume;
            audioSource.loop = true;
            audioSource.Play();
        }

        // screech once on spawn
        PlayScreech();
    }

    void Update()
    {
        if (hitCooldownTimer > 0)
            hitCooldownTimer -= Time.deltaTime;
    }

    public void PlayScreech()
    {
        if (screechClip != null)
            audioSource.PlayOneShot(screechClip, screechVolume);
    }

    public void PlayHit()
    {
        if (hitCooldownTimer > 0)
            return;

        hitCooldownTimer = hitCooldown;

        if (hitClip != null)
            audioSource.PlayOneShot(hitClip, hitVolume);
    }

    public void PlayDeath()
    {
        // stop idle loop so the death is clean
        audioSource.Stop();

        if (deathClip != null)
            audioSource.PlayOneShot(deathClip, deathVolume);
    }
}
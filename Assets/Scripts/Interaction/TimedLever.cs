using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedLever : MonoBehaviour
{
    [Header("Lever Settings")]
    public Transform pivotPoint;
    public float onAngleOffset = -60f;
    public float turnSpeed = 5f;
    public float activeDuration = 5f;

    [Header("Linked Object")]
    public BridgeMover bridgeMover;

    [Header("Audio")]
    public AudioSource tickingSource;
    public AudioSource sfxSource;
    public AudioClip activationClip;
    public AudioClip deactivationClip;

    private bool isActive = false;
    private float timer = 0f;
    private bool playerNearby = false;

    private Quaternion startRotation;
    private Quaternion onRotation;

    public BatSpawner spawner;

    void Start()
    {
        startRotation = pivotPoint.localRotation;
        onRotation = startRotation * Quaternion.Euler(0, 0, onAngleOffset);

        if (tickingSource != null)
        {
            tickingSource.loop = true;
            tickingSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            ActivateLever();
        }

        if (isActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
                DeactivateLever();
        }

        Quaternion targetRot = isActive ? onRotation : startRotation;

        pivotPoint.localRotation = Quaternion.Lerp(
            pivotPoint.localRotation,
            targetRot,
            Time.deltaTime * turnSpeed
        );
    }

    private void ActivateLever()
    {
        isActive = true;
        timer = activeDuration;

        if (bridgeMover != null)
            bridgeMover.isActive = true;

        if (tickingSource != null && !tickingSource.isPlaying)
            tickingSource.Play();

        if (sfxSource != null && activationClip != null)
            sfxSource.PlayOneShot(activationClip);
    }

    private void DeactivateLever()
    {
        isActive = false;

        if (bridgeMover != null)
            bridgeMover.isActive = false;

        if (tickingSource != null && tickingSource.isPlaying)
            tickingSource.Stop();

        if (sfxSource != null && deactivationClip != null)
            sfxSource.PlayOneShot(deactivationClip);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = false;
    }
}
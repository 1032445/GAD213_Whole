using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTarget : MonoBehaviour, ISwitchTarget
{
    [Header("Movement")]
    public Vector3 openOffset = new Vector3(0, 3f, 0);
    public float speed = 2f;

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool isOpen = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip moveClip;

    private bool previousState = false;

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + openOffset;

        previousState = isOpen;
    }

    public void Activate()
    {
        if (!isOpen)
            PlayMoveSound();

        isOpen = true;
    }

    public void Deactivate()
    {
        if (isOpen)
            PlayMoveSound();

        isOpen = false;
    }

    void Update()
    {
        Vector3 target = isOpen ? openPos : closedPos;

        transform.position = Vector3.Lerp(
            transform.position,
            target,
            Time.deltaTime * speed
        );
    }

    void PlayMoveSound()
    {
        if (audioSource != null && moveClip != null)
            StartCoroutine(DelayedPlay());
    }

    IEnumerator DelayedPlay()
    {
        yield return new WaitForSeconds(0.05f); // small delay to avoid overlap
        audioSource.PlayOneShot(moveClip);
    }
}
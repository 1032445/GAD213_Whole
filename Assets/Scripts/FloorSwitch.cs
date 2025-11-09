using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSwitch : MonoBehaviour
{
    [Header("Switch Settings")]
    public Transform switchPlate;
    public float pressedHeight = 0.05f;
    public float speed = 2f;
    public AudioSource switchSound;

    [Header("Bridge Settings")]
    public BridgeMover bridgeMover;

    [Header("Audio")]
    public AudioSource audioSource; 
    public AudioClip pressClip;
    public AudioClip releaseClip;

    private Vector3 originalPosition;
    private Vector3 pressedPosition;
    private bool isPressed = false;

    private void Start()
    {
        if (switchPlate == null)
        {
            Debug.LogError("Switchplate not assigned");
            return;
        }

        originalPosition = switchPlate.position;
        pressedPosition = originalPosition - new Vector3(0, pressedHeight, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("Pickup")) && !isPressed)
        {
            isPressed = true;

            if (bridgeMover != null)
                bridgeMover.isActive = true;

            if (audioSource != null && pressClip != null)
                audioSource.PlayOneShot(pressClip);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Pickup"))
        {
            isPressed = false;

            if (bridgeMover != null)
                bridgeMover.isActive = false;

            if (audioSource != null && releaseClip != null)
                audioSource.PlayOneShot(releaseClip);

        }
    }

    private void FixedUpdate()
    {
        Vector3 targetPos = isPressed ? pressedPosition : originalPosition;
        switchPlate.position = Vector3.Lerp(switchPlate.position, targetPos, Time.fixedDeltaTime * speed);
    }
}

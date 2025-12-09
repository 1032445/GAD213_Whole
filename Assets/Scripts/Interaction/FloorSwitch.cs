using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSwitch : MonoBehaviour
{
    public enum SwitchMode { Normal, OneTime }

    [Header("Switch Settings")]
    public Transform switchPlate;
    public float pressedHeight = 0.05f;
    public float speed = 2f;
    public SwitchMode mode = SwitchMode.Normal;

    [Header("Target Object")]
    public MonoBehaviour targetBehaviour;
    private ISwitchTarget target;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pressClip;
    public AudioClip releaseClip;

    private Vector3 originalPos;
    private Vector3 pressedPos;

    private int objectsOnSwitch = 0;
    private bool isPressed = false;
    private bool hasActivatedOnce = false;

    void Start()
    {
        target = targetBehaviour as ISwitchTarget;

        originalPos = switchPlate.position;
        pressedPos = originalPos - new Vector3(0, pressedHeight, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Pickup"))
            return;

        objectsOnSwitch++;

        if (mode == SwitchMode.Normal)
            HandleNormalPress();
        else
            HandleOneTimePress();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Pickup"))
            return;

        objectsOnSwitch = Mathf.Max(0, objectsOnSwitch - 1);

        if (mode == SwitchMode.Normal)
            HandleNormalRelease();
        // never releases
    }

    // normal mode switch press
    void HandleNormalPress()
    {
        if (!isPressed)
        {
            isPressed = true;
            target?.Activate();
            PlayClip(pressClip);
        }
    }

    void HandleNormalRelease()
    {
        if (objectsOnSwitch == 0 && isPressed)
        {
            isPressed = false;
            target?.Deactivate();
            PlayClip(releaseClip);
        }
    }

    // one-time press
    void HandleOneTimePress()
    {
        if (!hasActivatedOnce)
        {
            hasActivatedOnce = true;
            isPressed = true;  // permanently down
            target?.Activate();
            PlayClip(pressClip);
        }
    }

    void FixedUpdate()
    {
        Vector3 targetPos =
            mode == SwitchMode.Normal
                ? (isPressed ? pressedPos : originalPos)
                : (hasActivatedOnce ? pressedPos : originalPos);

        switchPlate.position = Vector3.Lerp(
            switchPlate.position,
            targetPos,
            Time.fixedDeltaTime * speed
        );
    }

    void PlayClip(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    // state for PuzzleManager
    public bool IsPressed()
    {
        return mode == SwitchMode.Normal ? isPressed : hasActivatedOnce;
    }
}
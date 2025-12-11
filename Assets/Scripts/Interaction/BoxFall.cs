using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxFall : MonoBehaviour
{
    [Header("Object to Move")]
    public Rigidbody targetObject;
    public Vector3 pushForce = new Vector3(0, -3f, 0);

    [Header("Rotation Settings")]
    public Vector3 torqueForce = new Vector3(80f, 0f, 0f);

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Settings")]
    public bool triggerOnce = true;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        // play crash
        if (audioSource != null)
        {
            audioSource.Play();
        }

        // apply force + rotation torque
        if (targetObject != null)
        {
            targetObject.isKinematic = false;   // allow physics

            // push it off the shelf
            targetObject.AddForce(pushForce, ForceMode.Impulse);

            // add rotation to make it tip forward
            targetObject.AddTorque(torqueForce, ForceMode.Impulse);
        }
    }
}

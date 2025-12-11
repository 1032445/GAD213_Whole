using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightKillTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        Flashlight flashlight = other.GetComponentInChildren<Flashlight>();
        if (flashlight != null)
        {
            flashlight.KillFlashlight();
        }
    }
}
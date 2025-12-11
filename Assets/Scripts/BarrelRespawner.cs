using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelRespawner : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip barrelDissolveClip;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            // play acid sound
            if (audioSource != null && barrelDissolveClip != null)
                audioSource.PlayOneShot(barrelDissolveClip);

            // tell this specific barrel to respawn
            BarrelSelfRespawn respawn = other.GetComponent<BarrelSelfRespawn>();
            if (respawn != null)
                respawn.Respawn();
        }
    }
}
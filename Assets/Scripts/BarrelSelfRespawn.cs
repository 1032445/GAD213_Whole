using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelSelfRespawn : MonoBehaviour
{
    [Header("Respawn")]
    public Transform respawnPoint;

    public void Respawn()
    {
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
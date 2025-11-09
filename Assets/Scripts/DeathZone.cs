using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            other.transform.position = respawnPoint.position;
            other.GetComponent<Rigidbody>().velocity = Vector3.zero; // stop any momentum
        }
    }
}

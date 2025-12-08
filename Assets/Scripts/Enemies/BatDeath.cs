using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatDeath : MonoBehaviour
{
    public float fallSpeed = 3f;
    public float tumbleSpeed = 180f;
    public float destroyDelay = 3f;

    private bool isDead = false;
    private Rigidbody rb;
    private Animator anim;
    private BatEnemy batEnemy;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        batEnemy = GetComponent<BatEnemy>();
    }

    public void StartDeath()
    {
        if (isDead) return;
        isDead = true;

        // stop bat AI
        if (batEnemy != null)
            batEnemy.enabled = false;

        // stop flapping animation
        if (anim != null)
            anim.enabled = false;

        // enable gravity so it falls
        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.velocity = Vector3.zero; // reset any weird movement
        }

        // tumble
        rb.angularVelocity = Random.insideUnitSphere * tumbleSpeed;

        // destroy after delay
        Destroy(gameObject, destroyDelay);
    }
}

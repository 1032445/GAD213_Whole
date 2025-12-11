using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEnemy : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 3f;
    private float currentHealth;
    private bool isDead = false;

    [Header("References")]
    private BatAudio audioHandler;
    private BatEnemy batEnemy;
    private Rigidbody rb;

    [Header("Knockback Settings")]
    public float knockbackForce = 4f;
    public float maxKnockbackSpeed = 5f;


    void Start()
    {
        currentHealth = maxHealth;

        audioHandler = GetComponent<BatAudio>();
        batEnemy = GetComponent<BatEnemy>();
        rb = GetComponent<Rigidbody>();

        if (rb == null)
            Debug.LogError("LightEnemy requires a Rigidbody!");
    }

    void FixedUpdate()
    {
        // clamp knockback speed
        if (rb.velocity.magnitude > maxKnockbackSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxKnockbackSpeed;
        }
    }

    public void TakeLightDamage(float amount)
    {
        if (isDead)
            return;

        currentHealth -= amount;

        if (audioHandler != null)
            audioHandler.PlayHit();

        ApplyKnockback();

        if (currentHealth <= 0)
            Die();
    }

    void ApplyKnockback()
    {
        if (batEnemy == null || batEnemy.player == null || rb == null)
            return;

        // direction away from the player
        Vector3 dir = transform.position - batEnemy.player.position;
        dir.y = 0f;
        dir = dir.normalized;

        // knockback burst
        rb.AddForce(dir * knockbackForce, ForceMode.VelocityChange);
    }


    void Die()
    {
        isDead = true;

        // stop movement logic
        if (batEnemy != null)
            batEnemy.enabled = false;

        if (audioHandler != null && audioHandler.deathClip != null)
            audioHandler.PlayDeath();

        BatDeath death = GetComponent<BatDeath>();
        if (death != null)
            death.StartDeath();
        else
            Destroy(gameObject);
    }
}
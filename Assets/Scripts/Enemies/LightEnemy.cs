using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEnemy : MonoBehaviour
{
    public float maxHealth = 3f;
    float currentHealth;

    [Header("References")]
    private BatAudio audioHandler;
    private BatEnemy batEnemy;

    [Header("Knockback")]
    public float knockbackStrength = 2f;
    public float knockbackDuration = 0.1f;

    private float knockbackTimer = 0f;
    private Vector3 knockbackDirection;

    void Start()
    {
        currentHealth = maxHealth;
        audioHandler = GetComponent<BatAudio>();
        batEnemy = GetComponent<BatEnemy>();
    }

    void Update()
    {
        // apply knockback if active
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;

            // move opposite of player direction
            transform.position += knockbackDirection * knockbackStrength * Time.deltaTime;
        }
    }

    public void TakeLightDamage(float amount)
    {
        currentHealth -= amount;

        if (audioHandler != null)
            audioHandler.PlayHit();

        ApplyKnockback();

        if (currentHealth <= 0)
            Die();
    }

    void ApplyKnockback()
    {
        if (batEnemy == null || batEnemy.player == null)
            return;

        // direction away from player
        Vector3 dir = transform.position - batEnemy.player.position;
        dir.y = 0; // keep horizontal

        knockbackDirection = dir.normalized;
        knockbackTimer = knockbackDuration;
    }

    void Die()
    {
        if (audioHandler != null && audioHandler.deathClip != null)
            audioHandler.PlayDeath();

        BatDeath death = GetComponent<BatDeath>();
        if (death != null)
        {
            death.StartDeath();
        }
        else
        {
            Destroy(gameObject); // fallback
        }
    }
}
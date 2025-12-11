using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 1f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;

    private float attackTimer = 0f;

    [Header("References")]
    public Transform player;
    private PlayerHealth playerHealth;

    void Start()
    {
        if (player != null)
            playerHealth = player.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (player == null || playerHealth == null)
            return;

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange && attackTimer <= 0f)
        {
            // knockback using attacker position
            playerHealth.TryHit(damage, transform.position);
            attackTimer = attackCooldown;
        }
    }
}
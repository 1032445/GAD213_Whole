using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEnemy : MonoBehaviour
{
    public float maxHealth = 3f;
    float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeLightDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 5f;
    public float damageCooldown = 0.5f;
    public Slider healthSlider;

    private float currentHealth;
    private float damageTimer = 0f;

    [Header("Hit Feedback")]
    public float knockbackForce = 8f;
    public float shakeDuration = 0.15f;
    public float shakeStrength = 0.2f;

    [Header("Damage Flash")]
    public CanvasGroup damageFlash;
    public float flashInSpeed = 10f;
    public float flashOutSpeed = 10f;
    private bool flashing = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] groans;
    public AudioClip lowHealthClip;
    public float lowHealthThreshold = 1.5f;
    public float lowHealthVolume = 0.8f;
    private bool lowHealthActive = false;

    private Rigidbody rb;
    private Transform cam;

    private float shakeTimer = 0f;
    private Vector3 originalCamPos;

    void Start()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>().transform;
        originalCamPos = cam.localPosition;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    void Update()
    {
        if (damageTimer > 0f)
            damageTimer -= Time.deltaTime;

        HandleShake();
        HandleFlash();
        HandleLowHealthAudio();
    }

    // attackerPosition where the bat was when it hit
    public void TakeDamage(float amount, Vector3 attackerPosition)
    {
        if (damageTimer > 0f)
            return;

        damageTimer = damageCooldown;
        currentHealth -= amount;
        healthSlider.value = currentHealth;

        Debug.Log("PLAYER HIT! Health = " + currentHealth);

        PlayHurtSound();
        ApplyKnockback(attackerPosition);
        StartShake();
        StartFlash();

        if (currentHealth <= 0)
            Die();
    }

    void ApplyKnockback(Vector3 attackerPos)
    {
        // push player away from the enemy
        Vector3 dir = (transform.position - attackerPos).normalized;
        dir.y = 0.35f;

        rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
    }

    void StartShake()
    {
        shakeTimer = shakeDuration;
    }

    void HandleShake()
    {
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            cam.localPosition = originalCamPos + Random.insideUnitSphere * shakeStrength;
        }
        else
        {
            cam.localPosition = Vector3.Lerp(cam.localPosition, originalCamPos, Time.deltaTime * 10f);
        }
    }

    void HandleFlash()
    {
        if (damageFlash == null) return;

        if (flashing)
        {
            // fade OUT
            damageFlash.alpha = Mathf.Lerp(
                damageFlash.alpha,
                0f,
                Time.deltaTime * flashOutSpeed
            );

            if (damageFlash.alpha < 0.02f)
            {
                damageFlash.alpha = 0f;
                flashing = false;
            }
        }
    }

    void StartFlash()
    {
        if (damageFlash == null) return;

        flashing = true;
        damageFlash.alpha = 0.6f;
    }

    void PlayHurtSound()
    {
        if (audioSource == null || groans.Length == 0)
            return;

        int index = Random.Range(0, groans.Length);
        audioSource.PlayOneShot(groans[index]);
    }

    void HandleLowHealthAudio()
    {
        if (audioSource == null || lowHealthClip == null)
            return;

        // activation
        if (currentHealth <= lowHealthThreshold && !lowHealthActive)
        {
            lowHealthActive = true;
            audioSource.clip = lowHealthClip;
            audioSource.volume = lowHealthVolume;
            audioSource.loop = true;
            audioSource.Play();
        }

        // deactivation
        else if (currentHealth > lowHealthThreshold && lowHealthActive)
        {
            lowHealthActive = false;
            audioSource.Stop();
            audioSource.clip = null;
        }
    }

    void Die()
    {
        Debug.Log("PLAYER DIED");
    }
}
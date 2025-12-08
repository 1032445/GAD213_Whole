using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flashlight : MonoBehaviour
{
    [Header("Flashlight Basics")]
    public Light flashlightLight;
    public KeyCode toggleKey = KeyCode.F;
    public float maxBattery = 100f;
    public float batteryDrainRate = 5f;
    public bool isOn = false;
    private float currentBattery;
    public Slider batterySlider;

    [Header("Combat")]
    public float range = 10f;
    public float angle = 35f;
    public float damagePerSecond = 1f;

    [Header("Hit Feedback")]
    public float shakeAmount = 0.05f;
    public float shakeSpeed = 20f;

    [Range(0f, 1f)]
    public float flickerPercent = 0.9f;
    public float flickerRecoverSpeed = 20f;

    private float shakeTimer = 0f;
    private bool flicker = false;

    [Header("Cone Flicker")]
    [Range(0f, 1f)]
    public float conePercent = 0.9f; // shrink cone to 90% on hit
    public float coneRecoverSpeed = 100f;

    private float originalConeAngle;

    private Vector3 originalLocalPos;
    private float originalIntensity;

    void Start()
    {
        currentBattery = maxBattery;
        flashlightLight.enabled = isOn;

        originalLocalPos = transform.localPosition;
        originalIntensity = flashlightLight.intensity;
        originalConeAngle = flashlightLight.spotAngle;

        if (batterySlider != null)
            batterySlider.value = BatteryPercent();
    }

    void Update()
    {
        HandleToggle();
        HandleBatteryDrain();

        if (isOn)
        {
            ApplyLightDamage();
        }

        HandleShake();
        HandleFlicker();

        if (batterySlider != null)
            batterySlider.value = BatteryPercent();
    }


    // light toggle
    void HandleToggle()
    {
        if (Input.GetKeyDown(toggleKey) && currentBattery > 0)
        {
            isOn = !isOn;
            flashlightLight.enabled = isOn;

            if (!isOn)
            {
                // reset visuals
                transform.localPosition = originalLocalPos;
                flashlightLight.intensity = originalIntensity;
            }
        }
    }


    // battery
    void HandleBatteryDrain()
    {
        if (!isOn) return;

        currentBattery -= batteryDrainRate * Time.deltaTime;

        if (currentBattery <= 0)
        {
            currentBattery = 0;
            isOn = false;
            flashlightLight.enabled = false;
        }
    }

    public float BatteryPercent()
    {
        return currentBattery / maxBattery;
    }


    // damage and feedback triggers
    public void ApplyLightDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out LightEnemy enemy))
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                float dot = Vector3.Dot(transform.forward, dir);

                if (dot > Mathf.Cos(angle * Mathf.Deg2Rad))
                {
                    // apply Damage
                    enemy.TakeLightDamage(damagePerSecond * Time.deltaTime);

                    // trigger shake + flicker
                    shakeTimer = 0.1f;
                    flicker = true;
                }
            }
        }
    }


    // shake effect
    void HandleShake()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            transform.localPosition = originalLocalPos +
                Random.insideUnitSphere * shakeAmount;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                originalLocalPos,
                Time.deltaTime * shakeSpeed
            );
        }
    }

    // flicker
    void HandleFlicker()
    {
        if (!isOn) return;

        // percentage targets
        float targetIntensity = originalIntensity * flickerPercent;
        float targetCone = originalConeAngle * conePercent;

        if (flicker)
        {
            // flicker brightness
            flashlightLight.intensity = Mathf.Lerp(
                flashlightLight.intensity,
                targetIntensity,
                Time.deltaTime * flickerRecoverSpeed
            );

            // shrink cone
            flashlightLight.spotAngle = Mathf.Lerp(
                flashlightLight.spotAngle,
                targetCone,
                Time.deltaTime * coneRecoverSpeed
            );

            // stop flicker once close enough
            if (Mathf.Abs(flashlightLight.intensity - targetIntensity) < 0.05f)
                flicker = false;
        }
        else
        {
            // recover brightness
            flashlightLight.intensity = Mathf.Lerp(
                flashlightLight.intensity,
                originalIntensity,
                Time.deltaTime * 5f
            );

            // recover cone
            flashlightLight.spotAngle = Mathf.Lerp(
                flashlightLight.spotAngle,
                originalConeAngle,
                Time.deltaTime * 10f
            );
        }
    }
}
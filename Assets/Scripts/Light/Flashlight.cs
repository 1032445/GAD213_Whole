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
    public bool isOn = false;
    private float currentBattery;
    public Slider batterySlider;
    private bool isKilled = false;

    [Header("Drain Rates")]
    public float idleDrainRate = 0.1f;
    public float combatDrainRate = 1f;
    private bool isDealingDamage = false;

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
    public float conePercent = 0.9f;
    public float coneRecoverSpeed = 100f;

    private float originalConeAngle;
    private float originalIntensity;
    private Vector3 originalLocalPos;

    [Header("Battery Behaviour")]
    public float lowBatteryPercent = 0.30f;
    public float criticalBatteryPercent = 0.10f;

    public float lowIntensityMultiplier = 0.6f;
    public float criticalIntensityMultiplier = 0.2f;

    public float lowConeMultiplier = 0.7f;
    public float criticalConeMultiplier = 0.3f;

    [Header("Forced Kill Settings")]
    public float killShakeAmount = 0.1f;
    public float killShakeDuration = 0.25f;
    public float killFlickerIntensity = 0.5f;
    public bool killImmediately = true;

    [Header("UI Flicker")]
    public Image batteryFillImage;
    public Color normalColor = Color.yellow;
    public Color combatColor = Color.white;
    public float uiFlickerSpeed = 10f;
    public float uiFlickerAmount = 0.3f;

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
            ApplyLightDamage();

        HandleShake();
        HandleFlicker();
        HandleBatteryStateEffects();

        if (batterySlider != null)
            batterySlider.value = BatteryPercent();
        HandleUIFlicker();
    }

    // toggle
    void HandleToggle()
    {
        if (isKilled) return;

        if (Input.GetKeyDown(toggleKey) && currentBattery > 0)
        {
            isOn = !isOn;
            flashlightLight.enabled = isOn;

            if (!isOn)
            {
                transform.localPosition = originalLocalPos;
                flashlightLight.intensity = originalIntensity;
            }
        }
    }


    // battery drain
    void HandleBatteryDrain()
    {
        // if flashlight killed, keep off
        if (isKilled)
            return;

        if (!isOn)
            return;

        float drainRate = isDealingDamage ? combatDrainRate : idleDrainRate;

        currentBattery -= drainRate * Time.deltaTime;
    }

    public float BatteryPercent()
    {
        return currentBattery / maxBattery;
    }

    // damage & hit feedback
    public void ApplyLightDamage()
    {
        isDealingDamage = false; // reset

        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out LightEnemy enemy))
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                float dot = Vector3.Dot(transform.forward, dir);

                if (dot > Mathf.Cos(angle * Mathf.Deg2Rad))
                {
                    // damaging enemy
                    isDealingDamage = true;

                    enemy.TakeLightDamage(damagePerSecond * Time.deltaTime);

                    // hit feedback
                    shakeTimer = 0.1f;
                    flicker = true;
                }
            }
        }
    }


    // light shake
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

        float targetIntensity = originalIntensity * flickerPercent;
        float targetCone = originalConeAngle * conePercent;

        if (flicker)
        {
            flashlightLight.intensity = Mathf.Lerp(
                flashlightLight.intensity,
                targetIntensity,
                Time.deltaTime * flickerRecoverSpeed
            );

            flashlightLight.spotAngle = Mathf.Lerp(
                flashlightLight.spotAngle,
                targetCone,
                Time.deltaTime * coneRecoverSpeed
            );

            if (Mathf.Abs(flashlightLight.intensity - targetIntensity) < 0.05f)
                flicker = false;
        }
        else
        {
            flashlightLight.intensity = Mathf.Lerp(
                flashlightLight.intensity,
                originalIntensity,
                Time.deltaTime * 5f
            );

            flashlightLight.spotAngle = Mathf.Lerp(
                flashlightLight.spotAngle,
                originalConeAngle,
                Time.deltaTime * 10f
            );
        }
    }

    // low battery behaviour

    void HandleBatteryStateEffects()
    {
        if (!isOn) return;

        float batteryRatio = currentBattery / maxBattery;

        // normal (above low)
        if (batteryRatio > lowBatteryPercent)
        {
            return;
        }

        if (batteryRatio > criticalBatteryPercent)
        {
            // unstable flicker
            flashlightLight.intensity = Mathf.Lerp(
                flashlightLight.intensity,
                originalIntensity * lowIntensityMultiplier,
                Time.deltaTime * 2f
            );

            flashlightLight.spotAngle = Mathf.Lerp(
                flashlightLight.spotAngle,
                originalConeAngle * lowConeMultiplier,
                Time.deltaTime * 2f
            );

            return;
        }


        // critical battery

        flashlightLight.intensity = Mathf.Lerp(
            flashlightLight.intensity,
            originalIntensity * criticalIntensityMultiplier,
            Time.deltaTime * 5f
        );

        flashlightLight.spotAngle = Mathf.Lerp(
            flashlightLight.spotAngle,
            originalConeAngle * criticalConeMultiplier,
            Time.deltaTime * 5f
        );
    }

    public void KillFlashlight()
    {
        isKilled = true;

        // force battery to 0
        currentBattery = 0f;

        // apply shake
        shakeAmount = killShakeAmount;
        shakeTimer = killShakeDuration;

        // custom flicker strength
        flickerPercent = killFlickerIntensity;
        flicker = true;

        // flicker before turning off
        StartCoroutine(FlickerThenDie());
    }

    private IEnumerator FlickerThenDie()
    {
        // flicker a few times
        for (int i = 0; i < 3; i++)
        {
            flashlightLight.enabled = false;
            yield return new WaitForSeconds(0.05f);
            flashlightLight.enabled = true;
            yield return new WaitForSeconds(0.05f);
        }

        // now fully off
        flashlightLight.enabled = false;
        isOn = false;
    }

    void HandleUIFlicker()
    {
        if (batteryFillImage == null) return;

        if (isDealingDamage)
        {
            // flicker between two colors
            float t = (Mathf.Sin(Time.time * uiFlickerSpeed) + 1f) / 2f;
            batteryFillImage.color = Color.Lerp(normalColor, combatColor, t * uiFlickerAmount);
        }
        else
        {
            // smoothly return to normal color
            batteryFillImage.color = Color.Lerp(batteryFillImage.color, normalColor, Time.deltaTime * 5f);
        }
    }
}
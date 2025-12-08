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

    void Start()
    {
        currentBattery = maxBattery;
        flashlightLight.enabled = isOn;

        if (batterySlider != null)
            batterySlider.value = BatteryPercent();
    }

    void Update()
    {
        // Toggle flashlight
        if (Input.GetKeyDown(toggleKey) && currentBattery > 0)
        {
            isOn = !isOn;
            flashlightLight.enabled = isOn;
        }

        // Drain battery if on
        if (isOn)
        {
            currentBattery -= batteryDrainRate * Time.deltaTime;
            if (currentBattery <= 0)
            {
                currentBattery = 0;
                isOn = false;
                flashlightLight.enabled = false;
            }

            ApplyLightDamage();
        }

        // Update UI
        if (batterySlider != null)
            batterySlider.value = BatteryPercent();
    }

    // Getter for battery level
    public float BatteryPercent()
    {
        return currentBattery / maxBattery;
    }

    public void ApplyLightDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out LightEnemy enemy))
            {
                // Vector to enemy
                Vector3 dir = (hit.transform.position - transform.position).normalized;

                // Check angle
                float dot = Vector3.Dot(transform.forward, dir);
                if (dot < Mathf.Cos(angle * Mathf.Deg2Rad))
                    continue;

                // Check distance
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist > range)
                    continue;

                // Apply damage
                enemy.TakeLightDamage(damagePerSecond * Time.deltaTime);
            }
        }
    }
}
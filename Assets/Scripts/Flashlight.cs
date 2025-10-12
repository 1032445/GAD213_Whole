using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flashlight : MonoBehaviour
{
    public Light flashlightLight;
    public KeyCode toggleKey = KeyCode.F;
    public float maxBattery = 100f;
    public float batteryDrainRate = 5f;
    public bool isOn = false;
    private float currentBattery;
    public Slider batterySlider;

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
}
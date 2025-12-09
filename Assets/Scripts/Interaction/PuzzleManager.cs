using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("Switches Required")]
    public FloorSwitch switchA;
    public FloorSwitch switchB;

    [Header("Final Door")]
    public MonoBehaviour finalDoorTarget;
    private ISwitchTarget doorTarget;
    private bool doorOpen = false;

    void Start()
    {
        doorTarget = finalDoorTarget as ISwitchTarget;

        if (doorTarget == null)
            Debug.LogError("Final door does not implement ISwitchTarget!");
    }

    void Update()
    {
        bool bothPressed = switchA.IsPressed() && switchB.IsPressed();

        if (bothPressed && !doorOpen)
        {
            doorTarget.Activate();
            doorOpen = true;
        }
        else if (!bothPressed && doorOpen)
        {
            doorTarget.Deactivate();
            doorOpen = false;
        }
    }
}
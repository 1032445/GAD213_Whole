using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDropper : MonoBehaviour, ISwitchTarget
{
    public GameObject box;

    public void Activate()
    {
        if (box != null)
            box.SetActive(true);
    }

    public void Deactivate()
    {

    }
}
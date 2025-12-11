using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    public EndScreenUI uiManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiManager.ShowEndScreen();
        }
    }
}
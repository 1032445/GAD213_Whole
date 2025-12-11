using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPanelManager : MonoBehaviour
{
    public GameObject controlsPanel;

    private bool isOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleControls();
        }
    }

    public void ToggleControls()
    {
        isOpen = !isOpen;
        controlsPanel.SetActive(isOpen);

        if (isOpen)
        {
            // unlock cursor + pause game
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
        else
        {
            // resume game + hide cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }

    public void CloseControls()
    {
        isOpen = false;
        controlsPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }
}

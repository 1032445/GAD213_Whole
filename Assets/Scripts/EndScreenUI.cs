using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenUI : MonoBehaviour
{
    public GameObject endScreenPanel;
    public PlayerController player;
    public Rigidbody playerRb;

    public void ShowEndScreen()
    {
        endScreenPanel.SetActive(true);

        // Freeze player movement
        if (player != null)
            player.enabled = false;

        if (playerRb != null)
        {
            playerRb.velocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();;
    }
}
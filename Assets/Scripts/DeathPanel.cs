using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathPanel : MonoBehaviour
{
    public void RestartGame()
    {
        // restore time
        Time.timeScale = 1f;

        // lock & hide cursor again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
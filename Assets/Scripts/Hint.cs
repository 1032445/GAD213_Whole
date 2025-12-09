using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hint : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    public float maxDistance = 3f;

    private Transform playerCamera;
    private PlayerController player;

    void Start()
    {
        hintText.enabled = false;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.GetComponent<PlayerController>();
            playerCamera = player.cameraTransform;
        }
        else
            Debug.LogError("Player not found!");
    }

    void Update()
    {
        if (playerCamera == null)
            return;

        if (player.heldObject != null)
        {
            Show("Press E again to drop");
            return;
        }

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            if (hit.collider.CompareTag("Pickup"))
            {
                Show("Press E to pick up");
                return;
            }
        }

        Hide();
    }

    public void Show(string msg)
    {
        hintText.text = msg;
        hintText.enabled = true;
    }

    public void Hide()
    {
        hintText.enabled = false;
    }
}
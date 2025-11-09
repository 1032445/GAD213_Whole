using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hint : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    public float detectionRadius = 3f;

    private Transform player;

    private void Start()
    {
        hintText.enabled = false;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("Player not found");
    }

    private void Update()
    {
        if (player == null)
            return;

        // check for pickups near player
        Collider[] hits = Physics.OverlapSphere(player.position, detectionRadius);

        bool foundPickup = false;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Pickup"))
            {
                foundPickup = true;
                break;
            }
        }

        if (foundPickup)
            Show("Press E to pick up");
        else
            Hide();
    }

    public void Show(string message = "Press E to pick up")
    {
        hintText.text = message;
        hintText.enabled = true;
    }

    public void Hide()
    {
        hintText.enabled = false;
    }
}
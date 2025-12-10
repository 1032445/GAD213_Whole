using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEnemy : MonoBehaviour
{
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 6f;
    public float stopDistance = 1.5f;

    [Header("Hovering")]
    public float hoverAmplitude = 0.25f;
    public float hoverFrequency = 3f;
    public float heightOffset = 0.5f;

    [Header("Model Offset")]
    public float modelRotationOffset = 0f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        Vector3 toPlayer = player.position - rb.position;

        float fullDist = toPlayer.magnitude;

        Vector3 flatDir = new Vector3(toPlayer.x, 0f, toPlayer.z);

        if (flatDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(flatDir.normalized)
                               * Quaternion.Euler(0, modelRotationOffset, 0);

            rb.MoveRotation(
                Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime)
            );
        }

        // movement
        Vector3 newPos = rb.position;

        if (fullDist > stopDistance)
        {
            newPos += toPlayer.normalized * moveSpeed * Time.fixedDeltaTime;
        }

        // hover & descent
        bool closeToPlayer = fullDist < stopDistance + 0.3f;

        float targetY;

        if (closeToPlayer)
        {
            // move to player height directly
            targetY = player.position.y + 0.2f;
        }
        else
        {
            // normal hover above player head
            targetY = player.position.y + heightOffset;
        }

        // hover wobble
        float hover = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;

        Vector3 pos = newPos;
        pos.y = Mathf.Lerp(rb.position.y, targetY, Time.fixedDeltaTime * 6f) + hover;

        rb.MovePosition(pos);
    }
}
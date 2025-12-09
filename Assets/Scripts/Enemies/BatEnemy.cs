using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEnemy : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2f;
    public float rotationSpeed = 6f;
    public float stopDistance = 1.5f;

    public float hoverAmplitude = 0.25f;
    public float hoverFrequency = 3f;
    public float heightOffset = 0.5f;

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
        Vector3 flatDir = new Vector3(toPlayer.x, 0f, toPlayer.z);
        float dist = flatDir.magnitude;

        // rotation
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

        if (dist > stopDistance)
            newPos += flatDir.normalized * moveSpeed * Time.fixedDeltaTime;

        // hover
        float heightTarget = player.position.y + heightOffset;
        float hover = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;

        newPos.y = Mathf.Lerp(rb.position.y, heightTarget, Time.fixedDeltaTime * 0.5f) + hover;

        rb.MovePosition(newPos);
    }
}
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

    public float modelRotationOffset = 0f;

    private float baseY;

    void Start()
    {
        baseY = transform.position.y;
    }

    void Update()
    {
        if (player == null)
            return;

        // Horizontal look direction
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0f;

        if (lookPos.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookPos)
                                  * Quaternion.Euler(0, modelRotationOffset, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        float dist = lookPos.magnitude;

        // Move if not close enough
        if (dist > stopDistance)
        {
            transform.position += lookPos.normalized * moveSpeed * Time.deltaTime;
        }

        // Hover effect
        float hover = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        Vector3 pos = transform.position;
        pos.y = baseY + hover;
        transform.position = pos;
    }
}
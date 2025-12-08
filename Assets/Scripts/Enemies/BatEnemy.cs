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

    private float baseY;

    void Start()
    {
        baseY = transform.position.y;
    }

    void Update()
    {
        if (player == null)
            return;

        // face the player
        Vector3 lookPos = player.position - transform.position;
        float horizontalDist = new Vector2(lookPos.x, lookPos.z).magnitude;
        lookPos.y = 0f; // keep rotation horizontal

        if (lookPos.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookPos)
                                  * Quaternion.Euler(0, modelRotationOffset, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // move towards player
        if (horizontalDist > stopDistance)
        {
            Vector3 moveDir = new Vector3(lookPos.x, 0f, lookPos.z).normalized;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        // hover based on player height
        float targetY = player.position.y + heightOffset;
        float hover = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;

        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * 0.5f) + hover;
        transform.position = pos;
    }
}
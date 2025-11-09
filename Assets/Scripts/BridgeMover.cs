using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeMover : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float speed = 2f;

    [HideInInspector]
    public bool isActive = false;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Bridge requires a rb");
            return;
        }

        rb.isKinematic = true; 
        rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        Vector3 target = isActive ? endPoint.position : startPoint.position;
        Vector3 newPos = Vector3.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }
}

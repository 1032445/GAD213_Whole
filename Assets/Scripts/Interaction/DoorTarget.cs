using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTarget : MonoBehaviour, ISwitchTarget
{
    public Vector3 openOffset = new Vector3(0, 3f, 0);
    public float speed = 2f;

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool isOpen = false;

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + openOffset;
    }

    public void Activate()
    {
        isOpen = true;
    }

    public void Deactivate()
    {
        isOpen = false;
    }

    void Update()
    {
        Vector3 target = isOpen ? openPos : closedPos;

        transform.position = Vector3.Lerp(
            transform.position, target, Time.deltaTime * speed);
    }
}
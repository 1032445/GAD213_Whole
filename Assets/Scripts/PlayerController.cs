using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header ("Movement")]
    public float speed = 5f;
    public float jumpForce = 5f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float jumpGravityMultiplier = 1f;

    [Header("Crouching")]
    public float crouchHeight = 1.5f;
    public float standHeight = 2f;
    public float crouchMoveSpeed = 2.5f;
    public KeyCode crouchKey = KeyCode.LeftControl;
    private bool isCrouching = false;
    private CapsuleCollider capsuleCollider;

    [Header("Sprinting")]
    public float sprintSpeed = 10f;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header ("Camera")]
    public Transform cameraTransform;
    public float mouseSensitivity = 500f;

    [Header("Pickup")]
    public Transform holdPoint;
    public float holdDistance = 2f;
    public float holdSmooth = 10f;
    public float pickupRange = 3f;
    public LayerMask pickupLayer;
    public KeyCode pickupKey = KeyCode.E;
    private Rigidbody heldObject;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;

    private Rigidbody rb;
    private float xRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);


        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");


        float currentSpeed = isCrouching ? crouchMoveSpeed :
                            (Input.GetKey(sprintKey) ? sprintSpeed : speed);

        Vector3 move = transform.forward * vertical + transform.right * horizontal;
        Vector3 velocity = move * currentSpeed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;

  
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }

        if (rb.velocity.y > 0) // Going up
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (jumpGravityMultiplier - 1) * Time.deltaTime;

            if (!Input.GetButton("Jump"))
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }
        else if (rb.velocity.y < 0) // Falling
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }


        if (Input.GetKeyDown(crouchKey))
        {
            isCrouching = true;
            capsuleCollider.height = crouchHeight;
            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, crouchHeight * 0.5f, cameraTransform.localPosition.z);
        }
        else if (Input.GetKeyUp(crouchKey))
        {
            Ray ray = new Ray(transform.position, Vector3.up);
            if (!Physics.Raycast(ray, standHeight))
            {
                isCrouching = false;
                capsuleCollider.height = standHeight;
                cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, standHeight * 0.5f, cameraTransform.localPosition.z);
            }
        }

        if (Input.GetKeyDown(pickupKey))
        {
            if (heldObject != null)
                Drop();
            else
                Pickup();
        }

        if (heldObject != null)
        {
            Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * holdDistance;
            heldObject.position = Vector3.Lerp(heldObject.position, targetPosition, holdSmooth * Time.deltaTime);
            heldObject.rotation = Quaternion.identity;
        }
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    void Pickup()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            if (hit.collider.CompareTag("Pickup"))
            {
                Rigidbody rbHit = hit.collider.GetComponent<Rigidbody>();
                if (rbHit != null)
                {
                    heldObject = rbHit;
                    rbHit.useGravity = false;
                    rbHit.constraints = RigidbodyConstraints.FreezeRotation;
                }
            }
        }
    }

        void Drop()
    {
        if (heldObject != null)
        {
            heldObject.useGravity = true;
            heldObject.constraints = RigidbodyConstraints.None; // restore rotation
            heldObject = null;
        }
    }
}
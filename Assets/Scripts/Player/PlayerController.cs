using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float jumpForce = 5f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float jumpGravityMultiplier = 1f;

    [Header("Sprinting")]
    public float sprintSpeed = 10f;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Camera")]
    public Transform cameraTransform;
    public float mouseSensitivity = 500f;
    private float xRotation = 0f;

    [Header("Crouching")]
    public KeyCode crouchKey = KeyCode.LeftControl;

    private float standCameraHeight;
    public float crouchCameraHeight = 0.5f;

    public float standColliderCenter = 1f;
    public float crouchColliderCenter = 0.5f;

    public float crouchTransitionSpeed = 8f;
    private bool isCrouching = false;
    private CapsuleCollider capsule;

    public float standColliderHeight = 2f;
    public float crouchColliderHeight = 1.0f;

    [Header("Pickup")]
    public Transform holdPoint;
    public float holdDistance = 2f;
    public float holdSmooth = 10f;
    public float pickupRange = 3f;
    public KeyCode pickupKey = KeyCode.E;
    public LayerMask pickupLayer;

    public Rigidbody heldObject;
    public Hint pickupHint;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        capsule.height = standColliderHeight;
        capsule.center = new Vector3(0, standColliderHeight / 2f, 0);

        standColliderCenter = capsule.center.y;
        standCameraHeight = cameraTransform.localPosition.y;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Time.time < 0.05f)
            return;

        HandleLook();
        HandleMovement();
        HandleJumpGravity();
        HandleCrouch();
        HandlePickup();
    }

    // camera look
    void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // movement
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float currentSpeed =
            isCrouching ? speed * 0.5f :
            (Input.GetKey(sprintKey) ? sprintSpeed : speed);

        Vector3 move = transform.forward * vertical + transform.right * horizontal;
        Vector3 velocity = move * currentSpeed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }
    }

    // jump settings
    void HandleJumpGravity()
    {
        if (rb.velocity.y > 0) // jumping
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (jumpGravityMultiplier - 1) * Time.deltaTime;

            if (!Input.GetButton("Jump"))
                rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y < 0) // falling
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    // crouching
    void HandleCrouch()
    {
        bool crouchHeld = Input.GetKey(crouchKey);

        if (crouchHeld)
        {
            isCrouching = true;
        }
        else
        {
            // only stand if space above is clear
            if (CanStandUp())
                isCrouching = false;
        }

        // collider height change
        capsule.height = isCrouching ? crouchColliderHeight : standColliderHeight;
        capsule.center = new Vector3(0, capsule.height / 2f, 0);

        // smooth camera movement
        float targetCamY = isCrouching ? crouchCameraHeight : standCameraHeight;
        Vector3 camPos = cameraTransform.localPosition;
        camPos.y = Mathf.Lerp(camPos.y, targetCamY, Time.deltaTime * crouchTransitionSpeed);
        cameraTransform.localPosition = camPos;
    }


    // checks if space to stand
    bool CanStandUp()
    {
        float radius = capsule.radius * 0.9f;
        float standHeight = standColliderHeight;

        Vector3 bottom = transform.position + Vector3.up * radius;
        Vector3 top = transform.position + Vector3.up * (standHeight - radius);

        return !Physics.CheckCapsule(bottom, top, radius, groundMask,
            QueryTriggerInteraction.Ignore);
    }

    // pick ups
    void HandlePickup()
    {
        // hint raycast
        if (holdPoint != null)
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
            {
                if (hit.collider.CompareTag("Pickup") && heldObject == null)
                    pickupHint?.Show("Press E to pick up");
                else if (heldObject != null)
                    pickupHint?.Show("Press E to drop");
                else
                    pickupHint?.Hide();
            }
            else
                pickupHint?.Hide();
        }

        if (Input.GetKeyDown(pickupKey))
        {
            if (heldObject != null)
                Drop();
            else
                TryPickup();
        }

        // held object
        if (heldObject != null)
        {
            Vector3 targetPosition =
                cameraTransform.position + cameraTransform.forward * holdDistance;

            heldObject.position =
                Vector3.Lerp(heldObject.position, targetPosition, holdSmooth * Time.deltaTime);

            heldObject.rotation = Quaternion.identity;
        }
    }

    void TryPickup()
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
            heldObject.constraints = RigidbodyConstraints.None;
            heldObject = null;
        }
    }

    // ground check
    bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }
}
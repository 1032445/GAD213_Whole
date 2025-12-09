using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource footstepSource;
    public AudioClip walkClip;
    public float walkInterval = 0.5f;
    public AudioClip sprintClip;
    public float sprintInterval = 0.3f;
    private float stepTimer = 0f;
    public AudioSource pantingSource;
    public AudioClip pantingClip;
    public float pantingFadeSpeed = 4f;
    
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

    [Header("Head Bob")]
    public float walkBobSpeed = 8f;
    public float walkBobAmount = 0.05f;

    public float sprintBobSpeed = 12f;
    public float sprintBobAmount = 0.1f;

    private Vector3 cameraInitialPos;
    private float currentBaseCameraY;

    private float bobTimer = 0f;
    private Vector3 bobOffset = Vector3.zero;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        capsule.height = standColliderHeight;
        capsule.center = new Vector3(0, standColliderHeight / 2f, 0);

        standColliderCenter = capsule.center.y;
        standCameraHeight = cameraTransform.localPosition.y;
        cameraInitialPos = cameraTransform.localPosition;
        currentBaseCameraY = cameraInitialPos.y;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Time.time < 0.05f)
            return;

        HandleLook();
        HandleFootsteps();
        HandleMovement();
        HandleJumpGravity();
        HandleCrouch();
        HandlePickup();
        HeadSway();
        HandlePanting();
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
        currentBaseCameraY = camPos.y;
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

    void HeadSway()
    {
        // horizontal movement only
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        float moveSpeed = flatVel.magnitude;

        // if not grounded or barely moving, reset bob
        if (!IsGrounded() || moveSpeed < 0.2f)
        {
            bobOffset = Vector3.Lerp(bobOffset, Vector3.zero, Time.deltaTime * 10f);

            ApplyBobOffset();
            return;
        }

        bool isSprinting = Input.GetKey(sprintKey) && !isCrouching;

        float bobSpeed = isSprinting ? sprintBobSpeed : walkBobSpeed;
        float bobAmount = isSprinting ? sprintBobAmount : walkBobAmount;

        bobTimer += Time.deltaTime * bobSpeed;

        // vertical bob
        float yBob = Mathf.Sin(bobTimer) * bobAmount;

        // small side sway
        float xBob = Mathf.Cos(bobTimer * 0.5f) * (bobAmount * 0.3f);

        bobOffset = new Vector3(xBob, yBob, 0);

        ApplyBobOffset();
    }

    void ApplyBobOffset()
    {
        cameraTransform.localPosition = new Vector3(
            cameraInitialPos.x + bobOffset.x,
            currentBaseCameraY + bobOffset.y,   // use crouch/stand height
            cameraInitialPos.z
        );
    }

    void HandleFootsteps()
    {
        if (!IsGrounded())
        {
            stepTimer = 0f;
            return;
        }

        // horizontal movement speed
        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float moveSpeed = flatVel.magnitude;

        // no footsteps when crouching or standing still
        if (isCrouching || moveSpeed < 0.2f)
        {
            stepTimer = 0.1f; // small delay
            return;
        }

        bool isSprinting = Input.GetKey(sprintKey);

        float interval = isSprinting ? sprintInterval : walkInterval;
        AudioClip clip = isSprinting ? sprintClip : walkClip;

        // countdown
        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0f)
        {
            footstepSource.PlayOneShot(clip);
            stepTimer = interval; // reset proper interval
        }
    }

    void HandlePanting()
    {
        bool isSprinting = Input.GetKey(sprintKey) && !isCrouching && IsGrounded();
        bool isMovingFast = rb.velocity.magnitude > speed * 0.6f; // ensures you're actually moving

        if (isSprinting && isMovingFast)
        {
            // start breathing if not already playing
            if (!pantingSource.isPlaying)
            {
                pantingSource.clip = pantingClip;
                pantingSource.loop = true;
                pantingSource.Play();
            }

            // fade IN breathing volume
            pantingSource.volume = Mathf.Lerp(pantingSource.volume, 1f, Time.deltaTime * pantingFadeSpeed);
        }
        else
        {
            // fade OUT breathing
            pantingSource.volume = Mathf.Lerp(pantingSource.volume, 0f, Time.deltaTime * pantingFadeSpeed);

            // stop when completely inaudible
            if (pantingSource.volume < 0.01f && pantingSource.isPlaying)
                pantingSource.Stop();
        }
    }

    // ground check
    bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }
}
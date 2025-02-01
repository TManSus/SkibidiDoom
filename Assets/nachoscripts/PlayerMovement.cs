using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Movement settings
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    // Head bobbing settings
    [Header("Head Bobbing Settings")]
    public float walkHeadBobSpeed = 2f;
    public float sprintHeadBobSpeed = 3f;
    public float headBobAmount = 0.05f;
    public float positionSmoothingFactor = 5f;
    public float headBobStartDelay = 1f;

    // FOV settings
    [Header("FOV Settings")]
    public float walkFOV = 60f;
    public float sprintFOV = 70f;
    public float fovSmoothingFactor = 5f;

    // Stamina settings
    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaDepletionRate = 10f;
    public float staminaRecoveryRate = 5f;
    public float staminaRecoveryDelay = 2f;

    // Private variables
    private CharacterController controller;
    private Camera playerCamera;
    private Vector3 cameraInitialLocalPosition;
    private Vector3 velocity;
    private float currentStamina;
    private float staminaRecoveryTimer;
    private float movementStartTime;
    private float headBobTimer;
    private float fovVelocity;
    private bool isSprinting;
    private bool isMoving;
    private bool isGrounded;

    void Start()
    {
        InitializeComponents();
        ResetStamina();
        SetInitialFOV();
    }

    void Update()
    {
        UpdateGroundedState();
        HandleMovement();
        HandleHeadBobbing();
        HandleFOV();
        HandleStamina();
    }

    #region Initialization Methods
    private void InitializeComponents()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        cameraInitialLocalPosition = playerCamera.transform.localPosition;
    }

    private void ResetStamina()
    {
        currentStamina = maxStamina;
    }

    private void SetInitialFOV()
    {
        playerCamera.fieldOfView = walkFOV;
    }
    #endregion

    #region Movement Handling
    private void UpdateGroundedState()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Keep the player grounded with a small downward force
        }
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;

        // Normalize movement direction to prevent faster diagonal movement
        if (moveDirection.magnitude > 1)
        {
            moveDirection.Normalize();
        }

        // Determine if the player is moving
        isMoving = moveDirection.magnitude > 0;

        // Sprint logic
        bool sprintKeyPressed = Input.GetKey(KeyCode.LeftShift);
        isSprinting = sprintKeyPressed && isMoving && currentStamina > 0;

        // Apply movement speed
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        // Jumping logic
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Reset movement start time if not moving
        if (!isMoving)
        {
            movementStartTime = 0f;
        }
    }
    #endregion

    #region Head Bobbing
    private void HandleHeadBobbing()
    {
        if (isMoving && isGrounded)
        {
            movementStartTime += Time.deltaTime;

            if (movementStartTime >= headBobStartDelay)
            {
                headBobTimer += Time.deltaTime * (isSprinting ? sprintHeadBobSpeed : walkHeadBobSpeed);
                float bobbingOffset = Mathf.Sin(headBobTimer) * headBobAmount;

                playerCamera.transform.localPosition = new Vector3(
                    cameraInitialLocalPosition.x,
                    cameraInitialLocalPosition.y + bobbingOffset,
                    cameraInitialLocalPosition.z
                );
            }
        }
        else
        {
            // Smoothly reset camera position when not moving or in the air
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                cameraInitialLocalPosition,
                positionSmoothingFactor * Time.deltaTime
            );

            headBobTimer = 0f;
            movementStartTime = 0f;
        }
    }
    #endregion

    #region FOV Handling
    private void HandleFOV()
    {
        float targetFOV = isSprinting ? sprintFOV : walkFOV;
        playerCamera.fieldOfView = Mathf.SmoothDamp(playerCamera.fieldOfView, targetFOV, ref fovVelocity, fovSmoothingFactor);
    }
    #endregion

    #region Stamina Handling
    private void HandleStamina()
    {
        if (isSprinting)
        {
            // Deplete stamina while sprinting
            currentStamina -= staminaDepletionRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            staminaRecoveryTimer = 0f; // Reset recovery timer when sprinting
        }
        else
        {
            // Increment recovery timer when not sprinting
            staminaRecoveryTimer += Time.deltaTime;

            if (staminaRecoveryTimer >= staminaRecoveryDelay)
            {
                currentStamina += staminaRecoveryRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            }
        }
    }
    #endregion

    // Expose stamina-related variables for external access
    public float CurrentStamina => currentStamina; // Read-only property for current stamina
    public float MaxStamina => maxStamina;        // Read-only property for max stamina
}
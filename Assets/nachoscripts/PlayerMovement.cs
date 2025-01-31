using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float walkHeadBobSpeed = 2f;
    public float sprintHeadBobSpeed = 3f;
    public float headBobAmount = 0.05f;
    public float positionSmoothingFactor = 5f; // Smoothing factor for camera position reset
    public float fovSmoothingFactor = 5f; // Smoothing factor for FOV transition
    public float sprintFOV = 70f;
    public float walkFOV = 60f;
    public float jumpHeight = 2f; // Height of the jump
    public float gravity = -9.81f; // Gravity force

    // Stamina variables
    public float maxStamina = 100f; // Maximum stamina
    public float currentStamina; // Current stamina
    public float staminaDepletionRate = 10f; // Stamina depletion rate per second
    public float staminaRecoveryRate = 5f; // Stamina recovery rate per second
    public float staminaRecoveryDelay = 2f; // Delay before stamina starts recovering

    private CharacterController controller;
    private Camera playerCamera;
    private Vector3 cameraInitialLocalPosition;
    private float headBobTimer = 0f;
    private bool isSprinting = false;
    private bool isMoving = false;
    private bool isGrounded;
    private float fovVelocity = 0f;
    private float movementStartTime = 0f;
    public float headBobStartDelay = 1f; // Delay before head bobbing starts
    private Vector3 velocity; // Current velocity
    private float staminaRecoveryTimer = 0f; // Timer to track stamina recovery delay

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        cameraInitialLocalPosition = playerCamera.transform.localPosition;
        playerCamera.fieldOfView = walkFOV;
        currentStamina = maxStamina; // Initialize current stamina
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        HandleMovement();
        HandleHeadBobbing();
        HandleFOV();
        HandleStamina();
    }

    void HandleMovement()
    {
        bool sprintKeyPressed = Input.GetKey(KeyCode.LeftShift);
        isSprinting = sprintKeyPressed && isMoving && currentStamina > 0;
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        if (move.magnitude > 1)
        {
            move.Normalize();
        }

        // Apply movement
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Apply gravity
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small value to keep the player grounded
        }

        // Jumping
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Apply vertical movement
        controller.Move(velocity * Time.deltaTime);

        // Determine if the player is moving
        isMoving = move.magnitude > 0;

        // Reset movement start time if the player is not moving
        if (!isMoving)
        {
            movementStartTime = 0f;
        }
    }

    void HandleHeadBobbing()
    {
        if (isMoving && controller.isGrounded)
        {
            // Increment movement start time
            movementStartTime += Time.deltaTime;

            // Start head bobbing after the specified delay
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
            // Smoothly reset the camera position when not moving or in the air
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                cameraInitialLocalPosition,
                positionSmoothingFactor * Time.deltaTime
            );
            headBobTimer = 0f;
            movementStartTime = 0f; // Reset movement start time when stopping or in the air
        }
    }

    void HandleFOV()
    {
        float targetFOV = isSprinting ? sprintFOV : walkFOV;
        playerCamera.fieldOfView = Mathf.SmoothDamp(playerCamera.fieldOfView, targetFOV, ref fovVelocity, fovSmoothingFactor);
    }

    void HandleStamina()
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

            // Recover stamina after delay
            if (staminaRecoveryTimer >= staminaRecoveryDelay)
            {
                currentStamina += staminaRecoveryRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            }
        }
    }
}
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Tilt Settings")]
    public float tiltAngle = 15f; // Maximum tilt angle in degrees
    public float tiltSpeed = 5f; // Speed of the tilt
    public float returnSpeed = 3f; // Speed at which the camera returns to default
    public float fallTiltAngle = 10f; // Additional tilt angle when falling
    public float jumpTiltAngle = 5f; // Additional tilt angle when jumping (upwards)

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 100f; // Mouse sensitivity
    public float lookSpeed = 2f; // Rotation speed
    public Transform player; // Reference to the player's transform

    private float xRotation = 0f; // Vertical rotation angle
    private float currentTilt = 0f; // Current tilt angle (horizontal)
    private float currentForwardTilt = 0f; // Current tilt angle (vertical)
    private float fallTilt = 0f; // Current tilt angle when falling
    private Camera playerCamera;
    private CharacterController characterController;

    void Start()
    {
        InitializeComponents();
        LockCursor();
    }

    void Update()
    {
        HandleMouseLook();
        HandleHorizontalTilt();
        HandleVerticalTilt();
        HandleFallTilt();
        ApplyCameraRotation();
    }

    #region Initialization Methods
    private void InitializeComponents()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = player.GetComponent<CharacterController>();

        if (playerCamera == null || characterController == null)
        {
            Debug.LogError("Camera or CharacterController not found. Ensure the player has these components.");
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    #endregion

    #region Mouse Look Handling
    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        // Calculate vertical rotation
        xRotation -= mouseY * mouseSensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Prevent over-rotating vertically

        // Apply horizontal rotation to the player
        player.Rotate(Vector3.up * mouseX * mouseSensitivity * Time.deltaTime);
    }
    #endregion

    #region Horizontal Tilt Handling
    private void HandleHorizontalTilt()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float targetTilt = -horizontalInput * tiltAngle;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);
    }
    #endregion

    #region Vertical Tilt Handling
    private void HandleVerticalTilt()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float targetForwardTilt = verticalInput * tiltAngle;
        currentForwardTilt = Mathf.Lerp(currentForwardTilt, targetForwardTilt, Time.deltaTime * tiltSpeed);
    }
    #endregion

    #region Fall Tilt Handling
    private void HandleFallTilt()
    {
        if (!characterController.isGrounded)
        {
            if (characterController.velocity.y < 0) // Falling
            {
                fallTilt = Mathf.Lerp(fallTilt, fallTiltAngle, Time.deltaTime * tiltSpeed);
            }
            else if (characterController.velocity.y > 0) // Jumping
            {
                fallTilt = Mathf.Lerp(fallTilt, jumpTiltAngle, Time.deltaTime * tiltSpeed);
            }
        }
        else
        {
            // Smoothly reset fall tilt when grounded
            fallTilt = Mathf.Lerp(fallTilt, 0f, Time.deltaTime * returnSpeed);
        }
    }
    #endregion

    #region Camera Rotation Application
    private void ApplyCameraRotation()
    {
        // Combine mouse look with tilt and fall tilt
        Quaternion cameraRotation = Quaternion.Euler(
            xRotation + currentForwardTilt + fallTilt,
            0f,
            currentTilt
        );
        playerCamera.transform.localRotation = cameraRotation;
    }
    #endregion
}
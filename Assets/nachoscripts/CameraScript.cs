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
    private Camera playerCamera;
    private CharacterController characterController; // Reference to the CharacterController
    private float fallTilt = 0f; // Current tilt angle when falling

    void Start()
    {
        // Initialize the camera and character controller
        playerCamera = GetComponentInChildren<Camera>(); // Find the camera attached to this object
        characterController = player.GetComponent<CharacterController>(); // Get the CharacterController component
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // Hide cursor
    }

    void Update()
    {
        // Mouse look logic
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        xRotation -= mouseY * mouseSensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Prevent over-rotating vertically

        // Apply vertical rotation to the camera
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, currentTilt);

        // Apply horizontal rotation to the player
        player.Rotate(Vector3.up * mouseX * mouseSensitivity * Time.deltaTime);

        // Horizontal tilt logic
        float horizontalInput = Input.GetAxis("Horizontal");
        float targetTilt = -horizontalInput * tiltAngle;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);

        // Forward and backward tilt logic
        float verticalInput = Input.GetAxis("Vertical");
        float targetForwardTilt = verticalInput * tiltAngle;
        currentForwardTilt = Mathf.Lerp(currentForwardTilt, targetForwardTilt, Time.deltaTime * tiltSpeed);

        // Check if the player is falling
        if (!characterController.isGrounded && characterController.velocity.y < 0)
        {
            // Smoothly tilt the camera upwards when falling
            fallTilt = Mathf.Lerp(fallTilt, fallTiltAngle, Time.deltaTime * tiltSpeed);
        }
        else if (!characterController.isGrounded && characterController.velocity.y > 0)
        {
            // Apply jump tilt when the player is ascending
            fallTilt = Mathf.Lerp(fallTilt, jumpTiltAngle, Time.deltaTime * tiltSpeed);
        }
        else
        {
            // Smoothly reset the fall tilt when not falling or jumping
            fallTilt = Mathf.Lerp(fallTilt, 0f, Time.deltaTime * returnSpeed);
        }

        // Combine mouse look with tilt and fall tilt
        Quaternion cameraRotation = Quaternion.Euler(xRotation + currentForwardTilt + fallTilt, 0f, currentTilt);
        playerCamera.transform.localRotation = cameraRotation;
    }
}
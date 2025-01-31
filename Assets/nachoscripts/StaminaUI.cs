using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    public Image staminaBar; // Reference to the UI Image representing the stamina bar
    private PlayerMovement playerMovement; // Reference to the PlayerMovement script

    void Start()
    {
        AssignPlayerMovement(); // Automatically find and assign the PlayerMovement script
    }

    void Update()
    {
        if (playerMovement == null)
            return;

        // Update the stamina bar fill amount
        float fillAmount = playerMovement.currentStamina / playerMovement.maxStamina;
        staminaBar.fillAmount = fillAmount;
    }

    public void AssignPlayerMovement(PlayerMovement movement)
    {
        playerMovement = movement;
    }

    public void AssignPlayerMovement()
    {
        // Automatically find the PlayerMovement script in the scene
        playerMovement = FindFirstObjectByType<PlayerMovement>();

        if (playerMovement == null)
        {
            Debug.LogError("No PlayerMovement script found in the scene. Please ensure the player has the PlayerMovement component.");
        }
    }
}
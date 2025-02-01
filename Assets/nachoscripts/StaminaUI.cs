using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [Header("UI References")]
    public Image staminaBar; // Reference to the UI Image representing the stamina bar

    [Header("Player Settings")]
    [SerializeField] private PlayerMovement playerMovement; // Reference to the PlayerMovement script

    void Start()
    {
        InitializePlayerMovement();
    }

    void Update()
    {
        if (playerMovement == null)
            return;

        // Update the stamina bar fill amount
        float fillAmount = playerMovement.CurrentStamina / playerMovement.MaxStamina;
        staminaBar.fillAmount = Mathf.Clamp01(fillAmount); // Clamp to avoid invalid values
    }

    /// <summary>
    /// Assigns the PlayerMovement script manually (useful for inspector or external scripts).
    /// </summary>
    /// <param name="movement">The PlayerMovement component to assign.</param>
    public void AssignPlayerMovement(PlayerMovement movement)
    {
        playerMovement = movement;
    }

    /// <summary>
    /// Automatically finds and assigns the PlayerMovement script in the scene.
    /// </summary>
    private void InitializePlayerMovement()
    {
        if (playerMovement == null)
        {
            playerMovement = FindFirstObjectByType<PlayerMovement>();
        }

        if (playerMovement == null)
        {
            Debug.LogError("No PlayerMovement script found in the scene. Please ensure the player has the PlayerMovement component.");
        }
    }
}
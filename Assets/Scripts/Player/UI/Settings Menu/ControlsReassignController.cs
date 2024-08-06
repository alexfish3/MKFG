using UnityEngine;

public class ControlsReassignController : MonoBehaviour
{
    // Private variables
    private InputProfileSO inputProfileToAdjust;

    // Getters/Setters
    public InputProfileSO InputProfileToAdjust { get { return inputProfileToAdjust; } set { inputProfileToAdjust = value; } }

    public void OnPrimaryButtonAssign(string keyName, KeyCode keyToAssign)
    {
        if (inputProfileToAdjust == null) return;

        // Capture the next key/controller button the player presses

        // Check if key/button exists for another command
            // If yes remove the other key?

        // Change assignments and save to profile

        // Change button text to become 
    }

    public void OnSecondaryButtonAssign(string keyName, KeyCode keyToAssign)
    {
        if (inputProfileToAdjust == null) return;
     
        // Wait for player key press

        // Check if key exists for another command
    }

    public void InputProfileSet(InputProfileSO inputProfile)
    {
        // Safety check
        if (inputProfile == null) return;

        inputProfileToAdjust = inputProfile;

        // Update UI
    }
}

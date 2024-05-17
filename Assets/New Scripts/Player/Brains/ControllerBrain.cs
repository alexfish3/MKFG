///
/// Created by Alex Fischer | May 2024
/// 

using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Holds information spesific to the controller brain
/// </summary>
public class ControllerBrain : GenericBrain
{
    /// <summary>
    /// Initalizes the controller brain with passed in values
    /// </summary>
    /// <param name="PlayerID">The player ID to initalize</param>
    /// <param name="DeviceID">The device ID to initalize</param>
    /// <param name="InputManager">The controller input manager to initalize</param>
    public void InitializeBrain(int PlayerID, int DeviceID, ControllerInputManager InputManager)
    {
        playerID = PlayerID;
        deviceID = DeviceID;
        inputManager = InputManager;
    }

    /// <summary>
    /// Detects press based on callback context used in Unity's new input system. 
    /// Any input on the player calls this method
    /// </summary>
    public void DetectPress(InputAction.CallbackContext context)
    {
        string actionName = context.action.name;

        // Spawn player 1
        if (actionName == "Left Shoulder" && playerBody == null)
        {
            SpawnBody(0);
            return;
        }
        // Spawn player 2
        else if (actionName == "Right Shoulder" && playerBody == null)
        {
            SpawnBody(1);
            return;
        }

        // Destroy when player hits Select, can happen before player spawns body
        if (actionName == "Select" && context.canceled)
        {
            DestroyObject();
            return;
        }

        //// If not spawned, do not handle button presses
        //if (playerBody == null)
        //    return;

        for (int i = 0; i < currentProfile.controllerInputs.Length; i++)
        {
            string input = currentProfile.controllerInputs[i].actionName;

            if (actionName == input)
            {
                if (context.performed)
                {
                    // If button is pressed
                    if (buttonSates[i] == false)
                    {
                        Debug.Log("Trigger");

                        buttonSates[i] = true;
                        button[i]?.Invoke(true);
                    }
                }
                else if (context.canceled)
                {
                    // If button is released
                    if (buttonSates[i] == true)
                    {
                        buttonSates[i] = false;
                        button[i]?.Invoke(false);
                    }
                }
            }

        }
    }

    private void OnDestroy()
    {
        DestroyObject();
    }
}

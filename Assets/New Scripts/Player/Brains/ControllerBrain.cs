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
            DestroyBrain();
            return;
        }

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
                        HandleInputEvent(i, true);
                    }
                }
                else if (context.canceled)
                {
                    // If button is released
                    if (buttonSates[i] == true)
                    {
                        HandleInputEvent(i, false);
                    }
                }
            }

        }
    }

    public void DetectAxis(InputAction.CallbackContext context)
    {
        // determine different axis here which can be seperated with an if
        string actionName = context.action.name;

        if(actionName == "Left Stick")
        {
            leftAxis = context.ReadValue<Vector2>();
            Debug.Log(context.action.name + ": " + leftAxis);
        }
        else if (actionName == "Right Stick")
        {
            rightAxis = context.ReadValue<Vector2>();
            Debug.Log(context.action.name + ": " + rightAxis);
        }
    }

    private void OnDestroy()
    {
        DestroyBrain();
    }
}

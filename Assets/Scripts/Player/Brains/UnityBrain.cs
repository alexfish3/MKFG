///
/// Created by Alex Fischer | May 2024
/// 

using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Holds information spesific to the unity input system brain
/// </summary>
public class UnityBrain : GenericBrain
{
    [SerializeField] PlayerInput playerInput;

    public enum NewInputSystemControllerType
    {
        Gamepad,
        Keyboard
    }
    NewInputSystemControllerType controllerType;
    public NewInputSystemControllerType ControllerType { get { return controllerType; } }

    /// <summary>
    /// Initalizes the unity input system brain with passed in values
    /// </summary>
    /// <param name="PlayerID">The player ID to initalize</param>
    /// <param name="DeviceID">The device ID to initalize</param>
    /// <param name="InputManager">The controller input manager to initalize</param>
    public void InitializeBrain(int PlayerID, int DeviceID, UnityInputManager InputManager)
    {
        SetPlayerID(PlayerID);
        deviceID = DeviceID;
        inputManager = InputManager;

        // Sets the action map to controller if brain is spawned by a controller
        if (playerInput.currentControlScheme == "Gamepad")
        {
            brainInputType = InputType.UnityController;

            controllerType = NewInputSystemControllerType.Gamepad;
            playerInput.SwitchCurrentActionMap("Controller");

            foreach(InputAction action in playerInput.currentActionMap.actions)
            {
                // Checks if action type is value which is for axis'
                if(action.type == InputActionType.Value)
                {
                    action.performed += DetectAxis;
                    action.canceled += DetectAxis;
                }
                // Checks if action type is for buttons
                else
                {
                    action.performed += DetectPressController;
                    action.canceled += DetectPressController;
                }
            }
        }
        // Sets the action map to controller if brain is spawned by a keyboard
        else if (playerInput.currentControlScheme == "Keyboard")
        {
            brainInputType = InputType.UnityKeyboard;

            controllerType = NewInputSystemControllerType.Keyboard;
            playerInput.SwitchCurrentActionMap("Keyboard");

            foreach (InputAction action in playerInput.currentActionMap.actions)
            {
                action.performed += DetectPressKeyboard;
                action.canceled += DetectPressKeyboard;
            }
        }
    }

    /// <summary>
    /// Detects press for controller based on callback context used in Unity's new input system. 
    /// Any input on the player calls this method
    /// </summary>
    public void DetectPressController(InputAction.CallbackContext context)
    {
        // Return if there is no control profile
        if (currentProfile == null)
            return;

        string actionName = context.action.name;

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

    /// <summary>
    /// Detects press for keyboard based on callback context used in Unity's new input system. 
    /// Any input on the player calls this method
    /// </summary>
    public void DetectPressKeyboard(InputAction.CallbackContext context)
    {
        string actionName = context.action.name;

        Debug.Log(actionName);

        //// Destroy when player hits Select, can happen before player spawns body
        //if (actionName == "Space" && context.canceled)
        //{
        //    DestroyBrain();
        //    return;
        //}

        if (currentProfile == null)
            return;

        for (int i = 0; i < currentProfile.keyboardInputs.Length; i++)
        {
            string input = currentProfile.keyboardInputs[i].keycode;

            if (actionName == input)
            {
                if (context.performed)
                {
                    Debug.Log("PRESSED");

                    // If button is pressed
                    if (buttonSates[i] == false)
                    {
                        HandleInputEvent(i, true);
                    }
                }
                else if (context.canceled)
                {
                    Debug.Log("Released");
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
            playerBodyAxisActions[0]?.Invoke(context.ReadValue<Vector2>());
        }
        else if (actionName == "Right Stick")
        {
            playerBodyAxisActions[1]?.Invoke(context.ReadValue<Vector2>());
        }
    }

    private void OnDestroy()
    {
        DestroyBrain();
    }
}

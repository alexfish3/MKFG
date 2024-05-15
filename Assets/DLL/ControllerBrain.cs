using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerBrain : GenericBrain
{
    public void InitializeBrain(int PlayerID, int DeviceID, ControllerInputManager InputManager)
    {
        playerID = PlayerID;
        deviceID = DeviceID;
        inputManager = InputManager;
    }


    public void DetectPress(InputAction.CallbackContext context)
    {
        string actionName = context.action.name;
        //Debug.Log("Action name is: " + actionName);

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

        // If not spawned, do not handle button presses
        if (playerBody == null)
            return;

        for (int i = 0; i < currentProfile.controllerInputs.Length - 1; i++)
        {
            string input = currentProfile.controllerInputs[i].actionName;

            if (actionName == input)
            {
                Debug.Log("Trigger");

                if (context.performed)
                {
                    // If button is pressed
                    if (currentProfile.controllerInputs[i].state == false)
                    {
                        currentProfile.controllerInputs[i].state = true;
                        currentProfile.controllerInputs[i].button?.Invoke(true);
                    }
                }
                else if (context.canceled)
                {
                    // If button is pressed
                    if (currentProfile.controllerInputs[i].state == true)
                    {
                        currentProfile.controllerInputs[i].state = false;
                        currentProfile.controllerInputs[i].button?.Invoke(false);
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

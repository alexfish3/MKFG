using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerBrain : GenericBrain
{
    [System.Serializable] public class ControllerInputAction : PlayerInputAction
    {
        public string actionName = "";

        // Rebinds keycode
        public void SetKey(string newActionName)
        {
            actionName = newActionName;
        }
    }

    [Header("Button Data")]
    [SerializeField] new ControllerInputAction[] inputs;

    public override void SetBodyEvents()
    {
        playerBody.SetBodyDeviceID(deviceID);

        inputs[0].press += () => { playerBody.Up(inputs[0].state); };
        inputs[1].press += () => { playerBody.Left(inputs[1].state); };
        inputs[2].press += () => { playerBody.Down(inputs[2].state); };
        inputs[3].press += () => { playerBody.Right(inputs[3].state); };
        inputs[4].press += () => { playerBody.Drift(inputs[4].state); };
        inputs[5].press += () => { playerBody.Attack(inputs[5].state); };
        inputs[6].press += () => { playerBody.Special(inputs[6].state); };

        inputs[0].release += () => { playerBody.Up(inputs[0].state); };
        inputs[1].release += () => { playerBody.Left(inputs[1].state); };
        inputs[2].release += () => { playerBody.Down(inputs[2].state); };
        inputs[3].release += () => { playerBody.Right(inputs[3].state); };
        inputs[4].release += () => { playerBody.Drift(inputs[4].state); };
        inputs[5].release += () => { playerBody.Attack(inputs[5].state); };
        inputs[6].release += () => { playerBody.Special(inputs[6].state); };
    }

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

        for (int i = 0; i < inputs.Length; i++)
        {
            string input = inputs[i].actionName;

            if (actionName == input)
            {
                if (context.performed)
                {
                    // If button is pressed
                    if (inputs[i].state == false)
                    {
                        inputs[i].state = true;
                        inputs[i].press?.Invoke();
                    }
                }
                else if (context.canceled)
                {
                    // If button is pressed
                    if (inputs[i].state == true)
                    {
                        inputs[i].state = false;
                        inputs[i].release?.Invoke();
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

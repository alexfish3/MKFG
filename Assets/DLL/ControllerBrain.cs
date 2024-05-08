using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerBrain : MonoBehaviour
{
    ControllerInputManager inputManager;

    [SerializeField] int playerID = 0;
    public int getPlayerID() { return playerID; }

    [SerializeField] int deviceID = 0;
    public int getDeviceID() { return deviceID; }

    [SerializeField] private PlayerMain playerBody;
    public void SetPlayerBody(PlayerMain pm) { playerBody = pm; SetBodyEvents(); }
    public PlayerMain GetPlayerBody() { return playerBody; }

    [Header("Button Data")]
    [SerializeField] ControllerInputAction[] inputs;
    public delegate void Keystroke();

    [System.Serializable]
    public class ControllerInputAction
    {
        public string actionName = "";
        public bool state = false;
        public Keystroke press;
        public Keystroke release;

        // Rebinds keycode
        public void SetKey(string newActionName)
        {
            actionName = newActionName;
        }
    }

    // Status
    bool destroyed = false;

    private void OnDestroy()
    {
        DestroyObject();
    }

    public void InitializeBrain(int PlayerID, int DeviceID, ControllerInputManager InputManager)
    {
        playerID = PlayerID;
        deviceID = DeviceID;
        inputManager = InputManager;
    }

    public void SpawnBody(int playerToSpawn)
    {
        SetPlayerBody(PlayerList.Instance.SpawnCharacterBody(playerToSpawn));
    }

    public void SetBodyEvents()
    {
        playerBody.SetBodyDeviceID(deviceID);

        inputs[0].press += playerBody.MoveUp;
        inputs[1].press += playerBody.MoveLeft;
        inputs[2].press += playerBody.MoveDown;
        inputs[3].press += playerBody.MoveRight;

        inputs[1].release += playerBody.ResetMovement;
        inputs[3].release += playerBody.ResetMovement;
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

    public void DestroyObject()
    {
        if (destroyed == true)
            return;

        destroyed = true;
        inputManager.DeletePlayerBrain(deviceID);

        if(playerBody != null)
            inputManager.playerSpawnSystem.AddDisconnectedPlayerBody(playerBody);
    }
}

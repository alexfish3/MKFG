using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardBrain : MonoBehaviour
{
    KeyboardInputManager inputManager;

    [SerializeField] int playerID = 0;
    public int getPlayerID() { return playerID; }

    [SerializeField] int deviceID = 0;
    public int getDeviceID() { return deviceID; }

    [SerializeField] private PlayerMain playerBody;
    public void SetPlayerBody(PlayerMain pm) { playerBody = pm; SetBodyEvents(); }
    public PlayerMain GetPlayerBody() { return playerBody; }

    [Header("Button Data")]
    [SerializeField] KeyboardInputAction[] inputs;
    public delegate void Keystroke();

    [System.Serializable]
    public class KeyboardInputAction
    {
        public string inputName;
        public char keycode = 'A';
        public bool state = false;
        public Keystroke press;
        public Keystroke release;

        // Rebinds keycode
        public void SetKey(char newKey)
        {
            keycode = newKey;
        }
    }

    // Status
    bool destroyed = false;

    private void OnDestroy()
    {
        DestroyObject();
    }

    public void InitializeBrain(int PlayerID, int DeviceID, KeyboardInputManager InputManager)
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
        inputs[3].release += playerBody.MoveRight;

    }

    public void DetectPress(int Press, int Release)
    {
        char press = (char)Press;
        char release = (char)Release;

        // Spawn player 1
        if (press == 49 && playerBody == null)
        {
            SpawnBody(0);
            return;
        }
        // Spawn player 2
        else if (press == 50 && playerBody == null)
        {
            SpawnBody(1);
            return;
        }

        // Destroy when player hits Space, can happen before player spawns body
        if (Release == 32)
        {
            DestroyObject();
            return;
        }

        // If not spawned, do not handle button presses
        if (playerBody == null)
            return;

        for (int i = 0; i < inputs.Length;i++)
        {
            char key = inputs[i].keycode;

            if (press == key)
            {
                // If button is pressed
                if (inputs[i].state == false)
                {
                    inputs[i].state = true;
                    inputs[i].press?.Invoke();
                }
            }
            else if(release == key)
            {
                // If button is released
                if (inputs[i].state == true)
                {
                    inputs[i].state = false;
                    inputs[i].release?.Invoke();
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

        if (playerBody != null)
            inputManager.playerSpawnSystem.AddDisconnectedPlayerBody(playerBody);
    }
}

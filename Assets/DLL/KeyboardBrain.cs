using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardBrain : GenericBrain
{
    [System.Serializable] public class KeyboardInputAction : PlayerInputAction
    {
        public char keycode = 'A';

        // Rebinds keycode
        public void SetKey(char newKey)
        {
            keycode = newKey;
        }
    }

    [Header("Button Data")]
    [SerializeField] new KeyboardInputAction[] inputs;

    public override void SetBodyEvents()
    {
        playerBody.SetBodyDeviceID(deviceID);

        inputs[0].press += playerBody.MoveUp;
        inputs[1].press += playerBody.MoveLeft;
        inputs[2].press += playerBody.MoveDown;
        inputs[3].press += playerBody.MoveRight;

        inputs[1].release += playerBody.ResetMovement;
        inputs[3].release += playerBody.ResetMovement;
    }

    public void InitializeBrain(int PlayerID, int DeviceID, KeyboardInputManager InputManager)
    {
        playerID = PlayerID;
        deviceID = DeviceID;
        inputManager = InputManager;
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

    private void OnDestroy()
    {
        DestroyObject();
    }
}

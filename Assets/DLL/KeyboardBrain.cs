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

        inputs[0].press += () => { playerBody.Up(inputs[0].state); };
        inputs[1].press += () => { playerBody.Left(inputs[1].state); };
        inputs[2].press += () => { playerBody.Down(inputs[2].state); };
        inputs[3].press += () => { playerBody.Right(inputs[3].state); };
        inputs[4].press += () => { playerBody.Drift(inputs[4].state); };

        inputs[0].release += () => { playerBody.Up(inputs[0].state); };
        inputs[1].release += () => { playerBody.Left(inputs[1].state); };
        inputs[2].release += () => { playerBody.Down(inputs[2].state); };
        inputs[3].release += () => { playerBody.Right(inputs[3].state); };
        inputs[4].release += () => { playerBody.Drift(inputs[4].state); };
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

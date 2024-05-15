using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardBrain : GenericBrain
{
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

        for (int i = 0; i < currentProfile.keyboardInputs.Length - 1;i++)
        {
            char key = currentProfile.keyboardInputs[i].keycode;

            if (press == key)
            {
                // If button is pressed
                if (currentProfile.keyboardInputs[i].state == false)
                {
                    Debug.Log("Trigger");

                    currentProfile.keyboardInputs[i].state = true;
                    currentProfile.keyboardInputs[i].button?.Invoke(true);
                }
            }
            else if(release == key)
            {
                // If button is released
                if (currentProfile.keyboardInputs[i].state == true)
                {
                    currentProfile.keyboardInputs[i].state = false;
                    currentProfile.keyboardInputs[i].button?.Invoke(false);
                }
            }
        }
    }

    private void OnDestroy()
    {
        DestroyObject();
    }
}

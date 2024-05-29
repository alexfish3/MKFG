///
/// Created by Alex Fischer | May 2024
/// 

using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Holds information spesific to the keyboard brain
/// </summary>
public class KeyboardBrain : GenericBrain
{
    /// <summary>
    /// Initalizes the keyboard brain with passed in values
    /// </summary>
    /// <param name="PlayerID">The player ID to initalize</param>
    /// <param name="DeviceID">The device ID to initalize</param>
    /// <param name="InputManager">The keyboard input manager to initalize</param>
    public void InitializeBrain(int PlayerID, int DeviceID, KeyboardInputManager InputManager)
    {
        playerID = PlayerID;
        deviceID = DeviceID;
        inputManager = InputManager;
    }

    /// <summary>
    /// Detects press based on parsed button press or release's int value (Space is 32 in ASCII) 
    /// </summary>
    /// <param name="Press"></param>
    /// <param name="Release"></param>
    public void DetectPress(int Press, int Release)
    {
        char press = (char)Press;
        char release = (char)Release;

        // Spawn player 1
        if (press == '1' && playerBody == null)
        {
            SpawnBody(0);
            return;
        }
        // Spawn player 2
        else if (press == '2' && playerBody == null)
        {
            SpawnBody(1);
            return;
        }

        // Destroy when player hits Space, can happen before player spawns body
        if (Release == 32)
        {
            DestroyBrain();
            return;
        }

        // Loops through buttons in the inputs gameobject
        for (int i = 0; i < currentProfile.keyboardInputs.Length;i++)
        {
            char key = currentProfile.keyboardInputs[i].keycode;

            if (press == key)
            {
                // If button is pressed
                if (buttonSates[i] == false)
                {
                    HandleInputEvent(i, true);
                }
            }
            else if(release == key)
            {
                // If button is released
                if (buttonSates[i] == true)
                {
                    HandleInputEvent(i, false);
                }
            }
        }
    }

    private void OnDestroy()
    {
        DestroyBrain();
    }
}

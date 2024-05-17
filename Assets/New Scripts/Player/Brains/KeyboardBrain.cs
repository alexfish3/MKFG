///
/// Created by Alex Fischer | May 2024
/// 

using UnityEngine;

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
                if (buttonSates[i] == false)
                {
                    Debug.Log("Trigger");

                    buttonSates[i] = true;
                    button[i]?.Invoke(true);
                }
            }
            else if(release == key)
            {
                // If button is released
                if (buttonSates[i] == true)
                {
                    buttonSates[i] = false;
                    button[i]?.Invoke(false);
                }
            }
        }
    }

    private void OnDestroy()
    {
        DestroyObject();
    }
}

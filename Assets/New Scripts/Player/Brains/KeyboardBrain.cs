///
/// Created by Alex Fischer | May 2024
/// 

using UnityEngine;

/// <summary>
/// Holds information spesific to the keyboard brain
/// </summary>
public class KeyboardBrain : GenericBrain
{
    enum SpecialKeys
    {
        Escape = 27,
        Tab = 9,
        CapsLock = 20,
        Shift = 16,
        CTRL = 17,
        ALT = 18,
        Enter = 13,
        Space = 32,
    }

    string press = "";
    string release = "";

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

        press = CheckKeyboardKeys(Press);
        release = CheckKeyboardKeys(Release);

        //if (press == "")
        //    press = ((char)Press).ToString();

        //if(release == null)
        //    release = ((char)Release).ToString();

        // Print what key is pressed or released
        if (Press != 0)
        {
            // Print what key is pressed or released
            Debug.Log("Pressed " + press);
        }
        else
        {
            // Print what key is pressed or released
            Debug.Log("Released " + release);
        }

        // Spawn player 1
        if (press == "1" && playerBody == null)
        {
            SpawnBody(0);
            return;
        }
        // Spawn player 2
        else if (press == "2" && playerBody == null)
        {
            SpawnBody(1);
            return;
        }

        // Destroy when player hits Space, can happen before player spawns body
        if (release == "Space")
        {
            DestroyBrain();
            return;
        }

        // Loops through buttons in the inputs gameobject
        for (int i = 0; i < currentProfile.keyboardInputs.Length;i++)
        {
            string key = currentProfile.keyboardInputs[i].keycode;

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

    /// <summary>
    ///  Checks the keys the key value could be before simply converting it into char
    /// </summary>
    /// <param name="keyValue"></param>
    /// <returns></returns>
    public string CheckKeyboardKeys(int keyValue)
    {
        switch ((SpecialKeys)keyValue)
        {
            case SpecialKeys.Escape:
                return "Escape";
            case SpecialKeys.Tab:
                return "Tab";
            case SpecialKeys.CapsLock:
                return "Caps Lock";
            case SpecialKeys.Shift:
                return "Shift";
            case SpecialKeys.CTRL:
                return "CTRL";
            case SpecialKeys.ALT:
                return "ALT";
            case SpecialKeys.Enter:
                return "Enter";
            case SpecialKeys.Space:
                return "Space";
            default: // If the int is not any of the special keys, convert to char for input
                return ((char)keyValue).ToString();
        }
    }

    private void OnDestroy()
    {
        DestroyBrain();
    }
}

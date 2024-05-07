using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputReciever : MonoBehaviour
{
    MouseInputManager inputManager;

    [SerializeField] Image player;

    [SerializeField] int playerID = 0;
    public int getPlayerID() { return playerID; }

    [SerializeField] int deviceID = 0;
    public int getDeviceID() { return deviceID; }

    [Header("Button Data")]
    [SerializeField] char[] buttons;
    [SerializeField] bool[] buttonStates;
    [SerializeField] Color[] playerColors;

    bool destroyed = false;

    private void OnDestroy()
    {
        DestroyObject();
    }

    public void Initalize(int PlayerID, int DeviceID, MouseInputManager InputManager)
    {
        //Debug.Log($"Adding player with player id:{playerID} and device id:{deviceID}");
        playerID = PlayerID;
        deviceID = DeviceID;
        inputManager = InputManager;

        buttonStates = new bool[buttons.Length];
    }

    public void DetectPress(int Press, int Release)
    {
        char press = (char)Press;
        char release = (char)Release;

        Debug.Log($"press {Press} release {Release}");
        
        // Destroy when player hits Space
        if (Release == 32)
        {
            DestroyObject();
            return;
        }

        for(int i = 0; i < buttons.Length;i++)
        {
            char button = buttons[i];

            if (press == button)
            {
                player.color = playerColors[i];

                // If button is pressed
                if (buttonStates[i] == false)
                {
                    Debug.Log($"Player {playerID} pressed {(char)press}");
                    buttonStates[i] = true;
                }
            }
            else if(release == button)
            {
                player.color = Color.white;
                // If button is released
                if (buttonStates[i] == true)
                {
                    Debug.Log($"Player {playerID} released {(char)release}");
                    buttonStates[i] = false;
                }
            }
        }
    }

    public void DestroyObject()
    {
        if (destroyed == true)
            return;

        destroyed = true;
        inputManager.deletePlayer(deviceID);
    }
}

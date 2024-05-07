using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputReciever : MonoBehaviour
{
    [SerializeField] Image player;

    [SerializeField] int PlayerID = 0;
    public int getPlayerID() { return PlayerID; }

    [SerializeField] int DeviceID = 0;
    public int getDeviceID() { return DeviceID; }

    [Header("Button Data")]
    [SerializeField] char[] buttons;
    [SerializeField] bool[] buttonStates;
    [SerializeField] Color[] playerColors;

    public void Initalize(int playerID, int deviceID)
    {
        //Debug.Log($"Adding player with player id:{playerID} and device id:{deviceID}");
        PlayerID = playerID;
        DeviceID = deviceID;

        buttonStates = new bool[buttons.Length];
    }

    public void DetectPress(int Press, int Release)
    {
        char press = (char)Press;
        char release = (char)Release;

        Debug.Log($"press {Press} release {Release}");

        for(int i = 0; i < buttons.Length;i++)
        {
            char button = buttons[i];

            if (press == button)
            {
                player.color = playerColors[i];

                // If button is pressed
                if (buttonStates[i] == false)
                {
                    Debug.Log($"Player {PlayerID} pressed {(char)press}");
                    buttonStates[i] = true;

                }
            }
            else if(release == button)
            {
                player.color = Color.white;
                // If button is released
                if (buttonStates[i] == true)
                {
                    Debug.Log($"Player {PlayerID} released {(char)release}");
                    buttonStates[i] = false;
                }
            }
        }
    }
}

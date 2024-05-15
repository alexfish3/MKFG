using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericBrain : MonoBehaviour
{
    protected GenericInputManager inputManager;

    [SerializeField] protected int playerID = 0;
    public int getPlayerID() { return playerID; }

    [SerializeField] protected int deviceID = -1;
    public int getDeviceID() { return deviceID; }

    [SerializeField] protected PlayerMain playerBody;
    public void SetPlayerBody(PlayerMain pm) { playerBody = pm; SetBodyEvents(); }
    public PlayerMain GetPlayerBody() { return playerBody; }

    public delegate void Keystroke();
    [SerializeField] protected InputProfile[] inputProfileOptions;

    // Status
    protected bool destroyed = false;

    public enum inputProfileTypes
    {
        UI = 0,
        Driving = 1
    }
    [SerializeField] protected inputProfileTypes currentProfile = 0;
    public inputProfileTypes getCurrentProfile() { return currentProfile; }
    public void setCurrentProfile(inputProfileTypes newProfile) { currentProfile = newProfile; }

    public void SetBodyEvents()
    {
        playerBody.SetBodyDeviceID(deviceID);

        for (int i = 0; i <= inputProfileOptions.Length - 1; i++)
        {
            // Setting controller inputs
            inputProfileOptions[i].controllerInputs[0].button += playerBody.Up;
            inputProfileOptions[i].controllerInputs[1].button += playerBody.Left;
            inputProfileOptions[i].controllerInputs[2].button += playerBody.Down;
            inputProfileOptions[i].controllerInputs[3].button += playerBody.Right;
            inputProfileOptions[i].controllerInputs[4].button += playerBody.Drift;
            inputProfileOptions[i].controllerInputs[5].button += playerBody.Attack;
            inputProfileOptions[i].controllerInputs[6].button += playerBody.Special;

            // Set keyboard inputs
            inputProfileOptions[i].keyboardInputs[0].button += playerBody.Up;
            inputProfileOptions[i].keyboardInputs[1].button += playerBody.Left;
            inputProfileOptions[i].keyboardInputs[2].button += playerBody.Down;
            inputProfileOptions[i].keyboardInputs[3].button += playerBody.Right;
            inputProfileOptions[i].keyboardInputs[4].button += playerBody.Drift;
            inputProfileOptions[i].keyboardInputs[5].button += playerBody.Attack;
            inputProfileOptions[i].keyboardInputs[6].button += playerBody.Special;
        }
    }

    public void SpawnBody(int playerToSpawn)
    {
        SetPlayerBody(PlayerList.Instance.SpawnCharacterBody(playerToSpawn));
    }

    public void DestroyObject()
    {
        if (destroyed == true)
            return;

        destroyed = true;

        // If device id is not 0, means it was valid and needs to be removed
        if(deviceID != -1)
            inputManager.DeletePlayerBrain(deviceID);

        // If player body is not null, add disconnected body to list
        if (playerBody != null)
            inputManager.playerSpawnSystem.AddDisconnectedPlayerBody(playerBody);
    }
}

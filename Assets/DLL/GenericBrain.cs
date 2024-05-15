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
    [SerializeField] protected List<InputProfile> inputProfileOptionsResource = new List<InputProfile>();
    [SerializeField] protected List<InputProfile> inputProfileOptions = new List<InputProfile>();

    // Status
    protected bool destroyed = false;

    public enum inputProfileTypes
    {
        UI = 0,
        Driving = 1
    }

    [SerializeField] protected InputProfile currentProfile;
    public InputProfile getCurrentProfile() { return currentProfile; }
    public void setCurrentProfile(int newProfile) { currentProfile = inputProfileOptions[newProfile]; }

    public void Awake()
    {
        // Copy list into profile resources
        int totalListOfProfiles = inputProfileOptionsResource.Count;

        for (int i = 0; i < totalListOfProfiles; i++)
        {
            inputProfileOptions.Add((InputProfile)inputProfileOptionsResource[i].Clone());
        }

        currentProfile = inputProfileOptions[1];
    }

    public void SetBodyEvents()
    {
        playerBody.SetBodyDeviceID(deviceID);

        // Set keyboard inputs
        currentProfile.keyboardInputs[0].button += playerBody.Up;
        currentProfile.keyboardInputs[1].button += playerBody.Left;
        currentProfile.keyboardInputs[2].button += playerBody.Down;
        currentProfile.keyboardInputs[3].button += playerBody.Right;
        currentProfile.keyboardInputs[4].button += playerBody.Drift;
        currentProfile.keyboardInputs[5].button += playerBody.Attack;
        currentProfile.keyboardInputs[6].button += playerBody.Special;

        //// Setting controller inputs
        currentProfile.controllerInputs[0].button += playerBody.Up;
        currentProfile.controllerInputs[1].button += playerBody.Left;
        currentProfile.controllerInputs[2].button += playerBody.Down;
        currentProfile.controllerInputs[3].button += playerBody.Right;
        currentProfile.controllerInputs[4].button += playerBody.Drift;
        currentProfile.controllerInputs[5].button += playerBody.Attack;
        currentProfile.controllerInputs[6].button += playerBody.Special;
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

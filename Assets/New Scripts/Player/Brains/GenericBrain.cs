///
/// Created by Alex Fischer | May 2024
/// 

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The generic brain. holds info both controller and keyboard brains call upon
/// </summary>
public abstract class GenericBrain : MonoBehaviour
{
    protected GenericInputManager inputManager;

    [SerializeField] protected int playerID = 0;
    public int getPlayerID() { return playerID; }

    [SerializeField] protected int deviceID = -1;
    public int getDeviceID() { return deviceID; }

    [SerializeField] protected PlayerMain playerBody;
    public void SetPlayerBody(PlayerMain pm) { playerBody = pm; SetBodyEvents(); } // Sets player body to passed in player main and sets events when called
    public PlayerMain GetPlayerBody() { return playerBody; } // Returns the player body connected to brain

    public delegate void Keystroke();
    [SerializeField] protected List<InputProfile> inputProfileOptionsResource = new List<InputProfile>(); // The input profiles that are to be copied
    [SerializeField] protected List<InputProfile> inputProfileOptions = new List<InputProfile>(); // The list of copied input profiles

    // Status
    protected bool destroyed = false;

    public enum inputProfileTypes
    {
        UI = 0,
        Driving = 1
    }

    [SerializeField] protected InputProfile currentProfile;
    public InputProfile GetCurrentProfile() { return currentProfile; } // Returns the current control profile
    public void SetCurrentProfile(int newProfile) { currentProfile = inputProfileOptions[newProfile]; } // Sets the current profile to new based on int

    public void Awake()
    {
        CopyControlProfiles();
    }

    /// <summary>
    /// Caches and sets current profile
    /// </summary>
    private void CopyControlProfiles()
    {
        // Cache length of profile options
        int totalListOfProfiles = inputProfileOptionsResource.Count;

        // Clones each of the options into copy array
        for (int i = 0; i < totalListOfProfiles; i++)
        {
            inputProfileOptions.Add((InputProfile)inputProfileOptionsResource[i].Clone());
        }

        // Sets current profile to be "Driving"
        currentProfile = inputProfileOptions[1];
    }

    /// <summary>
    /// Sets the events that will be triggered when the profile's button is pressed
    /// </summary>
    public void SetBodyEvents()
    {
        playerBody.SetBodyDeviceID(deviceID);

        // If current profile is not set, set it
        if(currentProfile == null)
            CopyControlProfiles();

        // Set keyboard inputs
        currentProfile.keyboardInputs[0].button += playerBody.Up;
        currentProfile.keyboardInputs[1].button += playerBody.Left;
        currentProfile.keyboardInputs[2].button += playerBody.Down;
        currentProfile.keyboardInputs[3].button += playerBody.Right;
        currentProfile.keyboardInputs[4].button += playerBody.Drift;
        currentProfile.keyboardInputs[5].button += playerBody.Attack;
        currentProfile.keyboardInputs[6].button += playerBody.Special;

        // Set controller inputs
        currentProfile.controllerInputs[0].button += playerBody.Up;
        currentProfile.controllerInputs[1].button += playerBody.Left;
        currentProfile.controllerInputs[2].button += playerBody.Down;
        currentProfile.controllerInputs[3].button += playerBody.Right;
        currentProfile.controllerInputs[4].button += playerBody.Drift;
        currentProfile.controllerInputs[5].button += playerBody.Attack;
        currentProfile.controllerInputs[6].button += playerBody.Special;
    }

    /// <summary>
    /// Spawns a player body to connect to the brain. The passed in int is the player's ID
    /// </summary>
    /// <param name="playerToSpawn"></param>
    public void SpawnBody(int playerToSpawn)
    {
        SetPlayerBody(PlayerList.Instance.SpawnCharacterBody(playerToSpawn));
    }

    /// <summary>
    /// Destroys the brain and the connection to the body
    /// </summary>
    public void DestroyObject()
    {
        // If already destroyed return
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

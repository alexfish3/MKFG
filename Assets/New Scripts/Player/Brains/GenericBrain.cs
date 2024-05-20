///
/// Created by Alex Fischer | May 2024
/// 

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The generic brain. holds info both controller and keyboard brains call upon
/// </summary>
public abstract class GenericBrain : MonoBehaviour
{
    bool initalized = false;

    protected GenericInputManager inputManager;

    [SerializeField] protected int playerID = 0;
    public int getPlayerID() { return playerID; }

    [SerializeField] protected int deviceID = -1;
    public int getDeviceID() { return deviceID; }

    [Header("UI Controlls")]
    [SerializeField] protected GenericUI uiController;

    [Header("Player Body")]
    [SerializeField] protected PlayerMain playerBody;
    public void SetPlayerBody(PlayerMain pm) { playerBody = pm; SetBodyEvents(); } // Sets player body to passed in player main and sets events when called
    public PlayerMain GetPlayerBody() { return playerBody; } // Returns the player body connected to brain

    public delegate void Keystroke();
    [SerializeField] protected List<InputProfile> inputProfileOptionsResource = new List<InputProfile>(); // The input profiles that are to be copied
    ControlProfile currentControlProfile;

    public ControlProfile controlProfileSerialize; // TEMP
    ControlProfile lastControlProfile; // Temp

    // Status
    protected bool destroyed = false;

    [SerializeField] protected InputProfile currentProfile;
    public InputProfile GetCurrentProfile() { return currentProfile; } // Returns the current control profile
    public void SetCurrentProfile(int newProfile) { currentProfile = inputProfileOptionsResource[newProfile]; } // Sets the current profile to new based on int

    public Action<bool>[] playerBodyActions;
    public Action<bool, int>[] uiActions;
    public bool[] buttonSates;

    public void Awake()
    {
        // Setup arrays
        playerBodyActions = new Action<bool>[9];
        uiActions = new Action<bool, int>[7];

        buttonSates = new bool[9];
        initalized = true;

        // Sets control profile to be whats on prefab when spawns
        currentControlProfile = controlProfileSerialize;
        SetCurrentProfile((int)currentControlProfile);
    }

    public void Update()
    {
        if(currentControlProfile != controlProfileSerialize)
        {
            // Caches current for later
            lastControlProfile = currentControlProfile;

            // Sets current to be new control profile
            currentControlProfile = controlProfileSerialize;
            ChangeControlType(currentControlProfile);
        }
    }

    public void ChangeUIToControl(UITypes uiType)
    {
        switch (uiType)
        {
            case UITypes.MainMenu:
                return;
            case UITypes.CharacterSelect:
                ChangeControlType(ControlProfile.UI, PlayerSelectUI.Instance);
                return;
            case UITypes.PauseMenu:
                return;
        }
    }

    /// <summary>
    /// Saves the ui that will be controlled alongside
    /// </summary>
    /// <param name="controlProfile">The passed in control profile type being switched to</param>
    /// <param name="uiToBeControlled">The passed in ui that will be controlled</param>
    public void ChangeControlType(ControlProfile controlProfile, GenericUI uiToBeControlled)
    {
        // Only want to call this when changing ui control types
        if (controlProfile != ControlProfile.UI)
            return;

        uiController = uiToBeControlled;
        ChangeControlType(controlProfile);
    }

    /// <summary>
    /// Sets the player's control profile based on a passed in enum
    /// </summary>
    /// <param name="controlProfile">The passed in control profile type being switched to</param>
    public void ChangeControlType(ControlProfile controlProfile)
    {
        currentControlProfile = controlProfile;
        SetCurrentProfile((int)currentControlProfile);

        SetBodyEvents();
    }

    /// <summary>
    /// Sets the events that will be triggered when the profile's button is pressed
    /// </summary>
    public void SetBodyEvents()
    {
        Debug.Log("Set Body Events");

        // If current profile is not set, set it to default
        if (currentProfile == null)
        {
            SetCurrentProfile((int)controlProfileSerialize);
        }

        // Initalizes player arrays if they arent already
        if (initalized == false)
        {
            playerBodyActions = new Action<bool>[9];
            uiActions = new Action<bool, int>[7];

            buttonSates = new bool[9];
            initalized = true;
        }

        // If control type is UI
        if (currentProfile.controlType == 0)
        {
            // If no ui controller is detected
            if (uiController == null)
                uiController = PlayerSelectUI.Instance;

            // Clear input events
            for (int i = 0; i < uiActions.Length; i++)
            {
                uiActions[i] = null;
            }

            // Set inputs
            uiActions[0] += uiController.Up;
            uiActions[1] += uiController.Left;
            uiActions[2] += uiController.Down;
            uiActions[3] += uiController.Right;
            uiActions[4] += uiController.Confirm;
            uiActions[5] += uiController.Return;
        }
        // If control type is driving
        else
        {
            // If player body is null, switch back to last control profile
            if(playerBody == null)
            {
                Debug.LogWarning("Switching To Profile At Wrong Time... Reverting");
                controlProfileSerialize = lastControlProfile;
                return;
            }

            // Clear input events
            for (int i = 0; i < playerBodyActions.Length; i++)
            {
                playerBodyActions[i] = null;
            }

            playerBody.SetBodyDeviceID(deviceID);

            // Set inputs
            playerBodyActions[0] += playerBody.Up;
            playerBodyActions[1] += playerBody.Left;
            playerBodyActions[2] += playerBody.Down;
            playerBodyActions[3] += playerBody.Right;
            playerBodyActions[4] += playerBody.Drift;
            playerBodyActions[5] += playerBody.Attack;
            playerBodyActions[6] += playerBody.Special;
            playerBodyActions[7] += playerBody.Drive;
            playerBodyActions[8] += playerBody.Reverse;

            Debug.Log(playerBodyActions[0].Method.Name);
        }
    }

    public void TEST(bool test)
    {
        Debug.Log("this is a test");
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
    /// Handles input event
    /// </summary>
    /// <param name="i">the button input position being detected as pressed or released</param>
    /// <param name="pressed">the bool saying whether it was pressed or released</param>
    protected void HandleInputEvent(int i, bool pressed)
    {
        buttonSates[i] = pressed;

        // If control type is ui, invoke ui action events
        if (currentProfile.controlType == InputProfile.ControlType.UI)
        {
            uiActions[i]?.Invoke(pressed, playerID);
        }
        // If control type is player, invoke body action events
        else
        {
            playerBodyActions[i]?.Invoke(pressed);
        }
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

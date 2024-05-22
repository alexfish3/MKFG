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
    public ControlProfile currentControlProfile;
    public ControlProfile profileToStartWith;
    ControlProfile controlProfileCache;
    ControlProfile lastControlProfile; // Temp

    // Status
    protected bool destroyed = false;

    [SerializeField] protected InputProfile currentProfile;
    public InputProfile GetCurrentProfile() { return currentProfile; } // Returns the current control profile
    public void SetCurrentProfile(ControlProfile newProfile) // Sets the current profile to new based on int
    {
        currentControlProfile = newProfile;
        controlProfileCache = currentControlProfile;
        currentProfile = inputProfileOptionsResource[(int)newProfile];
    }

    public Action<bool>[] playerBodyActions;
    public Action<bool, GenericBrain>[] uiActions;
    public bool[] buttonSates;

    //bool CharacterSelectUIInitalized;

    public void Awake()
    {
        InitalizeBrain();
    }

    /// <summary>
    /// Initalizes the brain's core functions
    /// </summary>
    private void InitalizeBrain()
    {
        // Initalizes once if not initalized already
        if(initalized == false)
        {
            // Setup arrays
            playerBodyActions = new Action<bool>[9];
            uiActions = new Action<bool, GenericBrain>[7];

            buttonSates = new bool[9];
            initalized = true;

            Debug.Log("Initalize Brain");

            // If brain is initalized when players are in playerSelect
            if (GameManagerNew.Instance.CurrentState == GameStateNew.PlayerSelect)
            {
                // Sets control profile to be whats on prefab when spawns
                SetCurrentProfile(ControlProfile.UI);
                ChangeUIToControl(UITypes.CharacterSelect);
            }
            else
            {
                if (currentControlProfile == ControlProfile.None)
                {
                    // Sets control profile to be whats on prefab when spawns
                    SetCurrentProfile(profileToStartWith);
                }
            }
        }
    }

    public void Update()
    {
        if (controlProfileCache != currentControlProfile)
        {
            // Caches current for later
            lastControlProfile = currentControlProfile;

            // Sets current to be new control profile
            ChangeControlType(currentControlProfile);
        }
    }

    public void ChangeUIToControl(UITypes uiType)
    {
        if(uiController != null)
            uiController.RemovePlayerUI(this);

        switch (uiType)
        {
            case UITypes.MainMenu:
                return;
            case UITypes.CharacterSelect:
                ChangeControlType(ControlProfile.UI, CharacterSelectUI.Instance);
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
        if (uiToBeControlled == null)
            return;

        // Only want to call this when changing ui control types
        if (controlProfile != ControlProfile.UI)
            return;

        uiController = uiToBeControlled;
        uiController.AddPlayerToUI(this);

        ChangeControlType(controlProfile);
    }

    /// <summary>
    /// Sets the player's control profile based on a passed in enum
    /// </summary>
    /// <param name="controlProfile">The passed in control profile type being switched to</param>
    public void ChangeControlType(ControlProfile controlProfile)
    {
        currentControlProfile = controlProfile;
        Debug.Log("TEST 2");
        SetCurrentProfile(currentControlProfile);
        Debug.Log("START 2");
        SetBodyEvents();
    }

    /// <summary>
    /// Sets the events that will be triggered when the profile's button is pressed
    /// </summary>
    public void SetBodyEvents()
    {
        // Checks to see if brain needs to be initalized every time new body events are set
        InitalizeBrain();

        // If current profile is not set, set it to default
        if (currentProfile == null || currentControlProfile == ControlProfile.None)
        {
            SetCurrentProfile(profileToStartWith);
        }

        Debug.Log("Current profile is " + currentProfile.name);

        // If control type is UI
        if (currentProfile.controlType == 0)
        {
            // If no ui controller is detected
            if (uiController == null)
            {
                try
                {
                    uiController = CharacterSelectUI.Instance;
                }
                catch
                {
                    Debug.LogError("Could Not Find Character Select UI");
                    return;
                }
            }

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
                currentControlProfile = lastControlProfile;
                return;
            }

            // Clear input events
            for (int i = 0; i < playerBodyActions.Length; i++)
            {
                playerBodyActions[i] = null;
            }

            playerBody.SetBodyDeviceID(deviceID);

            Debug.Log("Setting body actions " + playerBody.name);

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
        }
    }

    /// <summary>
    /// Spawns a player body to connect to the brain. The passed in int is the player's ID
    /// </summary>
    /// <param name="playerToSpawn"></param>
    public void SpawnBody(int playerToSpawn)
    {
        // If body is already spawned, return
        if (playerBody != null)
            return;

        Debug.Log("START 1");

        SetPlayerBody(PlayerList.Instance.SpawnCharacterBody(playerToSpawn));
    }

    /// <summary>
    /// Connects the player brain to an already spawned body
    /// </summary>
    /// <param name="playerToSpawn"></param>
    public void ConnectBody(PlayerMain bodyToConnect)
    {
        // If body is already spawned, return
        if (playerBody != null)
            return;

        Debug.Log("START 2");
        SetPlayerBody(bodyToConnect);
    }

    /// <summary>
    /// Deletes the player's body from the world
    /// </summary>
    public void DestroyBody() 
    {
        // If there is no body, return
        if (playerBody == null)
            return;

        PlayerList.Instance.DeletePlayerBody(playerBody);
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
            uiActions[i]?.Invoke(pressed, this);
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
    public void DestroyBrain()
    {
        // If already destroyed return
        if (destroyed == true)
            return;

        destroyed = true;

        // If in player select ui when brain is destroyed
        if(uiController != null && uiController.uiType == UITypes.CharacterSelect)
        {
            Debug.Log("Remoe Plaer UI");
            uiController.RemovePlayerUI(this);
            DestroyBody();
        }

        // If device id is not 0, means it was valid and needs to be removed
        if (deviceID != -1)
            inputManager.DeletePlayerBrain(deviceID);

        // If player body is not null, add disconnected body to list
        if (playerBody != null)
            inputManager.playerSpawnSystem.AddDisconnectedPlayerBody(playerBody);
    }
}

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
    GameManagerNew gameManager;
    bool initalized = false;
    [SerializeField] bool isActiveBrain = false;
        public bool GetIsActiveBrain() { return isActiveBrain; }
        public void SetIsActiveBrain(bool newStatus) { isActiveBrain = newStatus; }

    protected GenericInputManager inputManager;

    [Header("Brain Stats")]

    [SerializeField] protected InputType brainInputType;
        public InputType GetBrainInputType() { return brainInputType; }

    [SerializeField] private int playerID = -1;
        public int GetPlayerID() { return playerID; } // Returns the player ID
        public void SetPlayerID(int newPlayerID) { playerID = newPlayerID; } // Sets the player ID

    [SerializeField] protected int deviceID = -1;
        public int GetDeviceID() { return deviceID; } // Returns the device ID

    [SerializeField] protected string playerUsername = "";
        public string GetPlayerUsername() { return playerUsername; } // Returns the player username
        public void SetPlayerUsername(string PlayerUsername) { playerUsername = PlayerUsername; } // Sets the player username

    [SerializeField] protected int characterID = -1;
        public int GetCharacterID() { return characterID; } // Returns the character ID
        public void SetCharacterID(int newCharacterID) { characterID = newCharacterID; } // Sets the character ID to a set value

    [SerializeField] int defaultCharacterID = 0; // The default ID to spawn if player is glitched

    [SerializeField] protected int teamID = -1;
        public int GetTeamID() { return teamID; } // Returns the team ID
        public void SetTeamID(int newTeamID) { teamID = newTeamID; } // Sets the team ID to a set value

    [SerializeField] protected Color teamColor = Color.white;
        public Color GetTeamColor() { return teamColor; } // Returns the team color
        public void SetTeamColor(Color newTeamColor) { teamColor = newTeamColor; } // Sets the team color to a set value

    [Header("UI Controlls")]
    [SerializeField] protected GenericUI uiController;

    [Header("Player Body")]
    [SerializeField] protected PlayerMain playerBody;
        public PlayerMain GetPlayerBody() { return playerBody; } // Returns the player body connected to brain
        public void SetPlayerBody(PlayerMain pm) { playerBody = pm; SetBodyEvents(); } // Sets player body to passed in player main and sets events when called

    public delegate void Keystroke();
    [SerializeField] protected List<InputProfileSO> inputProfileOptionsResource = new List<InputProfileSO>(); // The input profiles that are to be copied
    ControlProfile currentControlProfile = ControlProfile.None;
    public ControlProfile controlProfileSerialize;
    ControlProfile lastControlProfile; // Temp

    [SerializeField] protected InputProfileSO currentProfile;
        public InputProfileSO GetCurrentProfile() { return currentProfile; } // Returns the current control profile
        public void SetCurrentProfile(ControlProfile controlProfile) { controlProfileSerialize = controlProfile;  currentProfile = inputProfileOptionsResource[(int)controlProfile]; } // Sets the current profile to new based on int

    public GameStates localBrainGamestate;

    const int MaxInputValue = 12;
    public Action<bool>[] playerBodyActions;
    public Action<Vector2>[] playerBodyAxisActions;
    public Action<bool, GenericBrain>[] uiActions;
    public bool[] buttonSates;

    [Header("Status")]
    protected bool destroyed = false;

    /// <summary>
    /// Initalizes the brain's core functions
    /// </summary>
    public void InitalizeBrain()
    {
        gameManager = GameManagerNew.Instance;
        // Initalizes once if not initalized already
        if (initalized == false)
        {
            // Setup arrays
            playerBodyActions = new Action<bool>[MaxInputValue];
            playerBodyAxisActions = new Action<Vector2>[MaxInputValue];

            uiActions = new Action<bool, GenericBrain>[MaxInputValue];

            buttonSates = new bool[MaxInputValue];
            initalized = true;

            // Sets player username to random name
            SetPlayerUsername(GetRandomNameFromList());

            Debug.Log("Initalize Brain");

            SwapWhatsBeingControlledForGamestate(gameManager.CurrentState);

            InitalizeEvents();
        }
    }

    /// <summary>
    /// Initalizes events on the brain to handle state swapping and input changes
    /// </summary>
    public void InitalizeEvents()
    {
        // Adds to swapped game state event
        // Also updates the brain to be whatever is the current ui
        GameManagerNew.Instance.SwappedGameState += SwapWhatsBeingControlledForGamestate;

        // Sets the player to begin driving when entering map
        GameManagerNew.Instance.OnSwapLoadMatch += () => { controlProfileSerialize = ControlProfile.None; };
        GameManagerNew.Instance.OnSwapMainLoop += () => { controlProfileSerialize = ControlProfile.Driving; };
    }

    // Handles when we should deinitalize the player brain from events of the state swapping
    private void DeinitalizeEvents()
    {
        GameManagerNew.Instance.SwappedGameState -= SwapWhatsBeingControlledForGamestate;
        GameManagerNew.Instance.OnSwapLoadMatch -= () => { controlProfileSerialize = ControlProfile.None; };
        GameManagerNew.Instance.OnSwapMainLoop -= () => { controlProfileSerialize = ControlProfile.Driving; };
    }

    public void Update()
    {
        // If the current control profile is not the one serialized, cache and set it
        if (currentControlProfile != controlProfileSerialize)
        {
            UnsubscribeInputs();

            currentControlProfile = controlProfileSerialize;
            // Caches current for later
            lastControlProfile = currentControlProfile;

            // Sets current to be new control profile
            ChangeControlType(currentControlProfile);
        }
    }

    /// <summary>
    /// Calls when gamestate is being changed, updates the brain's controller to control the specified controls
    /// </summary>
    /// <param name="newGameState">The new gamestate the game is in</param>
    public void SwapWhatsBeingControlledForGamestate(GameStates newGameState)
    {
        // If the state being swapped is the same state as what is currently tracked on the brain, we return.
        // This is to prevent the intitalization and removal of the brain from the ui that it is on that the state is trying to set it to
        // IE currently on char select and gamestate swaps to char select, we dont want to reinitalize since we're already on char select
        if (localBrainGamestate == newGameState)
        {
            Debug.Log($"@@ Already in {newGameState} for {deviceID}");
            return;
        }

        localBrainGamestate = newGameState;
        Debug.Log($"@@ Setting gamestate {newGameState} for {deviceID}");

        switch (newGameState)
        {
            case GameStates.MainMenu:
            case GameStates.Options:
            case GameStates.PlayerSelect:
            case GameStates.GameModeSelect:
            case GameStates.MapSelect:
            case GameStates.RuleSelect:
            case GameStates.Results:
            case GameStates.Paused:
                ChangeUIHookedUpToTheBrain(newGameState);
                break;
            default:
                if (currentControlProfile == ControlProfile.None)
                {
                    // Sets control profile to be whats on prefab when spawns
                    SetCurrentProfile(controlProfileSerialize);
                }
                break;
        }
    }

    /// <summary>
    /// Changes which ui the player controls.
    /// </summary>
    /// <param name="uiType">The passed in type that the player will attempt to find to control</param>
    public void ChangeUIHookedUpToTheBrain(GameStates newGameState)
    {
        Debug.Log($"Changing Which UI Is Hooked Up To: {newGameState.ToString()} in device id {deviceID}");

        SetCurrentProfile(ControlProfile.UI);

        // If the player is already connected to another UI... remove it
        if (uiController != null)
        {
            Debug.Log($"@@ DEBUG 1 for {deviceID}");
            uiController.RemovePlayerUI(this);
        }

        // Switch to find the type I want to control
        switch (newGameState)
        {
            case GameStates.MainMenu:
                if(MainMenuUI.Instance != null)
                    SetControlToUI(MainMenuUI.Instance);
                return;
            case GameStates.Options:
                if (SettingsMenuUI.Instance != null)
                    SetControlToUI(SettingsMenuUI.Instance);
                return;
            case GameStates.GameModeSelect:
                if (GameModeSelectUI.Instance != null)
                    SetControlToUI(GameModeSelectUI.Instance);
                return;
            case GameStates.PlayerSelect:
                if (CharacterSelectUI.Instance != null)
                    SetControlToUI(CharacterSelectUI.Instance);
                return;
            case GameStates.MapSelect:
                if (MapSelectUI.Instance != null)
                    SetControlToUI(MapSelectUI.Instance);
                return;
            case GameStates.RuleSelect:
                if (RuleSelectUI.Instance != null)
                    SetControlToUI(RuleSelectUI.Instance);
                return;
            case GameStates.Results:
                if (ResultsMenuUI.Instance != null)
                    SetControlToUI(ResultsMenuUI.Instance);
                return;
            case GameStates.Paused: // Instead sets everyone to own pause menu instead of global menu
                if (playerBody != null && playerBody.pauseMenuUI != null)
                    SetControlToUI(playerBody.pauseMenuUI);
                return;
        }
    }

    /// <summary>
    /// Saves the ui that will be controlled alongside setting the brain to control the said ui
    /// </summary>
    /// <param name="controlProfile">The passed in control profile type being switched to</param>
    /// <param name="uiToBeControlled">The passed in ui that will be controlled</param>
    public void SetControlToUI(GenericUI uiToBeControlled)
    {
        if (uiToBeControlled == null)
            return;

        if (isActiveBrain == false)
            return;

        // Zeros out body ball if player body exists
        if(playerBody != null)
        {
            playerBody.StopDriving();
        }

        //ResetButtonStates();

        uiController = uiToBeControlled;
        uiController.AddPlayerToUI(this);

        ChangeControlType(ControlProfile.UI);
    }

    /// <summary>
    /// Sets the player's control profile based on a passed in enum
    /// </summary>
    /// <param name="controlProfile">The passed in control profile type being switched to</param>
    public void ChangeControlType(ControlProfile controlProfile)
    {
        if (isActiveBrain == false)
            return;

        currentControlProfile = controlProfile;

        Debug.Log("Setting Profile:" + controlProfile.ToString());
        SetCurrentProfile(currentControlProfile);

        // If setting profile to none, return
        if (controlProfile == ControlProfile.None)
            return;

        // Set the body events otherwise
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
            Debug.Log("Setting Profile:" + controlProfileSerialize.ToString());
            SetCurrentProfile(controlProfileSerialize);
        }

        if (currentProfile == null)
            return;

        Debug.Log("Current profile is " + currentProfile.name);

        UnsubscribeInputs();

        // If control type is UI
        if (currentProfile.controlType == 0)
        {
            // If no ui controller is detected
            if (uiController == null)
            {
                ChangeUIHookedUpToTheBrain(GameManagerNew.Instance.CurrentState);
                return;
            }

            if(uiController != null)
            {
                // Set inputs
                uiActions[0] += uiController.Up;
                uiActions[1] += uiController.Left;
                uiActions[2] += uiController.Down;
                uiActions[3] += uiController.Right;

                uiActions[4] += uiController.Pause;

                uiActions[5] += uiController.Confirm;
                uiActions[6] += uiController.Return;

                uiActions[7] += uiController.Button1;
                uiActions[8] += uiController.Button2;

                /// UI Inputs for spesific buttons should match their position on driving. IE pause is ui action 10, so on both driving and ui control it should be 10
            }
        }
        // If control type is driving
        else
        {
            // If player body is null, switch back to last control profile
            if (playerBody == null)
            {
                Debug.LogWarning("Switching To Profile At Wrong Time... Reverting");
                currentControlProfile = lastControlProfile;
                return;
            }

            ResetButtonStates();

            // Clear input events
            for (int i = 0; i < playerBodyActions.Length; i++)
            {
                playerBodyActions[i] = null;
            }

            InitalizeBodyInfoFromBrain();

            // After the player body is initalized, we initalize podium stats
            playerBody.playerMatchStats.InitalizePodiumStats();

            Debug.Log("Setting body actions " + playerBody.name);

            // Set inputs
            playerBodyActions[0] += playerBody.Up;
            playerBodyActions[1] += playerBody.Left;
            playerBodyActions[2] += playerBody.Down;
            playerBodyActions[3] += playerBody.Right;

            playerBodyActions[4] += playerBody.Pause;

            playerBodyActions[5] += playerBody.Attack;
            playerBodyActions[6] += playerBody.Special;
            playerBodyActions[7] += playerBody.Drift;

            playerBodyActions[8] += playerBody.Drive;
            playerBodyActions[9] += playerBody.Reverse;
            playerBodyActions[10] += playerBody.ReflectCamera;

            // Left Stick
            playerBodyAxisActions[0] += playerBody.LeftStick;

            // Right Stick
            playerBodyAxisActions[1] += playerBody.RightStick;
        }
    }

    /// <summary>
    /// Initalizes the body connected to the brain
    /// </summary>
    private void InitalizeBodyInfoFromBrain()
    {
        if(playerBody == null)
        {
            Debug.LogError("Missing Player Body When trying to initalize from brain");
            return;
        }

        playerBody.SetBodyDeviceID(deviceID);
        playerBody.SetBodyPlayerID(playerID);

        // If the player username is not null set body's username to match
        if (playerUsername != "")
            playerBody.SetPlayerUsername(playerUsername);

        // Sets the team information and team color information
        if (teamID != -1)
        {
            playerBody.SetBodyTeamID(teamID);
            playerBody.SetBodyTeamColor(teamColor);
        }
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
        if (currentProfile.controlType == InputProfileSO.ControlType.UI)
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
    /// Gets a random name from the generated list and returns it for the player's username
    /// </summary>
    /// <returns>player username string</returns>
    private string GetRandomNameFromList()
    {
        int maxNameCount = gameManager.LoadedNames.Length;

        return gameManager.LoadedNames[UnityEngine.Random.Range(0, maxNameCount)];
    }

    /// <summary>
    /// Used by the map manager to set the player's body positions
    /// </summary>
    /// <param name="spawnPosition">The position to set the player body</param>
    public void SetBodyPosition(Vector3 spawnPosition)
    {
        //Debug.Log($"Player ID: {playerID} is resetting position: {spawnPosition}");

        if (playerBody != null)
        {
            // Sets the spawned body to be at the spawn position, passed in from the map manager
            playerBody.playerBodyBall.transform.position = spawnPosition;
            //Debug.Log($"New position set to: {playerBody.transform.position}");
        }
        else
        {
            Debug.LogError("Player body is not assigned.");
        }
    }

    /// <summary>
    /// Spawns a player body to connect to the brain. The passed in int is the player's ID
    /// Returns if it was successful in spawning a body
    /// </summary>
    /// <param name="spawnPosition">The position to set the player body</param>
    /// <param name="spawnRotation">The rotation to set the player body</param>
    public bool SpawnBody(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        // If body is already spawned, return
        if (playerBody != null)
            return false;

        // Means somehow player did not choose character, spawn the default player based on default ID
        if (characterID == -1)
            characterID = defaultCharacterID;

        //Debug.Log("Player ID to spawn " + characterID);
        SetPlayerBody(PlayerList.Instance.SpawnCharacterBody(this, characterID));

        InitalizeBodyInfoFromBrain();

        //Debug.Log($"Player ID: {playerID} is spawning at position: {spawnPosition}");
        // Sets the spawned body to be at the spawn position, passed in from the map manager
        playerBody.transform.position = new Vector3(0,0,0);
        playerBody.playerBodyBall.transform.position = spawnPosition;
        playerBody.ballDriving.SetKartRotation(spawnRotation.eulerAngles);

        return true;
    }

    /// <summary>
    /// Connects the player brain to an already spawned body
    /// </summary>
    /// <param name="bodyToConnect">The player main body to connect to the brain</param>
    public void ConnectBody(PlayerMain bodyToConnect)
    {
        // If body is already spawned, return
        if (playerBody != null)
            return;

        SetPlayerBody(bodyToConnect);
    }

    /// <summary>
    /// Toggles to set the brain to active or not
    /// </summary>
    /// <param name="setActive"></param>
    public void ToggleActivateBrain(bool setActive)
    {
        PlayerSpawnSystem playerSpawnSystem = PlayerSpawnSystem.Instance;

        if (playerSpawnSystem == null)
        {
            Debug.LogError("No Player Spawn System in scene, must have been deleted");
            return;
        }

        isActiveBrain = setActive;

        if (setActive == true)
        {
            InitalizeEvents();
            playerSpawnSystem.AddActivePlayerBrain(this);
            playerSpawnSystem.RemoveIdlePlayerBrain(this);

            // Resets ID to match new position
            playerID = playerSpawnSystem.ActiveBrains.Count - 1;
        }
        else if (setActive == false)
        {
            DeinitalizeEvents();
            //UnsubscribeInputs();

            uiActions[5] += ActivateBrainFromIdlePool;

            // If in player select ui when brain is destroyed
            if (uiController != null)
            {
                uiController.RemovePlayerUI(this);
            }

            SetPlayerID(-1);

            // Sets state to defualt/null as brain is inactive in-game
            localBrainGamestate = GameStates.Default;

            playerSpawnSystem.RemoveActivePlayerBrain(this);
            playerSpawnSystem.AddIdlePlayerBrain(this);
        }
    }

    public void UnsubscribeInputs()
    {
        if (uiController != null)
        {
            uiActions[0] -= uiController.Up;
            uiActions[1] -= uiController.Left;
            uiActions[2] -= uiController.Down;
            uiActions[3] -= uiController.Right;

            uiActions[4] -= uiController.Pause;

            uiActions[5] -= uiController.Confirm;
            uiActions[6] -= uiController.Return;

            uiActions[7] -= uiController.Button1;
            uiActions[8] -= uiController.Button2;
        }

        if (playerBody != null)
        {
            // Set inputs
            playerBodyActions[0] -= playerBody.Up;
            playerBodyActions[1] -= playerBody.Left;
            playerBodyActions[2] -= playerBody.Down;
            playerBodyActions[3] -= playerBody.Right;

            playerBodyActions[4] -= playerBody.Pause;

            playerBodyActions[5] -= playerBody.Attack;
            playerBodyActions[6] -= playerBody.Special;
            playerBodyActions[7] -= playerBody.Drift;

            playerBodyActions[8] -= playerBody.Drive;
            playerBodyActions[9] -= playerBody.Reverse;
            playerBodyActions[10] -= playerBody.ReflectCamera;

            // Left Stick
            playerBodyAxisActions[0] -= playerBody.LeftStick;

            // Right Stick
            playerBodyAxisActions[1] -= playerBody.RightStick;
        }
    }

    private void ActivateBrainFromIdlePool(bool buttonStatus, GenericBrain player)
    {
        if (buttonStatus)
        {
            uiActions[5] -= ActivateBrainFromIdlePool;
            Debug.Log("@@ Adding player back");
            ToggleActivateBrain(true);
            SwapWhatsBeingControlledForGamestate(GameStates.PlayerSelect);
        }
    }

    /// <summary>
    /// Resets the button states to all be false, useful for when swapping states between menus or driving
    /// </summary>
    public void ResetButtonStates()
    {
        for(int i = 0; i < buttonSates.Length; i++)
        {
            buttonSates[i] = false;
        }
    }

    /// <summary>
    /// Deletes the player's body from the world
    /// </summary>
    public void DestroyBody() 
    {
        // If there is no body, return
        if (playerBody == null)
            return;

        PlayerList.Instance.DeletePlayerBody(this, playerBody);
    }

    /// <summary>
    /// Destroys the brain and the connection to the body
    /// </summary>
    public void DestroyBrain()
    {
        // If already destroyed return
        if (destroyed == true)
            return;

        Debug.Log("Destroying Brain");
        destroyed = true;

        DeinitalizeEvents();
        UnsubscribeInputs();

        // If in player select ui when brain is destroyed
        if (uiController != null)
        {
            uiController.RemovePlayerUI(this);
            uiController.ReinitalizePlayerIDs(playerID);
            DestroyBody();
        }

        // If device id is not 0, means it was valid and needs to be removed
        if (deviceID != -1)
        {
            PlayerSpawnSystem.Instance.RemoveActivePlayerBrain(this);
            PlayerSpawnSystem.Instance.RemoveIdlePlayerBrain(this);
            inputManager.DeletePlayerBrain(deviceID);
        }

        // If player body is not null, add disconnected body to list
        if (playerBody != null)
            inputManager.playerSpawnSystem.AddDisconnectedPlayerBody(playerBody);
    }
}

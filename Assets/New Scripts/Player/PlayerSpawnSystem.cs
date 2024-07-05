///
/// Created by Alex Fischer | May 2024
/// 

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The main player spawn system, intended to spawn player brains
/// </summary>
public class PlayerSpawnSystem : SingletonMonobehaviour<PlayerSpawnSystem>
{
    [SerializeField] UnityInputManager unityInputManager;
    public UnityInputManager UnityInputManager { get { return unityInputManager; } }

    [SerializeField] DLLInputManager dllInputManager;
    public DLLInputManager DllInputManager { get { return dllInputManager; } }

    [Space(10)]
    [Header("Player Count")]
    const int MAX_PLAYER_COUNT = 4;
    public int GetMaxPlayerCount() { return MAX_PLAYER_COUNT; }

    [SerializeField] int playerBrainCount = 0;
    Rect[] cameraRects;
    public void SetPlayerBrainCount(int newPlayerCount) { playerBrainCount = newPlayerCount; } // Setter for player count
    public int GetPlayerBrainCount() { return playerBrainCount; } // Getter for player count
    public void AddPlayerBrainCount(int value) // adds passed in number to player count
    {
        playerBrainCount += value;
    }
    public void SubtractPlayerCount(int value) // subtracts passed in number to player count
    {
        if (playerBrainCount > 0)
        {
            playerBrainCount -= value;
        }
    }

    [SerializeField] bool multikeyboardEnabled;
    public bool GetMultikeyboardEnabled() { return multikeyboardEnabled; }
    public void SetMultikeyboardEnabled(bool passIn) 
    {
        dllInputManager.enabled = passIn;
        multikeyboardEnabled = passIn; 
    }

    [Header("Spawned Player Brains")]
    Dictionary<int, GenericBrain> spawnedBrains = new Dictionary<int, GenericBrain>();
    public Dictionary<int, GenericBrain> SpawnedBrains {  get { return spawnedBrains; } }
    public void AddPlayerBrain(GenericBrain brain) // adds passed in brain to list
    {
        if(multikeyboardEnabled == false && brain.GetComponent<DllBrain>() != null)
        {
            dllInputManager.DeletePlayerBrain(brain.GetDeviceID());
        }

        Debug.Log("adding " + brain.GetPlayerID());

        spawnedBrains.Add(brain.GetPlayerID(), brain);
        OnAddPlayerBrain?.Invoke();
    }
    public void DeletePlayerBrain(GenericBrain brain) // removes passed in brain from list
    {
        spawnedBrains.Remove(brain.GetPlayerID());
    }

    public event Action OnAddPlayerBrain;

    [Header("Spawned Player Bodies")]
    Dictionary<GenericBrain, PlayerMain> spawnedBodies = new Dictionary<GenericBrain, PlayerMain>();
    public void AddPlayerBody(GenericBrain brain, PlayerMain body) // adds passed in player main to list
    { 
        spawnedBodies.Add(brain, body); 
        UpdatePlayerCameraRects();
    }
    public void DeletePlayerBody(GenericBrain brain) // removes passed in player main from list
    { 
        spawnedBodies.Remove(brain); 
        UpdatePlayerCameraRects(); 
    }
    public void ReinitalizePlayerBody(GenericBrain brain, PlayerMain body)
    {
        foreach (KeyValuePair<GenericBrain, PlayerMain> spawnedPlayer in spawnedBodies)
        {
            // We found the body already in the dictionary
            if (spawnedPlayer.Value == body)
            {
                Debug.Log("Reinitalizing body device id from " + spawnedPlayer.Key + " to " + brain.GetDeviceID());
                spawnedBodies[brain] = body;
                //spawnedBodies.Remove(brain);
                //spawnedBodies.Add(brain, body);
                return;
            }
        }
    }

    [Header("Disconnected Player Bodies")]
    [SerializeField] List<PlayerMain> disconnectedBodies;
    public List<PlayerMain> GetDisconnectedBodies() {return disconnectedBodies; } // returns list of disconnected bodies
    public void AddDisconnectedPlayerBody(PlayerMain body) { disconnectedBodies.Add(body); } // adds player body to disconnected body list
    public void RemoveDisconnectedBody(int pos) {disconnectedBodies.RemoveAt(pos); } // removes player body from disconnected body list

    public void Start()
    {
        SetMultikeyboardEnabled(multikeyboardEnabled);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            SetMultikeyboardEnabled(true);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            SetMultikeyboardEnabled(false);
        }
    }

    /// <summary>
    /// Checks the amount of players
    /// </summary>
    /// <returns> True if another player can spawn</returns>
    public bool CheckPlayerCount()
    {
        if (playerBrainCount >= MAX_PLAYER_COUNT)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Finds the next open player slot, loops through possible lobby size to find open slot
    /// Returns player count + 1 if there is no empty slot
    /// </summary>
    /// <returns></returns>
    public int FindNextOpenPlayerSlot()
    {
        // Loops for all slots to try finding empty slot
        for(int i = 0; i < MAX_PLAYER_COUNT; i++)
        {
            // If the dictionary has no key for i, count that pos as empty and return it
            GenericBrain foundBrain = null;
            if (!spawnedBrains.TryGetValue(i, out foundBrain))
            {
                Debug.Log("Found open slot at pos " + i);
                return i;
            }
        }

        int nextPos = GetPlayerBrainCount();

        // Returns the player
        Debug.Log("Could not find open slot... adding to end: " + nextPos);
        return nextPos;
    }

    /// <summary>
    /// Finds body in disconnected bodies list based on device id, returning player main on body
    /// </summary>
    /// <returns> player main on found player in disconnected bodies </returns>
    public PlayerMain FindBodyByLastID(int deviceId)
    {
        // Loops through disconnected bodies
        foreach (PlayerMain body in disconnectedBodies)
        {
            // Checks if body device id matches passed id
            if(body.GetBodyDeviceID() == deviceId) { return body; }
        }
        return null;
    }

    ///<summary>
    /// Updates the camera rects on all players in scenes
    ///</summary>
    private void UpdatePlayerCameraRects()
    {
        // returns camera rects to use for cameras in-game
        cameraRects = CalculateRects();

        // Returns if there are less then 1 player
        if (cameraRects.Length <= 0)
            return;

        int cameraRectCounter = 0;

        // Loops for all spawned bodies and sets cameras to be the respective camera rect
        foreach (KeyValuePair<GenericBrain, PlayerMain> spawnedPlayer in spawnedBodies)
        {
            if (spawnedPlayer.Value != null)
            {
                if (cameraRectCounter < cameraRects.Length)
                {
                    Rect temp = cameraRects[cameraRectCounter];
                    spawnedPlayer.Value.playerCamera.rect = temp;
                    cameraRectCounter++;
                }
                else
                {
                    // Handle the case where there are more non-null player inputs than camera rects
                    break;
                }
            }
        }
    }

    ///<summary>
    /// Calculates the camera rects for when there are 1 - 4 players
    ///</summary>
    ///<returns> array of camera rects </returns>
    private Rect[] CalculateRects()
    {
        Rect[] viewportRects = new Rect[playerBrainCount];

        // 1 Player
        if (playerBrainCount == 1)
        {
            viewportRects[0] = new Rect(0, 0, 1, 1);
        }
        else if (playerBrainCount == 2)
        {
            viewportRects[0] = new Rect(0.25f, 0.5f, 0.5f, 0.5f);
            viewportRects[1] = new Rect(0.25f, 0, 0.5f, 0.5f);
        }
        else if (playerBrainCount == 3)
        {
            viewportRects[0] = new Rect(0, 0.5f, 0.5f, 0.5f);
            viewportRects[1] = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            viewportRects[2] = new Rect(0.25f, 0, 0.5f, 0.5f);
        }
        else if (playerBrainCount == 4)
        {
            viewportRects[0] = new Rect(0, 0.5f, 0.5f, 0.5f);
            viewportRects[1] = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            viewportRects[2] = new Rect(0, 0, 0.5f, 0.5f);
            viewportRects[3] = new Rect(0.5f, 0, 0.5f, 0.5f);
        }

        return viewportRects;
    }
}

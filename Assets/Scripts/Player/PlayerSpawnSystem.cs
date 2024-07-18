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
    [SerializeField] int MAX_PLAYER_COUNT = 4;
        public int GetMaxPlayerCount() { return MAX_PLAYER_COUNT; }
    Rect[] cameraRects;

    [SerializeField] bool multikeyboardEnabled;
        public bool GetMultikeyboardEnabled() { return multikeyboardEnabled; }
        public void SetMultikeyboardEnabled(bool passIn) 
    {
        dllInputManager.enabled = passIn;
        multikeyboardEnabled = passIn; 
    }

    [Header("Spawned Player Brains")]
    [SerializeField] List<GenericBrain> activeBrains = new List<GenericBrain>();
    public List<GenericBrain> ActiveBrains {  get { return activeBrains; } }

    [SerializeField] List<GenericBrain> idleBrains = new List<GenericBrain>();
    public List<GenericBrain> IdleBrains { get { return idleBrains; } }
        public int GetActiveBrainCount() { return activeBrains.Count; } // Getter for player count
        public void AddActivePlayerBrain(GenericBrain brain) // adds passed in brain to list
        {
            if(multikeyboardEnabled == false && brain.GetComponent<DllBrain>() != null)
            {
                dllInputManager.DeletePlayerBrain(brain.GetDeviceID());
            }

            activeBrains.Add(brain);
        }
        public void RemoveActivePlayerBrain(GenericBrain brain) // adds passed in brain to list
        {
            try
            {
                activeBrains.Remove(brain);
            }
            catch
            {
                Debug.LogError("Trying to remove active player brain not in active player brain list");
            }
        }
        public void RemoveIdlePlayerBrain(GenericBrain brain) // adds passed in brain to list
        {
            try
            {
                idleBrains.Remove(brain);
            }
            catch
            {
                Debug.LogError("Trying to remove idle player brain not in idle player brain list");
            }
        }
        public void AddIdlePlayerBrain(GenericBrain brain) // adds passed in brain to list
        {
            if (multikeyboardEnabled == false && brain.GetComponent<DllBrain>() != null)
            {
                dllInputManager.DeletePlayerBrain(brain.GetDeviceID());
            }

            idleBrains.Add(brain);
        }
        public void DeletePlayerBrain(GenericBrain brain) // removes passed in brain from list
    {
        activeBrains.Remove(brain);
    }

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

        /// <summary>
        /// Reinitalizes the passed in player body to be set to the ID of the new brain
        /// </summary>
        /// <param name="brain">The passed in new brain</param>
        /// <param name="body">The disconnected body to be reinitalized</param>
        public void ReinitalizePlayerBody(GenericBrain brain, PlayerMain body)
        {
            //foreach (KeyValuePair<GenericBrain, PlayerMain> spawnedPlayer in spawnedBodies)
            //{
            //    // We found the body already in the dictionary
            //    if (spawnedPlayer.Value == body)
            //    {
            //        Debug.Log("Reinitalizing body device id from " + spawnedPlayer.Key.GetPlayerID() + " to " + brain.GetDeviceID());
            //        //spawnedBodies[brain] = body;
            //        spawnedBodies.Remove(brain);
            //        spawnedBodies.Add(brain, body);
            //        return;
            //    }
            //}
        }

    [Header("Disconnected Player Bodies")]
    [SerializeField] List<PlayerMain> disconnectedBodies = new List<PlayerMain>();
        public List<PlayerMain> GetDisconnectedBodies() {return disconnectedBodies; } // returns list of disconnected bodies
        public void AddDisconnectedPlayerBody(PlayerMain body) { disconnectedBodies.Add(body); } // adds player body to disconnected body list
        public void RemoveDisconnectedBody(int pos) {disconnectedBodies.RemoveAt(pos); } // removes player body from disconnected body list

    public void Start()
    {
        SetMultikeyboardEnabled(multikeyboardEnabled);
    }

    /// <summary>
    /// Checks the amount of players
    /// </summary>
    /// <returns> True if another player can spawn</returns>
    public bool CheckPlayerCount()
    {
        if (GetActiveBrainCount() >= MAX_PLAYER_COUNT)
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
        //// Loops for all slots to try finding empty slot
        //for(int i = 0; i < MAX_PLAYER_COUNT; i++)
        //{
        //    // If the dictionary has no key for i, count that pos as empty and return it
        //    GenericBrain foundBrain = null;

        //    if (!spawnedBrains.TryGetValue(i, out foundBrain))
        //    {
        //        Debug.Log("Found open slot at pos " + i);
        //        return i;
        //    }
        //}

        int nextPos = activeBrains.Count; //GetPlayerBrainCount();

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
        Rect[] viewportRects = new Rect[GetActiveBrainCount()];

        // 1 Player
        if (GetActiveBrainCount() == 1)
        {
            viewportRects[0] = new Rect(0, 0, 1, 1);
        }
        else if (GetActiveBrainCount() == 2)
        {
            viewportRects[0] = new Rect(0.25f, 0.5f, 0.5f, 0.5f);
            viewportRects[1] = new Rect(0.25f, 0, 0.5f, 0.5f);
        }
        else if (GetActiveBrainCount() == 3)
        {
            viewportRects[0] = new Rect(0, 0.5f, 0.5f, 0.5f);
            viewportRects[1] = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            viewportRects[2] = new Rect(0.25f, 0, 0.5f, 0.5f);
        }
        else if (GetActiveBrainCount() == 4)
        {
            viewportRects[0] = new Rect(0, 0.5f, 0.5f, 0.5f);
            viewportRects[1] = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            viewportRects[2] = new Rect(0, 0, 0.5f, 0.5f);
            viewportRects[3] = new Rect(0.5f, 0, 0.5f, 0.5f);
        }

        return viewportRects;
    }
}

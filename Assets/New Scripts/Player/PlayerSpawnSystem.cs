///
/// Created by Alex Fischer | May 2024
/// 

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The main player spawn system, intended to spawn player brains
/// </summary>
public class PlayerSpawnSystem : SingletonMonobehaviour<PlayerSpawnSystem>
{
    [SerializeField] ControllerInputManager controllerInputManager;
    [SerializeField] KeyboardInputManager keyboardInputManager;

    [Header("Player Count")]
    const int MAX_PLAYER_COUNT = 4;
    [SerializeField] int playerCount;
    public void SetPlayerCount(int newPlayerCount) { playerCount = newPlayerCount; } // Setter for player count
    public int GetPlayerCount() { return playerCount; } // Getter for player count
    public void AddPlayerCount(int value)
    {
        playerCount += value;
    } // adds passed in number to player count
    public void SubtractPlayerCount(int value)
    {
        if (playerCount > 0)
        {
            playerCount -= value;
        }
    } // subtracts passed in number to player count

    [Header("Spawned Player Bodies")]
    [SerializeField] List<PlayerMain> spawnedBodies;
    [SerializeField] List<GameObject> spawnedPlayerDisplay;
    public void AddPlayerBody(PlayerMain body, GameObject playerDisplay) // adds passed in player main to list
    { 
        spawnedBodies.Add(body);
        spawnedPlayerDisplay.Add(playerDisplay);
        UpdatePlayerCameraRects();
    }
    public void DeletePlayerBody(PlayerMain body, GameObject playerDisplay)// removes passed in player main from list
    { 
        spawnedBodies.Remove(body); 
        spawnedPlayerDisplay.Remove(playerDisplay);
        UpdatePlayerCameraRects(); 
    }

    /// <summary>
    /// Checks the amount of players
    /// </summary>
    /// <returns> True if another player can spawn</returns>
    public bool CheckPlayerCount()
    {
        if (playerCount >= MAX_PLAYER_COUNT)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    [Header("Disconnected Player Bodies")]
    [SerializeField] List<PlayerMain> disconnectedBodies;
    public List<PlayerMain> GetDisconnectedBodies() {return disconnectedBodies; } // returns list of disconnected bodies
    public void AddDisconnectedPlayerBody(PlayerMain body) { disconnectedBodies.Add(body); } // adds player body to disconnected body list
    public void RemoveDisconnectedBody(int pos) {disconnectedBodies.RemoveAt(pos); } // removes player body from disconnected body list
    public void RemoveDisconnectedBody(PlayerMain body) { disconnectedBodies.Remove(body); } // removes player body from disconnected body list

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

    Rect[] cameraRects;
    Rect[] uiRects;

    ///<summary>
    /// Updates the camera rects on all players in scenes
    ///</summary>
    private void UpdatePlayerCameraRects()
    {
        // returns camera rects to use for cameras in-game
        cameraRects = CalculateCameraRects();
        uiRects = CalculateUIRects();
        // Returns if there are less then 1 player
        if (cameraRects.Length <= 0)
            return;

        int playerRectCounter = 0;

        // Loops for all spawned bodies and sets cameras to be the respective camera rect
        for (int i = 0; i < spawnedBodies.Count; i++)
        {
            if (spawnedBodies[i] != null)
            {
                if (playerRectCounter < cameraRects.Length)
                {
                    Rect temp = cameraRects[playerRectCounter];
                    spawnedBodies[i].playerCamera.rect = temp;

                    temp = uiRects[playerRectCounter];
                    spawnedPlayerDisplay[i].GetComponent<Transform>().localScale = new Vector2(temp.width, temp.height);
                    spawnedPlayerDisplay[i].GetComponent<Transform>().localPosition = new Vector2(temp.x * 1920, temp.y * 1080);

                    playerRectCounter++;
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
    private Rect[] CalculateCameraRects()
    {
        Rect[] viewportRects = new Rect[playerCount];

        // 1 Player
        if (playerCount == 1)
        {
            viewportRects[0] = new Rect(0, 0, 1, 1);
        }
        else if (playerCount == 2)
        {
            viewportRects[0] = new Rect(0.25f, 0.5f, 0.5f, 0.5f);
            viewportRects[1] = new Rect(0.25f, 0, 0.5f, 0.5f);
        }
        else if (playerCount == 3)
        {
            viewportRects[0] = new Rect(0, 0.5f, 0.5f, 0.5f);
            viewportRects[1] = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            viewportRects[2] = new Rect(0.25f, 0, 0.5f, 0.5f);
        }
        else if (playerCount == 4)
        {
            viewportRects[0] = new Rect(0, 0.5f, 0.5f, 0.5f);
            viewportRects[1] = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            viewportRects[2] = new Rect(0, 0, 0.5f, 0.5f);
            viewportRects[3] = new Rect(0.5f, 0, 0.5f, 0.5f);
        }

        return viewportRects;
    }

    ///<summary>
    /// Calculates the UI rects for when there are 1 - 4 players
    ///</summary>
    ///<returns> array of camera rects </returns>
    private Rect[] CalculateUIRects()
    {
        Rect[] viewportRects = new Rect[playerCount];

        // 1 Player
        if (playerCount == 1)
        {
            viewportRects[0] = new Rect(0, 0, 1, 1);
        }
        else if (playerCount == 2)
        {
            viewportRects[0] = new Rect(0, 0.25f, 0.5f, 0.5f);
            viewportRects[1] = new Rect(0, -0.25f, 0.5f, 0.5f);
        }
        else if (playerCount == 3)
        {
            viewportRects[0] = new Rect(0, 0.5f, 0.5f, 0.5f);
            viewportRects[1] = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            viewportRects[2] = new Rect(0.25f, 0, 0.5f, 0.5f);
        }
        else if (playerCount == 4)
        {
            viewportRects[0] = new Rect(0, 0.5f, 0.5f, 0.5f);
            viewportRects[1] = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            viewportRects[2] = new Rect(0, 0, 0.5f, 0.5f);
            viewportRects[3] = new Rect(0.5f, 0, 0.5f, 0.5f);
        }

        return viewportRects;
    }
}

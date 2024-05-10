using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnSystem : SingletonMonobehaviour<PlayerSpawnSystem>
{
    [SerializeField] ControllerInputManager controllerInputManager;
    [SerializeField] KeyboardInputManager keyboardInputManager;

    [Header("Player Count")]
    const int MAX_PLAYER_COUNT = 4;
    [SerializeField] int playerCount;
    public void SetPlayerCount(int newPlayerCount) { playerCount = newPlayerCount; }
    public int GetPlayerCount() { return playerCount; }
    public void AddPlayerCount(int value)
    {
        playerCount += value;
    }
    public void SubtractPlayerCount(int value)
    {
        if (playerCount > 0)
        {
            playerCount -= value;
        }
    }

    [Header("Spawned Player Bodies")]
    [SerializeField] List<PlayerMain> spawnedBodies;
    public void AddPlayerBody(PlayerMain body) { spawnedBodies.Add(body); UpdatePlayerCameraRects();}
    public void RemovePlayerBody(PlayerMain body) { spawnedBodies.Remove(body); UpdatePlayerCameraRects(); }

    /// <summary>
    /// Checks the amount of players and returns true if another player can spawn
    /// </summary>
    /// <returns></returns>
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
    public List<PlayerMain> GetDisconnectedBodies() {return disconnectedBodies;}
    public void AddDisconnectedPlayerBody(PlayerMain body) { disconnectedBodies.Add(body); }
    public void RemoveDisconnectedBody(int pos) {disconnectedBodies.RemoveAt(pos);}
    public PlayerMain FindBodyByLastID(int deviceId)
    {
        foreach (PlayerMain body in disconnectedBodies)
        {
            if(body.GetBodyDeviceID() == deviceId) { return body; }
        }
        return null;
    }

    Rect[] cameraRects;

    ///<summary>
    /// Updates the camera rects on all players in scenes
    ///</summary>
    private void UpdatePlayerCameraRects()
    {
        cameraRects = CalculateRects();

        if (cameraRects.Length <= 0)
            return;

        int cameraRectCounter = 0;

        for (int i = 0; i < spawnedBodies.Count; i++)
        {
            if (spawnedBodies[i] != null)
            {
                if (cameraRectCounter < cameraRects.Length)
                {
                    Rect temp = cameraRects[cameraRectCounter];
                    spawnedBodies[i].playerCamera.rect = temp;
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
    private Rect[] CalculateRects()
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
}

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
}

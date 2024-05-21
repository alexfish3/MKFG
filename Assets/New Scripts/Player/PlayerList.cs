///
/// Created by Alex Fischer | May 2024
/// 

using UnityEngine;

/// <summary>
/// The main player spawn system, intended to spawn player brains
/// </summary>
public class PlayerList : SingletonMonobehaviour<PlayerList>
{
    [SerializeField] PlayerSpawnSystem playerSpawnSystem;
    [SerializeField] GameObject[] characters;
    [SerializeField] GameObject bodyParent;

    public int spawnedPlayerCount;
    [SerializeField] GameObject[] spawnPositions;

    /// <summary>
    /// Spawns Character Body based on ID of character
    /// </summary>
    /// <param name="characterID"></param>
    /// <returns>
    /// Returns Player Main script on body
    /// </returns>
    public PlayerMain SpawnCharacterBody(int characterID)
    {
        GameObject character = Instantiate(characters[characterID], spawnPositions[spawnedPlayerCount++].transform.position, Quaternion.identity);
        character.transform.parent = bodyParent.transform;

        PlayerMain playerMain = character.GetComponent<PlayerMain>();

        playerSpawnSystem.AddPlayerBody(playerMain);

        return playerMain;
    }

    /// <summary>
    /// Removes the player body from the scene
    /// </summary>
    /// <param name="body">The body to remove</param>
    public void DeletePlayerBody(PlayerMain body)
    {
        playerSpawnSystem.DeletePlayerBody(body);
        Destroy(body.gameObject);
        spawnedPlayerCount--;
    }
}

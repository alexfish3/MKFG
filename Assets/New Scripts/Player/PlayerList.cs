///
/// Created by Alex Fischer | May 2024
/// 

using Unity.Netcode;
using UnityEngine;

/// <summary>
/// The main player spawn system, intended to spawn player brains
/// </summary>
public class PlayerList : SingletonMonobehaviour<PlayerList>
{
    [SerializeField] PlayerSpawnSystem playerSpawnSystem;
    [SerializeField] CharacterInformationSO[] characters;
    public CharacterInformationSO[] Characters { get { return characters; } }

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
    public PlayerMain SpawnCharacterBody(GenericBrain brain, int characterID)
    {
        CharacterInformationSO characterInfo = characters[characterID];

        GameObject character = Instantiate(characterInfo.GetCharacterGameobject(), 
            spawnPositions[spawnedPlayerCount++].transform.position, Quaternion.identity);

        // Spawns on network
        character.GetComponent<NetworkObject>().Spawn();

        character.transform.parent = bodyParent.transform;

        PlayerMain playerMain = character.GetComponent<PlayerMain>();

        playerSpawnSystem.AddPlayerBody(brain, playerMain);

        return playerMain;
    }

    /// <summary>
    /// Removes the player body from the scene
    /// </summary>
    /// <param name="body">The body to remove</param>
    public void DeletePlayerBody(GenericBrain brain, PlayerMain body)
    {
        playerSpawnSystem.DeletePlayerBody(brain);
        Destroy(body.gameObject);
        spawnedPlayerCount--;
    }
}

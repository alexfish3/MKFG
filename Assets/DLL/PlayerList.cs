using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerList : SingletonMonobehaviour<PlayerList>
{
    [SerializeField] PlayerSpawnSystem playerSpawnSystem;
    [SerializeField] GameObject[] characters;
    [SerializeField] GameObject bodyParent;

    int counter;
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
        GameObject character = Instantiate(characters[characterID], spawnPositions[counter++].transform.position, Quaternion.identity);
        character.transform.parent = bodyParent.transform;

        PlayerMain playerMain = character.GetComponent<PlayerMain>();

        playerSpawnSystem.AddPlayerBody(playerMain);

        return playerMain;
    }
}

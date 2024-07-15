///
/// Created by Alex Fischer | May 2024
/// 

using System.Collections.Generic;
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

    [HideInInspector] public List<Transform> uiArrows;

    /// <summary>
    /// Spawns Character Body based on ID of character
    /// </summary>
    /// <param name="characterID"></param>
    /// <returns>
    /// Returns Player Main script on body
    /// </returns>
    public PlayerMain SpawnCharacterBody(GenericBrain brain, int characterID)
    {
        if (spawnedPlayerCount >= playerSpawnSystem.GetMaxPlayerCount())
            return null;

        CharacterInformationSO characterInfo = characters[characterID];

        GameObject character = Instantiate(characterInfo.GetCharacterGameobject(), Vector3.zero, Quaternion.identity);

        character.transform.parent = bodyParent.transform;

        PlayerMain playerMain = character.GetComponent<PlayerMain>();

        playerSpawnSystem.AddPlayerBody(brain, playerMain);

        uiArrows.Add(playerMain.GetArrowPosition());

        return playerMain;
    }

    /// <summary>
    /// Removes the player body from the scene
    /// </summary>
    /// <param name="body">The body to remove</param>
    public void DeletePlayerBody(GenericBrain brain, PlayerMain body)
    {
        playerSpawnSystem.DeletePlayerBody(brain);
        uiArrows.Remove(body.GetArrowPosition());
        Destroy(body.gameObject);
        spawnedPlayerCount--;
    }
}
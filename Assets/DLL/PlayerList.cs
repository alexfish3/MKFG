using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerList : SingletonMonobehaviour<PlayerList>
{
    [SerializeField] GameObject[] characters;
    [SerializeField] GameObject bodyParent;

    int counter;
    [SerializeField] GameObject[] spawnPositions;

    public PlayerMain SpawnCharacterBody(int characterID)
    {
        GameObject character = Instantiate(characters[characterID], spawnPositions[counter++].transform.position, Quaternion.identity);
        character.transform.parent = bodyParent.transform;

        PlayerMain playerMain = character.GetComponent<PlayerMain>();
        return playerMain;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInformation", menuName = "Character Information", order = 1)]
public class CharacterInformationSO : ScriptableObject
{
    [Header("Character Information")]
    [SerializeField] string characterName;
    [SerializeField] GameObject characterGameobject;
    [SerializeField] Sprite characterSelectHeadshot;

    /// <summary>
    /// Returns the character name from the information
    /// </summary>
    /// <returns>The character's name as a string</returns>
    public string GetCharacterName()
    {
        return characterName;
    }


    /// <summary>
    /// Returns the character gameobject from the information
    /// </summary>
    /// <returns>The character gameobejct to be initalized</returns>
    public GameObject GetCharacterGameobject()
    {
        return characterGameobject;
    }

    /// <summary>
    /// Returns the character select headshot sprite
    /// </summary>
    /// <returns>The headshot sprite of the player</returns>
    public Sprite GetCharacterSelectHeadshot()
    {
        return characterSelectHeadshot;
    }

}

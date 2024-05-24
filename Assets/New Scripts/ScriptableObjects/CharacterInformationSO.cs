using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInformation", menuName = "Character Information", order = 1)]
public class CharacterInformationSO : ScriptableObject
{
    [Header("Character Information")]
    [SerializeField] string characterName;
    [SerializeField] GameObject characterGameobject;

    /// <summary>
    /// Returns the character gameobject from the information
    /// </summary>
    /// <returns>The character gameobejct to be initalized</returns>
    public GameObject GetCharacterGameobject()
    {
        return characterGameobject;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterSelectorNametag : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text characterName;

    public void SetPlayerName(string newPlayerName)
    {
        playerName.text = newPlayerName;
    }

    public void SetCharacterName(string newCharacterName)
    {
        characterName.text = newCharacterName;
    }
}

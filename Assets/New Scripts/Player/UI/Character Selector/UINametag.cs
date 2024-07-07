using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UINametag : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text characterName;
    [SerializeField] TMP_Text mapName;

    public void SetPlayerName(string newPlayerName)
    {
        playerName.gameObject.SetActive(true);
        characterName.gameObject.SetActive(true);
        mapName.gameObject.SetActive(false);
        playerName.text = newPlayerName;
    }

    public void SetCharacterName(string newCharacterName)
    {
        playerName.gameObject.SetActive(true);
        characterName.gameObject.SetActive(true);
        mapName.gameObject.SetActive(false);
        characterName.text = newCharacterName;
    }

    public void SetMapName(string newMapName)
    {
        playerName.gameObject.SetActive(false);
        characterName.gameObject.SetActive(false);
        mapName.gameObject.SetActive(true);
        mapName.text = newMapName;
    }
}

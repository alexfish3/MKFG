using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UINametag : MonoBehaviour
{
    [Header("Images")]
    [SerializeField] Image background;
    [SerializeField] Image characterIcon;
    [SerializeField] Image inputIcon;
    [SerializeField] Sprite[] inputIconTypes;

    [Header("Text")]
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text characterName;
    [SerializeField] TMP_Text mapName;

    public void Initalize(GenericBrain genericBrain)
    {
        SetInputIcon(genericBrain.GetBrainInputType());
        SetPlayerName("Player " + (genericBrain.GetPlayerID() + 1).ToString());
    }

    public void SetBackgroundColor(Color color)
    {
        background.color = color;
    }

    public void SetCharacterIcon(Sprite sprite)
    {
        characterIcon.gameObject.SetActive(true);
        characterIcon.sprite = sprite;
    }

    public void SetInputIcon(InputType brainInputType)
    {
        inputIcon.gameObject.SetActive(true);
        inputIcon.sprite = inputIconTypes[(int)brainInputType]; ;
    }

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
        characterIcon.gameObject.SetActive(false);
        inputIcon.gameObject.SetActive(false);
        playerName.gameObject.SetActive(false);
        characterName.gameObject.SetActive(false);
        mapName.gameObject.SetActive(true);
        mapName.text = newMapName;
    }
}

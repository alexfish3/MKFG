///
/// Created by Alex Fischer | July 2024
/// 

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UINametag : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] bool teamsOn;
    [SerializeField] Animator animator;

    [Header("Transform Positions")]
    public Transform teamSelectorTransform;
    public Transform playerTagSelectTransform;

    [Header("Images")]
    [SerializeField] Image background;
    [SerializeField] Image characterIcon;
    [SerializeField] Image inputIcon;
    [SerializeField] Sprite[] inputIconTypes;
    [SerializeField] Image teamColor;

    [Header("Text")]
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text characterName;
    [SerializeField] TMP_Text mapName;

    public void Initalize(GenericBrain genericBrain, bool isSolo)
    {
        SetInputIcon(genericBrain.GetBrainInputType());
        SetPlayerName("Player " + (genericBrain.GetPlayerID() + 1).ToString());

        // If intalizing into teams, set the trigger to show teams select instantly
        if(isSolo == false)
        {
            animator.SetTrigger("TeamsEnabledOnSpawn");
            animator.SetBool("Reveal Teams", true);
        }
    }

    /// <summary>
    /// Sets the nametag's background color, also sets the team color
    /// </summary>
    /// <param name="color">The color the background will become</param>
    public void SetBackgroundColor(Color color)
    {
        background.color = color;
        teamColor.color = color;
    }

    /// <summary>
    /// Sets the display icon to be the current selected player
    /// </summary>
    /// <param name="sprite">The sprite the icon will set itself to</param>
    public void SetCharacterIcon(Sprite sprite)
    {
        characterIcon.gameObject.SetActive(true);
        characterIcon.sprite = sprite;
    }

    /// <summary>
    /// Sets the input icon on the nametag, can either be keyboard or controller
    /// </summary>
    /// <param name="brainInputType">The brain type, either being keyboard or controller</param>
    public void SetInputIcon(InputType brainInputType)
    {
        inputIcon.gameObject.SetActive(true);
        inputIcon.sprite = inputIconTypes[(int)brainInputType]; ;
    }

    /// <summary>
    /// Sets the player's name on the nametag
    /// </summary>
    /// <param name="newPlayerName">The new player name to be displayed</param>
    public void SetPlayerName(string newPlayerName)
    {
        playerName.gameObject.SetActive(true);
        characterName.gameObject.SetActive(true);
        mapName.gameObject.SetActive(false);
        playerName.text = newPlayerName;
    }

    /// <summary>
    /// Sets the character's name on the nametag
    /// </summary>
    /// <param name="newCharacterName">The character's name to be displayed</param>
    public void SetCharacterName(string newCharacterName)
    {
        playerName.gameObject.SetActive(true);
        characterName.gameObject.SetActive(true);
        mapName.gameObject.SetActive(false);
        characterName.text = newCharacterName;
    }

    /// <summary>
    /// Sets the map's name on the nametag
    /// </summary>
    /// <param name="newMapName">The map's name to be displayed</param>
    public void SetMapName(string newMapName)
    {
        characterIcon.gameObject.SetActive(false);
        inputIcon.gameObject.SetActive(false);
        playerName.gameObject.SetActive(false);
        characterName.gameObject.SetActive(false);
        mapName.gameObject.SetActive(true);
        mapName.text = newMapName;
    }

    /// <summary>
    /// Toggles the team select ui above the nametag to slide in or out
    /// </summary>
    /// <param name="isSolo">The bool to determine if the ui should slide out or in</param>
    public void ToggleTeamSelect(bool isSolo)
    {
        animator.SetBool("Reveal Teams", !isSolo);
        teamsOn = !isSolo;
    }
}

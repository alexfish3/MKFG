///
/// Created by Alex Fischer | July 2024
/// 

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectorGameobject : MonoBehaviour
{
    [Header("Status")]
    public int playerID;
    public int deviceID;

    [SerializeField] int selectedPositionID;
        public int GetSelectedPositionID() { return selectedPositionID; }

    [SerializeField] bool confirmed = false;
        public bool GetConfirmedStatus() { return confirmed; }

    [SerializeField] CharacterSelectionPosition characterSelectionPosition = CharacterSelectionPosition.Characters;
        public void SetSelectedPlayersSelection(CharacterSelectionPosition newPosition) { characterSelectionPosition = newPosition; }
        public CharacterSelectionPosition GetSelectedPlayersSelection() { return characterSelectionPosition; }

    [Header("References")]
    [SerializeField] Image selectorImage;
    [SerializeField] TMP_Text playerText;
    [SerializeField] Color originalColor;
    [SerializeField] UINametag selectorNametag;
        public UINametag GetSelectorNametag() { return selectorNametag; }

    [SerializeField] Vector2 offsetPosition;
    public void SetOffsetPosition(Vector2 offset)
    {
        if(offsetPosition.x != 0 && offsetPosition.y != 0)
        {
            this.transform.position = new Vector3(transform.position.x - offsetPosition.x, transform.position.y - offsetPosition.y);

        }

        offsetPosition = offset;
        this.transform.position = new Vector3(transform.position.x + offsetPosition.x, transform.position.y + offsetPosition.y);
    }

    [SerializeField] Vector3 defaultScale;
    [SerializeField] Vector3 selectedScale;

    /// <summary>
    /// Initalizes the character selector
    /// </summary>
    /// <param name="newPlayerID">The id of the player connected to the selector</param>
    /// <param name="selectorColor">The color of the selector</param>
    /// <param name="newDeviceID">The id of the device connected to the selector</param>
    /// <param name="newSelectorNametag">The nametag to be connected to the selector</param>
    public void Initialize(int newPlayerID, Color selectorColor, int newDeviceID, UINametag newSelectorNametag)
    {
        playerID = newPlayerID;
        deviceID = newDeviceID;
        selectorNametag = newSelectorNametag;

        originalColor = selectorColor;
        selectorImage.color = originalColor;

        playerText.text = "P" + (playerID + 1).ToString();
    }

    /// <summary>
    /// Starts the selector at the default position
    /// </summary>
    /// <param name="defaultPos">The gameobject of the default pos</param>
    public void SetDefaultPosition(CharacterInformationSO characterInfo, GameObject defaultPos) // Sets the defualt position for character icons
    {
        selectedPositionID = 0;

        Vector3 newPos = new Vector3(defaultPos.transform.position.x + offsetPosition.x, defaultPos.transform.position.y + offsetPosition.y, defaultPos.transform.position.z);
        this.gameObject.transform.position = newPos;

        selectorNametag.SetCharacterName(characterInfo.GetCharacterName());
        selectorNametag.SetCharacterIcon(characterInfo.GetCharacterSelectHeadshot());
    }
    public void SetDefaultPosition(MapInformationSO mapInfo, GameObject defaultPos) // Sets the defualt position for map icons
    {
        selectedPositionID = 0;
        this.gameObject.transform.position = defaultPos.transform.position;

        selectorNametag.SetMapName(mapInfo.GetMapName());
    }

    /// <summary>
    /// Sets the selector's position to the character icons, and also sets the character id to match
    /// </summary>
    /// <param name="positionID">The ID of the new position to move the selector</param>
    /// <param name="characterInfo">The information about the currently selected character</param>
    /// <param name="characterIcon">The character icon gameobject currently selected</param>
    /// <param name="playerNumOffset">An extra offset to apply for the character select</param>
    public void SetSelectorPosition(int positionID, CharacterInformationSO characterInfo, GameObject characterIcon) // Used for scrolling character icons
    {
        // If player is confirmed, return
        if (confirmed == true)
            return;

        Vector3 newPos = new Vector3(characterIcon.transform.position.x + offsetPosition.x, characterIcon.transform.position.y + offsetPosition.y, characterIcon.transform.position.z);
        this.gameObject.transform.position = newPos;

        selectedPositionID = positionID;

        selectorNametag.SetCharacterName(characterInfo.GetCharacterName());
        selectorNametag.SetCharacterIcon(characterInfo.GetCharacterSelectHeadshot());

        this.gameObject.transform.SetParent(characterIcon.transform);
    }
    public void SetSelectorPosition(int positionID, MapInformationSO characterInfo, GameObject mapIcon) // Used for scrolling maps
    {
        // If player is confirmed, return
        if (confirmed == true)
            return;

        this.gameObject.transform.position = mapIcon.transform.position;
        selectedPositionID = positionID;

        selectorNametag.SetMapName(characterInfo.GetMapName());
    }
    public void SetSelectorPosition(Transform newPos) // USed to scroll other parts of character menu
    {
        // If player is confirmed, return
        if (confirmed == true)
            return;

        this.gameObject.transform.position = newPos.transform.position;
        this.gameObject.transform.SetParent(newPos);
    }

    /// <summary>
    /// Sets the selector's position to the new pos and also sets the selector parent
    /// </summary>
    /// <param name="positionID">The position id for the array of scrolled objects</param>
    /// <param name="newPos">The new position as a vector3</param>
    /// <param name="parent">The parent object the selector will set its parent to</param>
    public void SetSelectorPositionAndParent(int positionID, Vector3 newPos, GameObject parent)
    {
        // If player is confirmed, return
        if (confirmed == true)
            return;

        this.gameObject.transform.position = newPos;
        selectedPositionID = positionID;

        this.gameObject.transform.SetParent(parent.transform);
        this.transform.localScale = Vector3.one;
    }

    public void SetSelectorStatus(bool selectorStatus)
    {
        // Confirm selector
        if(selectorStatus == true)
        {
            confirmed = true;
            this.transform.localScale = selectedScale;
            selectorImage.color = new Color(originalColor.r - 0.4f, originalColor.g - 0.4f, originalColor.b - 0.4f);
        }
        else
        {
            confirmed = false;
            this.transform.localScale = defaultScale;
            selectorImage.color = originalColor;
        }
    }

}

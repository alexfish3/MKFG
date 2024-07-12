using System.Collections;
using System.Collections.Generic;
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

    [Header("References")]
    [SerializeField] Image selectorImage;
    [SerializeField] Color originalColor;
    [SerializeField] Sprite[] playerSprites;

    [SerializeField] UINametag selectorNametag;
        public UINametag GetSelectorNametag() { return selectorNametag; }

    [SerializeField] Vector3 defaultScale;
    [SerializeField] Vector3 selectedScale;

    public void Initialize(int newPlayerID, Color selectorColor, int newDeviceID, UINametag newSelectorNametag)
    {
        playerID = newPlayerID;
        deviceID = newDeviceID;
        selectorNametag = newSelectorNametag;

        originalColor = selectorColor;
        selectorImage.color = originalColor;

        selectorImage.sprite = playerSprites[newPlayerID];
    }

    /// <summary>
    /// Starts the selector at the default position
    /// </summary>
    /// <param name="defaultPos">The gameobject of the default pos</param>
    public void SetDefaultPosition(CharacterInformationSO characterInfo, GameObject defaultPos)
    {
        selectedPositionID = 0;
        this.gameObject.transform.position = defaultPos.transform.position;

        selectorNametag.SetCharacterName(characterInfo.GetCharacterName());
        selectorNametag.SetCharacterIcon(characterInfo.GetCharacterSelectHeadshot());
    }
    public void SetDefaultPosition(MapInformationSO mapInfo, GameObject defaultPos)
    {
        selectedPositionID = 0;
        this.gameObject.transform.position = defaultPos.transform.position;

        selectorNametag.SetMapName(mapInfo.GetMapName());
    }

    /// <summary>
    /// Sets the selector's position to the character icons, and also sets the character id to match
    /// </summary>
    /// <param name="characterIcon">The ui position of the selected character icon</param>
    /// <param name="newSelectorPosition">The selector position int</param>
    public void SetSelectorPosition(int positionID, CharacterInformationSO characterInfo, GameObject characterIcon)
    {
        // If player is confirmed, return
        if (confirmed == true)
            return;

        this.gameObject.transform.position = characterIcon.transform.position;
        selectedPositionID = positionID;

        selectorNametag.SetCharacterName(characterInfo.GetCharacterName());
        selectorNametag.SetCharacterIcon(characterInfo.GetCharacterSelectHeadshot());
    }
    public void SetSelectorPosition(int positionID, MapInformationSO characterInfo, GameObject mapIcon)
    {
        // If player is confirmed, return
        if (confirmed == true)
            return;

        this.gameObject.transform.position = mapIcon.transform.position;
        selectedPositionID = positionID;

        selectorNametag.SetMapName(characterInfo.GetMapName());
    }
    public void SetSelectorPosition(int positionID, GameObject newPos)
    {
        // If player is confirmed, return
        if (confirmed == true)
            return;

        this.gameObject.transform.position = newPos.transform.position;
        selectedPositionID = positionID;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectorGameobject : MonoBehaviour
{
    [SerializeField] bool confirmed = false;
    public bool GetConfirmedStatus() { return confirmed; }

    [SerializeField] int selectedPositionID;
    public int GetSelectedPositionID() { return selectedPositionID; } 

    [SerializeField] Image SelectorImage;
    [SerializeField] Sprite[] playerSprites;

    [SerializeField] UINametag selectorNametag;

    public int playerID;
    public int deviceID;

    public void Initialize(int newPlayerID, int newDeviceID, UINametag newSelectorNametag)
    {
        playerID = newPlayerID;
        deviceID = newDeviceID;
        selectorNametag = newSelectorNametag;

        // Change selector colors
        switch (newPlayerID)
        {
            case 0:
                SelectorImage.color = Color.red;
                break;
            case 1:
                SelectorImage.color = Color.blue;
                break;
            case 2:
                SelectorImage.color = Color.green;
                break;
            case 3:
                SelectorImage.color = Color.yellow;
                break;
            default:
                SelectorImage.color = Color.red;
                break;
        }

        SelectorImage.sprite = playerSprites[newPlayerID];
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
    public void SetSelectorPosition(int characterID, CharacterInformationSO characterInfo, GameObject characterIcon)
    {
        // If player is confirmed, return
        if (confirmed == true)
            return;

        this.gameObject.transform.position = characterIcon.transform.position;
        selectedPositionID = characterID;

        selectorNametag.SetCharacterName(characterInfo.GetCharacterName());
    }
    public void SetSelectorPosition(int characterID, MapInformationSO characterInfo, GameObject mapIcon)
    {
        // If player is confirmed, return
        if (confirmed == true)
            return;

        this.gameObject.transform.position = mapIcon.transform.position;
        selectedPositionID = characterID;

        selectorNametag.SetMapName(characterInfo.GetMapName());
    }
    public void SetSelectorPosition(int characterID, GameObject newPos)
    {
        // If player is confirmed, return
        if (confirmed == true)
            return;

        this.gameObject.transform.position = newPos.transform.position;
        selectedPositionID = characterID;
    }

    public void SetSelectorStatus(bool selectorStatus)
    {
        // Confirm selector
        if(selectorStatus == true)
        {
            confirmed = true;
            this.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            confirmed = false;
            this.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectorGameobject : MonoBehaviour
{
    [SerializeField] bool confirmed = false;
    public bool GetConfirmedStatus() { return confirmed; }

    [SerializeField] Image SelectorImage;
    [SerializeField] Sprite[] playerSprites;

    [SerializeField] CharacterSelectorNametag selectorNametag;

    public int playerID;
    public int deviceID;

    public int selectorPosition = 0;

    public void Initialize(int newPlayerID, int newDeviceID, CharacterSelectorNametag newSelectorNametag)
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
    public void SetDefaultPosition(GameObject defaultPos)
    {
        selectorPosition = 0;
        this.gameObject.transform.position = defaultPos.transform.position;
    }

    /// <summary>
    /// Sets the selector's position to the character icons, and also sets the character id to match
    /// </summary>
    /// <param name="characterIcon">The ui position of the selected character icon</param>
    /// <param name="newSelectorPosition">The selector position int</param>
    public void SetSelectorPosition(GameObject characterIcon, int newSelectorPosition)
    {
        // If player is confirmed, return
        if (confirmed == true)
            return; 

        selectorPosition = newSelectorPosition;
        this.gameObject.transform.position = characterIcon.transform.position;
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

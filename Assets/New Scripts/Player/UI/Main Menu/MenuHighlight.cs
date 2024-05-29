using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuHighlight : MonoBehaviour
{
    [SerializeField] Image SelectorImage;
    
    public int selectorPosition = 0;

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
        selectorPosition = newSelectorPosition;
        this.gameObject.transform.position = characterIcon.transform.position;
    }
}

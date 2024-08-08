using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ControlsReassignController : MonoBehaviour
{
    [SerializeField]
    private Button[] listOfPrimaryButtons;

    // Private variables
    private InputProfileSO inputProfileToAdjust;
    private bool listeningForKey;

    // Getters/Setters
    public InputProfileSO InputProfileToAdjust { get { return inputProfileToAdjust; } set { inputProfileToAdjust = value; } }

    public enum InputToChange
    {
        None,
        Up,
        Down,
        Left,
        Right,
        Pause,
        Attack,
        Special,
        Drift,
        Drive,
        Reverse,
        Reflect_Camera
    };

    public enum PrimaryOrSecondary
    {
        Primary,
        Secondary,
        None
    };

    public enum ControllerOrKeyboard
    {
        Controller,
        Keyboard
    };

    [HideInInspector]
    public ControllerOrKeyboard inputType; //What the player is using to remap

    private void OnEnable()
    {
        listeningForKey = false;
    }

    private void Awake()
    {
        inputType = ControllerOrKeyboard.Controller;
    }

    // Primary button clicked
    public void OnButtonClick(int buttonSelected)
    {
        buttonSelected++;
        CallForRebind((InputToChange) buttonSelected);
    }

    //This method is called when the button is clicked and calls the settingMenuUi script to listen for player input
    private void CallForRebind(InputToChange input)
    {
        if (inputProfileToAdjust == null) return;

        // Capture the next key/controller button the player presses
        listeningForKey = true;

        SettingsMenuUI.Instance.GetRebindOption(this);
        
        listOfPrimaryButtons[SettingsMenuUI.Instance.ButtonSelector.selectorPosition].GetComponentInChildren<TextMeshProUGUI>().text = "Listening";
        SettingsMenuUI.Instance.ListeningForInput = true;
    }

    //This method replaces the previous control action with the player input from settingMenuUI
    public void SetRebindKey(string key)
    {
        StartCoroutine(Wait()); //Wait a bit before the player can move through the setting buttons to ensure no accidents

        if (inputProfileToAdjust == null) return;

        int selectorPosition = SettingsMenuUI.Instance.ButtonSelector.selectorPosition;
        TextMeshProUGUI textToChange = listOfPrimaryButtons[selectorPosition].GetComponentInChildren<TextMeshProUGUI>();
        //If controller then change controller bindings
        if (inputType == ControllerOrKeyboard.Controller)
        {
            if (InputProfileToAdjust.controllerInputs[selectorPosition].actionName == key)
            {
                textToChange.text = key;
                listeningForKey = false;
                return;
            }

            InputProfileToAdjust.controllerInputs[selectorPosition].actionName = key;
            textToChange.text = key;
        }
        else //Vice versa
        {
            if (InputProfileToAdjust.keyboardInputs[selectorPosition].keycode == key)
            {
                textToChange.text = key;
                listeningForKey = false;
                return;
            }

            Debug.LogWarning(key + " at button position: " + selectorPosition);
            InputProfileToAdjust.keyboardInputs[selectorPosition].keycode = key;
            textToChange.text = key;
        }

        ButtonHadBeenRemapped(key);
    }

    //This method waits before allowing the player to move between button in the canvas
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.05f);
        SettingsMenuUI.Instance.ListeningForInput = false;
    }

    //This method is called on all instances of this script and helps find any duplicate actions
    private void ButtonHadBeenRemapped(string remapKey)
    {
        for (int i = 0; i < listOfPrimaryButtons.Length; i++)
        {
            Debug.Log(i + " vs " + SettingsMenuUI.Instance.ButtonSelector.selectorPosition);
            // If not the original button
            if (i != SettingsMenuUI.Instance.ButtonSelector.selectorPosition && InputProfileToAdjust != null && remapKey != String.Empty)
            {
                TextMeshProUGUI textToChange = listOfPrimaryButtons[i].GetComponentInChildren<TextMeshProUGUI>();

                switch (inputType) 
                {
                    // If controller remove duplicates and ensure all other actions are properly named
                    case ControllerOrKeyboard.Controller:
                        if (InputProfileToAdjust.controllerInputs[i].actionName == remapKey)
                        {
                            textToChange.text = "";
                            InputProfileToAdjust.controllerInputs[i].actionName = "";
                        }
                        break;

                    // If keyboard remove duplicates and ensure all other actions are properly named
                    case ControllerOrKeyboard.Keyboard:
                        if (InputProfileToAdjust.keyboardInputs[i].keycode == remapKey)
                        {
                            //Debug.LogWarning("Keyboard key: " + InputProfileToAdjust.keyboardInputs[i].keycode + " Matches " + remapKey + " at button position: " + i);
                            textToChange.text = "";
                            InputProfileToAdjust.keyboardInputs[i].keycode = "";
                        }
                        break;
                }
            }
        }
    }

    public void InputProfileSet(int inputProfileIndex)
    {
        // Safety check
        if (SettingsMenuUI.Instance.InputProfiles[inputProfileIndex] == null) return;

        // Set profile to Adjust
        inputProfileToAdjust = SettingsMenuUI.Instance.InputProfiles[inputProfileIndex];

        TextMeshProUGUI textToAdjust = listOfPrimaryButtons[inputProfileIndex].GetComponentInChildren<TextMeshProUGUI>();
    }

    public void InputProfileHover(int inputProfileHoveredIndex)
    {
        // Safety check
        if (SettingsMenuUI.Instance.InputProfiles[inputProfileHoveredIndex] == null) return;

        // Set profile to Adjust
        inputProfileToAdjust = SettingsMenuUI.Instance.InputProfiles[inputProfileHoveredIndex];

        for (int i = 0; i < listOfPrimaryButtons.Length; i++)
        {
            TextMeshProUGUI textToAdjust = listOfPrimaryButtons[i].GetComponentInChildren<TextMeshProUGUI>();

            //If controller then loop through all input names of the controls and write all actions to the text
            if (inputType == ControllerOrKeyboard.Controller)
                textToAdjust.text = inputProfileToAdjust.controllerInputs[i].actionName;
            else //If keyboard then loop through all input names of the controls and write all actions to the text
                textToAdjust.text = inputProfileToAdjust.keyboardInputs[i].keycode;
        }
    }
}

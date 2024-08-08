using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class ControlsReassignController : MonoBehaviour
{
    // Private variables
    private InputProfileSO inputProfileToAdjust;

    // Getters/Setters
    public InputProfileSO InputProfileToAdjust { get { return inputProfileToAdjust; } set { inputProfileToAdjust = value; } }

    //public InputProfileSO inputProfile;
    private TextMeshProUGUI text;
    private Button button;

    private SettingsMenuUI settingMenu;

    public bool listeningForKey;

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

    public InputToChange inputToChange;

    private int controllerInputPos;

    public enum PrimaryOrSecondary
    {
        Primary,
        Secondary
    };

    public PrimaryOrSecondary type;

    public enum ControllerOrKeyboard
    {
        Controller,
        Keyboard
    };

    [HideInInspector]
    public ControllerOrKeyboard inputType; //What the player is using to remap


    //Event to tell all other instances of this script that a button has been remapped. Used to find duplicates
    public static event Action<string> onButtonRemapped;
    public static void OnButtonRemapped(string remapKey)
    {
        onButtonRemapped?.Invoke(remapKey);
    }


    private void OnEnable()
    {
        controllerInputPos = 0;
        listeningForKey = false;
    }

    private void OnDestroy()
    {
        onButtonRemapped -= ButtonHadBeenRemapped;
    }

    private void Awake()
    {
        inputType = ControllerOrKeyboard.Controller;

        settingMenu = GetComponentInParent<SettingsMenuUI>();

        onButtonRemapped += ButtonHadBeenRemapped;

        //inputProfileToAdjust = inputProfile; //Used for testing delete or comment out when setting profile is being used or tested

        text = GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponent<Button>();

        if(type == PrimaryOrSecondary.Primary)
        {
            button.onClick.AddListener(CallForRebind);
        }
        else
        {

        }

    }

    //This method is called when the button is clicked and calls the settingMenuUi script to listen for player input
    public void CallForRebind()
    {
        if (inputProfileToAdjust == null) return;

        // Capture the next key/controller button the player presses
        listeningForKey = true;
        settingMenu.GetRebindOption(this);
        text.text = "Listening";
        settingMenu.ListeningForInput = true;

        // Check if key/button exists for another command
        // If yes remove the other key?

        // Change assignments and save to profile

        // Change button text to become 
    }

    //This method replaces the previous control action with the player input from settingMenuUI
    public void SetRebindKey(string key)
    {
        StartCoroutine(Wait()); //Wait a bit before the player can move through the setting buttons to ensure no accidents

        if (inputProfileToAdjust == null) return;


        //If controller then change controller bindings
        if (inputType == ControllerOrKeyboard.Controller)
        {
            if (InputProfileToAdjust.controllerInputs[controllerInputPos].actionName == key)
            {
                text.text = key;
                return;
            }

            InputProfileToAdjust.controllerInputs[controllerInputPos].actionName = key;
            text.text = key;
        }
        else //Vice versa
        {
            if (InputProfileToAdjust.keyboardInputs[controllerInputPos].keycode == key)
            {
                text.text = key;
                return;
            }

            InputProfileToAdjust.keyboardInputs[controllerInputPos].keycode = key;
            text.text = key;
        }

        //Invoke the event action to tell all other instances of this script that a button has been remapped
        OnButtonRemapped(key);
    }


    //This method waits before allowing the player to move between button in the canvas
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.05f);
        settingMenu.ListeningForInput = false;
    }

    //This method is called on all instances of this script and helps find any duplicate actions
    private void ButtonHadBeenRemapped(string remapKey)
    {
        //If checking the changed button then return since no need
        if (listeningForKey)
        {
            listeningForKey = false;
            return;
        }
        else
        {

            if (InputProfileToAdjust != null && remapKey != String.Empty)
            {
                //If controller remove duplicates and ensure all other actions are properly named
                if (inputType == ControllerOrKeyboard.Controller)
                {
                    if (InputProfileToAdjust.controllerInputs[controllerInputPos].actionName == remapKey)
                    {
                        text.text = "";
                        InputProfileToAdjust.controllerInputs[controllerInputPos].actionName = "";
                    }
                    else
                    {
                        text.text = InputProfileToAdjust.controllerInputs[controllerInputPos].actionName;
                    }
                }
                else //If keyboard remove duplicates and ensure all other actions are properly named
                {
                    if (InputProfileToAdjust.keyboardInputs[controllerInputPos].keycode == remapKey)
                    {
                        text.text = "";
                        InputProfileToAdjust.keyboardInputs[controllerInputPos].keycode = "";
                    }
                    else
                    {
                        text.text = InputProfileToAdjust.keyboardInputs[controllerInputPos].keycode;
                    }
                }
            }
        }
    }

    public void InputProfileSet(InputProfileSO inputProfile)
    {
        // Safety check
        if (inputProfile == null) return;

        inputProfileToAdjust = inputProfile;

        if (InputProfileToAdjust != null)
        {
            //If controller then loop through all input names of the controls and write all actions to the text
            if (inputType == ControllerOrKeyboard.Controller)
            {
                for (int i = 0; i < InputProfileToAdjust.controllerInputs.Length; i++)
                {
                    if (InputProfileToAdjust.controllerInputs[i].inputName == inputToChange.ToString().Replace('_', ' '))
                    {
                        text.text = InputProfileToAdjust.controllerInputs[i].actionName;
                        controllerInputPos = i;
                        break;
                    }
                }
            }
            else //If keyboard then loop through all input names of the controls and write all actions to the text
            {
                for (int i = 0; i < InputProfileToAdjust.keyboardInputs.Length; i++)
                {
                    if (InputProfileToAdjust.keyboardInputs[i].inputName == inputToChange.ToString().Replace('_', ' '))
                    {
                        text.text = InputProfileToAdjust.keyboardInputs[i].keycode;
                        controllerInputPos = i;
                        break;
                    }
                }
            }
        }

        // Update UI

        ButtonHadBeenRemapped("Nothing");
    }
}

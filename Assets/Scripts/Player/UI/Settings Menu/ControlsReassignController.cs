using System;
using System.Collections;
using TMPro;
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

    private bool waitTillNext;

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

    public enum PrimaryorSecondary
    {
        Primary,
        Secondary
    };

    public PrimaryorSecondary type;

    public static event Action<string> onButtonRemapped;
    public static void OnButtonRemapped(string remapKey)
    {
        onButtonRemapped?.Invoke(remapKey);
    }


    private void OnEnable()
    {
        listeningForKey = false;
        waitTillNext = false;
    }

    private void OnDestroy()
    {
        onButtonRemapped -= ButtonHadBeenRemapped;
    }

    private void Awake()
    {
        settingMenu = GetComponentInParent<SettingsMenuUI>();

        onButtonRemapped += ButtonHadBeenRemapped;

        //inputProfileToAdjust = inputProfile; //Used for testing delete or comment out when setting profile is being used or tested

        text = GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponent<Button>();

        if(type == PrimaryorSecondary.Primary)
        {
            button.onClick.AddListener(CallForRebind);
        }
        else
        {

        }

    }

    public void CallForRebind()
    {
        if (inputProfileToAdjust == null) return;

        if (!waitTillNext)
        {
            // Capture the next key/controller button the player presses
            listeningForKey = true;
            waitTillNext = true;
            settingMenu.GetRebindOption(this);
            text.text = "Listening";
        }

        // Check if key/button exists for another command
        // If yes remove the other key?

        // Change assignments and save to profile

        // Change button text to become 
    }

    public void SetRebindKey(string key)
    {
        if (inputProfileToAdjust == null) return;

        if (InputProfileToAdjust.controllerInputs[controllerInputPos].actionName == key)
            return;

        InputProfileToAdjust.controllerInputs[controllerInputPos].actionName = key;
        text.text = key;

        OnButtonRemapped(key);
    }


    private void ButtonHadBeenRemapped(string remapKey)
    {
        if (listeningForKey)
        {
            listeningForKey = false;
            return;
        }
        else
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
    }

    public void InputProfileSet(InputProfileSO inputProfile)
    {
        // Safety check
        if (inputProfile == null) return;

        inputProfileToAdjust = inputProfile;


        if (InputProfileToAdjust != null)
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

        // Update UI

        ButtonHadBeenRemapped("Test");
    }
}

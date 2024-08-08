using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ControlsReassignController : MonoBehaviour
{
    // Private variables
    private InputProfileSO inputProfileToAdjust;

    // Getters/Setters
    public InputProfileSO InputProfileToAdjust { get { return inputProfileToAdjust; } set { inputProfileToAdjust = value; } }

    public InputProfileSO inputProfile;
    private TextMeshProUGUI text;
    private Button button;

    [SerializeField] SettingsMenuUI settingMenu;

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

    public enum PrimaryorSecondary
    {
        Primary,
        Secondary
    };

    public PrimaryorSecondary type;

    private void OnEnable()
    {
        listeningForKey = false;
    }

    private void Awake()
    {
        inputProfileToAdjust = inputProfile; //Used for testing delete or comment out when setting profile is being used or tested

        text = GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponent<Button>();

        for (int i = 0; i < InputProfileToAdjust.controllerInputs.Length; i++)
        {
            if (InputProfileToAdjust.controllerInputs[i].inputName == inputToChange.ToString().Replace('_', ' '))
            {
                text.text = InputProfileToAdjust.controllerInputs[i].actionName;
                controllerInputPos = i;
                break;
            }
        }

        if(type == PrimaryorSecondary.Primary)
        {
            //button.onClick.
        }
        else
        {

        }

    }

    public void CallForRebind()
    {
        if (inputProfileToAdjust == null) return;

        // Capture the next key/controller button the player presses
        listeningForKey = true;
        settingMenu.GetRebindOption(this);

        // Check if key/button exists for another command
        // If yes remove the other key?

        // Change assignments and save to profile

        // Change button text to become 
    }

    public void SetRebindKey(string key)
    {
        if (inputProfileToAdjust == null) return;

        for (int i = 0; i < InputProfileToAdjust.controllerInputs.Length; i++)
        {
            if (InputProfileToAdjust.controllerInputs[i].actionName == key)
            {
                text.text = InputProfileToAdjust.controllerInputs[i].actionName;
                controllerInputPos = i;
                break;
            }
        }

        InputProfileToAdjust.controllerInputs[controllerInputPos].actionName = key;

        listeningForKey = false;
    }

    public void InputProfileSet(InputProfileSO inputProfile)
    {
        // Safety check
        if (inputProfile == null) return;

        inputProfileToAdjust = inputProfile;

        // Update UI

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
}

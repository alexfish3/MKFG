using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InputProfile", menuName = "Input Profile", order = 0)]
public class InputProfile : ScriptableObject, ICloneable
{
    [Header("Profile Information")]
    [SerializeField] string profileName = "New Profile";
    [SerializeField] bool staticProfile = false;

    public class PlayerInputAction
    {
        public string inputName;
        public bool state = false;
        [Space(20)]
        public Action<bool> button;
    }

    [System.Serializable]
    public class KeyboardInputAction : PlayerInputAction
    {
        public char keycode = 'A';

        // Rebinds keycode
        public void SetKey(char newKey)
        {
            keycode = newKey;
        }

        public KeyboardInputAction(KeyboardInputAction copy)
        {
            inputName = copy.inputName;
            state = copy.state;
            keycode = copy.keycode;
        }
    }

    public KeyboardInputAction[] keyboardInputs;


    [System.Serializable]
    public class ControllerInputAction : PlayerInputAction
    {
        public string actionName = "";

        // Rebinds keycode
        public void SetKey(string newActionName)
        {
            actionName = newActionName;
        }

        public ControllerInputAction(ControllerInputAction copy)
        {
            inputName = copy.inputName;
            state = copy.state;
            actionName = copy.actionName;
        }
    }

    public ControllerInputAction[] controllerInputs;

    public object Clone()
    {
        InputProfile clonedProfile = new InputProfile();

        clonedProfile.profileName = profileName;
        clonedProfile.staticProfile = staticProfile;

        clonedProfile.keyboardInputs = new KeyboardInputAction[keyboardInputs.Length];
        clonedProfile.controllerInputs = new ControllerInputAction[controllerInputs.Length];

        for (int i = 0; i < keyboardInputs.Length; i++)
        {
            clonedProfile.keyboardInputs[i] = new KeyboardInputAction(keyboardInputs[i]);
        }

        for (int i = 0; i < controllerInputs.Length; i++)
        {
            clonedProfile.controllerInputs[i] = new ControllerInputAction(controllerInputs[i]);
        }

        return clonedProfile;
    }
}

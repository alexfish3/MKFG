///
/// The input profile scriptable object to be used to save controller keybind data
/// Created by Alex Fischer | May 2024
/// 

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InputProfile", menuName = "Input Profile", order = 0)]
public class InputProfile : ScriptableObject, ICloneable
{
    [Header("Profile Information")]
    [SerializeField] string profileName = "New Profile";
    [SerializeField] bool staticProfile = false;

    /// <summary>
    /// Base class that keeps track of universal (keyboard and controller) input info
    /// </summary>
    public class PlayerInputAction
    {
        public string inputName;
        public bool state = false;
        [Space(20)]
        public Action<bool> button;
    }

    /// <summary>
    /// Calls upon base input action to use it for keyboard spesifics
    /// </summary>
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

    /// <summary>
    /// Calls upon base input action to use it for controller spesifics
    /// </summary>
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

    /// <summary>
    /// Clones the input profile and returns the deep cloned object
    /// </summary>
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

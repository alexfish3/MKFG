///
/// The input profile scriptable object to be used to save controller keybind data
/// Created by Alex Fischer | May 2024
/// 

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InputProfile", menuName = "Input Profile", order = 0)]
public class InputProfileSO : ScriptableObject
{
    [Header("Profile Information")]
    [SerializeField] string profileName = "New Profile";
    public ControlType controlType;
    [SerializeField] bool staticProfile = false;

    public enum ControlType
    {
        UI = 0,
        Player = 1
    }

    /// <summary>
    /// Base class that keeps track of universal (keyboard and controller) input info
    /// </summary>
    public class PlayerInputAction
    {
        public string inputName;
        public Action<bool> button;
    }

    /// <summary>
    /// Calls upon base input action to use it for keyboard spesifics
    /// </summary>
    [System.Serializable]
    public class KeyboardInputAction : PlayerInputAction
    {
        public int keycode = 'A';

        // Rebinds keycode
        public void SetKey(int newKey)
        {
            keycode = newKey;
        }

        public KeyboardInputAction(KeyboardInputAction copy)
        {
            inputName = copy.inputName;
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
            actionName = copy.actionName;
        }
    }

    public ControllerInputAction[] controllerInputs;
}

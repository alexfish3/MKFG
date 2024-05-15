using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InputProfile", menuName = "Input Profile", order = 0)]
public class InputProfile : ScriptableObject
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
    }

    public ControllerInputAction[] controllerInputs;
}

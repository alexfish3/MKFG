///
/// Parses raw input data from dll into useable format in unity 
/// Original concept by PeterSvP on Pastebin (https://pastebin.com/u/PeterSvP)
/// Modified by Alex Fischer for use with multi-keyboard | May 2024
/// 

using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// The keyboard input manager handles everything related to parsing keyboard inputs
/// </summary>
public class DLLInputManager : GenericInputManager
{
    [DllImport("RawInput")] private static extern bool init();
    [DllImport("RawInput")] private static extern bool kill();
    [DllImport("RawInput")] private static extern IntPtr poll();

    public const byte RE_DEVICE_DISCONNECT = 0;
    public const byte RE_KEYBOARD_MESSAGE = 1;

    public GameObject keyboardBrain;

    public bool initalized { get; private set; }

    [StructLayout(LayoutKind.Sequential)]
    public struct RawInputEvent
    {
        public int type;
        public int deviceID;
        public int press;
        public int release;
    }

    public class DllInput : GenericInput
    {
        private DllBrain keyboardBrain;
        public void SetInputReciever(DllBrain inp) { keyboardBrain = inp; }
        public DllBrain GetInputReciever() { return keyboardBrain; }
    }
    Dictionary<int, DllInput> pointersByDeviceId = new Dictionary<int, DllInput>();
    public Dictionary<int, DllInput> GetPointersByDeviceId()
    {
        return pointersByDeviceId;
    }

    public int keyboardCount = 0;
    public void OnEnable()
    {
        // If multikeyboard is disabled, do not setup
        if (playerSpawnSystem.GetMultikeyboardEnabled() == false)
            return;

        try
        {
            // When manager is enabled, clears any events that were queued during it being disabled
            IntPtr data = poll();
            Marshal.FreeCoTaskMem(data);
        }
        catch
        {
            Debug.LogError("Missing DLL Files");
            playerSpawnSystem.SetMultikeyboardEnabled(false);
        }
    }

    private void Start()
    {
        // If multikeyboard is disabled, do not setup
        if (playerSpawnSystem.GetMultikeyboardEnabled() == false)
            return;

        // Initializes keyboards and returns bool if successful
        initalized = init();
    }

    private void Update()
    {
        // If multikeyboard is disabled, do not setup
        if (playerSpawnSystem.GetMultikeyboardEnabled() == false)
            return;

        // Returns if not initalized
        if (initalized == false)
        {
            try
            {
                // When manager is enabled, clears any events that were queued during it being disabled
                IntPtr data = poll();
                Marshal.FreeCoTaskMem(data);
            }
            catch
            {
                Debug.LogError("Missing DLL Files");
                playerSpawnSystem.SetMultikeyboardEnabled(false);
            }
        }

        ReadDeviceData();
    }

    /// <summary>
    /// Adds player and connects player ID to spawned player
    /// </summary>
    /// <return> returns the spawned player's player id</return>
    public override int AddPlayerBrain(int deviceId)
    {
        // If multikeyboard is disabled, do not setup
        if (playerSpawnSystem.GetMultikeyboardEnabled() == false)
            return -1;

        // Creates keyboard input class and checks if device id exists for player
        DllInput dllInput = null;
        pointersByDeviceId.TryGetValue(deviceId, out dllInput);

        // If it does exist, return before spawning new player
        if(dllInput != null)
        {
            Debug.LogError("This device already has a player");
            return -1;
        }

        // Checks if another player can spawn
        if (playerSpawnSystem.CheckPlayerCount() == false)
        {
            Debug.LogError("Max Players Reached");
            return -1;
        }

        Debug.Log("Adding DeviceID " + deviceId);

        dllInput = new DllInput();

        // Sets the player id to be the next open slot
        dllInput.playerID = playerSpawnSystem.FindNextOpenPlayerSlot();

        dllInput.SetBrainGameobject(
            Instantiate(keyboardBrain, new Vector3(0, 0, 0), Quaternion.identity)); // Sets brain gameobejct 

        // Sets the parent of the brain
        dllInput.brainGameobject.transform.parent = brainParent;

        // Adds to the player gameobject and adds to the device dictionary
        playerSpawnSystem.AddPlayerCount(1);
        pointersByDeviceId[deviceId] = dllInput;

        // Spawn keyboard player brain
        dllInput.SetInputReciever((DllBrain)dllInput.brain);
        dllInput.GetInputReciever().InitializeBrain(dllInput.playerID, deviceId, this);

        // Adds player brain to brain dictionary, storing brain with pos
        playerSpawnSystem.AddPlayerBrain(dllInput.brain);
        Debug.Log(dllInput.brain.gameObject.name + dllInput.brain.GetPlayerID());
        keyboardCount++;

        dllInput.brain.InitalizeBrain();

        // Checks if any bodies have no brain
        List<PlayerMain> disconnectedBodies = playerSpawnSystem.GetDisconnectedBodies();
        if(disconnectedBodies.Count > 0)
        {
            // Trys to set it to be last played id, if it doesnt exist, set to be first player in list
            PlayerMain detectedLastIdPlayer = playerSpawnSystem.FindBodyByLastID(deviceId);
            if (detectedLastIdPlayer == null)
                detectedLastIdPlayer = disconnectedBodies[0];

            dllInput.GetInputReciever().SetPlayerBody(detectedLastIdPlayer);
            playerSpawnSystem.ReinitalizePlayerBody(dllInput.brain, detectedLastIdPlayer);
            playerSpawnSystem.RemoveDisconnectedBody(0);
        }

        return dllInput.playerID;
    }

    /// <summary>
    /// Deletes the player brain based on passed in player id
    /// </summary>
    public override void DeletePlayerBrain(int deviceId)
    {
        // Try get keyboard input that is in dictionary
        DllInput input;
        if (!pointersByDeviceId.TryGetValue(deviceId, out input))
            return;

        Debug.Log("Found Body To Delete");
        keyboardCount--;
        playerSpawnSystem.SubtractPlayerCount(1);

        // Removes player brain from dictionary
        playerSpawnSystem.DeletePlayerBrain(input.brain);

        Debug.Log("Removing Device " + deviceId);
        pointersByDeviceId.Remove(deviceId);
        Destroy(input.brainGameobject);

        Debug.Log($"There are now {pointersByDeviceId.Count} keyboards in game");
    }

    /// <summary>
    /// Clears device list of keybords
    /// </summary>
    private void ClearDeviceList()
    {
        pointersByDeviceId.Clear();

        playerSpawnSystem.SubtractPlayerCount(keyboardCount);
        keyboardCount = 0;

        foreach (Transform t in transform) Destroy(t.gameObject);
    }

    /// <summary>
    /// Updates and reads device data to parse for player input
    /// </summary>
    private void ReadDeviceData()
    {
        // If multikeyboard is disabled, do not setup
        if (playerSpawnSystem.GetMultikeyboardEnabled() == false)
            return;

        // Poll the events and properly update whatever we need
        IntPtr data = poll();

        // Reads first four byes to get number of events
        int numEvents = Marshal.ReadInt32(data);

        // Loops and handles every event
        for (int i = 0; i < numEvents; ++i)
        {
            var ev = new RawInputEvent();

            long offset = data.ToInt64() + sizeof(int) + i * Marshal.SizeOf(ev);

            ev.type = Marshal.ReadInt32(new IntPtr(offset + 0));
            ev.deviceID = Marshal.ReadInt32(new IntPtr(offset + 4));
            ev.press = Marshal.ReadInt32(new IntPtr(offset + 8));
            ev.release = Marshal.ReadInt32(new IntPtr(offset + 12));

            // If event type is a device disconnect
            if (ev.type == RE_DEVICE_DISCONNECT)
            {
                // Try get keyboard input that is in dictionary
                DllInput pointer = null;
                if (pointersByDeviceId.TryGetValue(ev.deviceID, out pointer))
                {
                    // Deletes if found
                    DeletePlayerBrain(ev.deviceID);
                }
                else
                {
                    Debug.Log("Unknown device detected to disconnect");
                }

            }
            // If event type is not a device disconnect and either the button being pressed or released is not zero
            else if (ev.type != RE_DEVICE_DISCONNECT && (ev.press != 0 | ev.release != 0))
            {
                // Try get keyboard input that is in dictionary
                DllInput pointer = null;
                if (pointersByDeviceId.TryGetValue(ev.deviceID, out pointer))
                {
                    //Debug.Log("Known device found");
                    // Since device is found, detect press for that player device
                    pointer.GetInputReciever().DetectPress(ev.press, ev.release);
                }
                else
                {
                    Debug.Log("Unknown device found");
                    AddPlayerBrain(ev.deviceID);
                }
            }
        }

        // Free memory after use
        Marshal.FreeCoTaskMem(data);
    }

    void OnApplicationQuit()
    {
        kill();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DLLInputManager))]
public class DLLInputManagerCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        // Get a reference to the target script
        var dllInputManager = (DLLInputManager)target;

        // Set up GUIStyle for the header text
        GUIStyle largeHeaderStyle = new GUIStyle(EditorStyles.boldLabel);
        largeHeaderStyle.fontSize = 18; // Adjust the font size as needed
        largeHeaderStyle.alignment = TextAnchor.MiddleCenter; // Center the text
        largeHeaderStyle.normal.textColor = Color.white; // White text

        // Add padding to the style for better layout
        largeHeaderStyle.padding = new RectOffset(10, 10, 5, 5);

        // Get the rect of the header
        Rect rect = EditorGUILayout.GetControlRect(false, 40); // Adjust height as needed

        // Draw the background
        EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f, 1f)); // Dark grey background

        // Draw the header label
        EditorGUI.LabelField(rect, "DLL Input Manager", largeHeaderStyle);

        // Status message styles
        GUIStyle statusStyle = new GUIStyle(EditorStyles.label);
        statusStyle.fontSize = 14; // Adjust the font size as needed
        statusStyle.alignment = TextAnchor.MiddleCenter; // Center the text
        if (dllInputManager.initalized)
        {
            statusStyle.normal.textColor = Color.green; // Green text for initialized
            EditorGUILayout.LabelField("Status: Initialized", statusStyle);
        }
        else
        {
            statusStyle.normal.textColor = Color.red; // Red text for not initialized
            EditorGUILayout.LabelField("Status: Not Initialized", statusStyle);
        }

        GUILayout.Space(10); // Add space between the header and the rest of the content

        // Show the 'keyboardBrain' field in the Inspector
        dllInputManager.keyboardBrain = (GameObject)EditorGUILayout.ObjectField(
            "DLL Brain",
            dllInputManager.keyboardBrain,
            typeof(GameObject),
            true
        );

        // Show the 'brainParent' field in the Inspector
        dllInputManager.brainParent = (Transform)EditorGUILayout.ObjectField(
            "Brain Parent",
            dllInputManager.brainParent,
            typeof(Transform),
            true
        );

        // Display connected keyboards
        EditorGUILayout.LabelField("Connected Keyboards: " + dllInputManager.keyboardCount);
        foreach (var keyValuePair in dllInputManager.GetPointersByDeviceId())
        {
            EditorGUILayout.LabelField("\t Device ID: " + keyValuePair.Key);
        }

        // Apply property modifications
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
#endif




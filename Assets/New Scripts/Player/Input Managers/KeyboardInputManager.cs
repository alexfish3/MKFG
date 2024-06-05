///
/// Parses raw input data from dll into useable format in unity 
/// Original concept by PeterSvP on Pastebin (https://pastebin.com/u/PeterSvP)
/// Modified by Alex Fischer for use with multi-keyboard | May 2024
/// 

using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

/// <summary>
/// The keyboard input manager handles everything related to parsing keyboard inputs
/// </summary>
public class KeyboardInputManager : GenericInputManager
{
    [DllImport("RawInput")] private static extern bool init();
    [DllImport("RawInput")] private static extern bool kill();
    [DllImport("RawInput")] private static extern IntPtr poll();

    public const byte RE_DEVICE_DISCONNECT = 0;
    public const byte RE_KEYBOARD_MESSAGE = 1;

    public GameObject keyboardBrain;

    [SerializeField] bool initalized = false;

    [StructLayout(LayoutKind.Sequential)]
    public struct RawInputEvent
    {
        public int type;
        public int devHandle;
        public int press;
        public int release;
    }

    private class KeyboardInput : GenericInput
    {
        private KeyboardBrain keyboardBrain;
        public void SetInputReciever(KeyboardBrain inp) { keyboardBrain = inp; }
        public KeyboardBrain GetInputReciever() { return keyboardBrain; }
    }
    Dictionary<int, KeyboardInput> pointersByDeviceId = new Dictionary<int, KeyboardInput>();

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
        KeyboardInput keyboardInput = null;
        pointersByDeviceId.TryGetValue(deviceId, out keyboardInput);

        // If it does exist, return before spawning new player
        if(keyboardInput != null)
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

        keyboardInput = new KeyboardInput();

        // Sets the player id to be the next open slot
        keyboardInput.playerID = playerSpawnSystem.FindNextOpenPlayerSlot();

        keyboardInput.SetBrainGameobject(
            Instantiate(keyboardBrain, new Vector3(0, 0, 0), Quaternion.identity)); // Sets brain gameobejct 
        
        // Adds to the player gameobject and adds to the device dictionary
        playerSpawnSystem.AddPlayerCount(1);
        pointersByDeviceId[deviceId] = keyboardInput;

        // Spawn keyboard player brain
        keyboardInput.SetInputReciever((KeyboardBrain)keyboardInput.brain);
        keyboardInput.GetInputReciever().InitializeBrain(keyboardInput.playerID, deviceId, this);

        // Adds player brain to brain dictionary, storing brain with pos
        playerSpawnSystem.AddPlayerBrain(keyboardInput.brain);
        Debug.Log(keyboardInput.brain.gameObject.name + keyboardInput.brain.GetPlayerID());
        keyboardCount++;

        keyboardInput.brain.InitalizeBrain();

        // Checks if any bodies have no brain
        List<PlayerMain> disconnectedBodies = playerSpawnSystem.GetDisconnectedBodies();
        if(disconnectedBodies.Count > 0)
        {
            // Trys to set it to be last played id, if it doesnt exist, set to be first player in list
            PlayerMain detectedLastIdPlayer = playerSpawnSystem.FindBodyByLastID(deviceId);
            if (detectedLastIdPlayer == null)
                detectedLastIdPlayer = disconnectedBodies[0];

            keyboardInput.GetInputReciever().SetPlayerBody(detectedLastIdPlayer);
            playerSpawnSystem.ReinitalizePlayerBody(keyboardInput.brain, detectedLastIdPlayer);
            playerSpawnSystem.RemoveDisconnectedBody(0);
        }

        return keyboardInput.playerID;
    }

    /// <summary>
    /// Deletes the player brain based on passed in player id
    /// </summary>
    public override void DeletePlayerBrain(int deviceId)
    {
        // Try get keyboard input that is in dictionary
        KeyboardInput input;
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
    /// Converts the brain which is DLL into a unity new input system
    /// </summary>
    public void ConvertBrainDLLToUNIS(KeyboardBrain oldBrain)
    {
        // creates keyboard brain and uses device id to create it
        playerSpawnSystem.KeyboardInputManager.AddPlayerBrain(oldBrain.GetDeviceID());

        // Find this new brain and cache it to use 
        ControllerBrain newBrain = playerSpawnSystem.SpawnedBrains[oldBrain.GetPlayerID()] as ControllerBrain;

        if (newBrain != null) 
        {
            // Initalizes the brain with the old brain
            newBrain.ReciveFromAnotherBrain(oldBrain);

            // Destroys old brain
            oldBrain.DestroyBrain();

        }
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
            ev.devHandle = Marshal.ReadInt32(new IntPtr(offset + 4));
            ev.press = Marshal.ReadInt32(new IntPtr(offset + 8));
            ev.release = Marshal.ReadInt32(new IntPtr(offset + 12));

            // If event type is a device disconnect
            if (ev.type == RE_DEVICE_DISCONNECT)
            {
                // Try get keyboard input that is in dictionary
                KeyboardInput pointer = null;
                if (pointersByDeviceId.TryGetValue(ev.devHandle, out pointer))
                {
                    // Deletes if found
                    DeletePlayerBrain(ev.devHandle);
                }
                else
                {
                    Debug.Log("Unknown device detected to disconnect");
                }

            }
            // If event type is a keyboard input and either button being pressed or released is not zero
            else if (/*ev.type == RE_KEYBOARD_MESSAGE &&*/ (ev.press != 0 | ev.release != 0))
            {
                // Try get keyboard input that is in dictionary
                KeyboardInput pointer = null;
                if (pointersByDeviceId.TryGetValue(ev.devHandle, out pointer))
                {
                    //Debug.Log("Known device found");
                    // Since device is found, detect press for that player device
                    pointer.GetInputReciever().DetectPress(ev.press, ev.release);
                }
                else
                {
                    Debug.Log("Unknown device found");
                    AddPlayerBrain(ev.devHandle);
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
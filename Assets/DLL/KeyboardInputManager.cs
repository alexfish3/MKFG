using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class KeyboardInputManager : GenericInputManager
{
    [DllImport("RawInput")] private static extern bool init();
    [DllImport("RawInput")] private static extern bool kill();
    [DllImport("RawInput")] private static extern IntPtr poll();

    public const byte RE_DEVICE_CONNECT = 0;
    public const byte RE_DEVICE_DISCONNECT = 1;
    public const byte RE_KEYBOARD = 2;

    public GameObject keyboardBrain;

    public string getEventName(int id)
    {
        switch (id)
        {
            case RE_DEVICE_CONNECT: return "RE_DEVICE_CONNECT";
            case RE_DEVICE_DISCONNECT: return "RE_DEVICE_DISCONNECT";
            case RE_KEYBOARD: return "RE_KEYBOARD";
        }
        return "UNKNOWN(" + id + ")";
    }

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

    void Start()
    {
        bool res = init();
    }

    public void OnEnable()
    {
        // When manager is enabled, clears any events that were queued during it being disabled
        IntPtr data = poll();
        Marshal.FreeCoTaskMem(data);
    }

    public override int AddPlayerBrain(int deviceId)
    {
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
        keyboardInput.playerID = playerSpawnSystem.GetPlayerCount();
        playerSpawnSystem.AddPlayerCount(1);

        pointersByDeviceId[deviceId] = keyboardInput;

        // Spawn keyboard player brain
        keyboardInput.brain = Instantiate(keyboardBrain, new Vector3(0,0,0), Quaternion.identity) as GameObject;
        keyboardInput.SetInputReciever(keyboardInput.brain.GetComponent<KeyboardBrain>());
        keyboardInput.GetInputReciever().InitializeBrain(keyboardInput.playerID, deviceId, this);

        keyboardCount++;

        // Checks if any bodies have no brain
        List<PlayerMain> disconnectedBodies = playerSpawnSystem.GetDisconnectedBodies();
        if(disconnectedBodies.Count > 0)
        {
            // Trys to set it to be last played id, if it doesnt exist, set to be first player in list
            PlayerMain detectedLastIdPlayer = playerSpawnSystem.FindBodyByLastID(deviceId);
            if (detectedLastIdPlayer == null)
                detectedLastIdPlayer = disconnectedBodies[0];

            keyboardInput.GetInputReciever().SetPlayerBody(detectedLastIdPlayer);
            playerSpawnSystem.RemoveDisconnectedBody(0);
        }

        return keyboardInput.playerID;
    }

    public override void DeletePlayerBrain(int deviceId)
    {
        KeyboardInput inp;
        if (!pointersByDeviceId.TryGetValue(deviceId, out inp))
            return;

        Debug.Log("Found Body To Delete");
        keyboardCount--;
        playerSpawnSystem.SubtractPlayerCount(1);

        Debug.Log("Removing Device " + deviceId);

        pointersByDeviceId.Remove(deviceId);
        Destroy(inp.brain);

        Debug.Log($"There are now {pointersByDeviceId.Count} keyboards in game");
    }

    void ClearDeviceList()
    {
        pointersByDeviceId.Clear();

        playerSpawnSystem.SubtractPlayerCount(keyboardCount);
        keyboardCount = 0;

        foreach (Transform t in transform) Destroy(t.gameObject);
    }

    void Update()
    {
        ReadDeviceData();
    }

    private void ReadDeviceData()
    {
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

            if (ev.type == RE_DEVICE_DISCONNECT)
            {
                KeyboardInput pointer = null;
                if (pointersByDeviceId.TryGetValue(ev.devHandle, out pointer))
                {
                    DeletePlayerBrain(ev.devHandle);
                }
                else
                {
                    Debug.Log("Unknown device detected to disconnect");
                }

            }
            else if (ev.type == RE_KEYBOARD && (ev.press != 0 | ev.release != 0))
            {
                KeyboardInput pointer = null;

                if (pointersByDeviceId.TryGetValue(ev.devHandle, out pointer))
                {
                    Debug.Log("Known device found");
                    pointer.GetInputReciever().DetectPress(ev.press, ev.release);
                }
                else
                {
                    Debug.Log("Unknown device found");
                    AddPlayerBrain(ev.devHandle);


                }
            }
        }
        Marshal.FreeCoTaskMem(data);
    }

    void OnApplicationQuit()
    {
        kill();
    }
}
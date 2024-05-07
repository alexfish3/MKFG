// Unity PINVOKE interface for pastebin.com/0Szi8ga6 
// Handles multiple cursors
// License: CC0

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class MouseInputManager : SingletonMonobehaviour<MouseInputManager>
{
    public static MouseInputManager instance;

    [DllImport("RawInput")]
    private static extern bool init();

    [DllImport("RawInput")]
    private static extern bool kill();

    [DllImport("RawInput")]
    private static extern IntPtr poll();

    public const byte RE_DEVICE_CONNECT = 0;
    public const byte RE_DEVICE_DISCONNECT = 1;
    public const byte RE_KEYBOARD = 2;

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

    public GameObject playerPrefab;
    public GameObject[] transforms;

    [StructLayout(LayoutKind.Sequential)]
    public struct RawInputEvent
    {
        public int type;
        public int devHandle;
        public int press;
        public int release;
    }

    public class KeyboardInput
    {
        public GameObject obj;
        public Vector2 position;
        public int deviceID;
        public int playerID;
        public float sensitivity;
        public bool spawned;
        public InputReciever inputReciever;
    }

    Dictionary<int, KeyboardInput> pointersByDeviceId = new Dictionary<int, KeyboardInput>();
    public int nextPlayerId = 0;
    public int keyboardCount = 0;

    Canvas canvas;

    // Update
    int lastEvents = 0;
    bool isInit = true;

    void Start()
    {
        instance = this;
        bool res = init();
        
        //enterSingleMode();
    }

    public void OnDestroy()
    {
        instance = null;
    }

    public void OnEnable()
    {
        // When manager is enabled, clears any events that were queued during it being disabled
        IntPtr data = poll();
        Marshal.FreeCoTaskMem(data);
    }

    int addPlayer(int deviceId)
    {
        if(!isInit)
        {
            Debug.LogError("Not initialized");
            return -1;
        }

        KeyboardInput keyboardInput = null;
        pointersByDeviceId.TryGetValue(deviceId, out keyboardInput);

        if(keyboardInput != null)
        {
            Debug.LogError("This device already has a cursor");
            return -1;
        }

        Debug.Log("Adding DeviceID " + deviceId);

        keyboardInput = new KeyboardInput();
        keyboardInput.playerID = nextPlayerId++;
        pointersByDeviceId[deviceId] = keyboardInput;
        //pointersByPlayerId[keyboardInput.playerID] = keyboardInput;

        keyboardInput.obj = Instantiate(playerPrefab, transforms[keyboardInput.playerID].transform) as GameObject;

        keyboardInput.inputReciever = keyboardInput.obj.GetComponent<InputReciever>();
        keyboardInput.inputReciever.Initalize(keyboardInput.playerID, deviceId, this);

        keyboardCount++;
        return keyboardInput.playerID;
    }

    public void deletePlayer(int deviceId)
    {
        keyboardCount--;
        nextPlayerId--;

        KeyboardInput inp;
        pointersByDeviceId.TryGetValue(deviceId, out inp);

        Debug.Log("Removing Device " + deviceId + " / " + inp.deviceID);

        pointersByDeviceId.Remove(deviceId);
        //pointersByPlayerId.Remove(inp.playerID);
        Destroy(inp.obj);
    }

    bool _isMultiplayer = true;
    KeyboardInput _spPointer;

    [SerializeField]
    public bool isMultiplayer
    {
        set
        {
            if (!value) 
                EnterSinglePlayer(); 
            else 
                EnterMultiPlayer();

            _isMultiplayer = value;
        }
        get { return _isMultiplayer; }
    }

    void EnterSinglePlayer()
    {
        ClearDeviceList();
        --nextPlayerId;
        addPlayer(0);
        _spPointer = pointersByDeviceId[0];
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

    void EnterMultiPlayer()
    {
        _spPointer = null;
        nextPlayerId = 0;
        ClearDeviceList();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void ClearDeviceList()
    {
        pointersByDeviceId.Clear();
        //pointersByPlayerId.Clear();
        nextPlayerId = 1;
        keyboardCount = 0;
        foreach (Transform t in transform) Destroy(t.gameObject);
    }

    public KeyboardInput GetPlayerID(int id)
    {
        KeyboardInput res = null;
        //pointersByPlayerId.TryGetValue(id, out res);
        return res;
    }

    void Update()
    {
        // Keyboard controls debug
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (isInit)
            {
                ClearDeviceList();
                kill();
                isInit = false;
            }
            else
            {
                init();
                isInit = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            isMultiplayer = !isMultiplayer;
        }

        // Single Player
        if (!_isMultiplayer)
        {
            var rt = _spPointer.obj.GetComponent<RectTransform>();
            rt.position = Input.mousePosition;
        }
        // Multi Player
        else
        {
            // Poll the events and properly update whatever we need
            IntPtr data = poll();

            // Reads first four byes to get number of events
            int numEvents = Marshal.ReadInt32(data);
            Debug.Log("Number of events: " + numEvents);

            if (numEvents > 0) lastEvents = numEvents;

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
                        deletePlayer(ev.devHandle);
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
                        pointer.inputReciever.DetectPress(ev.press, ev.release);
                    }
                    else
                    {
                        Debug.Log("Unknown device found");
                        addPlayer(ev.devHandle);
                    }
                }
            }
            Marshal.FreeCoTaskMem(data);
        }
    }

    void OnApplicationQuit()
    {
        kill();
    }
}
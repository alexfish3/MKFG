// Unity PINVOKE interface for pastebin.com/0Szi8ga6 
// Handles multiple cursors
// License: CC0

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class MouseInputManager : MonoBehaviour
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

    public GameObject cursor;
    public GameObject[] transforms;
    public float defaultMiceSensitivity = 1f;
    public float accelerationThreshold = 40;
    public float accelerationMultiplier = 2;
    public int screenBorderPixels = 16;

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
    Dictionary<int, KeyboardInput> pointersByPlayerId = new Dictionary<int, KeyboardInput>();
    int nextPlayerId = 1;
    int keyboardCount = 0;

    Canvas canvas;
    RectTransform canvasRect;
    float width, height;

    // Update
    int lastEvents = 0;
    bool isInit = true;

    void Start()
    {
        instance = this;
        bool res = init();
        Debug.Log("Init() ==> " + res);
        Debug.Log(Marshal.SizeOf(typeof(RawInputEvent)));
        canvas = GetComponent<Canvas>();
        canvasRect = GetComponent<RectTransform>();
        //enterSingleMode();
    }

    public void OnDestroy()
    {
        instance = null;
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
        pointersByPlayerId[keyboardInput.playerID] = keyboardInput;

        keyboardInput.obj = Instantiate(cursor, transforms[keyboardInput.playerID - 1].transform) as GameObject;

        keyboardInput.inputReciever = keyboardInput.obj.GetComponent<InputReciever>();
        keyboardInput.inputReciever.Initalize(keyboardInput.playerID, deviceId);

        ++keyboardCount;
        return keyboardInput.playerID;
    }

    void deletePlayer(int deviceId)
    {
        --keyboardCount;
        var mp = pointersByDeviceId[deviceId];
        pointersByDeviceId.Remove(mp.deviceID);
        pointersByPlayerId.Remove(mp.playerID);
        Destroy(mp.obj);
    }

    bool _isMultiplayer = true;
    KeyboardInput _spPointer;

    [SerializeField]
    public bool isMultiplayer
    {
        set
        {
            if (!value) 
                enterSingleMode(); 
            else 
                enterMultipleMode();

            _isMultiplayer = value;
        }
        get { return _isMultiplayer; }
    }

    void enterSingleMode()
    {
        clearCursorsAndDevices();
        --nextPlayerId;
        addPlayer(0);
        _spPointer = pointersByDeviceId[0];
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

    void enterMultipleMode()
    {
        _spPointer = null;
        nextPlayerId = 0;
        clearCursorsAndDevices();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void clearCursorsAndDevices()
    {
        pointersByDeviceId.Clear();
        pointersByPlayerId.Clear();
        nextPlayerId = 1;
        keyboardCount = 0;
        foreach (Transform t in transform) Destroy(t.gameObject);
    }

    public KeyboardInput getByPlayerId(int id)
    {
        KeyboardInput res = null;
        pointersByPlayerId.TryGetValue(id, out res);
        return res;
    }

    void Update()
    {
        // Keyboard controls debug
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (isInit)
            {
                clearCursorsAndDevices();
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
            width = canvasRect.rect.width;
            height = canvasRect.rect.height;
            var left = -width / 2;
            var right = width / 2;
            var top = -height / 2;
            var bottom = height / 2;

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

                //Debug.Log("TEST Down=" + ev.press + " Up=" + ev.release);

                if (ev.type == RE_DEVICE_DISCONNECT)
                {
                    deletePlayer(ev.devHandle);
                }
                else if (ev.type == RE_KEYBOARD && (ev.press != 0 | ev.release != 0))
                {
                    KeyboardInput pointer = null;

                    if (pointersByDeviceId.TryGetValue(ev.devHandle, out pointer))
                    {
                        //Debug.Log(getEventName(ev.type) + ":  H=" + ev.devHandle + ";  (" + ev.x + ";" + ev.y + ")  Down=" + (char)ev.press + " Up=" + (char)ev.release);

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
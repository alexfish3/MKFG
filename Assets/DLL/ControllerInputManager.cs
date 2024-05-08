using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerInputManager : MonoBehaviour
{
    public PlayerSpawnSystem playerSpawnSystem;

    public class ControllerInput
    {
        public GameObject brain;
        private ControllerBrain controllerBrain;
        public void SetInputReciever(ControllerBrain inp) { controllerBrain = inp; }
        public ControllerBrain GetInputReciever() { return controllerBrain; }

        public int deviceID;
        public int playerID;
        public bool spawned;
    }
    Dictionary<int, ControllerInput> pointersByDeviceId = new Dictionary<int, ControllerInput>();

    public int controllerCount = 0;

    public void AddPlayerBrain(PlayerInput playerInput)
    {
        int deviceId = playerInput.devices[0].deviceId;

        // Creates keyboard input class and checks if device id exists for player
        ControllerInput controllerInput = null;
        pointersByDeviceId.TryGetValue(deviceId, out controllerInput);

        // If it does exist, return before spawning new player
        if (controllerInput != null)
        {
            Debug.LogError("This device already has a player");
            return;
        }

        Debug.Log("Adding DeviceID " + deviceId);

        controllerInput = new ControllerInput();
        controllerInput.playerID = playerSpawnSystem.GetPlayerCount();
        controllerInput.brain = playerInput.gameObject;

        playerSpawnSystem.AddPlayerCount(1);

        pointersByDeviceId[deviceId] = controllerInput;

        // Spawn keyboard player brain
        controllerInput.SetInputReciever(controllerInput.brain.GetComponent<ControllerBrain>());
        controllerInput.GetInputReciever().InitializeBrain(controllerInput.playerID, deviceId, this);

        controllerCount++;

        // Checks if any bodies have no brain
        List<PlayerMain> disconnectedBodies = playerSpawnSystem.GetDisconnectedBodies();
        if (disconnectedBodies.Count > 0)
        {
            // Trys to set it to be last played id, if it doesnt exist, set to be first player in list
            PlayerMain detectedLastIdPlayer = playerSpawnSystem.FindBodyByLastID(deviceId);
            if (detectedLastIdPlayer == null)
                detectedLastIdPlayer = disconnectedBodies[0];

            controllerInput.GetInputReciever().SetPlayerBody(detectedLastIdPlayer);
            playerSpawnSystem.RemoveDisconnectedBody(0);
        }
    }

    public void DeletePlayerBrain(PlayerInput playerInput)
    {
        int deviceId = playerInput.devices[0].deviceId;
        ControllerInput inp;
        if (!pointersByDeviceId.TryGetValue(deviceId, out inp))
            return;

        Debug.Log("Found Body To Delete");
        controllerCount--;
        playerSpawnSystem.SubtractPlayerCount(1);

        Debug.Log("Removing Device " + deviceId);

        pointersByDeviceId.Remove(deviceId);
        Destroy(inp.brain);

        Debug.Log($"There are now {pointersByDeviceId.Count} controllers in game");
    }

    public void DeletePlayerBrain(int deviceId)
    {
        ControllerInput inp;
        if (!pointersByDeviceId.TryGetValue(deviceId, out inp))
            return;

        Debug.Log("Found Body To Delete");
        controllerCount--;
        playerSpawnSystem.SubtractPlayerCount(1);

        Debug.Log("Removing Device " + deviceId);

        pointersByDeviceId.Remove(deviceId);
        Destroy(inp.brain);

        Debug.Log($"There are now {pointersByDeviceId.Count} controllers in game");
    }
}

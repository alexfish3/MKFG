///
/// Created by Alex Fischer | May 2024
/// 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The controller input manager handles everything related to parsing controller inputs
/// </summary>
public class ControllerInputManager : GenericInputManager
{
    private class ControllerInput : GenericInput
    {
        private ControllerBrain controllerBrain;
        public void SetInputReciever(ControllerBrain inp) { controllerBrain = inp; }
        public ControllerBrain GetInputReciever() { return controllerBrain; }
    }

    Dictionary<int, ControllerInput> pointersByDeviceId = new Dictionary<int, ControllerInput>();

    public int controllerCount = 0;

    /// <summary>
    /// Adds the player brain based on the Unity input system "PlayerInput" class
    /// </summary>
    /// <param name="playerInput">The input system player input class</param>
    public override void AddPlayerBrain(PlayerInput playerInput)
    {
        InputDevice inputDevice = playerInput.devices[0];

        //// Sets the action map to controller if brain is spawned by a controller
        //if(playerInput.currentControlScheme == "Gamepad")
        //{
        //    playerInput.defaultActionMap = "Controller";
        //}
        //// Sets the action map to controller if brain is spawned by a keyboard
        //else if (playerInput.currentControlScheme == "Keyboard")
        //{
        //    playerInput.defaultActionMap = "Keyboard";
        //}

        // If multikeyboard is enabled and input brain is keyboard
        if (playerSpawnSystem.GetMultikeyboardEnabled() == true)
        {

        }

        // Calculate device ID
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

        // Checks if another player can spawn, if cant destroy spawned
        if(playerSpawnSystem.CheckPlayerCount() == false)
        {
            Debug.LogError("Max Players Reached");
            Destroy(playerInput.gameObject);
            return;
        }

        Debug.Log("Adding DeviceID " + deviceId);

        controllerInput = new ControllerInput();

        // Sets the player id to be the next open slot
        controllerInput.playerID = playerSpawnSystem.FindNextOpenPlayerSlot();

        controllerInput.SetBrainGameobject(playerInput.gameObject); // Sets brain gameobejct 

        // Adds to the player gameobject and adds to the device dictionary
        playerSpawnSystem.AddPlayerCount(1);
        pointersByDeviceId[deviceId] = controllerInput;

        // Spawn keyboard player brain
        controllerInput.SetInputReciever((ControllerBrain)controllerInput.brain);
        controllerInput.GetInputReciever().InitializeBrain(controllerInput.playerID, deviceId, this);

        // Adds player brain to brain dictionary, storing brain with pos
        playerSpawnSystem.AddPlayerBrain(controllerInput.brain);

        controllerCount++;

        controllerInput.brain.InitalizeBrain();

        // Checks if any bodies have no brain
        List<PlayerMain> disconnectedBodies = playerSpawnSystem.GetDisconnectedBodies();
        if (disconnectedBodies.Count > 0)
        {
            Debug.Log("Connect with disconnected body");
            // Trys to set it to be last played id, if it doesnt exist, set to be first player in list
            PlayerMain detectedLastIdPlayer = playerSpawnSystem.FindBodyByLastID(deviceId);
            if (detectedLastIdPlayer == null)
                detectedLastIdPlayer = disconnectedBodies[0];

            controllerInput.GetInputReciever().SetPlayerBody(detectedLastIdPlayer);
            playerSpawnSystem.ReinitalizePlayerBody(controllerInput.brain, detectedLastIdPlayer);
            playerSpawnSystem.RemoveDisconnectedBody(0);
        }
    }

    public override void DeletePlayerBrain(PlayerInput playerInput)
    {
        int deviceId = playerInput.devices[0].deviceId;
        HandleDelete(deviceId);
    }

    public override void DeletePlayerBrain(int deviceId)
    {
        HandleDelete(deviceId);
    }

    private void HandleDelete(int deviceId)
    {
        ControllerInput input;
        if (!pointersByDeviceId.TryGetValue(deviceId, out input))
            return;

        Debug.Log("Found Body To Delete");
        controllerCount--;
        playerSpawnSystem.SubtractPlayerCount(1);

        // Removes player brain from dictionary
        playerSpawnSystem.DeletePlayerBrain(input.brain);
        
        Debug.Log("Removing Device " + deviceId);

        pointersByDeviceId.Remove(deviceId);
        Destroy(input.brainGameobject);

        Debug.Log($"There are now {pointersByDeviceId.Count} controllers in game");
    }

}

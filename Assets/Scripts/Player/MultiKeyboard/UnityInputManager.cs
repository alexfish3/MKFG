///
/// Created by Alex Fischer | May 2024
/// 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static DLLInputManager;

/// <summary>
/// The controller input manager handles everything related to parsing controller inputs
/// </summary>

[RequireComponent(typeof(PlayerInputManager))]
public class UnityInputManager : GenericInputManager
{
    private class UnityInput : GenericInput
    {
        private UnityBrain controllerBrain;
        public void SetInputReciever(UnityBrain inp) { controllerBrain = inp; }
        public UnityBrain GetInputReciever() { return controllerBrain; }
    }

    Dictionary<int, UnityInput> pointersByDeviceId = new Dictionary<int, UnityInput>();

    public int controllerCount = 0;

    /// <summary>
    /// Adds the player brain based on the Unity input system "PlayerInput" class
    /// </summary>
    /// <param name="playerInput">The input system player input class</param>
    public override void AddPlayerBrain(PlayerInput playerInput)
    {
        // If multikeyboard is enabled and input brain is keyboard
        if (playerSpawnSystem.GetMultikeyboardEnabled() == true && playerInput.currentControlScheme == "Keyboard")
        {
            Debug.Log("DLL Enabled, Remove brain for new input system keyboard");
            Destroy(playerInput.gameObject);
            return;
        }

        // Calculate device ID
        int deviceId = playerInput.devices[0].deviceId;

        // Creates keyboard input class and checks if device id exists for player
        UnityInput unityInput = null;
        pointersByDeviceId.TryGetValue(deviceId, out unityInput);

        // If it does exist, return before spawning new player
        if (unityInput != null)
        {
            Debug.LogError("This device already has a player");
            return;
        }

        Debug.Log("Adding DeviceID " + deviceId);

        unityInput = new UnityInput();

        // Sets the player id to be the next open slot
        unityInput.playerID = playerSpawnSystem.FindNextOpenPlayerSlot();

        unityInput.SetBrainGameobject(playerInput.gameObject); // Sets brain gameobejct 

        // Sets the parent of the brain
        unityInput.brainGameobject.transform.parent = brainParent;

        // Adds to the player gameobject and adds to the device dictionary
        pointersByDeviceId[deviceId] = unityInput;

        // Spawn keyboard player brain
        unityInput.SetInputReciever((UnityBrain)unityInput.brain);
        unityInput.GetInputReciever().InitializeBrain(unityInput.playerID, deviceId, this);

        // Checks if another player can spawn
        if (playerSpawnSystem.CheckPlayerCount() == false)
        {
            // Adds player brain to brain list, storing brain with pos
            playerSpawnSystem.AddIdlePlayerBrain(unityInput.brain);
        }
        else
        {
            unityInput.brain.SetIsActiveBrain(true);
            // Adds player brain to brain list, storing brain with pos
            playerSpawnSystem.AddActivePlayerBrain(unityInput.brain);
        }

        controllerCount++;

        unityInput.brain.InitalizeBrain();

        // Checks if any bodies have no brain
        List<PlayerMain> disconnectedBodies = playerSpawnSystem.GetDisconnectedBodies();
        if (disconnectedBodies.Count > 0)
        {
            Debug.Log("Connect with disconnected body");
            // Trys to set it to be last played id, if it doesnt exist, set to be first player in list
            PlayerMain detectedLastIdPlayer = playerSpawnSystem.FindBodyByLastID(deviceId);
            if (detectedLastIdPlayer == null)
                detectedLastIdPlayer = disconnectedBodies[0];

            unityInput.GetInputReciever().SetPlayerBody(detectedLastIdPlayer);
            playerSpawnSystem.RemoveDisconnectedBody(0);

            // Since body is instantly being set, we need to initalize brain to match body
            // This includes setting the profile to drive, and setting the brain team info match the disconnected body.
            unityInput.brain.SetCurrentProfile(ControlProfile.Driving);
            unityInput.brain.SetTeamID(detectedLastIdPlayer.GetBodyTeamID());
            unityInput.brain.SetTeamColor(detectedLastIdPlayer.GetBodyTeamColor());
        }
    }

    public override void DeletePlayerBrain(int deviceId)
    {
        UnityInput input;
        if (!pointersByDeviceId.TryGetValue(deviceId, out input))
            return;

        Debug.Log("Found Body To Delete");
        controllerCount--;

        // Removes player brain from dictionary
        playerSpawnSystem.DeletePlayerBrain(input.brain);

        Debug.Log("Removing Device " + deviceId);

        pointersByDeviceId.Remove(deviceId);
        Destroy(input.brainGameobject);

        Debug.Log($"There are now {pointersByDeviceId.Count} controllers in game");
    }
}

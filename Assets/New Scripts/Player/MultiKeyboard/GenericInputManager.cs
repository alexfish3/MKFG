///
/// Created by Alex Fischer | May 2024
/// 

using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Abstract class that holds information to be used by both input manager types
/// </summary>
public abstract class GenericInputManager : MonoBehaviour
{
    public PlayerSpawnSystem playerSpawnSystem;
    public Transform brainParent;

    public class GenericInput
    {
        /// <summary>
        /// Sets the brain gameobject and also sets the brain
        /// </summary>
        /// <param name="newBrain">The new brain gameobject</param>
        public void SetBrainGameobject(GameObject newBrain)
        {
            brainGameobject = newBrain;
            brain = brainGameobject.GetComponent<GenericBrain>();
        }

        public GameObject brainGameobject;
        public GenericBrain brain;
        public int deviceID;
        public int playerID;
        public bool spawned;
    }

    public virtual void AddPlayerBrain(PlayerInput playerInput) { }

    public virtual void DeletePlayerBrain(PlayerInput playerInput) { }

    public virtual int AddPlayerBrain(int deviceId) { return 0; }

    public virtual void DeletePlayerBrain(int deviceId) { }

}

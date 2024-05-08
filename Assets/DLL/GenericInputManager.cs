using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class GenericInputManager : MonoBehaviour
{
    public PlayerSpawnSystem playerSpawnSystem;

    public class GenericInput
    {
        public GameObject brain;
        public int deviceID;
        public int playerID;
        public bool spawned;
    }

    public virtual void AddPlayerBrain(PlayerInput playerInput) { }

    public virtual void DeletePlayerBrain(PlayerInput playerInput) { }

    public virtual int AddPlayerBrain(int deviceId) { return 0; }

    public virtual void DeletePlayerBrain(int deviceId) { }

}

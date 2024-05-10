using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericBrain : MonoBehaviour
{
    protected GenericInputManager inputManager;

    [SerializeField] protected int playerID = 0;
    public int getPlayerID() { return playerID; }

    [SerializeField] protected int deviceID = -1;
    public int getDeviceID() { return deviceID; }

    [SerializeField] protected PlayerMain playerBody;
    public void SetPlayerBody(PlayerMain pm) { playerBody = pm; SetBodyEvents(); }
    public PlayerMain GetPlayerBody() { return playerBody; }

    [Header("Button Data")]
    [SerializeField] protected PlayerInputAction[] inputs;
    public delegate void Keystroke();

    // Status
    protected bool destroyed = false;

    public class PlayerInputAction
    {
        public string inputName;
        public bool state = false;
        [Space(20)]
        public Keystroke press;
        public Keystroke release;
    }

    public virtual void SetBodyEvents()
    {
    
    }

    public void SpawnBody(int playerToSpawn)
    {
        SetPlayerBody(PlayerList.Instance.SpawnCharacterBody(playerToSpawn));
    }

    public void DestroyObject()
    {
        if (destroyed == true)
            return;

        destroyed = true;

        // If device id is not 0, means it was valid and needs to be removed
        if(deviceID != -1)
            inputManager.DeletePlayerBrain(deviceID);

        // If player body is not null, add disconnected body to list
        if (playerBody != null)
            inputManager.playerSpawnSystem.AddDisconnectedPlayerBody(playerBody);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : SingletonMonobehaviour<MapManager>
{
    [SerializeField] Transform[] spawnPositions;

    bool initalized = false;

    GameManagerNew gameManager;
    PlayerSpawnSystem playerSpawnSystem;

    [SerializeField] bool DebugResetPositions = false;

    // Update is called once per frame
    void Update()
    {
        if (DebugResetPositions)
        {
            DebugResetPositions = false;
            InitalizeMap();
        }

        if (initalized == false && GameManagerNew.Instance != null && PlayerSpawnSystem.Instance != null)
        {
            initalized = true;
            PlayerSpawnSystem.Instance.OnAddPlayerBrain += InitalizeMap; // For debug, allows us to restart race when player is added after fact
            InitalizeMap();
        }
    }

    private void OnDestroy()
    {
        PlayerSpawnSystem.Instance.OnAddPlayerBrain -= InitalizeMap;
    }

    /// <summary>
    /// Initalizes the map, spawning players and elements
    /// </summary>
    private void InitalizeMap()
    {
        Debug.Log("Initalize Map");
        gameManager = GameManagerNew.Instance;
        playerSpawnSystem = PlayerSpawnSystem.Instance;

        int positionsToSpawnPlayersCounter = 0;

        // Spawn bodies for players
        foreach(KeyValuePair<int, GenericBrain> playerBrain in playerSpawnSystem.SpawnedBrains)
        {
            Debug.Log($"@@@ Checking body: {positionsToSpawnPlayersCounter}");
            bool successfulInSpawningBody = playerBrain.Value.SpawnBody(spawnPositions[positionsToSpawnPlayersCounter].position);

            // If brain did not spawn body, ie body already spawned, simply transform body
            if (successfulInSpawningBody == false)
            {
                Debug.Log($"@@@ body already spawned: {positionsToSpawnPlayersCounter}");
                playerBrain.Value.SetBodyPosition(spawnPositions[positionsToSpawnPlayersCounter].position);
            }

            positionsToSpawnPlayersCounter++;
        }

        // Sets the game to start
        gameManager.SetGameState(GameStates.Begin);
    }

}

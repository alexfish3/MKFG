using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : SingletonMonobehaviour<MapManager>
{
    [Header("Map Information")]
    [SerializeField] Transform[] spawnPositions;
    [SerializeField] bool DebugResetPositions = false;

    [Header("Countdown Information")]
    [SerializeField] int gameStartCountdownTimer = 3;

    bool initalized = false;

    GameManagerNew gameManager;
    PlayerSpawnSystem playerSpawnSystem;

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
            //PlayerSpawnSystem.Instance.OnAddPlayerBrain += InitalizeMap; // For debug, allows us to restart race when player is added after fact
            InitalizeMap();
        }
    }

    private void OnDestroy()
    {
        //PlayerSpawnSystem.Instance.OnAddPlayerBrain -= InitalizeMap;
    }

    /// <summary>
    /// Initalizes the map, spawning players and elements
    /// </summary>
    private void InitalizeMap()
    {
        gameManager = GameManagerNew.Instance;
        playerSpawnSystem = PlayerSpawnSystem.Instance;

        int positionsToSpawnPlayersCounter = 0;

        // Spawn bodies for players
        foreach(GenericBrain playerBrain in playerSpawnSystem.ActiveBrains)
        {
            Debug.Log($"@@@ Checking body: {positionsToSpawnPlayersCounter}");
            bool successfulInSpawningBody = playerBrain.SpawnBody(spawnPositions[positionsToSpawnPlayersCounter].position);

            Debug.Log("TT 3");

            // If brain did not spawn body, ie body already spawned, simply transform body
            if (successfulInSpawningBody == false)
            {
                Debug.Log($"@@@ body already spawned: {positionsToSpawnPlayersCounter}");
                playerBrain.SetBodyPosition(spawnPositions[positionsToSpawnPlayersCounter].position);
            }

            positionsToSpawnPlayersCounter++;
        }

        PlayerSpawnSystem.Instance.UpdatePlayerCameraRects();
        
        // Sets the game to start
        gameManager.SetGameState(GameStates.LoadMatch);

        // Once game is loaded, start countdown
        StartCoroutine(GameStartCountdown());
    }

    private IEnumerator GameStartCountdown()
    {
        for(int i = gameStartCountdownTimer; i > 0; i--)
        {
            Debug.Log("Countdown: " + i);
            yield return new WaitForSeconds(1f);
        }

        // Sets the game to start
        gameManager.SetGameState(GameStates.MainLoop);
    }

}

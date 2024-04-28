using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : SingletonMonobehaviour<SpawnManager>
{
    GameManager gameManager;
    PlayerInstantiate playerInstantiate;

    [Tooltip("The spawn positions of the players as they start the game")]
    [SerializeField] GameObject[] gameSpawnPositions = new GameObject[Constants.MAX_PLAYERS];
    [SerializeField] GameObject[] nonTutorialSpawnPositions = new GameObject[Constants.MAX_PLAYERS];
    [SerializeField] GameObject[] goldenPackageSpawnPositions = new GameObject[Constants.MAX_PLAYERS];

    ///<summary>
    /// On Enable of Script
    ///</summary>
    private void OnEnable()
    {
        gameManager = GameManager.Instance;
        playerInstantiate = PlayerInstantiate.Instance;

        gameManager.OnSwapStartingCutscene += SpawnPlayersStartOfGame;
        gameManager.OnSwapGoldenCutscene += SpawnPlayersFinalPackage;
    }

    ///<summary>
    /// On Disable of Script
    ///</summary>
    private void OnDisable()
    {
        gameManager.OnSwapStartingCutscene -= SpawnPlayersStartOfGame;
        gameManager.OnSwapGoldenCutscene -= SpawnPlayersFinalPackage;
    }

    private void Start()
    {
        //gameManager.SetGameState(GameState.StartingCutscene);
        // Set game to begin upon loading into scene
        if (TutorialManager.Instance.ShouldTutorialize)
        {
            gameManager.SetGameState(GameState.StartingCutscene);
        }
        else
        {
            gameManager.SetGameState(GameState.GoldenCutscene);
        }
    }

    ///<summary>
    /// This is executed when the OnSwapBegin event is called
    ///</summary>
    private void SpawnPlayersStartOfGame()
    {
        GameObject[] spawnPoints = gameSpawnPositions;
        // Loops for all spawned players
        for (int i = 0; i <= Constants.MAX_PLAYERS; i++)
        {
            try
            {
                if (playerInstantiate.PlayerInputs[i] == null)
                    continue;

                // Resets the velocity of the players
                playerInstantiate.PlayerInputs[i].GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;

                // reset position and rotation of ball and controller
                playerInstantiate.PlayerInputs[i].GetComponentInChildren<Rigidbody>().transform.position = spawnPoints[i].transform.position;
                playerInstantiate.PlayerInputs[i].GetComponentInChildren<Rigidbody>().transform.rotation = spawnPoints[i].transform.rotation;

                playerInstantiate.PlayerInputs[i].GetComponentInChildren<BallDriving>().transform.position = spawnPoints[i].transform.position;
                playerInstantiate.PlayerInputs[i].GetComponentInChildren<BallDriving>().transform.rotation = spawnPoints[i].transform.rotation;

                    // Initalize the compass ui on each of the players
                playerInstantiate.PlayerInputs[i].gameObject.GetComponentInChildren<CompassMarker>().InitalizeCompassUIOnAllPlayers();
            }
            catch { }
        }

        // After players have been placed, begin main loop
        gameManager.SetGameState(GameState.MainLoop);
    }

    ///<summary>
    /// This is executed when the OnSwapFinalPackage event is called
    ///</summary>
    public void SpawnPlayersFinalPackage()
    {
        // Loops for all spawned players
        for (int i = 0; i <= Constants.MAX_PLAYERS; i++)
        {
            try
            {
                if (playerInstantiate.PlayerInputs[i] == null)
                    continue;

                // Resets the velocity of the players
                playerInstantiate.PlayerInputs[i].GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;

                // reset position and rotation of ball and controller
                playerInstantiate.PlayerInputs[i].GetComponentInChildren<Rigidbody>().transform.position = goldenPackageSpawnPositions[i].transform.position;
                playerInstantiate.PlayerInputs[i].GetComponentInChildren<Rigidbody>().transform.rotation = goldenPackageSpawnPositions[i].transform.rotation;

                playerInstantiate.PlayerInputs[i].GetComponentInChildren<BallDriving>().transform.position = goldenPackageSpawnPositions[i].transform.position;
                playerInstantiate.PlayerInputs[i].GetComponentInChildren<BallDriving>().transform.rotation = goldenPackageSpawnPositions[i].transform.rotation;
            }
            catch { }
        }
    }
}

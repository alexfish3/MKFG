using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CheckpointType
{
    None,
    First,
    Last
};

public class CheckpointManager : SingletonMonobehaviour<CheckpointManager>
{
    [Header("Checkpoint Manager")]
    [SerializeField] private int totalLaps = 3;

    [Header("Game Information")]
    [SerializeField] private float tieDistance = 10f;

    private Checkpoint[] checkpoints;
    private int maxLap = 0; // highest running lap, so if the player in first is on lap 2 this value will be 2
    private int highestFirstPlace = 1; // max place a player can get during the race
    private int totalUniqueCheckpoints = 0; // number of total checkpoints a player must hit (actual number might be higher if there are shortcuts)
    private int playersFinished = 0;

    // getters and setters
    public int TotalLaps { get { return totalLaps; } }
    public float TieDistance { get { return tieDistance; } }
    public int TotalCheckpoints { get { return totalUniqueCheckpoints; } }
    public Checkpoint FirstCheckpoint { get { return checkpoints[0]; } }
    public Checkpoint LastCheckpoint { get { return checkpoints[totalUniqueCheckpoints - 1]; } }

    // events
    public Action OnCheckpointInit;

    private void Start()
    {
        ReadRuleset();

        int currIndex = 0;
        checkpoints = transform.GetComponentsInChildren<Checkpoint>();

        for(int i=0;i<checkpoints.Length; i++)
        {
            string scLabel = " (SC)";
            if (!checkpoints[i].KeepIndex) // for non-shortcut checkpoints
            {
                checkpoints[i].Index = currIndex;
                currIndex++;
                totalUniqueCheckpoints++;
                scLabel = "";
            }
            checkpoints[i].transform.name = checkpoints[i].Index.ToString() + scLabel;
        }

        // giving reference to the checkpoint's next checkpoint, basically a linked list
        foreach(Checkpoint checkpoint in checkpoints)
        {
            checkpoint.NextCheckpoint = checkpoint.KeepIndex ? FindCheckpointWithIndex(checkpoint.Index + 1, true) : FindCheckpointWithIndex(checkpoint.Index+1);
        }

        // can init handlers now that checkpoints are initialized
        OnCheckpointInit?.Invoke();
    }

    private void Update()
    {
        int currPlace = highestFirstPlace; // init the first place
        int skippedPlayerCount = 0;
        for (int lap = maxLap; lap >= 0; lap--) // check if the laps align
        {
            for (int i = checkpoints.Length - 1; i >= 0; i--) // loop through each checkpoint
            {
                // tie logic
                bool dirtySkippedPlayer = false;
                for (int j = 0; j < checkpoints[i].PlayersTracking.Count; j++) // will pop closest players to checkpoint and work downwards
                {
                    try // award placement and accumulate currPlace
                    {
                        if (checkpoints[i].PlayersTracking[j].Lap == lap)
                        {
                            // TODO: make the tie tracking work with laps
                            if (checkpoints[i].PlayersTracking[j].LocalPlacement == 1)
                            {
                                skippedPlayerCount++;
                                dirtySkippedPlayer = true;
                            }
                            else if(!dirtySkippedPlayer)
                            {
                                currPlace += 1;
                            }
                            else
                            {
                                currPlace += skippedPlayerCount > 0 ? skippedPlayerCount : 1;
                                skippedPlayerCount = 0;
                            }
                            checkpoints[i].PlayersTracking[j].Placement = currPlace;
                        }
                    }
                    catch // for null PlacementHandlers that show up for unknown reasons >:(
                    {
                        continue;
                    }
                }
                if (dirtySkippedPlayer)
                {
                    currPlace += skippedPlayerCount > 0 ? skippedPlayerCount : 1;
                    skippedPlayerCount = 0;
                }
            }
        }
    }

    /// <summary>
    /// Set the next checkpoint to track the player.
    /// </summary>
    /// <param name="playerGO">Player to be tracked</param>
    /// <param name="checkpointIndx">Index of their checkpoint</param>
    public void AdvanceCheckpoint(PlacementHandler playerGO, Checkpoint checkpoint)
    {
        Checkpoint newCheckpoint = checkpoint.NextCheckpoint;
        if(newCheckpoint.Index > checkpoint.Index)
        {
            playerGO.CheckpointsThisLap--;
        }
        else
        {
            if(playerGO.CheckpointsThisLap <= 1)
            {
                playerGO.Lap++;
                if(playerGO.Lap > totalLaps)
                {
                    highestFirstPlace++;
                    playerGO.FinishRace();
                    playersFinished++;
                    GameManagerNew.Instance.AddFinishedPlayer(playerGO);
                    if(playersFinished >= PlayerList.Instance.spawnedPlayerCount)
                    {
                        GameManagerNew.Instance.SetGameState(GameStates.Results);
                    }
                    return;
                }
                if (playerGO.Lap > maxLap)
                {
                    maxLap = playerGO.Lap;
                }
            }
            playerGO.CheckpointsThisLap = TotalCheckpoints;
        }
        newCheckpoint.AddPlayer(playerGO);
    }

    /// <summary>
    /// Finds a checkpoint of specified index.
    /// </summary>
    /// <param name="index">Index of checkpoint being searched for.</param>
    /// <param name="checkShortcuts">Will search through shortcut checkpoints first if true, then normal checkpoints. Doesn't search SC's if false</param>
    /// <returns></returns>
    public Checkpoint FindCheckpointWithIndex(int index, bool checkShortcuts = false)
    {
        Checkpoint outCheckpoint = null;

        // check shortcuts first. will return a checkpoint of passed in index with true keepIndex if one exists
        if (checkShortcuts)
        {
            foreach (Checkpoint checkpoint in checkpoints)
            {
                if (checkpoint.Index == index && checkpoint.KeepIndex)
                {
                    outCheckpoint = checkpoint;
                }
            }
        }
        
        if(outCheckpoint == null) // if a shortcut checkpoint can't be found or !checkShortcuts then will try and find a normal checkpoint with the index
        {
            foreach (Checkpoint checkpoint in checkpoints)
            {
                if (checkpoint.Index == index && !checkpoint.KeepIndex)
                {
                    outCheckpoint = checkpoint;
                }
            }
        }

        if(outCheckpoint == null) // if no checkpoint is found return the first checkpoint
        {
            outCheckpoint = checkpoints[0];
        }

        return outCheckpoint;
    }

    /// <summary>
    /// Reads the game ruleset from GameManager.
    /// </summary>
    private void ReadRuleset()
    {
        RulesetSO ruleset = GameManagerNew.Instance.Ruleset;
        if (GameManagerNew.Instance.CurrMapType != MapType.Straight)
        {
            totalLaps = ruleset.NumOfLaps;
        }
        else
        {
            totalLaps = 1;
        }
    }
}

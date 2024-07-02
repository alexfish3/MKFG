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
    [SerializeField] private int totalLaps = 3;
    private Checkpoint[] checkpoints;
    private int maxLap = 0;
    private int highestFirstPlace = 1; // max place a player can get during the race
    private int totalUniqueCheckpoints = 0;
    public int TotalLaps { get { return totalLaps; } }
    public int TotalCheckpoints { get { return totalUniqueCheckpoints; } }

    public Action OnCheckpointInit;

    public Checkpoint FirstCheckpoint { get { return checkpoints[0]; } }
    public Checkpoint LastCheckpoint { get { return checkpoints[totalUniqueCheckpoints-1]; } }
    private void Start()
    {
        int currIndex = 0;
        checkpoints = transform.GetComponentsInChildren<Checkpoint>();
        for(int i=0;i<checkpoints.Length; i++)
        {
            string scLabel = " (SC)";
            if (!checkpoints[i].KeepIndex)
            {
                checkpoints[i].Index = currIndex;
                currIndex++;
                totalUniqueCheckpoints++;
                scLabel = "";
            }
            checkpoints[i].transform.name = checkpoints[i].Index.ToString() + scLabel;
        }

        foreach(Checkpoint checkpoint in checkpoints)
        {
            checkpoint.NextCheckpoint = checkpoint.KeepIndex ? FindCheckpointWithIndex(checkpoint.Index + 1, true) : FindCheckpointWithIndex(checkpoint.Index+1);
        }
        OnCheckpointInit?.Invoke();
    }

    private void Update()
    {
        int currPlace = highestFirstPlace; // init the first place
        for (int lap = maxLap; lap >= 0; lap--) // check if the laps align
        {
            for (int i = checkpoints.Length - 1; i >= 0; i--) // loop through each checkpoint
            {
                for (int j = 0; j < checkpoints[i].PlayersTracking.Count; j++) // will pop closest players to checkpoint and work downwards
                {
                    try // award placement and accumulate currPlace
                    {
                        if (checkpoints[i].PlayersTracking[j].Lap == lap)
                        {
                            checkpoints[i].PlayersTracking[j].Placement = currPlace;
                            currPlace++;
                        }
                    }
                    catch // for null PlacementHandlers that show up for unknown reasons >:(
                    {
                        continue;
                    }
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
            if(playerGO.CheckpointsThisLap <= 0)
            {
                playerGO.Lap++;
                if(playerGO.Lap > totalLaps)
                {
                    highestFirstPlace++;
                    playerGO.FinishRace();
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
}

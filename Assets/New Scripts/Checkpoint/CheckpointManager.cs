using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckpointManager : SingletonMonobehaviour<CheckpointManager>
{
    [SerializeField] private Checkpoint[] checkpoints;
    private int maxLap = 0;

    private void Start()
    {
        for(int i=0;i<checkpoints.Length; i++)
        {
            checkpoints[i].Index = i;
        }
    }

    private void Update()
    {
        int currPlace = 1; // init the first place
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
    public void AdvanceCheckpoint(PlacementHandler playerGO, int checkpointIndx)
    {
        Checkpoint newCheckpoint;
        try // find the next checkpoint, or loop back to 0
        {
            newCheckpoint = checkpoints[checkpointIndx + 1];
        }
        catch
        {
            newCheckpoint = checkpoints[0];
            playerGO.Lap++;
            if(playerGO.Lap > maxLap)
            {
                maxLap = playerGO.Lap;
            }
        }

        newCheckpoint.AddPlayer(playerGO);
    }
}

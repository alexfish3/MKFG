using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckpointManager : SingletonMonobehaviour<CheckpointManager>
{
    [SerializeField] private Checkpoint[] checkpoints;

    private void Start()
    {
        for(int i=0;i<checkpoints.Length; i++)
        {
            checkpoints[i].Index = i;
        }
    }

    private void Update()
    {
        int currPlace = 1;
        for (int i = 0; i < checkpoints.Length; i++)
        {
            for (int j = 0; j < checkpoints[i].PlayersTracking.Count; j++)
            {
                try
                {
                    checkpoints[i].PlayersTracking[j].Placement = currPlace;
                    currPlace++;
                }
                catch
                {
                    continue;
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
        try
        {
            newCheckpoint = checkpoints[checkpointIndx + 1];
        }
        catch
        {
            newCheckpoint = checkpoints[0];
        }

        newCheckpoint.AddPlayer(playerGO);
    }
}

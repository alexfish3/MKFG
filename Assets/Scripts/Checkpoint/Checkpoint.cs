using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private bool keepIndex = false;

    private List<PlacementHandler> playersTracking = new List<PlacementHandler>();
    private Checkpoint nextCheckpoint;
    private CheckpointTrigger[] triggers;
    private RespawnPoint[] respawnPoints;

    // getters and setters
    public int Index { get { return index; } set { index = value; } }
    public bool KeepIndex { get { return keepIndex; } }
    public Checkpoint NextCheckpoint { get { return nextCheckpoint; } set {  nextCheckpoint = value; } }
    public List<PlacementHandler> PlayersTracking { get { return playersTracking; } }

    private void Start()
    {
        respawnPoints = transform.GetComponentsInChildren<RespawnPoint>();
        triggers = transform.GetComponentsInChildren<CheckpointTrigger>();
        CheckpointManager.Instance.OnCheckpointInit += CheckDirection;
    }

    private void OnDisable()
    {
        CheckpointManager.Instance.OnCheckpointInit -= CheckDirection;
    }

    private void Update()
    {
        Color debugColor = keepIndex ? Color.red : Color.blue;
        if (nextCheckpoint != null)
        {
            Debug.DrawLine(transform.position, nextCheckpoint.transform.position, debugColor);
        }
        for(int i=0;i<playersTracking.Count;i++)
        {
            try
            {
                playersTracking[i].LastDist = playersTracking[i].DistToCheckpoint;
                playersTracking[i].DistToCheckpoint = Mathf.Abs(Vector3.Distance(transform.position, playersTracking[i].transform.position));

                // debug
                Debug.DrawLine(this.transform.position, playersTracking[i].transform.position, Color.green);
            }
            catch
            {
                playersTracking.RemoveAt(i);
            }
        }

        CheckPlacements();

    }

    /// <summary>
    /// Starts tracking inPlayer.
    /// </summary>
    /// <param name="inPlayer">Player about to be big-brothered</param>
    public void AddPlayer(PlacementHandler inPlayer)
    {
        if (!playersTracking.Contains(inPlayer))
        {
            playersTracking.Add(inPlayer);
            inPlayer.AssignRSPs(respawnPoints);
            inPlayer.CurrentCheckpointIndex = index;
        }
    }

    /// <summary>
    /// Removes player from the watchful eye of the Chinese government.
    /// </summary>
    /// <param name="outPlayer">Player to be removed</param>
    public void RemovePlayer(PlacementHandler outPlayer)
    {
        if (playersTracking.Contains(outPlayer))
        {
            playersTracking.Remove(outPlayer);
            CheckpointManager.Instance.AdvanceCheckpoint(outPlayer, this);
        }
        else if(CheckpointManager.Instance.FindCheckpointWithIndex(index).TrackingPlayer(outPlayer)) // handles entering a shortcut trigger
        {
            CheckpointManager.Instance.FindCheckpointWithIndex(index).PlayersTracking.Remove(outPlayer);
            CheckpointManager.Instance.AdvanceCheckpoint(outPlayer, CheckpointManager.Instance.FindCheckpointWithIndex(index));
        }
        
    }

    /// <summary>
    /// Holds the player back if they drive backwards out of this checkpoint.
    /// </summary>
    /// <param name="player">Player to be held back</param>
    private void HoldBackPlayer(PlacementHandler player)
    {
        if (nextCheckpoint.PlayersTracking.Contains(player) && player.OutDistance > player.InDistance) // if the player goes backwards into a checkpoint
        {
            nextCheckpoint.playersTracking.Remove(player);
            player.CheckpointsThisLap++;
            AddPlayer(player);
            if(index == 0)
            {
                player.CheckpointsThisLap = CheckpointManager.Instance.TotalCheckpoints;
            }
        }
    }

    /// <summary>
    /// Checks the placements of players currently being tracked by the checkpoint.
    /// </summary>
    private void CheckPlacements()
    {
        if (playersTracking.Count == 1)
        {
            playersTracking[0].LocalPlacement = 1;
        }
        if(playersTracking.Count <= 1)
        {
            return;
        }

        playersTracking.Sort((i, j) => i.DistToCheckpoint.CompareTo(j.DistToCheckpoint));

        // check local placement
        int currLP = 1;
        for(int i=0; i<playersTracking.Count; i++)
        {
            for(int j=0;j<playersTracking.Count; j++)
            {
                if (playersTracking[i] == playersTracking[j])
                    continue;
                if (Vector3.Distance(playersTracking[i].transform.position, playersTracking[j].transform.position) <= CheckpointManager.Instance.TieDistance)
                {
                    playersTracking[i].LocalPlacement = currLP;
                    playersTracking[j].LocalPlacement = currLP;
                }
                else
                {
                    playersTracking[i].LocalPlacement = currLP;
                    currLP++;
                }
            }
        }
    }

    /// <summary>
    /// Checks the entry and exit triggers in the checkpoint.
    /// </summary>
    private void CheckDirection()
    {
        int min=1000000, max=0;
        float closest=100000, furthest=0;
        for(int i=0;i<triggers.Length;i++)
        {
            float testFurthest = Vector3.Distance(nextCheckpoint.transform.position, triggers[i].transform.position);
            if (testFurthest > furthest)
            {
                max = i;
                furthest = testFurthest;
            }
            
            if(testFurthest < closest)
            {
                min = i;
                closest = testFurthest;
            }
        }
        triggers[max].Type = CheckpointType.First;
        triggers[min].Type = CheckpointType.Last;

        // debug change colors
        triggers[max].GetComponent<MeshRenderer>().material.color = new Color(0,1,1,0.5f);
        triggers[min].GetComponent<MeshRenderer>().material.color = new Color(1,0,1,0.5f);
    }

    /// <summary>
    /// Checks if ph is being tracked by this checkpoint.
    /// </summary>
    /// <param name="ph">Player checked</param>
    /// <returns>False when not being tracked, true otherwise</returns>
    public bool TrackingPlayer(PlacementHandler ph)
    {
        return playersTracking.Contains(ph);
    }

    /// <summary>
    /// Called from a checkpoint trigger and handles the logic of a player entering a checkpoint.
    /// </summary>
    /// <param name="other">Collider of the player</param>
    public void CheckpointEnter(Collider other)
    {
        PlacementHandler ph;
        try
        {
            ph = other.gameObject.GetComponent<PlacementHandler>();
            ph.InDistance = Vector3.Distance(ph.transform.position, nextCheckpoint.transform.position);
            RemovePlayer(ph);
        }
        catch
        {
            return;
        }
    }

    /// <summary>
    /// Called from a checkpoint trigger and handles the logic of a player exiting a checkpoint.
    /// </summary>
    /// <param name="other">Collider of the player</param>
    public void CheckpointExit(Collider other)
    {
        PlacementHandler ph;
        try
        {
            ph = other.gameObject.GetComponent<PlacementHandler>();
            ph.OutDistance = Vector3.Distance(ph.transform.position, nextCheckpoint.transform.position);
            HoldBackPlayer(ph);
        }
        catch
        {
            return;
        }
    }
}

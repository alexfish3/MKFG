using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private List<PlacementHandler> playersTracking = new List<PlacementHandler>();
    public List<PlacementHandler> PlayersTracking { get { return playersTracking; } }
    private int index;
    public int Index { get { return index; } set { index = value; } }
    private RespawnPoint[] respawnPoints;
    private Checkpoint nextCheckpoint;

    public Checkpoint NextCheckpoint { get { return nextCheckpoint; } set {  nextCheckpoint = value; } }

    private void Start()
    {
        respawnPoints = transform.GetComponentsInChildren<RespawnPoint>();
    }

    private void Update()
    {
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

    public void AddPlayer(PlacementHandler inPlayer)
    {
        if (!playersTracking.Contains(inPlayer))
        {
            playersTracking.Add(inPlayer);
            inPlayer.AssignRSPs(respawnPoints);
            inPlayer.CurrentCheckpointIndex = index;
        }
    }

    public void RemovePlayer(PlacementHandler outPlayer)
    {
        if (playersTracking.Contains(outPlayer))
        {
            playersTracking.Remove(outPlayer);
            CheckpointManager.Instance.AdvanceCheckpoint(outPlayer, index);
        }
        
    }

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
        if (playersTracking.Count <= 1)
        {
            return;
        }

        playersTracking.Sort((i, j) => i.DistToCheckpoint.CompareTo(j.DistToCheckpoint));
    }

    private void OnTriggerEnter(Collider other)
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

    private void OnTriggerExit(Collider other)
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

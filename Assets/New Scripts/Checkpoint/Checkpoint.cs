using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private List<PlacementHandler> playersTracking = new List<PlacementHandler>();
    public List<PlacementHandler> PlayersTracking { get { return playersTracking; } }
    private int index;
    public int Index { get { return index; } set { index = value; } }
    private RespawnPoint[] respawnPoints;

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
            RemovePlayer(ph);
        }
        catch
        {
            return;
        }
    }
}

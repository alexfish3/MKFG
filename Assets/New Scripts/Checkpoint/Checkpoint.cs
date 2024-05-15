using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private List<PlacementHandler> playersTracking = new List<PlacementHandler>();
    public List<PlacementHandler> PlayersTracking { get { return playersTracking; } }
    private int index;
    public int Index { get { return index; } set { index = value; } }

    private void Update()
    {
        for(int i=0;i<playersTracking.Count;i++)
        {
            try
            {
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
        if(!playersTracking.Contains(inPlayer))
            playersTracking.Add(inPlayer);
    }

    public void RemovePlayer(PlacementHandler outPlayer)
    {
        if (playersTracking.Contains(outPlayer))
            playersTracking.Remove(outPlayer);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlacementHandler ph;
        try
        {
            ph = other.gameObject.GetComponent<PlacementHandler>();
            RemovePlayer(ph);
            CheckpointManager.Instance.AdvanceCheckpoint(ph, index);
            Debug.Log("player entered checkpoint!");
        }
        catch
        {
            return;
        }
    }

    private void CheckPlacements()
    {
        if(playersTracking.Count <= 1)
        {
            return;
        }

        playersTracking.Sort((i, j) => i.DistToCheckpoint.CompareTo(j.DistToCheckpoint));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private List<GameObject> playersTracking = new List<GameObject>();

    public void AddPlayer(GameObject inPlayer)
    {
        playersTracking.Add(inPlayer);
    }

    public void RemovePlayer(GameObject outPlayer)
    {
        if (playersTracking.Contains(outPlayer))
            playersTracking.Remove(outPlayer);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            RemovePlayer(other.gameObject);
        }
    }
}

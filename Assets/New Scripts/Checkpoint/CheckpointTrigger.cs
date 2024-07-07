using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for an individual trigger on the Checkpoint prefab.
/// </summary>
public class CheckpointTrigger : MonoBehaviour
{
    private Checkpoint checkpoint;
    private CheckpointType type;

    // getters and setters
    public CheckpointType Type { get { return type; } set { type = value; } }

    // Start is called before the first frame update
    void Start()
    {
        checkpoint = GetComponentInParent<Checkpoint>();
    }

    private void OnTriggerEnter(Collider other)
    {
        checkpoint.CheckpointEnter(other); 
    }

    private void OnTriggerExit(Collider other)
    {
        if(type == CheckpointType.First)
            checkpoint.CheckpointExit(other);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    // directional stuff
    [SerializeField] private Transform directionGuide; // GO to calculate direction player should be facing

    // getters and setters
    public Quaternion Facing { get { return Quaternion.LookRotation((directionGuide.position - transform.position).normalized); } }
    public Vector3 PlayerSpawn { get { return transform.position; } }

}

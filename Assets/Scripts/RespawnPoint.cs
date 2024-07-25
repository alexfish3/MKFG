using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    // directional stuff
    [SerializeField] private GameObject directionGuide; // GO to calculate direction player should be facing
    public Vector3 Facing { get { return directionGuide.transform.localPosition - transform.position; } }


    public Vector3 PlayerSpawn { get { return transform.position; } }
    public Vector3 PlayerFacingDirection { get { return playerFacingDirection; } }
    public bool InUse { get { return inUse; } set { inUse = value; } }

    private Vector3 playerFacingDirection;
    private bool inUse = false;

    /// <summary>
    /// This method is for initializing the point, specifially the inUse variable in case the gamestate changes mid-respawn.
    /// </summary>
    public void InitPoint()
    {
        inUse = false;
    }
}

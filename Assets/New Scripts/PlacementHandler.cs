using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementHandler : MonoBehaviour
{
    private int placement;
    public int Placement { get { return placement; } set { placement = value; } }
    private float distToCheckpoint;
    public float DistToCheckpoint { get {  return distToCheckpoint; } set {  distToCheckpoint = value; } }
}

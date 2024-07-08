using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Ruleset")]
public class Ruleset : ScriptableObject
{
    [SerializeField] private int numOfLaps = 3;
    [SerializeField] private float startingHealth = 100f;


    // getters
    public int NumOfLaps { get { return numOfLaps; } }
    public float StartingHealth { get { return startingHealth; } }
}

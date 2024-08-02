using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Ruleset")]
public class RulesetSO : ScriptableObject
{
    [SerializeField] private string nameOfRuleset = "null";
    [SerializeField] private int numOfLaps = 3;
    [SerializeField] private float startingHealth = 100f;
    [SerializeField] private bool overrideLaps = false;

    // getters
    public string NameOfRuleset { get { return nameOfRuleset; } }
    public int NumOfLaps { get { return numOfLaps; } }
    public float StartingHealth { get { return startingHealth; } }
    public bool OverrideLaps { get { return overrideLaps; } }
}

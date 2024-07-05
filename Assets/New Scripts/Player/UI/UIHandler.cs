using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIHandler : MonoBehaviour
{
    [Header("Player Information")]
    [SerializeField] private PlayerMain player;
    [SerializeField] private PlacementHandler placement;

    [Header("Text Refs")]
    [SerializeField] private TextMeshProUGUI lap;
    [SerializeField] private TextMeshProUGUI dir;
    [SerializeField] private TextMeshProUGUI place;
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI fwdSpeed;

    public TextMeshProUGUI Lap { get { return lap; } }
    public TextMeshProUGUI Dir { get {  return dir; } }
    public TextMeshProUGUI Place { get { return place; } }
    public TextMeshProUGUI Health { get {  return health; } }
    public TextMeshProUGUI FwdSpeed { get { return fwdSpeed; } }

    private void Update()
    {
        return; 

        if (!placement.IsFinished)
        {
            lap.text = $"Lap: {placement.Lap}/{CheckpointManager.Instance.TotalLaps}";
            place.text = $"Placement: {placement.Placement}";
        }
        else
        {
            lap.text = "";
            place.text = $"Finished In: {placement.Placement}";
        }
        health.text = $"Health: {Mathf.RoundToInt(player.GetHealthMultiplier()*100)}%";
    }
}

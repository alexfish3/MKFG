using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlacementHandler : MonoBehaviour
{
    [SerializeField] PlayerMain playerMain;
    [SerializeField] private float distanceCheckCooldown = 1f;

    UIHandler uiHandler;
    private bool isFinished = false;
    private bool hasStarted = false;
    private int placement;
    private int localPlacement;
    private float distToCheckpoint = 0;
    private float lastDist;
    private int lap = 0;
    private int checkpointsThisLap = 0;
    private bool wrongWay = false;
    private float distanceCheckCounter = 0;
    private int currentCheckpointIndex;
    private float inDistance, outDistance; // for backwards placement calculation
    private Respawn respawn;

    // getters and setters
    public int Placement { get { return placement; } set { placement = value; } }
    public int LocalPlacement { get { return localPlacement; } set { localPlacement = value; } }
    public float DistToCheckpoint { get {  return distToCheckpoint; } set {  distToCheckpoint = value; } }
    public float LastDist { set { lastDist = value; } }
    public int Lap { get { return lap; } set { lap = value; } }
    public int CheckpointsThisLap { get { return checkpointsThisLap; } set { checkpointsThisLap = value; } }
    public int CurrentCheckpointIndex { get { return currentCheckpointIndex; } set { currentCheckpointIndex = value; } }
    public bool IsFinished { get { return isFinished; } }
    public float InDistance { get { return inDistance; } set { inDistance = value; } }
    public float OutDistance { get { return outDistance; } set { outDistance = value; } }

    private TextMeshProUGUI placementText;
    private TextMeshProUGUI lapText;
    private TextMeshProUGUI directionText;

    private void Start()
    {
        respawn = GetComponent<Respawn>();
        InitHandler(); // TODO: change this so it works in character select screen
    }

    /// <summary>
    /// Inits the handler on startup. Assigns the first checkpoint on startup.
    /// </summary>
    public void InitHandler()
    {
        hasStarted = false;
        CheckpointManager.Instance.AdvanceCheckpoint(this, CheckpointManager.Instance.LastCheckpoint);
        uiHandler = playerMain.GetPlayerCanvas().transform.GetComponent<UIHandler>();
        placementText = uiHandler.Place;
        lapText = uiHandler.Lap;
        directionText = uiHandler.Dir;
        hasStarted = true;
    }

    private void Update()
    {
        //directionText.text = "";// wrongWay ? $"Wrong Way!" : "Right Way!"; // doesn't really work right now so I'm not gonna bother

        //distanceCheckCounter += Time.deltaTime;
        if(distanceCheckCounter > distanceCheckCooldown)
        {
            CheckDirection();
            distanceCheckCounter = 0;
        }
    }

    /// <summary>
    /// Checks if the player is facing the right direction.
    /// </summary>
    private void CheckDirection()
    {
        wrongWay = lastDist < distToCheckpoint;
    }

    /// <summary>
    /// Called when the player finishes a race. Checks if they're stunned and starts a coroutine
    /// </summary>
    public void CheckStun()
    {
        if (playerMain.isStunned)
        {
            StartCoroutine(WaitForStun());
        }
        else
        {
            FinishRace();
        }
    }

    /// <summary>
    /// Called when the player finishes a race.
    /// </summary>
    private void FinishRace()
    {
        isFinished = true;
        CheckpointManager.Instance.IncrementPlayersFinished(this);
    }

    private IEnumerator WaitForStun()
    {
        yield return new WaitUntil(() => playerMain.isStunned == false);
        FinishRace();
    }

    /// <summary>
    /// Assigns the legal respawn points based on current checkpoint.
    /// </summary>
    public void AssignRSPs(RespawnPoint[] inRSPs)
    {
        respawn.AssignRSPs(inRSPs);
    }
}

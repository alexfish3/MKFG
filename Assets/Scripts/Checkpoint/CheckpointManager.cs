using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum CheckpointType
{
    None,
    First,
    Last
};

public class CheckpointManager : SingletonMonobehaviour<CheckpointManager>
{
    [Header("Checkpoint Manager")]
    [SerializeField] private int totalLaps = 3;

    [Header("Other Information")]
    [SerializeField] private float postGameSeconds = 1f;
    private float tieDistance = 15f;

    [Header("Tie Breaker Handling")]
    [SerializeField] private Checkpoint stunnedCheckpoint;
    [SerializeField] private Checkpoint finishedCheckpoint;

    private Checkpoint[] checkpoints;
    private int maxLap = 0; // highest running lap, so if the player in first is on lap 2 this value will be 2
    private int totalUniqueCheckpoints = 0; // number of total checkpoints a player must hit (actual number might be higher if there are shortcuts)
    private int playersFinished = 0;
    private bool isTied;
    private float tiedTimer;
    private bool playerWon = false;
    private Vector3 neutral;
    private float stageToCheckpoint;
    private List<PlacementHandler> tiedPlayersAtFinish = new List<PlacementHandler>();

    // getters and setters
    public int TotalLaps { get { return totalLaps; } }
    public float TieDistance { get { return tieDistance; } }
    public int TotalCheckpoints { get { return totalUniqueCheckpoints; } }
    public Checkpoint FirstCheckpoint { get { return checkpoints[0]; } }
    public Checkpoint LastCheckpoint { get { return checkpoints[totalUniqueCheckpoints - 1]; } }
    public bool IsTied { get { return isTied; } }
    public Vector3 Neutral { get { return neutral; } }
    public float StageToCheckpoint { get { return stageToCheckpoint; } }

    // events
    public Action OnCheckpointInit;

    private void Start()
    {
        ReadRuleset();

        int currIndex = 0;
        checkpoints = transform.GetComponentsInChildren<Checkpoint>();

        for(int i=0;i<checkpoints.Length; i++)
        {
            string scLabel = " (SC)";
            if (!checkpoints[i].KeepIndex) // for non-shortcut checkpoints
            {
                checkpoints[i].Index = currIndex;
                currIndex++;
                totalUniqueCheckpoints++;
                scLabel = "";
            }
            checkpoints[i].transform.name = checkpoints[i].Index.ToString() + scLabel;
        }

        // giving reference to the checkpoint's next checkpoint, basically a linked list
        foreach(Checkpoint checkpoint in checkpoints)
        {
            checkpoint.NextCheckpoint = checkpoint.KeepIndex ? FindCheckpointWithIndex(checkpoint.Index + 1, true) : FindCheckpointWithIndex(checkpoint.Index+1);
        }

        // can init handlers now that checkpoints are initialized
        OnCheckpointInit?.Invoke();
    }

    private void Update()
    {
        int currPlace = 1; // init the first place
        int skippedPlayers = 0;
        int firstPlaceIndex = -1;
        List<PlacementHandler> playersInFirst = new List<PlacementHandler>();
        Checkpoint leadingCheckpoint = new Checkpoint();
        for (int lap = maxLap+1; lap >= 0; lap--) // check if the laps align
        {
            for (int i = checkpoints.Length - 1; i >= 0; i--) // loop through each checkpoint
            {
                for (int j = 0; j < checkpoints[i].PlayersTracking.Count; j++) // will pop closest players to checkpoint and work downwards
                {
                    try // award placement and accumulate currPlace
                    {
                        if (checkpoints[i].PlayersTracking[j].Lap == lap)
                        {
                            if (!checkpoints[i].PlayersTracking[j].PlacementLocked)
                            {
                                if (!playerWon) // nobody has finished the race yet
                                {
                                    if (currPlace == 1 && checkpoints[i].PlayersTracking[j].LocalPlacement == 1
                                        && (firstPlaceIndex == -1 || firstPlaceIndex == i)) // covers player in first and anyone tied with them
                                    {
                                        checkpoints[i].PlayersTracking[j].Placement = 1;
                                        playersInFirst.Add(checkpoints[i].PlayersTracking[j]);
                                        skippedPlayers++;
                                        firstPlaceIndex = i;
                                        leadingCheckpoint = checkpoints[i];
                                    }
                                    else if (skippedPlayers > 0) // players we're skipped so adjust accordingly
                                    {
                                        currPlace += skippedPlayers;
                                        skippedPlayers = 0;
                                        checkpoints[i].PlayersTracking[j].Placement = currPlace;
                                    }
                                    else // standard placement increments
                                    {
                                        currPlace++;
                                        checkpoints[i].PlayersTracking[j].Placement = currPlace;
                                    }

                                }
                                else // first already won so no point in tying
                                {
                                    checkpoints[i].PlayersTracking[j].Placement = currPlace;
                                    currPlace++;
                                }
                            }
                            else
                            {
                                currPlace++;
                            }
                        }
                    }
                    catch // for null PlacementHandlers that show up for unknown reasons >:(
                    {
                        continue;
                    }
                }
            }
        }
        if(playersInFirst.Count > 1)
        {
            tiedTimer = Constants.TOTAL_TIED_TIME;
        }

        if (isTied)
        {
            Vector3 sum = new Vector3();
            foreach (PlacementHandler ph in playersInFirst)
            {
                sum += ph.transform.position;
            }
            neutral = sum / playersInFirst.Count;
            stageToCheckpoint = Vector3.Distance(neutral, leadingCheckpoint.transform.position);
        }
        else
        {
            neutral = Vector3.zero;
            stageToCheckpoint = 0;
        }

        isTied = tiedTimer >= 0f;
        tiedTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Set the next checkpoint to track the player.
    /// </summary>
    /// <param name="playerGO">Player to be tracked</param>
    /// <param name="checkpointIndx">Index of their checkpoint</param>
    public void AdvanceCheckpoint(PlacementHandler playerGO, Checkpoint checkpoint)
    {
        Checkpoint newCheckpoint = checkpoint.NextCheckpoint;
        
        if(newCheckpoint != checkpoints[0])
        {
            playerGO.HasStarted = true;
        }

        if(newCheckpoint.Index > checkpoint.Index)
        {
            playerGO.CheckpointsThisLap--;
        }
        else
        {
            if(playerGO.CheckpointsThisLap <= 1)
            {
                playerGO.Lap++;
                if(playerGO.Lap > totalLaps) // player has finished the race
                {
                    VerifyVictory(playerGO);
                    return;
                }
                if (playerGO.Lap > maxLap) // new lap unlocked
                {
                    maxLap = playerGO.Lap;
                }
            }
            playerGO.CheckpointsThisLap = TotalCheckpoints;
        }
        newCheckpoint.AddPlayer(playerGO);
    }

    /// <summary>
    /// Finds a checkpoint of specified index.
    /// </summary>
    /// <param name="index">Index of checkpoint being searched for.</param>
    /// <param name="checkShortcuts">Will search through shortcut checkpoints first if true, then normal checkpoints. Doesn't search SC's if false</param>
    /// <returns></returns>
    public Checkpoint FindCheckpointWithIndex(int index, bool checkShortcuts = false)
    {
        Checkpoint outCheckpoint = null;

        // check shortcuts first. will return a checkpoint of passed in index with true keepIndex if one exists
        if (checkShortcuts)
        {
            foreach (Checkpoint checkpoint in checkpoints)
            {
                if (checkpoint.Index == index && checkpoint.KeepIndex)
                {
                    outCheckpoint = checkpoint;
                }
            }
        }
        
        if(outCheckpoint == null) // if a shortcut checkpoint can't be found or !checkShortcuts then will try and find a normal checkpoint with the index
        {
            foreach (Checkpoint checkpoint in checkpoints)
            {
                if (checkpoint.Index == index && !checkpoint.KeepIndex)
                {
                    outCheckpoint = checkpoint;
                }
            }
        }

        if(outCheckpoint == null) // if no checkpoint is found return the first checkpoint
        {
            outCheckpoint = checkpoints[0];
        }

        return outCheckpoint;
    }

    /// <summary>
    /// Reads the game ruleset from GameManager.
    /// </summary>
    private void ReadRuleset()
    {
        RulesetSO ruleset = GameManagerNew.Instance.Ruleset;
        if (GameManagerNew.Instance.CurrMap.GetMapType() == MapType.Straight)
        {
            totalLaps = 1;
        }
        else if(!ruleset.OverrideLaps)
        {
            totalLaps = GameManagerNew.Instance.CurrMap.GetDefaultLaps();
        }
        else
        {
            totalLaps = ruleset.NumOfLaps;
        }
    }

    /// <summary>
    /// Called when every player finishes a race. Checks for tiebreakers.
    /// </summary>
    private void CheckFinishConditions()
    {
        List<PlacementHandler> finishedPlayers = GameManagerNew.Instance.GetPlacementList();
        int firstPlacePlayers = 0;
        for(int i=0;i<finishedPlayers.Count;i++)
        {
            if (finishedPlayers[i].Placement == 1)
            {
                firstPlacePlayers++;
            }
            else
            {
                break;
            }
        }
        StartCoroutine(PostGameClarity(postGameSeconds));
    }

    private IEnumerator PostGameClarity(float seconds)
    {
        Debug.Log("Swap game to results");
        SoundManager.Instance.ChangeSnapshot("low");
        yield return new WaitForSeconds(seconds);
        SoundManager.Instance.ChangeSnapshot("default");
        GameManagerNew.Instance.SetGameState(GameStates.Results);
    }

    void VerifyVictory(PlacementHandler ph) // I adore alliteration
    {
        if(ph.IsStunned)
        {
            Debug.Log("Player is stunned");
            PlacementHandler stunnedPlayer = ph;
            ManuallyAddPlayer(stunnedPlayer, stunnedCheckpoint);
            stunnedPlayer.OnStunDropped += () => ManuallyRemovePlayer(stunnedPlayer, stunnedCheckpoint);
            stunnedPlayer.OnStunDropped += () => PlayerFinishedRace(stunnedPlayer);
            stunnedPlayer.SetUpStun();
        }
        else
        {
            Debug.Log("Player is NOT stunned");
            PlayerFinishedRace(ph);
        }
    }

    private void PlayerFinishedRace(PlacementHandler ph)
    {
        if(!playerWon)
        {
            ph.Placement = 1;
            if (!isTied)
            {
                playerWon = true;
            }
        }
        ph.FinishRace();
        ManuallyAddPlayer(ph, finishedCheckpoint);
        playersFinished++;
        GameManagerNew.Instance.AddFinishedPlayer(ph);
        if (playersFinished >= PlayerSpawnSystem.Instance.ActiveBrains.Count)
        {
            CheckFinishConditions();
        }
    }

    public void ManuallyAddPlayer(PlacementHandler ph, Checkpoint checkpoint)
    {
        checkpoint.AddPlayer(ph);
    }

    public void ManuallyRemovePlayer(PlacementHandler ph, Checkpoint checkpoint)
    {
        checkpoint.SneakyRemovePlayer(ph);
    }
}

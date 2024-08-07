using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManagerNew : SingletonMonobehaviour<GameManagerNew>
{
    [SerializeField] GameStates beginingGameState = GameStates.MainMenu;

    [Space(10)]
    [SerializeField] private GameStates currentState = GameStates.Default;

    [Space(10)]

    [Header("Game Information")]
    [SerializeField] bool isPaused;
    public bool IsPaused 
    { 
        get => isPaused;
    }

    [SerializeField] private RulesetSO ruleset;
    private MapInformationSO currMap;

    // getters setters
    public GameStates CurrentState { get { return currentState; } }
    public RulesetSO Ruleset { get { return ruleset; } }
    public MapInformationSO CurrMap { get { return currMap; } set { currMap = value; } }

    [SerializeField] private List<PlacementHandler> placementList = new List<PlacementHandler>();

    [Header("Name Info")]
    [SerializeField] TextAsset namesFile;
    [SerializeField] string[] loadedNames;
    public string[] LoadedNames
    {
        get => loadedNames;
    }

    [Header("Debug")]
    [SerializeField] bool toggleSwapOfGamestate = false;

    // game state events
    public event Action OnSwapEnterMenu;
    public event Action OnSwapGameModeSelect;
    public event Action OnSwapOptions;
    public event Action OnSwapPlayerSelect;
    public event Action OnSwapMapSelect;
    public event Action OnSwapRuleSelect;
    public event Action OnSwapLoading;

    public event Action OnSwapLoadMatch;
    public event Action OnSwapMainLoop;
    public event Action OnSwapTiebreaker;

    public event Action OnSwapPaused;

    public event Action OnSwapResults;
    public event Action OnSwapLoadMenu;

    public event Action<GameStates> SwappedGameState;

    /// <summary>
    /// Loads the names to be used in the game from the serialized text file
    /// </summary>
    public void LoadNamesFile()
    {
        loadedNames = namesFile.ToString().Split('\n');

        //foreach(string name in loadedNames)
        //{
        //    Debug.Log(name);
        //}
    }


    public void Start()
    {
        OnSwapEnterMenu += () => SoundManager.Instance.SetMusic("music_menu");
        OnSwapResults += () => SoundManager.Instance.SetMusic("music_results");

        SetGameState(beginingGameState);
        LoadNamesFile();
    }

    private void OnEnable()
    {
        OnSwapPaused += () => isPaused = true;
        OnSwapMainLoop += () => isPaused = false;
        OnSwapMainLoop += placementList.Clear;
    }

    private void GameManagerNew_OnSwapPaused()
    {
        throw new NotImplementedException();
    }

    private void OnDisable()
    {
        OnSwapEnterMenu -= () => SoundManager.Instance.SetMusic("music_menu");
        OnSwapResults -= () => SoundManager.Instance.SetMusic("music_results");

        OnSwapPaused -= () => isPaused = true;
        OnSwapMainLoop -= () => isPaused = false;
        OnSwapMainLoop -= placementList.Clear;
    }

    public void Update()
    {
        if (toggleSwapOfGamestate)
        {
            SetGameState(currentState);
            toggleSwapOfGamestate = false;
        }
    }

    ///<summary>
    /// Allows swapping of game states, and also invokes right event when swapping
    ///</summary>
    public void SetGameState(GameStates state)
    {
        Debug.Log($"Swap Game State To: {state.ToString()}");
        Time.timeScale = 1f;

        currentState = state;

        // Calls an event for the certain state that is swapped to
        switch(currentState)
        {
            case GameStates.MainMenu:
                OnSwapEnterMenu?.Invoke();
                break;
            case GameStates.GameModeSelect:
                OnSwapGameModeSelect?.Invoke();
                break;
            case GameStates.Options:
                OnSwapOptions?.Invoke();
                break;
            case GameStates.CharacterSelect:
                OnSwapPlayerSelect?.Invoke();
                break;
            case GameStates.MapSelect:
                OnSwapMapSelect?.Invoke();
                break;
            case GameStates.RuleSelect: 
                OnSwapRuleSelect?.Invoke();
                break;
            case GameStates.Loading:
                OnSwapLoading?.Invoke();
                break;
            case GameStates.LoadMatch:
                OnSwapLoadMatch?.Invoke();
                break;
            case GameStates.MainLoop:
                OnSwapMainLoop?.Invoke();
                break;
            case GameStates.Pause:
                OnSwapPaused?.Invoke();
                break;
            case GameStates.Results:
                placementList.Sort((i,j) => i.Placement.CompareTo(j.Placement));
                OnSwapResults?.Invoke();
                break;
            case GameStates.LoadMainMenu:
                OnSwapLoadMenu?.Invoke();
                break;
            case GameStates.Tiebreaker:
                OnSwapTiebreaker?.Invoke();
                break;
            default:
                break;
        }

        // Invokes the event that state was swapped, sending the new state
        SwappedGameState?.Invoke(state);
    }

    /// <summary>
    /// Sets the ruleset to passed in set.
    /// </summary>
    /// <param name="inSet">New ruleset</param>
    public void SetRuleset(RulesetSO inSet)
    {
        if (ruleset == inSet)
            return;

        ruleset = inSet;
    }

    public void AddFinishedPlayer(PlacementHandler inPH)
    {
        if(!placementList.Contains(inPH))
        {
            placementList.Add(inPH);
        }
    }

    public List<PlacementHandler> GetPlacementList()
    {
        return placementList;
    }
}

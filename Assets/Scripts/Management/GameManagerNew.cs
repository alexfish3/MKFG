using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerNew : SingletonMonobehaviour<GameManagerNew>
{
    [SerializeField] GameStates beginingGameState = GameStates.MainMenu;

    [Space(10)]
    [SerializeField] private GameStates currentState = GameStates.Default;

    [Space(10)]

    [Header("Game Information")]
    [SerializeField] private RulesetSO ruleset;
    private MapType currMapType;

    // getters setters
    public GameStates CurrentState { get { return currentState; } }
    public RulesetSO Ruleset { get { return ruleset; } }
    public MapType CurrMapType { get { return currMapType; } set { currMapType = value; } }

    private List<PlacementHandler> placementList = new List<PlacementHandler>();

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

    public event Action OnSwapPaused;

    public event Action OnSwapResults;
    public event Action OnSwapLoadMenu;

    public event Action<GameStates> SwappedGameState;

    public void Start()
    {
        SetGameState(beginingGameState);
    }

    private void OnEnable()
    {
        OnSwapEnterMenu += () => SoundManager.Instance.SetMusic("music_menu");
        OnSwapResults += () => SoundManager.Instance.SetMusic("music_results");
        OnSwapMainLoop += placementList.Clear;
    }

    private void OnDisable()
    {
        OnSwapEnterMenu -= () => SoundManager.Instance.SetMusic("music_menu");
        OnSwapResults += () => SoundManager.Instance.SetMusic("music_results");
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
            case GameStates.PlayerSelect:
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
            case GameStates.Results:
                OnSwapResults?.Invoke();
                break;
            case GameStates.LoadMainMenu:
                OnSwapLoadMenu?.Invoke();
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

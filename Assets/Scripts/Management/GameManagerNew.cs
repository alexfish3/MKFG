using DG.Tweening;
using System;
using UnityEngine;

public class GameManagerNew : SingletonMonobehaviour<GameManagerNew>
{
    [SerializeField] GameStates beginingGameState = GameStates.MainMenu;

    [Space(10)]
    [SerializeField] private GameStates currentState = GameStates.Default;

    [Space(10)]

    [SerializeField] private RulesetSO ruleset;

    public GameStates CurrentState { get { return currentState; } }
    public RulesetSO Ruleset { get { return ruleset; } }

    [Header("Debug")]
    [SerializeField] bool toggleSwapOfGamestate = false;

    // game state events
    public event Action OnSwapMenu;
    public event Action OnSwapGameModeSelect;
    public event Action OnSwapOptions;
    public event Action OnSwapPlayerSelect;
    public event Action OnSwapMapSelect;
    public event Action OnSwapRuleSelect;
    public event Action OnSwapLoading;

    public event Action OnSwapBegin;
    public event Action OnSwapMainLoop;
    public event Action OnSwapPaused;

    public event Action<GameStates> SwappedGameState;

    public void Start()
    {
        SetGameState(beginingGameState);

        OnSwapBegin += () => SoundManager.Instance.SetMusic("music_default_loop");
    }

    private void OnDisable()
    {
        OnSwapBegin -= () => SoundManager.Instance.SetMusic("music_default_loop");
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
                OnSwapMenu?.Invoke();
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
            case GameStates.Begin:
                OnSwapBegin?.Invoke();
                break;
            case GameStates.MainLoop:
                OnSwapMainLoop?.Invoke();
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
}
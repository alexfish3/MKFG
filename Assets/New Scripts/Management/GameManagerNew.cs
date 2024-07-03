using DG.Tweening;
using System;
using UnityEngine;

public class GameManagerNew : SingletonMonobehaviour<GameManagerNew>
{
    [SerializeField] GameStates beginingGameState = GameStates.MainMenu;

    [Space(10)]
    [SerializeField] private GameStates currentState = GameStates.Default;
    public GameStates CurrentState { get { return currentState; } }

    [Header("Debug")]
    [SerializeField] bool toggleSwapOfGamestate = false;

    // game state events
    public event Action OnSwapMenu;
    public event Action OnSwapGameModeSelect;
    public event Action OnSwapOptions;
    public event Action OnSwapPlayerSelect;
    public event Action OnSwapLoading;
    public event Action OnSwapBegin;
    public event Action OnSwapMainLoop;
    public event Action OnSwapPaused;

    public event Action<GameStates> SwappedGameState;

    public void Start()
    {
        SetGameState(beginingGameState);
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
}

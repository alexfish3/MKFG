using DG.Tweening;
using System;
using UnityEngine;

public enum GameStateNew { 
    MainMenu,
    GameModeSelect,
    Options,
    PlayerSelect,
    Loading,

    Begin,
    MainLoop,
    
    Paused,
    Default 
}

public class GameManagerNew : SingletonMonobehaviour<GameManagerNew>
{
    [SerializeField] GameStateNew beginingGameState = GameStateNew.PlayerSelect;

    [Space(10)]
    [SerializeField] private GameStateNew currentState = GameStateNew.Default;
    public GameStateNew CurrentState { get { return currentState; } }

    // game state events
    public event Action OnSwapMenu;
    public event Action OnSwapGameModeSelect;
    public event Action OnSwapOptions;
    public event Action OnSwapPlayerSelect;
    public event Action OnSwapLoading;
    public event Action OnSwapBegin;
    public event Action OnSwapMainLoop;

    public void Start()
    {
        SetGameState(beginingGameState);
    }

    ///<summary>
    /// Allows swapping of game states, and also invokes right event when swapping
    ///</summary>
    public void SetGameState(GameStateNew state)
    {
        Time.timeScale = 1f;

        currentState = state;

        // Calls an event for the certain state that is swapped to
        switch(currentState)
        {
            case GameStateNew.MainMenu:
                OnSwapMenu?.Invoke();
                break;
            case GameStateNew.GameModeSelect:
                OnSwapGameModeSelect?.Invoke();
                break;
            case GameStateNew.Options:
                OnSwapOptions?.Invoke();
                break;
            case GameStateNew.PlayerSelect:
                OnSwapPlayerSelect?.Invoke();
                break;
            case GameStateNew.Loading:
                OnSwapLoading?.Invoke();
                break;
            case GameStateNew.Begin:
                OnSwapBegin?.Invoke();
                break;
            case GameStateNew.MainLoop:
                OnSwapMainLoop?.Invoke();
                break;
            default:
                break;
        }
    }
}

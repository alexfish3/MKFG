using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuButtonMethods : MonoBehaviour
{
    public void MenuButtonPressed()
    {
        GameManagerNew.Instance.SetGameState(GameStates.LoadMainMenu);
    }

    public void ResumeButtonPressed()
    {
        GameManagerNew.Instance.SetGameState(GameStates.MainLoop);
    }
}

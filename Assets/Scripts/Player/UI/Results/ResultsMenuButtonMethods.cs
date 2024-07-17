using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsMenuButtonMethods : MonoBehaviour
{
    public void MenuButtonPressed()
    {
        GameManagerNew.Instance.SetGameState(GameStates.MainMenu);
    }

    public void QuitButtonPressed()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
                    Application.Quit();
        #endif
    }
}

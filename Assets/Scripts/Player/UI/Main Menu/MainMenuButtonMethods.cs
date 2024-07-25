using UnityEngine;

public class MainMenuButtonMethods : MonoBehaviour
{
    public void PlayButtonPressed()
    {
        GameManagerNew.Instance.SetGameState(GameStates.GameModeSelect);
    }

    public void SettingsButtonPressed()
    {
        //GameManagerNew.Instance.SetGameState(GameStates.Options);
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

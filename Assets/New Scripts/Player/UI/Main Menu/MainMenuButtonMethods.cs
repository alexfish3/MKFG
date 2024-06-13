using UnityEngine;

public class MainMenuButtonMethods : MonoBehaviour
{
    public void PlayButtonPressed()
    {
        Debug.Log("Play button has been pressed");
    }

    public void SettingsButtonPressed()
    {
        Debug.Log("Settings button has been pressed");
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

using UnityEngine;

public class MainMenuButtonMethods : MonoBehaviour
{
    [Header("Game Object")]

    [SerializeField] private GameObject gameModeSelectObject;

    [SerializeField] private GameObject mainMenuObject;

    [SerializeField] private GameObject settingsMenuObject;

    public void PlayButtonPressed()
    {
        gameModeSelectObject.SetActive(true);
        mainMenuObject.SetActive(false);

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

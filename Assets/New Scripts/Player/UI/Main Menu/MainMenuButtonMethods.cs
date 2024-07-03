using UnityEngine;

public class MainMenuButtonMethods : MonoBehaviour
{
    [Header("Game Object")]

    [SerializeField] private Canvas gamemodeSelectCanvas;

    [SerializeField] private Canvas mainMenuCanvas;

    [SerializeField] private Canvas settingsMenuCanvas;

    public void PlayButtonPressed()
    {
        Debug.Log("Play button has been pressed... Entering Game Mode Select");
        gamemodeSelectCanvas.enabled = true;
        mainMenuCanvas.enabled = false;

        GameManagerNew.Instance.SetGameState(GameStates.GameModeSelect);
    }

    public void SettingsButtonPressed()
    {
        Debug.Log("Setttings button has been pressed... Entering Settings");
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

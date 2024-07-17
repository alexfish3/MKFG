using UnityEngine;

public class GameModeSelectButtonMethods : MonoBehaviour
{
    [Header("Game Object")]

    [SerializeField] private Canvas gameModeSelectCanvas;
    [SerializeField] private Canvas mainMenuCanvas;
    [SerializeField] private Canvas settingsMenuCanvas;
    [SerializeField] private Canvas characterSelectCanvas;

    [Header("Sub scene caategories")]

    [SerializeField] private GameObject onlineObject;

    [SerializeField] private GameObject offlineObject;

    public void OnlineButtonPressed()
    {
        Debug.Log("Online button has been pressed");
    }

    public void OfflineButtonPressed()
    {
        Debug.Log("Offline button has been pressed");

        characterSelectCanvas.GetComponent<GenericUI>().InitalizeUI();
        characterSelectCanvas.enabled = true;
        gameModeSelectCanvas.enabled = false;

        GameManagerNew.Instance.SetGameState(GameStates.PlayerSelect);
    }

    public void BackButtonPressed()
    {
        Debug.Log("Back button has been pressed");

        //mainMenuCanvas.GetComponent<GenericUI>().InitalizeUI();
        mainMenuCanvas.enabled = true;
        gameModeSelectCanvas.enabled = false;

        GameManagerNew.Instance.SetGameState(GameStates.MainMenu);
    }
}

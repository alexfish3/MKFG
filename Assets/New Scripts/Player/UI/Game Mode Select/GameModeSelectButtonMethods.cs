using UnityEngine;

public class GameModeSelectButtonMethods : MonoBehaviour
{
    [Header("Game Object")]

    [SerializeField] private GameObject gameModeSelectObject;

    [SerializeField] private GameObject mainMenuObject;

    [SerializeField] private GameObject settingsMenuObject;

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
    }

    public void BackButtonPressed()
    {
        Debug.Log("Back button has been pressed");
    }
}

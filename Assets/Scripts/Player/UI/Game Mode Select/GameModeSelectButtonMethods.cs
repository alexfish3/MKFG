using UnityEngine;

public class GameModeSelectButtonMethods : MonoBehaviour
{
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

        GameManagerNew.Instance.SetGameState(GameStates.CharacterSelect);
    }

    public void BackButtonPressed()
    {
        Debug.Log("Back button has been pressed");

        GameManagerNew.Instance.SetGameState(GameStates.MainMenu);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultsMenu : MonoBehaviour
{
    [SerializeField] private Button[] placementButtons;
    private TextMeshProUGUI[] placementText;
    // Start is called before the first frame update
    void Start()
    {
        placementText = new TextMeshProUGUI[placementButtons.Length];
        for(int i=0; i<placementButtons.Length; i++)
        {
            placementText[i] = placementButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        }

        InitResultsMenu();
        GameManagerNew.Instance.SetGameState(GameStates.Results);       
    }

    private void InitResultsMenu()
    {
        List<PlacementHandler> players = GameManagerNew.Instance.GetPlacementList();

        for(int i=0;i<players.Count; i++)
        {
            placementButtons[i].gameObject.SetActive(true);
            placementText[i].text = $"{i+1}. {players[i].name}";
        }
    }
}

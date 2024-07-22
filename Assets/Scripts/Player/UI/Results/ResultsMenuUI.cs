using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsMenuUI : SingletonGenericUI<ResultsMenuUI>
{
    [Header("Main Menu UI Info")]
    [SerializeField] MenuHighlight buttonSelector;
    [SerializeField] List<GameObject> buttons = new List<GameObject>();

    [Space(10)]
    [Header("Results Info")]
    [SerializeField] GameObject resultsPodium;
    [SerializeField] List<ResultsPodium> resultsPoidiums = new List<ResultsPodium>();
    [SerializeField] GameObject podiumParent;

    public void Start()
    {
        GameManagerNew.Instance.OnSwapResults += InitResultsMenu;
    }

    public void OnDisable()
    {
        GameManagerNew.Instance.OnSwapResults -= InitResultsMenu;
    }

    public override void InitalizeUI()
    {
        buttonSelector.SetSelectorPosition(buttons[0], 0);
    }

    public override void Up(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        MovePlayerSelector(player.GetPlayerID(), Direction.Up);
    }

    public override void Left(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        MovePlayerSelector(player.GetPlayerID(), Direction.Left);
    }

    public override void Down(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        MovePlayerSelector(player.GetPlayerID(), Direction.Down);
    }

    public override void Right(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        MovePlayerSelector(player.GetPlayerID(), Direction.Right);
    }

    public void MovePlayerSelector(int playerID, Direction direction)
    {
        if (!DetermineIfPlayerCanInputInUI(playerID))
            return;

        int playerSelectorCurrentPosition = buttonSelector.selectorPosition;
        int newPos = 0;

        // Handle clicking left or up
        if (direction == Direction.Left && playerSelectorCurrentPosition - 1 > 0 || direction == Direction.Up && playerSelectorCurrentPosition - 1 > 0)
        {
            newPos = playerSelectorCurrentPosition - 1;
        }
        else if (direction == Direction.Left && playerSelectorCurrentPosition - 1 <= 0 || direction == Direction.Up && playerSelectorCurrentPosition - 1 <= 0)
        {
            // Do nothing
            newPos = 0;
        }

        // Handle clicking right
        if (direction == Direction.Right && playerSelectorCurrentPosition + 1 < buttons.Count - 1 || direction == Direction.Down && playerSelectorCurrentPosition + 1 < buttons.Count - 1)
        {
            newPos = playerSelectorCurrentPosition + 1;
        }
        else if (direction == Direction.Right && playerSelectorCurrentPosition + 1 >= buttons.Count - 1 || direction == Direction.Down && playerSelectorCurrentPosition + 1 >= buttons.Count - 1)
        {
            // Do Nothing
            newPos = buttons.Count - 1;
        }

        if(buttonSelector != null)
            buttonSelector.SetSelectorPosition(buttons[newPos], newPos);
    }

    public override void Confirm(bool status, GenericBrain player) // L key is confirm for some reason
    {
        if (status == false)
            return;

        int playerID = player.GetPlayerID();
        if (!DetermineIfPlayerCanInputInUI(playerID))
            return;

        buttons[buttonSelector.selectorPosition].GetComponent<Button>().onClick.Invoke();
        // Run button method
    }

    public override void Return(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        int playerID = player.GetPlayerID();
        if (!DetermineIfPlayerCanInputInUI(playerID))
            return;

        // Return to previous menu
    }

    /// <summary>
    /// Initalizes results menu based on positions
    /// </summary>
    private void InitResultsMenu()
    {
        List<PlacementHandler> players = GameManagerNew.Instance.GetPlacementList();

        for (int i = 0; i < players.Count; i++)
        {
            var podiumSpace = Instantiate(resultsPodium, podiumParent.transform).GetComponent<ResultsPodium>();
            resultsPoidiums.Add(podiumSpace);

            PodiumStats currentPlayerStats = players[i].PlayerMain.playerMatchStats.Stats;
            podiumSpace.InitalizeResultsPodium(players[i].Placement, currentPlayerStats);
        }
    }
}


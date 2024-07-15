using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : SingletonGenericUI<MainMenuUI>
{
    [Header("Main Menu UI Info")]
    [SerializeField] List<GameObject> buttons = new List<GameObject>();

    [Space(10)]
    [SerializeField] MenuHighlight buttonSelector;

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
}

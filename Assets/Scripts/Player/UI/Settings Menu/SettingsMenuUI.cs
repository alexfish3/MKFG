using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuUI : SingletonGenericUI<SettingsMenuUI>
{
    [Header("Settings Menu UI Info")]
    [SerializeField] List<GameObject> buttons = new List<GameObject>();

    [Space(10)]
    [SerializeField] MenuHighlight buttonSelector;


    [Header("Control Settings")]
    [Space(10)]
    [SerializeField] List<InputProfileSO> inputProfiles;

    [Space(10)]
    [SerializeField] GameObject[] availableButtons;


    private int maxInputProfiles = 7;
    public override void InitalizeUI()
    {
        // Get a list of input profiles

        // Display list
        for (int i = 0; i < availableButtons.Length; i++)
        {
            if (inputProfiles[i] == null || i >= maxInputProfiles) break;

            // Enable button
            availableButtons[i].SetActive(true);

            // Pass input profile info to button
            availableButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = inputProfiles[i].name;
        }

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
        if (direction == Direction.Left || direction == Direction.Up)
        {
            newPos = playerSelectorCurrentPosition - 1 < 0 ? playerSelectorCurrentPosition = buttons.Count - 1 : playerSelectorCurrentPosition - 1;
        }

        // Handle clicking right or down
        if (direction == Direction.Right || direction == Direction.Down)
        {
            newPos = playerSelectorCurrentPosition + 1 > buttons.Count - 1 ? 0 : playerSelectorCurrentPosition + 1;
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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : SingletonGenericUI<MainMenuUI>
{
    [Header("Main Menu UI Info")]
    [SerializeField] List<GameObject> buttons = new List<GameObject>();
    [SerializeField] HelperButtonUI helperButtonUI;

    [Space(10)]
    [SerializeField] MenuHighlight buttonSelector;

    private void OnEnable()
    {
        GameManagerNew.Instance.OnSwapEnterMenu += InitalizeUI;
    }

    private void OnDisable()
    {
        GameManagerNew.Instance.OnSwapEnterMenu -= InitalizeUI;
    }

    public override void AddPlayerToUI(GenericBrain player)
    {
        base.AddPlayerToUI(player);

        if (player.GetPlayerID() == 0)
            helperButtonUI.SetUIIconToMatchBrainType(player.GetBrainInputType());
    }

    private void Start()
    {
        GameManagerNew.Instance.SetGameState(GameStates.MainMenu);
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
        if (direction == Direction.Left || direction == Direction.Up)
        {
            newPos = playerSelectorCurrentPosition - 1 < 0 ? playerSelectorCurrentPosition = buttons.Count - 1 : playerSelectorCurrentPosition - 1;
        }

        // Handle clicking right or down
        if (direction == Direction.Right || direction == Direction.Down)
        {
            newPos = playerSelectorCurrentPosition + 1 > buttons.Count - 1 ? 0 : playerSelectorCurrentPosition + 1;
        }

        Debug.Log("new pos is: " + newPos);
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

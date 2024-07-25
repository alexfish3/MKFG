using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : GenericUI
{
    [Header("Pause Menu UI Info")]
    GameManagerNew gameManagerNew;

    [SerializeField] GameObject drivingMenu;
    [SerializeField] GameObject hostPauseMenu;
    [SerializeField] GameObject subPauseMenu;

    [SerializeField] PauseType currentPauseType = PauseType.Sub;
    public PauseType CurrentPauseType {  get { return currentPauseType; } set { currentPauseType = value; } }

    [SerializeField] List<GameObject> buttons = new List<GameObject>();

    [Space(10)]
    [SerializeField] MenuHighlight buttonSelector;

    public void OnEnable()
    {
        GameManagerNew.Instance.OnSwapMainLoop += DisablePause;
    }

    public void OnDisable()
    {
        GameManagerNew.Instance.OnSwapMainLoop -= DisablePause;
    }

    public override void AddPlayerToUI(GenericBrain player)
    {
        InitalizeUI();
        base.AddPlayerToUI(player);
    }

    public override void RemovePlayerUI(GenericBrain player)
    {
        base.RemovePlayerUI(player);
    }

    public override void InitalizeUI()
    {
        if (gameManagerNew == null)
        {
            gameManagerNew = GameManagerNew.Instance;
        }

        if(currentPauseType == PauseType.Host)
        {
            hostPauseMenu.SetActive(true);
        }
        else if (currentPauseType == PauseType.Sub)
        {
            subPauseMenu.SetActive(true);
        }

        buttonSelector.SetSelectorPosition(buttons[0], 0);
    }

    public override void Up(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        MovePlayerSelector(player, Direction.Up);
    }

    public override void Left(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        MovePlayerSelector(player, Direction.Left);
    }

    public override void Down(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        MovePlayerSelector(player, Direction.Down);
    }

    public override void Right(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        MovePlayerSelector(player, Direction.Right);
    }

    public override void Pause(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if(currentPauseType != PauseType.Host)
            return;

        // Unpausing the game
        if (gameManagerNew.IsPaused == true)
        {
            // When unpausing, reset the player's pause status to sub, and deactivate the menu
            gameManagerNew.SetGameState(GameStates.MainLoop);
        }
    }

    private void DisablePause()
    {
        currentPauseType = PauseType.Sub;

        if(hostPauseMenu != null)
            hostPauseMenu.SetActive(false);

        if (subPauseMenu != null)
            subPauseMenu.SetActive(false);

        drivingMenu.SetActive(true);
    }


    public void MovePlayerSelector(GenericBrain player, Direction direction)
    {
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

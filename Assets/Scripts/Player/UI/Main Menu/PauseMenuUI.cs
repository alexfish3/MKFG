using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : GenericUI
{
    [Header("Pause Menu UI Info")]
    GameManagerNew gameManagerNew;

    [SerializeField] GameObject pauseMenu;

    [SerializeField] PauseType currentPauseType = PauseType.Sub;
    public PauseType CurrentPauseType {  get { return currentPauseType; } set { currentPauseType = value; } }

    //[SerializeField] List<GameObject> buttons = new List<GameObject>();

    //[Space(10)]
    //[SerializeField] MenuHighlight buttonSelector;

    public void OnEnable()
    {
        
    }

    public void OnDisable()
    {
        
    }

    public override void AddPlayerToUI(GenericBrain player)
    {
        base.AddPlayerToUI(player);
        InitalizeUI();
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
            pauseMenu.SetActive(true);
        }
        else if (currentPauseType == PauseType.Sub)
        {

        }
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

        if (currentPauseType != PauseType.Host)
            return;

        // Unpausing the game
        if (gameManagerNew.IsPaused == true)
        {
            // When unpausing, reset the player's pause status to sub, and deactivate the menu
            currentPauseType = PauseType.Sub;
            pauseMenu.SetActive(false);
            gameManagerNew.SetGameState(GameStates.MainLoop);
        }
    }

    public void MovePlayerSelector(GenericBrain player, Direction direction)
    {
        //int playerSelectorCurrentPosition = buttonSelector.selectorPosition;
        //int newPos = 0;

        //// Handle clicking left or up
        //if (direction == Direction.Left || direction == Direction.Up)
        //{
        //    newPos = playerSelectorCurrentPosition - 1 < 0 ? playerSelectorCurrentPosition = buttons.Count - 1 : playerSelectorCurrentPosition - 1;
        //}

        //// Handle clicking right or down
        //if (direction == Direction.Right || direction == Direction.Down)
        //{
        //    newPos = playerSelectorCurrentPosition + 1 > buttons.Count - 1 ? 0 : playerSelectorCurrentPosition + 1;
        //}

        //Debug.Log("new pos is: " + newPos);
        //buttonSelector.SetSelectorPosition(buttons[newPos], newPos);
    }

    public override void Confirm(bool status, GenericBrain player) // L key is confirm for some reason
    {
        if (status == false)
            return;

        int playerID = player.GetPlayerID();
        if (!DetermineIfPlayerCanInputInUI(playerID))
            return;

        //buttons[buttonSelector.selectorPosition].GetComponent<Button>().onClick.Invoke();
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

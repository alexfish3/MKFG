using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectUI : SingletonGenericUI<CharacterSelectUI>
{
    [Header("Player Select UI Info")]
    [SerializeField] List<GameObject> characterIcons = new List<GameObject>();
    [SerializeField] int numberInRowsNormally;

    [Space(10)]
    [SerializeField] GameObject playerSelector;
    [SerializeField] List<CharacterSelectorGameobject> playerSelectors = new List<CharacterSelectorGameobject>();

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    public override void AddPlayerToUI(GenericBrain player)
    {
        var newSelector = Instantiate(playerSelector, this.transform).GetComponent<CharacterSelectorGameobject>();
        newSelector.Initialize(player.getPlayerID(), player.getDeviceID());
        newSelector.SetDefaultPosition(characterIcons[0]);

        playerSelectors.Add(newSelector);

        base.AddPlayerToUI(player);
    }

    public override void RemovePlayerUI(GenericBrain player)
    {
        CharacterSelectorGameobject selectorToRemove = null;

        foreach (var playerSelector in playerSelectors)
        {
            if(playerSelector.deviceID == player.getDeviceID())
            {
                selectorToRemove = playerSelector;
            }
        }

        playerSelectors.Remove(selectorToRemove);

        Destroy(selectorToRemove.gameObject);

        base.RemovePlayerUI(player);
    }

    public void MovePlayerSelector(int playerID, Direction direction) 
    {
        foreach(CharacterSelectorGameobject playerSelector in playerSelectors)
        {
            if(playerSelector.playerID == playerID)
            {

                int playerSelectorCurrentPosition = playerSelector.selectorPosition;
                int newPos = 0;

                // Handle clicking left
                if (direction == Direction.Left && playerSelectorCurrentPosition - 1 > 0)
                {
                    newPos = playerSelectorCurrentPosition - 1;
                }
                else if (direction == Direction.Left && playerSelectorCurrentPosition - 1 <= 0)
                {
                    // Do nothing
                    newPos = 0;
                }

                // Handle clicking right
                if (direction == Direction.Right && playerSelectorCurrentPosition + 1 < characterIcons.Count - 1)
                {
                    newPos = playerSelectorCurrentPosition + 1;
                }
                else if (direction == Direction.Right && playerSelectorCurrentPosition + 1 >= characterIcons.Count - 1)
                {
                    // Do Nothing
                    newPos = characterIcons.Count - 1;
                }

                // Handle clicking up
                if (direction == Direction.Up && playerSelectorCurrentPosition - numberInRowsNormally >= 0)
                {
                    newPos = playerSelectorCurrentPosition - numberInRowsNormally;
                }
                else if (direction == Direction.Up && playerSelectorCurrentPosition - numberInRowsNormally < 0)
                {
                    newPos = playerSelectorCurrentPosition;
                }

                // Handle clicking down
                if (direction == Direction.Down && playerSelectorCurrentPosition + numberInRowsNormally <= characterIcons.Count - 1)
                {
                    newPos = playerSelectorCurrentPosition + numberInRowsNormally;
                }
                else if (direction == Direction.Down && playerSelectorCurrentPosition + numberInRowsNormally > characterIcons.Count - 1)
                {
                    // final
                    newPos = playerSelectorCurrentPosition;
                }

                playerSelector.SetSelectorPosition(characterIcons[newPos], newPos);
            }
        }
    }

    public CharacterSelectorGameobject GetPlayerSelector(int playerID)
    {
        foreach (CharacterSelectorGameobject playerSelector in playerSelectors)
        {
            if (playerSelector.playerID == playerID)
            {
                return playerSelector;
            } 
        } 
        return null;
    }

    public override void Up(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if (!DeterminePlayerInput(player.getPlayerID()))
            return;

        MovePlayerSelector(player.getPlayerID(), Direction.Up);

        //base.Up(status, playerID);
    }

    public override void Left(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if (!DeterminePlayerInput(player.getPlayerID()))
            return;

        MovePlayerSelector(player.getPlayerID(), Direction.Left);

        //base.Left(status, playerID);
    }

    public override void Down(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if (!DeterminePlayerInput(player.getPlayerID()))
            return;

        MovePlayerSelector(player.getPlayerID(), Direction.Down);
        //base.Down(status, playerID);
    }

    public override void Right(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if (!DeterminePlayerInput(player.getPlayerID()))
            return;

        MovePlayerSelector(player.getPlayerID(), Direction.Right);
        //base.Right(status, playerID);
    }

    public override void Confirm(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        int playerID = player.getPlayerID();
        if (!DeterminePlayerInput(playerID))
            return;
        
        Debug.Log("Confirm UI");

        player.SpawnBody(GetPlayerSelector(playerID).selectorPosition);

        //base.Confirm(status, playerID);
    }

    public override void Return(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if (!DeterminePlayerInput(player.getPlayerID()))
            return;

        player.DestroyBody();
        //base.Return(status, playerID);
    }
}

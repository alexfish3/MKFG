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
    [SerializeField] GameObject playerSelectorParent;
    Dictionary<int, CharacterSelectorGameobject> playerSelectorsDict =  new Dictionary<int, CharacterSelectorGameobject>();

    [SerializeField] GameObject playerTag;
    [SerializeField] GameObject playerTagParent;
    Dictionary<int, CharacterSelectorNametag> playerTagDict = new Dictionary<int, CharacterSelectorNametag>();

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    public override void AddPlayerToUI(GenericBrain player)
    {
        var newSelector = Instantiate(playerSelector, playerSelectorParent.transform).GetComponent<CharacterSelectorGameobject>();
        var newNametag = Instantiate(playerTag, playerTagParent.transform).GetComponent<CharacterSelectorNametag>();

        newSelector.Initialize(player.GetPlayerID(), player.GetDeviceID(), newNametag);
        newSelector.SetDefaultPosition(characterIcons[0]);

        playerSelectorsDict.Add(player.GetPlayerID(), newSelector);
        playerTagDict.Add(player.GetPlayerID(), newNametag);

        base.AddPlayerToUI(player);
    }

    public override void RemovePlayerUI(GenericBrain player)
    {
        bool removed = false;
        CharacterSelectorGameobject selectorToRemove = null;
        if(playerSelectorsDict.TryGetValue(player.GetPlayerID(), out selectorToRemove))
        {
            playerSelectorsDict.Remove(player.GetPlayerID());
            //playerSelectors.Remove(selectorToRemove);

            Destroy(selectorToRemove.gameObject);
            removed = true;
        }

        CharacterSelectorNametag nametagToRemove = null;
        if (playerTagDict.TryGetValue(player.GetPlayerID(), out nametagToRemove))
        {
            playerTagDict.Remove(player.GetPlayerID());
            Destroy(nametagToRemove.gameObject);
            removed = true;
        }

        // Only call base if object was actually removed
        if (removed)
        {
            base.RemovePlayerUI(player);
        }
    }

    public void MovePlayerSelector(int playerID, Direction direction) 
    {
        foreach(KeyValuePair<int, CharacterSelectorGameobject> playerSelector in playerSelectorsDict)
        {
            if(playerSelector.Value.playerID == playerID)
            {
                // If selector is confirmed, dont move it
                if (playerSelector.Value.GetConfirmedStatus() == true)
                    return;

                int playerSelectorCurrentPosition = playerSelector.Value.selectorPosition;
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

                playerSelector.Value.SetSelectorPosition(characterIcons[newPos], newPos);
            }
        }
    }

    public void SetPlayerSelectorStatus(int playerID, bool selectorStatus)
    {
        foreach (KeyValuePair<int, CharacterSelectorGameobject> playerSelector in playerSelectorsDict)
        {
            if (playerSelector.Value.playerID == playerID)
            {
                playerSelector.Value.SetSelectorStatus(selectorStatus);
            }
        }
    }

    public CharacterSelectorGameobject GetPlayerSelector(int playerID)
    {
        CharacterSelectorGameobject selectorToRemove = null;
        if (playerSelectorsDict.TryGetValue(playerID, out selectorToRemove))
        {
            return selectorToRemove;
        }
        return null;
    }

    public override void Up(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if (!DeterminePlayerInput(player.GetPlayerID()))
            return;

        MovePlayerSelector(player.GetPlayerID(), Direction.Up);

        //base.Up(status, playerID);
    }

    public override void Left(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if (!DeterminePlayerInput(player.GetPlayerID()))
            return;

        MovePlayerSelector(player.GetPlayerID(), Direction.Left);

        //base.Left(status, playerID);
    }

    public override void Down(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if (!DeterminePlayerInput(player.GetPlayerID()))
            return;

        MovePlayerSelector(player.GetPlayerID(), Direction.Down);
        //base.Down(status, playerID);
    }

    public override void Right(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if (!DeterminePlayerInput(player.GetPlayerID()))
            return;

        MovePlayerSelector(player.GetPlayerID(), Direction.Right);
        //base.Right(status, playerID);
    }

    public override void Confirm(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        int playerID = player.GetPlayerID();
        if (!DeterminePlayerInput(playerID))
            return;
        
        Debug.Log("Confirm UI");

        SetPlayerSelectorStatus(player.GetPlayerID(), true);

        //player.SpawnBody(GetPlayerSelector(playerID).selectorPosition);

        //base.Confirm(status, playerID);
    }

    public override void Return(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if (!DeterminePlayerInput(player.GetPlayerID()))
            return;

        SetPlayerSelectorStatus(player.GetPlayerID(), false);

        //player.DestroyBody();
        //base.Return(status, playerID);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : SingletonGenericUI<CharacterSelectUI>
{
    [Header("Player Select UI Info")]
    [SerializeField] List<GameObject> characterIcons = new List<GameObject>();
    CharacterInformationSO[] charactersInformation;
    [SerializeField] int numberInRowsNormally;

    [Space(10)]
    [SerializeField] GameObject playerSelector;
    [SerializeField] GameObject playerSelectorParent;
    Dictionary<int, CharacterSelectorGameobject> playerSelectorsDict =  new Dictionary<int, CharacterSelectorGameobject>();

    [SerializeField] GameObject playerTag;
    [SerializeField] GameObject playerTagParent;
    Dictionary<int, CharacterSelectorNametag> playerTagDict = new Dictionary<int, CharacterSelectorNametag>();

    [Header("Ready Up Information")]
    [SerializeField] GameObject ReadyUpText;
    [SerializeField] bool allReadiedUp = false;
    public event Action OnReadiedUp;

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    protected void Start()
    {
        InitalizeUI();
    }

    public override void InitalizeUI()
    {
        charactersInformation = PlayerList.Instance.Characters;

        Debug.Log(PlayerList.Instance.Characters[1].GetCharacterName());
        for (int i = 0; i < characterIcons.Count; i++)
        {
            characterIcons[i].GetComponent<Image>().sprite = charactersInformation[i].GetCharacterSelectHeadshot();
        }
    }

    public override void AddPlayerToUI(GenericBrain player)
    {
        Debug.Log(player.gameObject.name + player.GetPlayerID());
        var newSelector = Instantiate(playerSelector, playerSelectorParent.transform).GetComponent<CharacterSelectorGameobject>();
        var newNametag = Instantiate(playerTag, playerTagParent.transform).GetComponent<CharacterSelectorNametag>();

        newSelector.Initialize(player.GetPlayerID(), player.GetDeviceID(), newNametag);
        newSelector.SetDefaultPosition(charactersInformation[0], characterIcons[0]);

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

    public override void Up(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if (!DetermineIfPlayerCanInputInUI(player.GetPlayerID()))
            return;

        MovePlayerSelector(player.GetPlayerID(), Direction.Up);

        //base.Up(status, playerID);
    }

    public override void Left(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if (!DetermineIfPlayerCanInputInUI(player.GetPlayerID()))
            return;

        MovePlayerSelector(player.GetPlayerID(), Direction.Left);

        //base.Left(status, playerID);
    }

    public override void Down(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if (!DetermineIfPlayerCanInputInUI(player.GetPlayerID()))
            return;

        MovePlayerSelector(player.GetPlayerID(), Direction.Down);
        //base.Down(status, playerID);
    }

    public override void Right(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if (!DetermineIfPlayerCanInputInUI(player.GetPlayerID()))
            return;

        MovePlayerSelector(player.GetPlayerID(), Direction.Right);
        //base.Right(status, playerID);
    }

    public override void Confirm(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        int playerID = player.GetPlayerID();
        if (!DetermineIfPlayerCanInputInUI(playerID))
            return;
        
        if(allReadiedUp == true)
        {
            // Only allow host to start game
            if(playerID == 0)
            {
                Debug.Log("Enter New Scene");
                OnReadiedUp?.Invoke();
            }
        }
        else // Allows players to confirm
        {
            Debug.Log("Confirm UI");
            SetPlayerSelectorStatus(player.GetPlayerID(), true);

            // When you confirm, set the selected player ID to the brain
            player.SetCharacterID(GetPlayerSelector(playerID).GetSelectedCharacterID());

            DetermineReadyUpStatus();
        }
    }

    public override void Return(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if (!DetermineIfPlayerCanInputInUI(player.GetPlayerID()))
            return;

        SetPlayerSelectorStatus(player.GetPlayerID(), false);

        // Set ID back to neg, just incase
        player.SetCharacterID(-1);

        DetermineReadyUpStatus();
    }

    /// <summary>
    /// Moves the spesific player's selector based on the player's input
    /// </summary>
    /// <param name="playerID">The ID of the player who is doing the action</param>
    /// <param name="direction">The direction in which the selector will move</param>
    private void MovePlayerSelector(int playerID, Direction direction)
    {
        foreach (KeyValuePair<int, CharacterSelectorGameobject> playerSelector in playerSelectorsDict)
        {
            if (playerSelector.Value.playerID == playerID)
            {
                // If selector is confirmed, dont move it
                if (playerSelector.Value.GetConfirmedStatus() == true)
                    return;

                // Character ID and selector position in UI is same thing, might change in future
                int playerSelectorCurrentPosition = playerSelector.Value.GetSelectedCharacterID();
                int newPos = 0;

                #region MenuMovement
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
                #endregion MenuMovement

                // Set the selector position data to match the new selected position
                playerSelector.Value.SetSelectorPosition(newPos, charactersInformation[newPos], characterIcons[newPos]);
            }
        }
    }

    /// <summary>
    /// Sets the player selector's status as either selected or not
    /// </summary>
    /// <param name="playerID">The ID of the player who is doing the action</param>
    /// <param name="selectorStatus">The to be set status of the player's selector</param>
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

    /// <summary>
    /// Gets the player selector given a player ID
    /// </summary>
    /// <param name="playerID">The ID of the player you are trying to get the selector of</param>
    /// <returns></returns>
    public CharacterSelectorGameobject GetPlayerSelector(int playerID)
    {
        CharacterSelectorGameobject selectorToRemove = null;
        if (playerSelectorsDict.TryGetValue(playerID, out selectorToRemove))
        {
            return selectorToRemove;
        }
        return null;
    }

    /// <summary>
    /// Runs to check the ready up status of all connected players
    /// </summary>
    public void DetermineReadyUpStatus()
    {
        int numReadiedUp = 0;

        // Check readied up status for all players
        foreach(KeyValuePair<int, CharacterSelectorGameobject> keyValuePair in playerSelectorsDict)
        {
            if(keyValuePair.Value.GetConfirmedStatus() == true)
            {
                numReadiedUp++;
            }
        }

        if(numReadiedUp == playerSelectorsDict.Count)
        {
            Debug.Log("All players readied up");
            allReadiedUp = true;
            ReadyUpText.SetActive(true);
        }
        else
        {
            Debug.Log("Not all players are readied up");
            allReadiedUp = false;
            ReadyUpText.SetActive(false);
        }
    }
}

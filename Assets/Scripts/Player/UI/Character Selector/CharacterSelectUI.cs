///
/// Created by Alex Fischer | July 2024
/// 

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] List<CharacterSelectorGameobject> playerSelectorsDict =  new List<CharacterSelectorGameobject>();
    [SerializeField] Vector2[] characterSelectorOffsets;

    [SerializeField] GameObject playerTag;
    [SerializeField] GameObject playerTagParent;
    [SerializeField] List<UINametag> playerTagDict = new List<UINametag>();
    public int otherPlayerSelector = 0;

    [Header("Ready Up Information")]
    [SerializeField] GameObject ReadyUpText;
    [SerializeField] bool allReadiedUp = false;
    public event Action OnReadiedUp;

    [Header("Team Info")]
    [SerializeField] bool isSolo = true;
    [SerializeField] TMP_Text teamModeText;
    [SerializeField] Color[] teamColors;

    protected void Start()
    {
        InitalizeUI();
    }

    public override void InitalizeUI()
    {
        charactersInformation = PlayerList.Instance.Characters;

        for (int i = 0; i < characterIcons.Count; i++)
        {
            characterIcons[i].GetComponentInChildren<Image>().sprite = charactersInformation[i].GetCharacterSelectHeadshot();
        }
    }

    public override void AddPlayerToUI(GenericBrain player)
    {
        base.AddPlayerToUI(player);

        int playerID = player.GetPlayerID();
        Debug.Log(player.gameObject.name + playerID);

        var newSelector = Instantiate(playerSelector, playerSelectorParent.transform).GetComponent<CharacterSelectorGameobject>();
        var newNametag = Instantiate(playerTag, playerTagParent.transform).GetComponent<UINametag>();

        newSelector.Initialize(playerID, teamColors[playerID], player.GetDeviceID(), newNametag);
        newSelector.SetOffsetPosition(characterSelectorOffsets[playerID]);

        newSelector.SetDefaultPosition(charactersInformation[0], characterIcons[0]);

        newNametag.Initalize(player, isSolo);

        playerSelectorsDict.Add(newSelector);
        playerTagDict.Add(newNametag);

        // Setup team information when spawning in
        if (isSolo == true)
        {
            GiveTeam(playerID, player, newSelector);
        }
        else if (isSolo == false)
        {
            GiveTeam(GetSmallerTeam(player, player.GetPlayerID()), player, newSelector);
        }

        // Reset all readed up info
        allReadiedUp = false;
        ReadyUpText.SetActive(false);
    }

    public override void RemovePlayerUI(GenericBrain player)
    {
        Debug.Log("Remove Player");

        // Remove the character selector from the list
        try
        {
            CharacterSelectorGameobject selectorToRemove = playerSelectorsDict[player.GetPlayerID()];
            playerSelectorsDict.Remove(selectorToRemove);
            Destroy(selectorToRemove.gameObject);

            UINametag nametagToRemove = playerTagDict[player.GetPlayerID()];
            playerTagDict.Remove(nametagToRemove);
            Destroy(nametagToRemove.gameObject);
        }
        catch (Exception ex) { }

        // Only call base if object was actually removed
        base.RemovePlayerUI(player);
    }

    public override void Up(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if (!DetermineIfPlayerCanInputInUI(player.GetPlayerID()))
            return;

        MovePlayerSelector(player.GetPlayerID(), player, Direction.Up);

        //base.Up(status, playerID);
    }

    public override void Left(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if (!DetermineIfPlayerCanInputInUI(player.GetPlayerID()))
            return;

        MovePlayerSelector(player.GetPlayerID(), player, Direction.Left);

        //base.Left(status, playerID);
    }

    public override void Down(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if (!DetermineIfPlayerCanInputInUI(player.GetPlayerID()))
            return;

        MovePlayerSelector(player.GetPlayerID(), player, Direction.Down);
        //base.Down(status, playerID);
    }

    public override void Right(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if (!DetermineIfPlayerCanInputInUI(player.GetPlayerID()))
            return;

        MovePlayerSelector(player.GetPlayerID(), player, Direction.Right);
        //base.Right(status, playerID);
    }

    public override void Confirm(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        int playerID = player.GetPlayerID();
        if (!DetermineIfPlayerCanInputInUI(playerID))
            return;
        
        // Big confirm text, takes priority over other inputs
        if(allReadiedUp == true)
        {
            // Only allow host to start game
            if(playerID == 0)
            {
                GameManagerNew.Instance.SetGameState(GameStates.MapSelect);
            }
        }
        else
        {
            // Makes sure players are on characters menu to confirm player select
            if (playerSelectorsDict[playerID].GetSelectedPlayersSelection() == CharacterSelectionPosition.Characters)
            {
                Debug.Log("Confirm UI");
                SetPlayerSelectorStatus(player.GetPlayerID(), true);

                // When you confirm, set the selected player ID to the brain
                player.SetCharacterID(GetPlayerSelector(playerID).GetSelectedPositionID());

                DetermineReadyUpStatus();
            }

            // Handles when player one hovers over other players and deletes them
            if (playerID == 0 && playerSelectorsDict[playerID].GetSelectedPlayersSelection() == CharacterSelectionPosition.PlayerTagMoveset)
            {
                if (otherPlayerSelector == 0)
                    return;

                int playerPosToDelete = otherPlayerSelector;

                MovePlayerSelector(playerID, player, Direction.Left);
                ReinitalizePlayerSelectorIDs(playerPosToDelete);
            }
        }
    }

    public void ReinitalizePlayerSelectorIDs(int positionRemoved)
    {
        connectedPlayers[positionRemoved].ToggleActivateBrain(false);

        Debug.Log($"Removed player at position {positionRemoved}. Will now begin at player position {positionRemoved + 1} and loop to setup");

        for (int i = positionRemoved; i < connectedPlayers.Count; i++)
        {
            // Set player ID to be new value
            connectedPlayers[i].SetPlayerID(i);
            playerSelectorsDict[i].playerID = i;

            Debug.Log("Looping to remove at pos " + i);
            // Cache nametag and selector
            CharacterSelectorGameobject playerToReinitSelector = playerSelectorsDict[i];
            UINametag playerToReinitNametag = playerTagDict[i];

            // Setup team information when spawning in
            if (isSolo == true)
            {
                GiveTeam(i, connectedPlayers[i], playerToReinitSelector);
            }
            else if (isSolo == false)
            {
                GiveTeam(GetSmallerTeam(connectedPlayers[i], connectedPlayers[i].GetPlayerID()), connectedPlayers[i], playerToReinitSelector);
            }
        }
    }

    public override void Return(bool status, GenericBrain player)
    {
        int playerID = player.GetPlayerID();

        if (status == false)
            return;

        if (!DetermineIfPlayerCanInputInUI(playerID))
            return;

        // Makes sure players are on characters menu to confirm
        if (playerSelectorsDict[playerID].GetSelectedPlayersSelection() != CharacterSelectionPosition.Characters)
            return;

        SetPlayerSelectorStatus(player.GetPlayerID(), false);

        // Set ID back to neg, just incase
        player.SetCharacterID(-1);

        DetermineReadyUpStatus();
    }

    /// <summary>
    /// Used to toggle solos or duos
    /// </summary>
    /// <param name="status">the status of button press on the brain</param>
    /// <param name="player">the brain being pressed</param>
    public override void Button1(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        // only player one can tab on the character select
        if (player.GetPlayerID() != 0)
            return;

        if (!DetermineIfPlayerCanInputInUI(player.GetPlayerID()))
            return;

        Debug.Log("Swap Modes");
        isSolo = !isSolo;

        UpdateTeams();

        if (isSolo == true)
        {
            teamModeText.text = "Swap to Mode: Teams";
        }
        else if (isSolo == false)
        {
            teamModeText.text = "Swap to Mode: Solo";
        }

        base.Button1(status, player);
    }

    /// <summary>
    /// Used to enter rule set menu
    /// </summary>
    /// <param name="status">the status of button press on the brain</param>
    /// <param name="player">the brain being pressed</param>
    public override void Button2(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        // only player one can tab on the character select
        if (player.GetPlayerID() != 0)
            return;

        if (!DetermineIfPlayerCanInputInUI(player.GetPlayerID()))
            return;

        GameManagerNew.Instance.SetGameState(GameStates.RuleSelect);

        base.Button2(status, player);
    }


    /// <summary>
    /// Moves the spesific player's selector based on the player's input
    /// </summary>
    /// <param name="playerID">The ID of the player who is doing the action</param>
    /// <param name="direction">The direction in which the selector will move</param>
    private void MovePlayerSelector(int playerID, GenericBrain playerBrain, Direction direction)
    {
        for (int i = 0; i < playerSelectorsDict.Count; i++)
        {
            CharacterSelectorGameobject playerSelector = playerSelectorsDict[i];

            // If current iterated selector is not player moving, return
            if (playerSelector.playerID != playerID)
                continue;

            // If selector is confirmed, dont move it
            if (playerSelector.GetConfirmedStatus() == true)
                return;

            int playerSelectorCurrentPosition = playerSelector.GetSelectedPositionID();

            if (playerSelector.GetSelectedPlayersSelection() == CharacterSelectionPosition.Characters)
            {
                // Character ID and selector position in UI is same thing, might change in future
                int newPos = 0;

                #region CharacterSelectMovement
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
                    newPos = playerSelectorCurrentPosition;

                    if (isSolo == true)
                    {
                        playerSelector.SetSelectedPlayersSelection(CharacterSelectionPosition.PlayerTagMoveset);

                        // Set the selector position data to match the new selected position
                        playerSelector.SetSelectorPosition(playerSelector.GetSelectorNametag().playerTagSelectTransform);
                        return;
                    }
                    else
                    {
                        playerSelector.SetSelectedPlayersSelection(CharacterSelectionPosition.Teams);

                        // Set the selector position data to match the new selected position
                        playerSelector.SetSelectorPosition(playerSelector.GetSelectorNametag().teamSelectorTransform);
                        return;
                    }

                }
                #endregion CharacterSelectMovement

                // Set the selector position data to match the new selected position
                playerSelector.SetSelectorPosition(newPos, charactersInformation[newPos], characterIcons[newPos]);
            }
            else if (playerSelector.GetSelectedPlayersSelection() == CharacterSelectionPosition.Teams)
            {
                int teamID = playerBrain.GetTeamID();

                #region TeamSelect
                // Handle clicking left and right
                if (direction == Direction.Left || direction == Direction.Right)
                {
                    if (teamID == 0)
                    {
                        GiveTeam(1, playerBrain, playerSelector);
                    }
                    else
                    {
                        GiveTeam(0, playerBrain, playerSelector);
                    }
                }

                // Handle clicking up
                if (direction == Direction.Up)
                {
                    playerSelector.SetSelectedPlayersSelection(CharacterSelectionPosition.Characters);

                    // Set the selector position data to match the new selected position
                    playerSelector.SetSelectorPosition(playerSelectorCurrentPosition, charactersInformation[playerSelectorCurrentPosition], characterIcons[playerSelectorCurrentPosition]);
                }

                // Handle clicking down
                if (direction == Direction.Down)
                {
                    playerSelector.SetSelectedPlayersSelection(CharacterSelectionPosition.PlayerTagMoveset);

                    // Set the selector position data to match the new selected position
                    playerSelector.SetSelectorPosition(playerSelector.GetSelectorNametag().playerTagSelectTransform);
                }
                #endregion TeamSelect
            }
            else if (playerSelector.GetSelectedPlayersSelection() == CharacterSelectionPosition.PlayerTagMoveset)
            {
                #region PlayerTagMoveset

                // Handles selecting other player's tags to delete
                if(playerID == 0)
                {
                    // Handle clicking left
                    if (direction == Direction.Left && otherPlayerSelector > 0)
                    {
                        otherPlayerSelector -= 1;
                        playerSelector.SetSelectorPosition(playerSelectorsDict[otherPlayerSelector].GetSelectorNametag().playerTagSelectTransform);
                    }
                    else if (direction == Direction.Left && otherPlayerSelector <= 0)
                    {
                        // Do nothing
                        otherPlayerSelector = playerSelectorsDict.Count - 1;
                        playerSelector.SetSelectorPosition(playerSelectorsDict[otherPlayerSelector].GetSelectorNametag().playerTagSelectTransform);
                    }

                    // Handle clicking right
                    if (direction == Direction.Right && otherPlayerSelector < playerSelectorsDict.Count - 1)
                    {
                        otherPlayerSelector += 1;
                        playerSelector.SetSelectorPosition(playerSelectorsDict[otherPlayerSelector].GetSelectorNametag().playerTagSelectTransform);
                    }
                    else if (direction == Direction.Right && otherPlayerSelector >= playerSelectorsDict.Count - 1)
                    {
                        // Do Nothing
                        otherPlayerSelector = 0;
                        playerSelector.SetSelectorPosition(playerSelectorsDict[otherPlayerSelector].GetSelectorNametag().playerTagSelectTransform);
                    }

                    // Handle clicking up
                    if (direction == Direction.Up)
                    {
                        // Reset back to zero
                        otherPlayerSelector = 0;
                    }
                }
                else
                {

                }
                // Handle clicking up
                if (direction == Direction.Up)
                {
                    if (isSolo == true)
                    {
                        playerSelector.SetSelectedPlayersSelection(CharacterSelectionPosition.Characters);
                        // Set the selector position data to match the new selected position
                        playerSelector.SetSelectorPosition(playerSelectorCurrentPosition, charactersInformation[playerSelectorCurrentPosition], characterIcons[playerSelectorCurrentPosition]);
                    }
                    else
                    {
                        playerSelector.SetSelectedPlayersSelection(CharacterSelectionPosition.Teams);
                        // Set the selector position data to match the new selected position
                        playerSelector.SetSelectorPosition(playerSelector.GetSelectorNametag().teamSelectorTransform);
                    }
                }
                
                #endregion PlayerTagMoveset
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
        for (int i = 0; i < playerSelectorsDict.Count; i++)
        {
            if (playerSelectorsDict[i].playerID == playerID)
            {
                playerSelectorsDict[i].SetSelectorStatus(selectorStatus);
                return;
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
        for (int i = 0; i < playerSelectorsDict.Count; i++)
        {
            if (playerSelectorsDict[i].playerID == playerID)
            {
                return playerSelectorsDict[i];
            }
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
        for (int i = 0; i < playerSelectorsDict.Count; i++)
        {
            if (playerSelectorsDict[i].GetConfirmedStatus() == true)
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

    public void UpdateTeams()
    {
        if(isSolo == true)
        {
            int teamID = 0;
            Debug.Log("Updating character teams to: Solo");
            // Loop and give each player a different team
            foreach (GenericBrain player in connectedPlayers)
            {
                GiveTeam(teamID, player, playerSelectorsDict[teamID]);
                teamID++;
            }
        }
        else if (isSolo == false)
        {
            Debug.Log("Updating character teams to: Teams");
            // Loop and give each player a different team
            foreach (GenericBrain player in connectedPlayers)
            {
                int teamID = GetSmallerTeam(player, player.GetPlayerID());
                int playerID = player.GetPlayerID();

                GiveTeam(teamID, player, playerSelectorsDict[playerID]);
            }
        }
    }

    /// <summary>
    /// Gives a team to a player's selector and nametag
    /// </summary>
    /// <param name="teamID">The team ID to be used in team updating</param>
    /// <param name="brain">The brain of the player</param>
    /// <param name="selector">The player's selector</param>
    public void GiveTeam(int teamID, GenericBrain brain, CharacterSelectorGameobject selector)
    {
        // Initalize the selector for the color
        selector.Initialize(brain.GetPlayerID(), teamColors[teamID], brain.GetDeviceID(), selector.GetSelectorNametag());

        Debug.Log($"Giving Selector: {selector.playerID} the team ID of: {teamID}");
        selector.GetSelectorNametag().SetBackgroundColor(teamColors[teamID]);
        selector.GetSelectorNametag().ToggleTeamSelect(isSolo);
        selector.GetSelectorNametag().SetPlayerName("Player " + (brain.GetPlayerID() + 1).ToString());

        // For each player, if swapping to solo and the position is on teams, put selector to player tag
        if (isSolo && selector.GetSelectedPlayersSelection() == CharacterSelectionPosition.Teams)
        {
            selector.SetSelectedPlayersSelection(CharacterSelectionPosition.PlayerTagMoveset);
            selector.SetSelectorPosition(selector.GetSelectorNametag().playerTagSelectTransform);
        }

        brain.SetTeamID(teamID);
        brain.SetTeamColor(teamColors[teamID]);

        selector.SetOffsetPosition(characterSelectorOffsets[brain.GetPlayerID()]);
    }

    /// <summary>
    /// Returns the team with the smaller amount of players
    /// </summary>
    /// <returns>the id of the smaller team</returns>
    private int GetSmallerTeam(GenericBrain playerToCheck, int countUnitl)
    {
        int counterForTeam1 = 0;
        int counterForTeam2 = 0;

        Debug.Log("We are counting unitl " + countUnitl);

        // Loops for all players except for player you are calculating team for, ie most recent player added to connected players
        for(int i = 0; i < countUnitl; i++)
        {
            GenericBrain player = connectedPlayers[i];
            int PlayerTeamID = player.GetTeamID();
            Debug.Log("Team ID For this player is " + PlayerTeamID);

            if (player.GetTeamID() == 0)
            {
                counterForTeam1++;
            }
            else if (player.GetTeamID() == 1)
            {
                counterForTeam2++;
            }
        }

        Debug.Log($"The team counters are... \n" +
            $"Team 1: {counterForTeam1}\n" +
            $"Team 2: {counterForTeam2}");

        if (counterForTeam2 >= counterForTeam1)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }
}

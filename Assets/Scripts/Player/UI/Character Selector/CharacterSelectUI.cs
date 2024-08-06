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
    [SerializeField] List<CharacterSelectorGameobject> playerSelectorsList =  new List<CharacterSelectorGameobject>();
    [SerializeField] Vector2[] characterSelectorOffsets;

    [SerializeField] GameObject playerTag;
    [SerializeField] GameObject playerTagParent;
    [SerializeField] List<UINametag> playerTagsList = new List<UINametag>();
    [SerializeField] GameObject joinTag;
    public int otherPlayerSelector = 0;

    [Header("Ready Up Information")]
    [SerializeField] GameObject ReadyUpText;
    [SerializeField] bool allReadiedUp = false;
    public event Action OnReadiedUp;

    [Header("Team Info")]
    [SerializeField] bool isSolo = true;
    [SerializeField] TMP_Text teamModeText;
    [SerializeField] Color[] teamColors;

    [Header("Hold Info")]
    [SerializeField] bool isHolding;
    [SerializeField] HoldRingUI holdRing;

    [Header("Spectator List")]
    [SerializeField] GameObject spectatorListParent;
    [SerializeField] List<UISpectatorName> spectatorNames = new List<UISpectatorName>();
    [SerializeField] GameObject spectatorNamePrefab;

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

        holdRing.OnFillRing += () => { GameManagerNew.Instance.SetGameState(GameStates.MainMenu); };
    }

    private void OnDisable()
    {
        holdRing.OnFillRing -= () => { GameManagerNew.Instance.SetGameState(GameStates.MainMenu); };
    }

    public void Update()
    {
        if (isHolding == true)
        {
            holdRing.TickFill();
        }
    }

    public override void AddPlayerToUI(GenericBrain player)
    {
        if (player == null)
            return;

        base.AddPlayerToUI(player);

        int playerID = player.GetPlayerID();
        int deviceID = player.GetDeviceID();

        var newSelector = Instantiate(playerSelector, playerSelectorParent.transform).GetComponent<CharacterSelectorGameobject>();
        var newNametag = Instantiate(playerTag, playerTagParent.transform).GetComponent<UINametag>();

        newSelector.Initialize(playerID, teamColors[playerID], deviceID, newNametag);
        newSelector.SetOffsetPosition(characterSelectorOffsets[playerID]);

        newSelector.SetDefaultPosition(charactersInformation[0], characterIcons[0]);

        newNametag.Initalize(player, isSolo, playerID, deviceID);

        playerSelectorsList.Add(newSelector);
        playerTagsList.Add(newNametag);

        CheckToUpdateEmptyPodium();

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

        Debug.Log("ADDING PLAYER having " + playerSelectorsList.Count);
    }

    public override void RemovePlayerUI(GenericBrain player)
    {
        Debug.Log("Remove Player ID: " + player.GetPlayerID());

        foreach(CharacterSelectorGameobject selector in playerSelectorsList)
        {
            if(selector.playerID == player.GetPlayerID())
            {
                playerSelectorsList.Remove(selector);
                Destroy(selector.gameObject);
                break;
            }
        }

        foreach (UINametag nametag in playerTagsList)
        {
            if (nametag.playerID == player.GetPlayerID())
            {
                playerTagsList.Remove(nametag);
                Destroy(nametag.gameObject);
                break;
            }
        }

        isHolding = false;

        // Only call base if object was actually removed
        base.RemovePlayerUI(player);

        CheckToUpdateEmptyPodium();
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
            if (playerSelectorsList[playerID].GetSelectedPlayersSelection() == CharacterSelectionPosition.Characters)
            {
                Debug.Log("Confirm UI");
                SetPlayerSelectorStatus(player.GetPlayerID(), true);

                // When you confirm, set the selected player ID to the brain
                player.SetCharacterID(GetPlayerSelector(playerID).GetSelectedPositionID());

                DetermineReadyUpStatus();
            }

            if (connectedPlayers.Count == 1)
                return;

            // Handles when player one hovers over other players and deletes them
            if (playerID == 0 && playerSelectorsList[playerID].GetSelectedPlayersSelection() == CharacterSelectionPosition.PlayerTagMoveset)
            {
                //if (otherPlayerSelector == 0)
                //    return;

                int playerPosToDelete = otherPlayerSelector;

                MovePlayerSelector(playerID, player, Direction.Left);

                connectedPlayers[playerPosToDelete].ToggleActivateBrain(false);

                ReinitalizePlayerIDs(playerPosToDelete);
            }
            // Handles when other players hover over themselves and remove themself
            else if(playerID > 0 && playerSelectorsList[playerID].GetSelectedPlayersSelection() == CharacterSelectionPosition.PlayerTagMoveset)
            {
                MovePlayerSelector(playerID, player, Direction.Left);

                connectedPlayers[playerID].ToggleActivateBrain(false);

                ReinitalizePlayerIDs(playerID);
            }
        }
    }

    public override void Return(bool status, GenericBrain player)
    {
        int playerID = player.GetPlayerID();

        if (!DetermineIfPlayerCanInputInUI(playerID))
            return;

        // If player is confirmed, and wants to deconfirm to change character
        if (playerSelectorsList[playerID].GetConfirmedStatus() == true)
        {
            if (status == false)
                return;

            // Makes sure players are on characters menu to confirm
            if (playerSelectorsList[playerID].GetSelectedPlayersSelection() != CharacterSelectionPosition.Characters)
                return;

            SetPlayerSelectorStatus(player.GetPlayerID(), false);

            // Set ID back to neg, just incase
            player.SetCharacterID(-1);

            DetermineReadyUpStatus();
        }
        else // Return to last menu if player 0 inputs
        {
            if (playerID != 0)
                return;

            // If we let go of button, ie status becomes false, we set fill to zero
            if(status == false)
            {
                holdRing.SetFillZero();
            }

            isHolding = status;
        }
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
        for (int i = 0; i < playerSelectorsList.Count; i++)
        {
            CharacterSelectorGameobject playerSelector = playerSelectorsList[i];

            //Debug.Log($"Attempting to move selector id {playerSelector.playerID} with id {playerID}");

            // If current iterated selector is not player moving, return
            if (playerSelector.playerID != playerID)
                continue;
            Debug.Log($"Attempting to move selector id {playerSelector.playerID} with id {playerID}");
            Debug.Log("Attempting to move selector successful");

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
                        playerSelector.SetSelectorPosition(playerSelectorsList[otherPlayerSelector].GetSelectorNametag().playerTagSelectTransform);
                    }
                    else if (direction == Direction.Left && otherPlayerSelector <= 0)
                    {
                        // Do nothing
                        otherPlayerSelector = playerSelectorsList.Count - 1;
                        playerSelector.SetSelectorPosition(playerSelectorsList[otherPlayerSelector].GetSelectorNametag().playerTagSelectTransform);
                    }

                    // Handle clicking right
                    if (direction == Direction.Right && otherPlayerSelector < playerSelectorsList.Count - 1)
                    {
                        otherPlayerSelector += 1;
                        playerSelector.SetSelectorPosition(playerSelectorsList[otherPlayerSelector].GetSelectorNametag().playerTagSelectTransform);
                    }
                    else if (direction == Direction.Right && otherPlayerSelector >= playerSelectorsList.Count - 1)
                    {
                        // Do Nothing
                        otherPlayerSelector = 0;
                        playerSelector.SetSelectorPosition(playerSelectorsList[otherPlayerSelector].GetSelectorNametag().playerTagSelectTransform);
                    }

                    // Handle clicking up
                    if (direction == Direction.Up)
                    {
                        // Reset back to zero
                        otherPlayerSelector = 0;
                    }
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
    /// Reinitalizes the player selector ID's when a player is removed from the menu
    /// </summary>
    /// <param name="positionRemoved">The position the player was removed at</param>
    public override void ReinitalizePlayerIDs(int positionRemoved)
    {
        Debug.Log($"Removed player at position {positionRemoved}. Will now begin at player position {positionRemoved + 1} and loop to setup");

        for (int i = positionRemoved; i < connectedPlayers.Count; i++)
        {
            // Set player ID to be new value
            connectedPlayers[i].SetPlayerID(i);
            playerSelectorsList[i].playerID = i;
            playerTagsList[i].playerID = i;

            Debug.Log("Looping to remove at pos " + i);
            // Cache nametag and selector
            CharacterSelectorGameobject playerToReinitSelector = playerSelectorsList[i];
            UINametag playerToReinitNametag = playerTagsList[i];

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

    /// <summary>
    /// Sets the player selector's status as either selected or not
    /// </summary>
    /// <param name="playerID">The ID of the player who is doing the action</param>
    /// <param name="selectorStatus">The to be set status of the player's selector</param>
    public void SetPlayerSelectorStatus(int playerID, bool selectorStatus)
    {
        for (int i = 0; i < playerSelectorsList.Count; i++)
        {
            if (playerSelectorsList[i].playerID == playerID)
            {
                playerSelectorsList[i].SetSelectorStatus(selectorStatus);
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
        for (int i = 0; i < playerSelectorsList.Count; i++)
        {
            if (playerSelectorsList[i].playerID == playerID)
            {
                return playerSelectorsList[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Adds the brain to the spectator list visual by spawning new text gameobject
    /// </summary>
    /// <param name="genericBrain"></param>
    public void AddSpectatorName(GenericBrain genericBrain)
    {
        UISpectatorName tempSpectatorName = Instantiate(spectatorNamePrefab, spectatorListParent.transform).GetComponent<UISpectatorName>();
        tempSpectatorName.InitalizeUISpectatorName(genericBrain);
        spectatorNames.Add(tempSpectatorName);
    }

    /// <summary>
    /// Removes the brain from the spectator list visual by deleting the text gameobject
    /// </summary>
    /// <param name="genericBrain"></param>
    public void RemoveSpectatorName(GenericBrain genericBrain)
    {
        // Loops through spectator names and caches name that is for the brain
        UISpectatorName cacheSpectator = null;
        foreach (UISpectatorName spectatorName in spectatorNames)
        {
            if (spectatorName.PlayerBrain == genericBrain)
            {
                cacheSpectator = spectatorName;
                break;
            }
        }

        // If the brain is not null, ie found one to remove, remove it
        if (cacheSpectator != null)
        {
            spectatorNames.Remove(cacheSpectator);
            Destroy(cacheSpectator.gameObject);
        }
    }

    /// <summary>
    /// Runs to check the ready up status of all connected players
    /// </summary>
    public void DetermineReadyUpStatus()
    {
        int numReadiedUp = 0;

        // Check readied up status for all players
        for (int i = 0; i < playerSelectorsList.Count; i++)
        {
            if (playerSelectorsList[i].GetConfirmedStatus() == true)
            {
                numReadiedUp++;
            }
        }

        if(numReadiedUp == playerSelectorsList.Count)
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

    /// <summary>
    /// Updates the teams of the current players
    /// </summary>
    public void UpdateTeams()
    {
        if(isSolo == true)
        {
            int teamID = 0;
            Debug.Log("Updating character teams to: Solo");
            // Loop and give each player a different team
            foreach (GenericBrain player in connectedPlayers)
            {
                GiveTeam(teamID, player, playerSelectorsList[teamID]);
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

                GiveTeam(teamID, player, playerSelectorsList[playerID]);
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

    /// <summary>
    /// Checks if the empty podium on screen should be rendered
    /// </summary>
    private void CheckToUpdateEmptyPodium()
    {
        if (connectedPlayers.Count >= PlayerSpawnSystem.Instance.GetMaxPlayerCount())
        {
            joinTag.SetActive(false);
        }
        else
        {
            joinTag.SetActive(true);
            joinTag.transform.SetSiblingIndex(connectedPlayers.Count + 1);
        }
    }
}

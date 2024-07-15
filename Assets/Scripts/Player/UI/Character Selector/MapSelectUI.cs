using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectUI : SingletonGenericUI<MapSelectUI>
{
    [Header("Map Select UI Info")]
    [SerializeField] List<GameObject> mapIcons = new List<GameObject>();
    [SerializeField] MapInformationSO[] mapInformation;
    [SerializeField] int numberInRowsNormally;

    [Space(10)]
    [SerializeField] CharacterSelectorGameobject playerSelector;

    [SerializeField] UINametag lobbyTag;

    [Header("Ready Up Information")]
    [SerializeField] GameObject ReadyUpText;
    [SerializeField] bool allSelected = false;
    public event Action OnReadiedUp;

    bool initalized = false;
    protected void Start()
    {
        InitalizeUI();
    }

    public override void InitalizeUI()
    {
        for (int i = 0; i < mapIcons.Count; i++)
        {
            mapIcons[i].GetComponent<Image>().sprite = mapInformation[i].GetMapIcon();
        }

        lobbyTag.SetMapName(mapInformation[0].GetMapName());
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

        if (allSelected == false)
        {
            Debug.Log("Confirm UI");
            SetPlayerSelectorStatus(player.GetPlayerID(), true);

            SceneManager.Instance.SetDrivingScene(mapInformation[playerSelector.GetSelectedPositionID()].GetSceneFile());

            OnReadiedUp?.Invoke();
            allSelected = true;
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
    }

    /// <summary>
    /// Moves the spesific player's selector based on the player's input
    /// </summary>
    /// <param name="playerID">The ID of the player who is doing the action</param>
    /// <param name="direction">The direction in which the selector will move</param>
    private void MovePlayerSelector(int playerID, Direction direction)
    {
        if (playerSelector.playerID == playerID)
        {
            // If selector is confirmed, dont move it
            if (playerSelector.GetConfirmedStatus() == true)
                return;

            // Character ID and selector position in UI is same thing, might change in future
            int playerSelectorCurrentPosition = playerSelector.GetSelectedPositionID();
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
            if (direction == Direction.Right && playerSelectorCurrentPosition + 1 < mapIcons.Count - 1)
            {
                newPos = playerSelectorCurrentPosition + 1;
            }
            else if (direction == Direction.Right && playerSelectorCurrentPosition + 1 >= mapIcons.Count - 1)
            {
                // Do Nothing
                newPos = mapIcons.Count - 1;
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
            if (direction == Direction.Down && playerSelectorCurrentPosition + numberInRowsNormally <= mapIcons.Count - 1)
            {
                newPos = playerSelectorCurrentPosition + numberInRowsNormally;
            }
            else if (direction == Direction.Down && playerSelectorCurrentPosition + numberInRowsNormally > mapIcons.Count - 1)
            {
                // final
                newPos = playerSelectorCurrentPosition;
            }
            #endregion MenuMovement

            // Set the selector position data to match the new selected position
            playerSelector.SetSelectorPosition(newPos, mapInformation[newPos], mapIcons[newPos]);
        }
    }

    /// <summary>
    /// Sets the player selector's status as either selected or not
    /// </summary>
    /// <param name="playerID">The ID of the player who is doing the action</param>
    /// <param name="selectorStatus">The to be set status of the player's selector</param>
    public void SetPlayerSelectorStatus(int playerID, bool selectorStatus)
    {
        if (playerSelector.playerID == playerID)
        {
            playerSelector.SetSelectorStatus(selectorStatus);
        }
    }
}

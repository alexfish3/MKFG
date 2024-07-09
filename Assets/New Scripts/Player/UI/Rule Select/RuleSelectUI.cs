using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuleSelectUI : SingletonGenericUI<RuleSelectUI>
{
    [SerializeField] RulesetSO currentRuleset;
    [SerializeField] RulesetSO[] savedRules;

    [SerializeField] CharacterSelectorGameobject playerSelector;
    [SerializeField] List<GameObject> displayedRules;

    [SerializeField] Scrollbar scroll;

    [Header("Rule UI Gameobject Info")]
    [SerializeField] GameObject newRuleButton;
    [SerializeField] GameObject rulesetButtonGameobject;
    [SerializeField] GameObject rulesetButtonParent;

    protected void Start()
    {
        InitalizeUI();
    }

    public override void InitalizeUI()
    {
        // Load rules from file

        currentRuleset = savedRules[0];

        foreach(RulesetSO ruleset in savedRules)
        {
            var newButton = Instantiate(rulesetButtonGameobject, rulesetButtonParent.transform).GetComponent<RulesetButton>();

            newButton.SetRuleText(ruleset.NameOfRuleset);
            newButton.RuleButton.onClick.AddListener(() => { currentRuleset = ruleset; });

            displayedRules.Add(newButton.gameObject);
        }

        newRuleButton.transform.parent = rulesetButtonParent.transform;

        scroll.value = 1f;

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

        Debug.Log("Chose Rules");
    }

    public override void Return(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if (!DetermineIfPlayerCanInputInUI(player.GetPlayerID()))
            return;

        //SetPlayerSelectorStatus(player.GetPlayerID(), false);
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

            // Handle clicking up
            if (direction == Direction.Up && playerSelectorCurrentPosition >= 0)
            {
                newPos = playerSelectorCurrentPosition - 1;
            }

            // Handle clicking down
            if (direction == Direction.Down && playerSelectorCurrentPosition <= savedRules.Length - 1)
            {
                newPos = playerSelectorCurrentPosition + 1;
            }
            #endregion MenuMovement

            // Set the selector position data to match the new selected position
            playerSelector.SetSelectorPosition(newPos, displayedRules[newPos]);
        }
    }
}

using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RuleSelectUI : SingletonGenericUI<RuleSelectUI>
{
    GameManagerNew gameManager;
    [SerializeField] RulesetSO currentRuleset;
    [SerializeField] RulesetSO[] savedRules;
    [SerializeField] int maxRulesDisplayedOnScreen;
    public float scrollbarStep;

    [SerializeField] CharacterSelectorGameobject playerSelector;
    [SerializeField] List<GameObject> displayedRules;

    [SerializeField] Scrollbar scroll;

    [Header("Rule UI Gameobject Info")]
    [SerializeField] GameObject newRuleButton;
    [SerializeField] GameObject rulesetButtonGameobject;
    [SerializeField] GameObject rulesetButtonParent;

    [Header("Rule Info Display")]
    [SerializeField] TMP_Text ruleName;
    [SerializeField] TMP_Text chosenRuleName;
    [SerializeField] TMP_Text[] ruleNumbers;
    protected void Start()
    {
        InitalizeUI();
    }

    public override void InitalizeUI()
    {
        gameManager = GameManagerNew.Instance;

        // Load rules from file

        // Loads the default rules first
        currentRuleset = savedRules[0];
        if (gameManager != null)
            gameManager.SetRuleset(currentRuleset);

        UpdateRuleDisplay(currentRuleset);
        ChooseRuleset(currentRuleset);

        foreach (RulesetSO ruleset in savedRules)
        {
            var newButton = Instantiate(rulesetButtonGameobject, rulesetButtonParent.transform).GetComponent<RulesetButton>();

            newButton.SetRuleText(ruleset.NameOfRuleset);
            newButton.RuleButton.onClick.AddListener(() => 
            {
                ChooseRuleset(ruleset);
                UpdateRuleDisplay(ruleset);
            });

            displayedRules.Add(newButton.gameObject);
        }

        newRuleButton.transform.SetParent(rulesetButtonParent.transform);
        newRuleButton.transform.localScale = Vector3.one;

        scrollbarStep = 1f / maxRulesDisplayedOnScreen;

        scroll.value = 1f;

        playerSelector.transform.position = Vector3.zero;
        // Set the selector position data to match the new selected position
        playerSelector.SetSelectorPositionAndParent(0, displayedRules[0].transform.position, displayedRules[0]);
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

        Debug.Log("Choose Rules");

        ChooseRuleset(savedRules[playerSelector.GetSelectedPositionID()]);

        if (gameManager != null)
            gameManager.SetRuleset(currentRuleset);
    }

    public override void Return(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        if (!DetermineIfPlayerCanInputInUI(player.GetPlayerID()))
            return;

        GameManagerNew.Instance.SetGameState(GameStates.PlayerSelect);
        this.gameObject.GetComponent<Canvas>().enabled = false;

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
            if (direction == Direction.Up && playerSelectorCurrentPosition > 0)
            {
                newPos = playerSelectorCurrentPosition - 1;

                if(newPos >= maxRulesDisplayedOnScreen || newPos <= (savedRules.Length - maxRulesDisplayedOnScreen))
                {
                    scroll.value += scrollbarStep;
                }

            }
            else if (direction == Direction.Up && playerSelectorCurrentPosition == 0)
            {
                newPos = playerSelectorCurrentPosition;
            }

            // Handle clicking down
            if (direction == Direction.Down && playerSelectorCurrentPosition < savedRules.Length - 1)
            {
                newPos = playerSelectorCurrentPosition + 1;

                if (newPos >= (savedRules.Length - maxRulesDisplayedOnScreen))
                {
                    scroll.value -= scrollbarStep;
                }

            }
            else if (direction == Direction.Down && playerSelectorCurrentPosition == savedRules.Length - 1)
            {
                newPos = playerSelectorCurrentPosition;
            }
            #endregion MenuMovement

            Debug.Log(newPos);

            // Set the selector position data to match the new selected position
            playerSelector.SetSelectorPositionAndParent(newPos, displayedRules[newPos].transform.position, displayedRules[newPos]);

            UpdateRuleDisplay(savedRules[newPos]);
        }
    }

    private void UpdateRuleDisplay(RulesetSO selectedRuleset)
    {
        ruleName.text = selectedRuleset.NameOfRuleset;

        ruleNumbers[0].text = selectedRuleset.NumOfLaps.ToString();
        ruleNumbers[1].text = selectedRuleset.StartingHealth.ToString();
    }

    private void ChooseRuleset(RulesetSO selectedRuleset)
    {
        currentRuleset = selectedRuleset;

        chosenRuleName.text = selectedRuleset.NameOfRuleset;
        currentRuleset = selectedRuleset;

        if (gameManager != null)
            gameManager.SetRuleset(currentRuleset);
    }
}

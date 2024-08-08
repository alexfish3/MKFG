using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuUI : SingletonGenericUI<SettingsMenuUI>
{
    [Header("Settings Menu UI Info")]
    [SerializeField] List<GameObject> buttonSetA = new List<GameObject>();
    [SerializeField] List<GameObject> buttonSetB = new List<GameObject>();

    [Space(10)]
    [SerializeField] MenuHighlight buttonSelector;

    [Header("Control Settings")]
    [Space(10)]
    [SerializeField] List<InputProfileSO> inputProfiles;

    private int maxInputProfiles = 8;

    private bool inputProfileSelected = false;

    public bool InputProfileSelected { get { return inputProfileSelected; } set { inputProfileSelected = value; } }


    [Header("Button Remaps")]
    [SerializeField] List<GameObject> drivingButtons = new List<GameObject>();
    [SerializeField] List<GameObject> uiButtons = new List<GameObject>();
    private ControlsReassignController controlToBeRemapped;


    private List<ControlsReassignController> reassignButtons = new List<ControlsReassignController>();

    private void OnEnable()
    {
        GameManagerNew.Instance.OnSwapEnterMenu += InitalizeUI;
    }

    private void OnDisable()
    {
        GameManagerNew.Instance.OnSwapEnterMenu -= InitalizeUI;
    }

    public override void InitalizeUI()
    {
        // Get a list of input profiles -- Preset defaults
        //DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath);
        //FileInfo[] data = dir.GetFiles("*_IP.asset", SearchOption.AllDirectories);

        //foreach (var inputProfileData in data)
        //{
        //    InputProfileSO temp = ScriptableObject.CreateInstance<InputProfileSO>();
        //    temp = BinarySerialization.ReadFromBinaryFile<InputProfileSO>(inputProfileData.FullName);

        //    inputProfiles.Add(temp);
        //    Debug.LogWarning(temp.name);
        //}

        foreach(GameObject button in buttonSetB)
        {
            reassignButtons.Add(button.GetComponent<ControlsReassignController>());
        }
        foreach (ControlsReassignController c in reassignButtons)
        {
            c.InputProfileSet(inputProfiles[0]);
        }

        // Display list
        for (int i = 0; i < buttonSetA.Count; i++)
        {
            if (i >= inputProfiles.Count) break;
            if (i >= maxInputProfiles) break;
            if (inputProfiles[i] == null) break;

            // Enable button
            buttonSetA[i].SetActive(true);
            //buttonSetA.Add(availableButtons[i]);

            // Pass input profile info to button
            buttonSetA[i].GetComponentInChildren<TextMeshProUGUI>().text = inputProfiles[i].name;
        }

        // DONT SET SELECTOR POSITION, IT WILL DEFAULT TO 0,0,0 DUE TO THE SCRIPT INITIALIZATION ORDER
    }

    public override void AddPlayerToUI(GenericBrain player)
    {
        base.AddPlayerToUI(player);
    }

    public void GetRebindOption(ControlsReassignController control)
    {
        if (connectedPlayers.Count != 0)
        {
            connectedPlayers[0].OnPressInput += SetRebindOption;
            controlToBeRemapped = control;
        }
    }

    private void SetRebindOption(string pressed)
    {
        connectedPlayers[0].OnPressInput -= SetRebindOption;
        Debug.Log("Rebinding To " + pressed);
        controlToBeRemapped.SetRebindKey(pressed);
    }

    public override void Up(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        MovePlayerSelector(player.GetPlayerID(), Direction.Up);
    }

    public override void Left(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        MovePlayerSelector(player.GetPlayerID(), Direction.Left);
    }

    public override void Down(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        MovePlayerSelector(player.GetPlayerID(), Direction.Down);
    }

    public override void Right(bool status, GenericBrain player)
    {
        if (status == false)
            return;

        MovePlayerSelector(player.GetPlayerID(), Direction.Right);
    }

    public void MovePlayerSelector(int playerID, Direction direction)
    {
        if (!DetermineIfPlayerCanInputInUI(playerID))
            return;

        int playerSelectorCurrentPosition = buttonSelector.selectorPosition;
        int newPos = 0;

        // Handle clicking left or up
        if (direction == Direction.Left || direction == Direction.Up)
        {
            if (inputProfileSelected)
                newPos = playerSelectorCurrentPosition - 1 < 0 ? playerSelectorCurrentPosition = buttonSetB.Count - 1 : playerSelectorCurrentPosition - 1;
            else
            {
                newPos = playerSelectorCurrentPosition - 1 < 0 ? playerSelectorCurrentPosition = buttonSetA.Count - 1 : playerSelectorCurrentPosition - 1;

                if (reassignButtons != null)
                {
                    foreach (ControlsReassignController c in reassignButtons)
                    {
                        c.InputProfileSet(inputProfiles[newPos]);
                    }
                }
            }
        }

        // Handle clicking right or down
        if (direction == Direction.Right || direction == Direction.Down)
        {
            if (inputProfileSelected)
                newPos = playerSelectorCurrentPosition + 1 > buttonSetB.Count - 1 ? 0 : playerSelectorCurrentPosition + 1;
            else
            {
                newPos = playerSelectorCurrentPosition + 1 > buttonSetA.Count - 1 ? 0 : playerSelectorCurrentPosition + 1;

                if (reassignButtons != null)
                {
                    foreach (ControlsReassignController c in reassignButtons)
                    {
                        c.InputProfileSet(inputProfiles[newPos]);
                    }
                }
            }
        }

        if (inputProfileSelected)
            buttonSelector.SetSelectorPosition(buttonSetB[newPos], newPos);
        else
            buttonSelector.SetSelectorPosition(buttonSetA[newPos], newPos);
    }

    public override void Confirm(bool status, GenericBrain player) // L key is confirm for some reason
    {
        if (status == false)
            return;

        int playerID = player.GetPlayerID();
        if (!DetermineIfPlayerCanInputInUI(playerID))
            return;

        if (inputProfileSelected)
        {
            buttonSetB[buttonSelector.selectorPosition].GetComponent<Button>().onClick.Invoke();
        }
        else
        {
            // Run button method
            buttonSetA[buttonSelector.selectorPosition].GetComponent<Button>().onClick.Invoke();
            
            // Set button position
            buttonSelector.SetSelectorPosition(buttonSetB[0], 0);
            
            inputProfileSelected = true;
        }
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
        if (inputProfileSelected)
        {
            inputProfileSelected = false;

            // Save changes
            buttonSelector.SetSelectorPosition(buttonSetA[0], 0);

            // Reset pointer
            buttonSelector.SetSelectorPosition(buttonSetA[0], 0);
        }
        else
        {
            GameManagerNew.Instance.SetGameState(GameStates.MainMenu);
        }
    }
}

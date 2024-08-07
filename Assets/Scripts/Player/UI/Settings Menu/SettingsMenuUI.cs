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

    [SerializeField] InputProfileSO tempFile;

    [Space(10)]
    [SerializeField] GameObject[] availableButtons;

    private int maxInputProfiles = 7;
    private bool inputProfileSelected = false;

    public bool InputProfileSelected { get { return inputProfileSelected; } set { inputProfileSelected = value; } }

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
        // Create a new input profile
        BinarySerialization.WriteToBinaryFile("C:\\Users\\Default\\AppData\\Local", tempFile);

        // Get a list of input profiles
        DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath);
        FileInfo[] data = dir.GetFiles("*_IP.asset", SearchOption.AllDirectories);

        foreach (var inputProfileData in data)
        {
            InputProfileSO temp = ScriptableObject.CreateInstance<InputProfileSO>();
            temp = BinarySerialization.ReadFromBinaryFile<InputProfileSO>(inputProfileData.FullName);

            inputProfiles.Add(temp);
            Debug.LogWarning(temp.name);
        }

        // Display list
        for (int i = 0; i < availableButtons.Length; i++)
        {
            if (i >= inputProfiles.Count) break;
            if (i >= maxInputProfiles) break;
            if (inputProfiles[i] == null) break;

            // Enable button
            availableButtons[i].SetActive(true);
            buttonSetA.Add(availableButtons[i]);

            // Pass input profile info to button
            availableButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = inputProfiles[i].name;
        }

        buttonSelector.SetSelectorPosition(buttonSetA[0], 0);
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
                newPos = playerSelectorCurrentPosition - 1 < 0 ? playerSelectorCurrentPosition = buttonSetA.Count - 1 : playerSelectorCurrentPosition - 1;

        }

        // Handle clicking right or down
        if (direction == Direction.Right || direction == Direction.Down)
        {
            if (inputProfileSelected)
                newPos = playerSelectorCurrentPosition + 1 > buttonSetB.Count - 1 ? 0 : playerSelectorCurrentPosition + 1;
            else
                newPos = playerSelectorCurrentPosition + 1 > buttonSetA.Count - 1 ? 0 : playerSelectorCurrentPosition + 1;
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
            buttonSetB[buttonSelector.selectorPosition].GetComponent<Button>().onClick.Invoke();
        else
            buttonSetA[buttonSelector.selectorPosition].GetComponent<Button>().onClick.Invoke();
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

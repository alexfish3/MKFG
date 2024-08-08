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
    [SerializeField] ControlsReassignController controlsReassignControllerScript;

    [Header("Control Settings")]
    [Space(10)]
    [SerializeField] List<InputProfileSO> inputProfiles;
    [SerializeField] InputProfileSO[] defaultInputProfiles;

    // Private Variables
    private int maxInputProfiles = 8;
    private int buttonSetAPosition = -1;
    private bool inputProfileSelected = false;
    private bool listeningForInput = false;

    // Getters & Setters
    public bool InputProfileSelected { get { return inputProfileSelected; } set { inputProfileSelected = value; } }
    public List<InputProfileSO> InputProfiles { get { return inputProfiles; } }
    public MenuHighlight ButtonSelector { get { return buttonSelector; } }
    public bool ListeningForInput { set { listeningForInput = value; } }

    private void OnEnable()
    {
        GameManagerNew.Instance.OnSwapEnterMenu += InitalizeUI;
    }

    private void OnDisable()
    {
        // Save changes
        // Get first Generic driving input profile
        InputProfileSO temp = ScriptableObject.CreateInstance<InputProfileSO>();
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] data = dir.GetFiles("GenericDriving 1_IP.txt", SearchOption.TopDirectoryOnly);

        BinarySerialization.ReadFromBinaryFile(data[0].FullName, temp);
        // Assign it to the 3rd slot in array
        //GenericBrain. inputProfileOptionsResource[2] = temp;

        GameManagerNew.Instance.OnSwapEnterMenu -= InitalizeUI;
    }

    public override void InitalizeUI()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);

        // Create default profiles if none exist
        foreach (InputProfileSO profile in defaultInputProfiles)
        {
            if (profile != null)
            {
                FileInfo[] dataSet = dir.GetFiles(profile.name + "_IP.txt", SearchOption.TopDirectoryOnly);
                string filePath = Path.Combine(Application.persistentDataPath, profile.name + "_IP.txt");

                if (dataSet.Length == 0)
                {
                    BinarySerialization.WriteToBinaryFile(filePath, profile);

                }
                else
                {
                    foreach (FileInfo file in dataSet)
                    {
                        if (file.FullName != filePath)
                        {
                            BinarySerialization.WriteToBinaryFile(filePath, profile);
                            Debug.LogWarning("Writing to file");
                        }
                    }
                }
            }
        }

        // Get a list of input profiles -- Preset defaults

        FileInfo[] data = dir.GetFiles("*_IP.txt" , SearchOption.TopDirectoryOnly);
        inputProfiles.Clear();
        foreach (var inputProfileData in data)
        {
            InputProfileSO temp = ScriptableObject.CreateInstance<InputProfileSO>();
            BinarySerialization.ReadFromBinaryFile(inputProfileData.FullName, temp);

            inputProfiles.Add(temp);
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
            buttonSetA[i].GetComponentInChildren<TextMeshProUGUI>().text = inputProfiles[i].ProfileName;
        }

        // DONT SET SELECTOR POSITION, IT WILL DEFAULT TO 0,0,0 DUE TO THE SCRIPT INITIALIZATION ORDER
    }

    public override void AddPlayerToUI(GenericBrain player)
    {
        base.AddPlayerToUI(player);

        //If player 0 is controller make it so that buttons display controller scheme
        if (connectedPlayers[0].GetBrainInputType() == InputType.UnityController)
        {
            controlsReassignControllerScript.inputType = ControlsReassignController.ControllerOrKeyboard.Controller;
        }
        else//If player 0 is keyboard make it so that buttons display controller scheme
        {
            controlsReassignControllerScript.inputType = ControlsReassignController.ControllerOrKeyboard.Keyboard;
        }

        //Rewrite the text of the buttons for appropriate actions
        if(InputProfiles.Count > 0)
            controlsReassignControllerScript.InputProfileHover(0);
    }

    public void GetRebindOption(ControlsReassignController control)
    {
        if (connectedPlayers.Count != 0)
        {
            //Listen to input
            connectedPlayers[0].OnPressInput += SetRebindOption;
        }
    }

    private void SetRebindOption(string pressed)
    {
        //Unsubscribe from listening to input and send the input to the controlRemapped script
        connectedPlayers[0].OnPressInput -= SetRebindOption;
        controlsReassignControllerScript.SetRebindKey(pressed);
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

        //If listening for input wait
        if (listeningForInput)
            return;

        int playerSelectorCurrentPosition = buttonSelector.selectorPosition;
        int newPos = 0;

        // Handle clicking left or up
        if (direction == Direction.Left || direction == Direction.Up)
        {
            if (inputProfileSelected) // In button set B
                newPos = playerSelectorCurrentPosition - 1 < 0 ? playerSelectorCurrentPosition = buttonSetB.Count - 1 : playerSelectorCurrentPosition - 1;
            else // IN button set A
            {
                newPos = playerSelectorCurrentPosition - 1 < 0 ? playerSelectorCurrentPosition = buttonSetA.Count - 1 : playerSelectorCurrentPosition - 1;

                //If going through schemes then display that schemes button inputs
                controlsReassignControllerScript.InputProfileHover(newPos);
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

                //If going through schemes then display that schemes button inputs
                controlsReassignControllerScript.InputProfileHover(newPos);
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

        //If listening for input wait
        if (listeningForInput)
            return;

        if (inputProfileSelected)
        {
            buttonSetB[buttonSelector.selectorPosition].GetComponent<Button>().onClick.Invoke();
        }
        else
        {
            // Run button method
            buttonSetA[buttonSelector.selectorPosition].GetComponent<Button>().onClick.Invoke();
            
            buttonSetAPosition = ButtonSelector.selectorPosition;

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

        //If listening for input wait
        if (listeningForInput)
            return;

        // Go back to button set A
        if (inputProfileSelected)
        {
            inputProfileSelected = false;

            // Save single profile
            InputProfileSO profile = ScriptableObject.CreateInstance<InputProfileSO>();
            profile = inputProfiles[buttonSetAPosition];
            defaultInputProfiles[buttonSetAPosition] = profile;
            if (profile != null)
            {
                string filePath = Path.Combine(Application.persistentDataPath, profile.name + "_IP.txt");
                BinarySerialization.WriteToBinaryFile(filePath, profile);
                defaultInputProfiles[buttonSetAPosition] = profile;
            }

            // Reset pointer
            buttonSelector.SetSelectorPosition(buttonSetA[buttonSetAPosition], buttonSetAPosition);

            controlsReassignControllerScript.InputProfileSet(buttonSetAPosition);
        }
        // Return to previous menu
        else
        {
            // Save changes
            foreach (InputProfileSO profile in InputProfiles)
            {
                if (profile != null)
                {
                    string filePath = Path.Combine(Application.persistentDataPath, profile.name + "_IP.txt");

                    BinarySerialization.WriteToBinaryFile(filePath, profile);
                }
            }

            GameManagerNew.Instance.SetGameState(GameStates.MainMenu);
        }
    }
}

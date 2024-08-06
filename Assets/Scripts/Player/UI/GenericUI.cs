///
/// Created by Alex Fischer | June 2024
/// 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericUI : MonoBehaviour
{
    [Header("Generic UI Info")]
    public bool OnePlayerControl = false;
    public bool ToggleCanvas = true;

    public UITypes uiType;

    protected List<GenericBrain> connectedPlayers = new List<GenericBrain>();

    public Canvas canvas;
    bool isCanvasEnabled = false;

    public virtual void InitalizeUI() { }

    public void OnDestroy()
    {
        foreach (GenericBrain brain in connectedPlayers)
        {
            if (brain == null)
                continue;

            brain.UnsubscribeInputs();
        }
    }

    public virtual void AddPlayerToUI(GenericBrain player) 
    {
        if (player == null)
            return;

        if(isCanvasEnabled == false && ToggleCanvas == true && connectedPlayers.Count <= 0)
        {
            canvas.enabled = true;
            isCanvasEnabled = true;
        }

        connectedPlayers.Add(player);
    }

    public virtual void RemovePlayerUI(GenericBrain player)
    {        
        player.UnsubscribeInputs();

        if (player == null)
            return;

        connectedPlayers.Remove(player);

        // Compares the state of the brain to the current ui
        // On any normal menu change, these two would be different indicating brain is to be moved to another generic ui
        // If they are the same, this means that the controller was disconnected, in which we dont want the menu to disable
        if (String.Equals(player.GetLocalBrainGamestate().ToString(), uiType.ToString()))
        {
            Debug.Log($"@@ {player.GetLocalBrainGamestate().ToString()} is equal to {uiType.ToString()}");
            //Debug.Log("Player Removed but not going to new menu!!!");
        }
        else
        {
            Debug.Log($"@@{player.GetLocalBrainGamestate().ToString()} is not equal to {uiType.ToString()}");

            if (isCanvasEnabled == true && ToggleCanvas == true && connectedPlayers.Count <= 0)
            {
                canvas.enabled = false;
                isCanvasEnabled = false;
            }
        }
    }

    // When player disconnects, reinitalize all players 
    public virtual void ReinitalizePlayerIDs(int positionRemoved)
    {
        for (int i = positionRemoved; i < connectedPlayers.Count; i++)
        {
            // Set player ID to be new value
            connectedPlayers[i].SetPlayerID(i);
        }
    }

    public void ClearConnectedPlayers() { connectedPlayers.Clear(); }

    public virtual GenericUI ReturnGenericUI()
    {
        return this;
    }

    /// <summary>
    /// The generic method for when up is pressed
    /// </summary>
    public virtual void Up(bool status, GenericBrain player)
    {

    }

    /// <summary>
    /// The generic method for when left is pressed
    /// </summary>
    public virtual void Left(bool status, GenericBrain player)
    {

    }

    /// <summary>
    /// The generic method for when down is pressed
    /// </summary>
    public virtual void Down(bool status, GenericBrain player)
    {

    }

    /// <summary>
    /// The generic method for when right is pressed
    /// </summary>
    public virtual void Right(bool status, GenericBrain player)
    {

    }

    /// <summary>
    /// The generic method for when confirm is pressed
    /// </summary>
    public virtual void Confirm(bool status, GenericBrain player)
    {

    }

    /// <summary>
    /// The generic method for when return is pressed
    /// </summary>
    public virtual void Return(bool status, GenericBrain player)
    {

    }

    /// <summary>
    /// The generic method for when button 1 is pressed
    /// </summary>
    public virtual void Button1(bool status, GenericBrain player)
    {

    }

    /// <summary>
    /// The generic method for when button 2 is pressed
    /// </summary>
    public virtual void Button2(bool status, GenericBrain player)
    {

    }

    /// <summary>
    /// The generic method for when pause is pressed
    /// </summary>
    public virtual void Pause(bool status, GenericBrain player)
    {

    }

    /// <summary>
    /// Determines if the player with the passed in player ID can control the UI
    /// Returns True if the player can control
    /// Returns False if the player cannot control
    /// </summary>
    /// <param name="playerID">The passed in player ID of the player input</param>
    /// <returns></returns>
    public bool DetermineIfPlayerCanInputInUI(int playerID)
    {
        // If only player 1 is allowed and input is player 1
        if (OnePlayerControl == true && playerID == 0)
        {
            return true;
        }
        // If only player 1 is allowed and input is not player 1
        else if (OnePlayerControl == true && playerID != 0)
        {
            return false;
        }

        // Returns if one player control is false
        return true;
    }
}

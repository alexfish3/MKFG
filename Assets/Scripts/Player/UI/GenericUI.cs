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

    public UITypes uiType;

    public List<GenericBrain> connectedPlayers = new List<GenericBrain>();

    public Canvas canvas;
    bool isCanvasEnabled = false;

    public virtual void InitalizeUI() { }

    public virtual void AddPlayerToUI(GenericBrain player) 
    {
        if(isCanvasEnabled == false && connectedPlayers.Count <= 0)
        {
            canvas.enabled = true;
            isCanvasEnabled = true;
        }

        connectedPlayers.Add(player);
    }

    public virtual void RemovePlayerUI(GenericBrain player)
    {
        connectedPlayers.Remove(player);

        if (isCanvasEnabled == true && connectedPlayers.Count <= 0)
        {
            canvas.enabled = false;
            isCanvasEnabled = false;
        }
    }

    public void ClearConnectedPlayers() { connectedPlayers.Clear(); }

    public virtual GenericUI ReturnGenericUI()
    {
        return this;
    }

    /// <summary>
    /// The generic up method for when up is pressed
    /// </summary>
    public virtual void Up(bool status, GenericBrain player)
    {

    }

    /// <summary>
    /// The generic left method for when left is pressed
    /// </summary>
    public virtual void Left(bool status, GenericBrain player)
    {

    }

    /// <summary>
    /// The generic down method for when down is pressed
    /// </summary>
    public virtual void Down(bool status, GenericBrain player)
    {

    }

    /// <summary>
    /// The generic right method for when right is pressed
    /// </summary>
    public virtual void Right(bool status, GenericBrain player)
    {

    }

    /// <summary>
    /// The generic down method for when confirm is pressed
    /// </summary>
    public virtual void Confirm(bool status, GenericBrain player)
    {

    }

    /// <summary>
    /// The generic right method for when return is pressed
    /// </summary>
    public virtual void Return(bool status, GenericBrain player)
    {

    }

    /// <summary>
    /// The generic right method for when button 1 is pressed
    /// </summary>
    public virtual void Button1(bool status, GenericBrain player)
    {

    }

    /// <summary>
    /// The generic right method for when button 2 is pressed
    /// </summary>
    public virtual void Button2(bool status, GenericBrain player)
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

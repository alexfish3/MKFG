using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericUI : MonoBehaviour
{
    public bool OnePlayerControl = false;

    public UITypes uiType;

    public List<GenericBrain> connectedPlayers = new List<GenericBrain>();
    public void AddPlayerToUI(GenericBrain player) {connectedPlayers.Add(player);}
    public void ClearConnectedPlayers() { connectedPlayers.Clear(); }

    public virtual GenericUI ReturnGenericUI()
    {
        return this;
    }

    /// <summary>
    /// The generic up method for when up is pressed
    /// </summary>
    public virtual void Up(bool status)
    {

    }

    /// <summary>
    /// The generic left method for when left is pressed
    /// </summary>
    public virtual void Left(bool status)
    {

    }

    /// <summary>
    /// The generic down method for when down is pressed
    /// </summary>
    public virtual void Down(bool status)
    {

    }

    /// <summary>
    /// The generic right method for when right is pressed
    /// </summary>
    public virtual void Right(bool status)
    {

    }

    /// <summary>
    /// The generic down method for when confirm is pressed
    /// </summary>
    public virtual void Confirm(bool status)
    {

    }

    /// <summary>
    /// The generic right method for when return is pressed
    /// </summary>
    public virtual void Return(bool status)
    {

    }

}

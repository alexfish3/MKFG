using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectUI : SingletonGenericUI<PlayerSelectUI>
{
    public override void Up(bool status, int playerID)
    {
        if (RestrictInputBasedOnPlayerID(playerID))
            return;

        Debug.Log("Player Select UI");
        //base.Up(status, playerID);
    }

    public override void Left(bool status, int playerID)
    {
        if (RestrictInputBasedOnPlayerID(playerID))
            return;

        Debug.Log("Player Select UI");
        //base.Left(status, playerID);
    }

    public override void Down(bool status, int playerID)
    {
        if (RestrictInputBasedOnPlayerID(playerID))
            return;

        Debug.Log("Player Select UI");
        //base.Down(status, playerID);
    }

    public override void Right(bool status, int playerID)
    {
        if (RestrictInputBasedOnPlayerID(playerID))
            return;

        Debug.Log("Player Select UI");
        //base.Right(status, playerID);
    }

    public override void Confirm(bool status, int playerID)
    {
        if (RestrictInputBasedOnPlayerID(playerID))
            return;

        Debug.Log("Player Select UI");
        //base.Confirm(status, playerID);
    }

    public override void Return(bool status, int playerID)
    {
        if (RestrictInputBasedOnPlayerID(playerID))
            return;

        Debug.Log("Player Select UI");
        //base.Return(status, playerID);
    }
}

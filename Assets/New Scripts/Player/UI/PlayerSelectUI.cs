using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectUI : SingletonGenericUI<PlayerSelectUI>
{
    public override void Up(bool status)
    {
        Debug.Log("Player Select UI");
        base.Up(status);
    }

    public override void Left(bool status)
    {
        Debug.Log("Player Select UI");
        base.Left(status);
    }

    public override void Down(bool status)
    {
        Debug.Log("Player Select UI");
        base.Down(status);
    }

    public override void Right(bool status)
    {
        Debug.Log("Player Select UI");
        base.Right(status);
    }

    public override void Confirm(bool status)
    {
        Debug.Log("Player Select UI");
        base.Confirm(status);
    }

    public override void Return(bool status)
    {
        Debug.Log("Player Select UI");
        base.Return(status);
    }
}

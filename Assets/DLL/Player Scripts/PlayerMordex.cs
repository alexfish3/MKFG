using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMordex : PlayerMain
{
    public override void MoveDown()
    {
        Debug.Log("Mordex Move Down");
        base.MoveDown();
    }

    public override void MoveLeft()
    {
        Debug.Log("Mordex Move Left");
        base.MoveLeft();
    }

    public override void MoveRight()
    {
        Debug.Log("Mordex Move Right");
        base.MoveRight();
    }

    public override void MoveUp()
    {
        Debug.Log("Mordex Move Up");
        base.MoveUp();
    }
}

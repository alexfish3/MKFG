using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOrion : PlayerMain
{
    public override void MoveDown()
    {
        Debug.Log("Orion Move Down");
        base.MoveDown();
    }

    public override void MoveLeft()
    {
        Debug.Log("Orion Move Left");
        base.MoveLeft();
    }

    public override void MoveRight()
    {
        Debug.Log("Orion Move Right");
        base.MoveRight();
    }

    public override void MoveUp()
    {
        Debug.Log("Orion Move Up");
        base.MoveUp();
    }
}

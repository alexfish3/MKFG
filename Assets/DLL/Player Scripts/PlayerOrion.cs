using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOrion : PlayerMain
{

    public override void Down(bool status)
    {
        Debug.Log("Orion Move Down");
        base.Down(status);
    }

    public override void Left(bool status)
    {
        Debug.Log("Orion Move Left");
        base.Left(status);
    }

    public override void Right(bool status)
    {
        Debug.Log("Orion Move Right");
        base.Right(status);
    }

    public override void Up(bool status)
    {
        Debug.Log("Orion Move Up");
        base.Up(status);
    }

    public override void Drift(bool status)
    {
        Debug.Log("Orion Drift");
        base.Drift(status);
    }

    public override void Attack(bool status)
    {
        LeftAttack();
        Debug.Log("Orion Attack");
        base.Attack(status);
    }

    public override void Special(bool status)
    {
        Debug.Log("Orion Special");
        base.Special(status);
    }

    public void LeftAttack()
    {
        leftAttack.SetActive(true);
    }

    //public void Update()
    //{
    //    //Test add damage
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        //Add left attack button
    //        LeftAttack();
    //    }
    //}
}

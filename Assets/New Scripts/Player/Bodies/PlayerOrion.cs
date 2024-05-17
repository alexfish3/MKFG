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
        //check for direction of attack
        if (!isPlayerAttacking())
        {
            if (ballDriving.left)
            {
                SideAttack(true);
            }
            else if (ballDriving.right)
            {
                SideAttack(false);
            }
            else if (ballDriving.up)
            {
                ForwardAttack();
            }
            else if (ballDriving.down)
            {
                BackAttack();
            }
            else
            {
                NeutralAttack();
            }
        }

        Debug.Log("Orion Attack");
        base.Attack(status);
    }

    public override void Special(bool status)
    {
        Debug.Log("Orion Special");
        base.Special(status);
    }

    public override void Drive(bool status)
    {
        base.Drive(status);
    }

    public override void Reverse(bool status)
    {
        base.Reverse(status);
    }

    public void SideAttack(bool left)
    {
        sideAttack.SetActive(true);
        //Direction of side attack
        if (left)
        {
            sideAttack.transform.localScale = new Vector3(1, sideAttack.transform.localScale.y, sideAttack.transform.localScale.z);
        } else
        {
            sideAttack.transform.localScale = new Vector3(-1, sideAttack.transform.localScale.y, sideAttack.transform.localScale.z);
        }
    }

    public void ForwardAttack()
    {
        forwardAttack.SetActive(true);
    }

    public void BackAttack()
    {
        backAttack.SetActive(true);
    }

    public void NeutralAttack()
    {
        neutralAttack.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOrion : PlayerMain
{
    public override void Down(bool status)
    {
        if (!IsClientPlayer()) return;

        Debug.Log("Orion Move Down");
        base.Down(status);
    }

    public override void Left(bool status)
    {
        if (!IsClientPlayer()) return;

        Debug.Log("Orion Move Left");
        base.Left(status);
    }

    public override void Right(bool status)
    {
        if (!IsClientPlayer()) return;

        Debug.Log("Orion Move Right");
        base.Right(status);
    }

    public override void Up(bool status)
    {
        if (!IsClientPlayer()) return;

        Debug.Log("Orion Move Up");
        base.Up(status);
    }

    public override void Drift(bool status)
    {
        if (!IsClientPlayer()) return;

        Debug.Log("Orion Drift");
        base.Drift(status);
    }

    public override void Attack(bool status)
    {
        if (!IsClientPlayer()) return;

        //check for direction of attack
        if (!isPlayerAttacking() && stunTime <= 0)
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
        if (!IsClientPlayer()) return;

        //check for direction of attack
        if (!isPlayerAttacking())
        {
            if (ballDriving.left)
            {
                SideSpecial(true);
            }
            else if (ballDriving.right)
            {
                SideSpecial(false);
            }
            else if (ballDriving.up)
            {
                ForwardSpecial();
            }
            else if (ballDriving.down)
            {
                BackSpecial();
            }
            else
            {
                NeutralSpecial();
            }
        }

        base.Special(status);
    }

    public override void Drive(bool status)
    {
        if (!IsClientPlayer()) return;

        base.Drive(status);
    }

    public override void Reverse(bool status)
    {
        if (!IsClientPlayer()) return;

        base.Reverse(status);
    }

    public void SideAttack(bool left)
    {
        if (!IsClientPlayer()) return;

        //Direction of side attack
        if (left)
        {
            attacks[0].transform.localScale = new Vector3(1, attacks[0].transform.localScale.y, attacks[0].transform.localScale.z);
        } else
        {

            attacks[0].transform.localScale = new Vector3(-1, attacks[0].transform.localScale.y, attacks[0].transform.localScale.z);
        }

        attacks[0].SetActive(true);
    }

    public void ForwardAttack()
    {
        if (!IsClientPlayer()) return;

        attacks[1].SetActive(true);
    }

    public void BackAttack()
    {
        if (!IsClientPlayer()) return;

        attacks[2].SetActive(true);
    }

    public void NeutralAttack()
    {
        if (!IsClientPlayer()) return;

        attacks[3].SetActive(true);
    }

    public void SideSpecial(bool left)
    {
        if (!IsClientPlayer()) return;

        specials[0].SetActive(true);
        //Direction of side attack
        if (!left)
        {
            specials[0].transform.localScale = new Vector3(1, specials[0].transform.localScale.y, specials[0].transform.localScale.z);
        }
        else
        {
            specials[0].transform.localScale = new Vector3(-1, specials[0].transform.localScale.y, specials[0].transform.localScale.z);
        }
    }

    public void ForwardSpecial()
    {
        if (!IsClientPlayer()) return;

        specials[1].SetActive(true);
    }

    public void BackSpecial()
    {
        if (!IsClientPlayer()) return;

        specials[2].SetActive(true);
    }

    public void NeutralSpecial()
    {
        if (!IsClientPlayer()) return;

        specials[3].SetActive(true);
    }
}

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
        if (!isPlayerAttacking() && stunTime <= 0)
        {
            //Handle Controller Deadzone

            //Diagonal Up Left
            if (ballDriving.left && ballDriving.up && ballDriving.leftStick.x != 0)
            {
                //if more left or more up then do x
                if (ballDriving.leftStick.x * -1 >= ballDriving.leftStick.y)
                {
                    SideAttack(true);
                } else
                {
                    ForwardAttack();
                }
            }
            //Diagonal Up Right
            else if (ballDriving.right && ballDriving.up && ballDriving.leftStick.x != 0)
            {
                //if more right or more up then do x
                if (ballDriving.leftStick.x >= ballDriving.leftStick.y)
                {
                    SideAttack(true);
                }
                else
                {
                    ForwardAttack();
                }
            }
            //Down prioritized
            else if (ballDriving.down)
            {
                BackAttack();
            }
            else if (ballDriving.left)
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
            else
            {
                NeutralAttack();
            }
        }

        Debug.Log("Orion Attack WORKING");
        base.Attack(status);
    }

    public override void Special(bool status)
    {
        //check for direction of attack
        if (!isPlayerAttacking() && stunTime <= 0)
        {
            //Diagonal Up Left
            if (ballDriving.left && ballDriving.up && ballDriving.leftStick.x != 0)
            {
                //if more left or more up then do x
                if (ballDriving.leftStick.x * -1 >= ballDriving.leftStick.y)
                {
                    sideSpecialCooldownTimer = specialsInfo[0].specialRecoveryTime;
                    SideSpecial(true);
                }
                else
                {
                    forwardSpecialCooldownTimer = specialsInfo[1].specialRecoveryTime;
                    ForwardSpecial();
                }
            }
            //Diagonal Up Right
            else if (ballDriving.right && ballDriving.up && ballDriving.leftStick.x != 0)
            {
                //if more right or more up then do x
                if (ballDriving.leftStick.x >= ballDriving.leftStick.y)
                {
                    sideSpecialCooldownTimer = specialsInfo[0].specialRecoveryTime;
                    SideSpecial(true);
                }
                else
                {
                    forwardSpecialCooldownTimer = specialsInfo[1].specialRecoveryTime;
                    ForwardSpecial();
                }
            }
            else if (ballDriving.down)
            {
                if (backSpecialCooldownTimer <= 0)
                {
                    backSpecialCooldownTimer = specialsInfo[2].specialRecoveryTime;
                    BackSpecial();
                }
            }
            else if (ballDriving.left)
            {
                if (sideSpecialCooldownTimer <= 0)
                {
                    sideSpecialCooldownTimer = specialsInfo[0].specialRecoveryTime;
                    SideSpecial(true);
                }
            }
            else if (ballDriving.right)
            {
                if (sideSpecialCooldownTimer <= 0)
                {
                    sideSpecialCooldownTimer = specialsInfo[0].specialRecoveryTime;
                    SideSpecial(false);
                }
            }
            else if (ballDriving.up)
            {
                if (forwardSpecialCooldownTimer <= 0)
                {
                    forwardSpecialCooldownTimer = specialsInfo[1].specialRecoveryTime;
                    ForwardSpecial();
                }
            }
            else
            {
                if (neutralSpecialCooldownTimer <= 0)
                {
                    neutralSpecialCooldownTimer = specialsInfo[3].specialRecoveryTime;
                    NeutralSpecial();
                }
            }
        }

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
        attacks[1].SetActive(true);
    }

    public void BackAttack()
    {
        attacks[2].SetActive(true);
    }

    public void NeutralAttack()
    {
        attacks[3].SetActive(true);
    }

    public void SideSpecial(bool left)
    {
        specials[0].SetActive(true);
        //Direction of side attack
        if (left)
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
        specials[1].SetActive(true);
    }

    public void BackSpecial()
    {
        specials[2].SetActive(true);
    }

    public void NeutralSpecial()
    {
        specials[3].SetActive(true);
    }
}

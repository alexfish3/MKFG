using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDefault : PlayerMain
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
        base.Attack(status);
        if (status == false)
            return;

        //check for direction of attack
        if (!isPlayerAttacking() && stunTime <= 0)
        {
            //Handle Controller Deadzone

            //Diagonal Up Left
            if (ballDriving.left && ballDriving.up && ballDriving.leftStick.x < 0)
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
            else if (ballDriving.right && ballDriving.up && ballDriving.leftStick.x > 0)
            {
                //if more right or more up then do x
                if (ballDriving.leftStick.x >= ballDriving.leftStick.y)
                {
                    SideAttack(false);
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
    }

    public override void Special(bool status)
    {
        base.Special(status);
        if (status == false)
            return;

        //check for direction of attack
        if (!isPlayerAttacking() && stunTime <= 0)
        {
            //Diagonal Up Left
            if (ballDriving.left && ballDriving.up && ballDriving.leftStick.x != 0)
            {
                //if more left or more up then do x
                if (ballDriving.leftStick.x * -1 >= ballDriving.leftStick.y)
                {
                    SideSpecial(true);
                }
                else
                {
                    ForwardSpecial();
                }
            }
            //Diagonal Up Right
            else if (ballDriving.right && ballDriving.up && ballDriving.leftStick.x != 0)
            {
                //if more right or more up then do x
                if (ballDriving.leftStick.x >= ballDriving.leftStick.y)
                {
                    SideSpecial(false);
                }
                else
                {
                    ForwardSpecial();
                }
            }
            else if (ballDriving.down)
            {
                BackSpecial();
            }
            else if (ballDriving.left)
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
            else
            {
                NeutralSpecial();
            }
        }
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

            attacksInfo[0].animationTrigger = "LeftLightAttack";
        } else
        {

            attacks[0].transform.localScale = new Vector3(-1, attacks[0].transform.localScale.y, attacks[0].transform.localScale.z);

            attacksInfo[0].animationTrigger = "RightLightAttack";
        }

        attacks[0].SetActive(true);
        //soundPool.PlaySound("orion_attack", playerBodyBall.transform.position);
    }

    public void ForwardAttack()
    {
        attacks[1].SetActive(true);
        //soundPool.PlaySound("orion_attack", playerBodyBall.transform.position);
    }

    public void BackAttack()
    {
        attacks[2].SetActive(true);
        //soundPool.PlaySound("orion_attack", playerBodyBall.transform.position);
    }

    public void NeutralAttack()
    {
        attacks[3].SetActive(true);
        //soundPool.PlaySound("orion_attack", playerBodyBall.transform.position);
    }

    public void SideSpecial(bool left)
    {
        if (!specials[0].activeInHierarchy && sideSpecialCooldownTimer <= 0)
        {
            //Direction of side attack
            if (left)
            {
                specials[0].transform.localScale = new Vector3(1, specials[0].transform.localScale.y, specials[0].transform.localScale.z);

                specialsInfo[0].animationTrigger = "LeftSpecialAttack";

            }
            else
            {
                specials[0].transform.localScale = new Vector3(-1, specials[0].transform.localScale.y, specials[0].transform.localScale.z);

                specialsInfo[0].animationTrigger = "RightSpecialAttack";
            }

            specials[0].SetActive(true);
            //soundPool.PlaySound("orion_side_special", playerBodyBall.transform.position);
        }
    }

    public void ForwardSpecial()
    {
        if (!specials[1].activeInHierarchy && forwardSpecialCooldownTimer <= 0)
        {
            specials[1].SetActive(true);
            //soundPool.PlaySound("orion_forward_special", playerBodyBall.transform.position);
        }
    }

    public void BackSpecial()
    {
        if (!specials[2].activeInHierarchy && backSpecialCooldownTimer <= 0)
        {
            specials[2].SetActive(true);
            //soundPool.PlaySound("orion_back_special", playerBodyBall.transform.position);
        }
    }

    public void NeutralSpecial()
    {
        if (!specials[3].activeInHierarchy && neutralSpecialCooldownTimer <= 0)
        {
            specials[3].SetActive(true);
            //soundPool.PlaySound("orion_neutral_special", playerBodyBall.transform.position);
        }
    }
}
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

    public void LeftAttack()
    {
        leftAttack.SetActive(true);
    }

    public void OnHit(Vector3 dir, float force, float stun, float damage)
    {
        base.stunTime = stun;
        ballDriving.rb.AddForce(dir * force, ForceMode.Force);
        SetHealthMultiplier(GetHealthMultiplier() - damage);
    }

    public void Update()
    {
        //Test add damage
        if (Input.GetKeyDown(KeyCode.P))
        {
            //Add left attack button
            LeftAttack();
        }
    }
}

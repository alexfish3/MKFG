using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAttack : MonoBehaviour
{
    //each attack derives from lightattack
    [SerializeField] GameObject[] hitboxes;
    HitBoxInfo[] hitboxesInfo;
    [SerializeField] float attackRecoveryIfLanded = 0;
    bool startup, active, recovery;
    [SerializeField] bool attackLanded;

    int currentHitBox = 0;
    float attackTimer = 0;

    public bool hasLanded()
    {
        for (int i = 0; i < hitboxes.Length; i++)
        {
            if (hitboxesInfo[i].attackLanded)
            {
                return true;
            }
        }
        return false;
    }

    void OnEnable()
    {
        //set default values
        startup = true;
        active = false;
        recovery = false;
        currentHitBox = 0;
        attackTimer = 0;

        //Set hitbox info
        hitboxesInfo = new HitBoxInfo[hitboxes.Length];
        for (int i = 0; i < hitboxes.Length; i++)
        {
            hitboxesInfo[i] = hitboxes[i].GetComponent<HitBoxInfo>();
        }

    }


    // Update is called once per frame
    void Update()
    {
        //if hitbox startup, go through first hitbox
        if (startup && attackTimer <= hitboxesInfo[currentHitBox].startupTime)
        {
            attackTimer += Time.deltaTime;
        } else if (startup) //When hitbox startup finishes
        {
            startup = false;
            active = true;
            attackTimer = 0;
        }

        //if hitbox active, enable hitbox
        if (active && attackTimer <= hitboxesInfo[currentHitBox].activeTime)
        {
            hitboxes[currentHitBox].SetActive(true);
            attackTimer += Time.deltaTime;
        }
        else if (active) //When hitbox startup finishes
        {
            active = false;
            recovery = true;
            attackTimer = 0;
        }

        //if hitbox recovery, disable hitbox and wait
        if (recovery && attackTimer <= hitboxesInfo[currentHitBox].recoveryTime)
        {
            hitboxes[currentHitBox].SetActive(false);
            attackTimer += Time.deltaTime;
        }
        else if (recovery) //When hitbox startup finishes
        {
            currentHitBox += 1;
            recovery = false;
            attackTimer = 0;
            
            //if not last hitbox then add to currenthitbox and go next active
            if (currentHitBox == hitboxesInfo.Length)
            {
                gameObject.SetActive(false);
            } else
            {
                startup = true;
            }
            
        }

    }

    private void FixedUpdate()
    {
        //If attack lands then set recovery to attack landed recovery frames
        if (hasLanded()) { 
        } else
        {
        }
        ///
        ///
    }
}

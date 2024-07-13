using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAttack : MonoBehaviour
{
    //each attack derives from lightattack
    [SerializeField] PlayerMain player;
    [SerializeField] GameObject kart;

    [SerializeField] GameObject[] hitboxes;
    public HitBoxInfo[] hitboxesInfo;
    bool startup, active, recovery;
    public float activeTimeRemaining = 0;
    public bool isUtility = false;
    [SerializeField] public float specialRecoveryTime = 0;

    int currentHitBox = 0;
    float attackTimer = 0;
    public bool hasLanded = false;
    public bool flipped = false;

    void OnEnable()
    {
        //set default values
        startup = true;
        active = false;
        recovery = false;
        currentHitBox = 0;
        attackTimer = 0;
        hasLanded = false;

        //Set hitbox info
        hitboxesInfo = new HitBoxInfo[hitboxes.Length];
        for (int i = 0; i < hitboxes.Length; i++)
        {
            hitboxesInfo[i] = hitboxes[i].GetComponent<HitBoxInfo>();
        }

        //If first hitbox has player stun then set stun
        if (hitboxesInfo[currentHitBox].lockPlayerMovement)
        {
            player.stunTime = hitboxesInfo[currentHitBox].startupTime;
            player.stunTime += hitboxesInfo[currentHitBox].activeTime;
            player.stunTime += hitboxesInfo[currentHitBox].recoveryTime;
        }
        if (player.lastAttack == gameObject && player.sameAttackTimer > 0)
        {
            this.gameObject.SetActive(false);
        }

        //Initialize Active Time 
        activeTimeRemaining = hitboxesInfo[0].activeTime + hitboxesInfo[0].startupTime;
    }

    private void OnDisable()
    {
        if (hasLanded)
        {
            //Allow for chase dash
            player.ballDriving.chaseDashInputTimer = 0;
        }

        //Make sure steer multiplier gets set back to 1
        player.steerMultiplier = 1;

        player.lastAttack = this.gameObject;
        if (player.sameAttackTimer <= 0)
        {
            player.sameAttackTimer = player.sameAttackTime;
        }
    }


    // Update is called once per frame
    void Update()
    {
        //End Attack Override
        if (currentHitBox >= hitboxesInfo.Length)
        {
            gameObject.SetActive(false);
            return;
        }

        #region Startup
        //if hitbox startup, go through first hitbox
        if (startup && attackTimer < hitboxesInfo[currentHitBox].startupTime)
        {
            attackTimer += Time.deltaTime;
        } else if (startup) //When hitbox startup finishes
        {
            startup = false;
            active = true;
            activeTimeRemaining = hitboxesInfo[currentHitBox].activeTime;
            hitboxes[currentHitBox].SetActive(true);

            //Steer Multiplier
            player.steerMultiplier = hitboxesInfo[currentHitBox].steerMultiplier;

            attackTimer = 0;
        }
        #endregion

        #region Active
        //if hitbox active, enable hitbox
        if (active && attackTimer < hitboxesInfo[currentHitBox].activeTime)
        {
            attackTimer += Time.deltaTime;
            activeTimeRemaining = hitboxesInfo[currentHitBox].activeTime - attackTimer;

            //If hitbox lands
            if (hitboxesInfo[currentHitBox].attackLanded)
            {
                hasLanded = true;
            }
        }
        else if (active) //When hitbox active finishes
        {
            active = false;
            activeTimeRemaining = 0;
            recovery = true;
            attackTimer = 0;

            //If hitbox lands
            if (hitboxesInfo[currentHitBox].attackLanded)
            {
                hasLanded = true;
            }

            hitboxes[currentHitBox].SetActive(false);
        }
        #endregion

        #region Recovery
        //if hitbox recovery, disable hitbox and wait
        if (recovery && attackTimer < hitboxesInfo[currentHitBox].recoveryTime)
        {
            attackTimer += Time.deltaTime;
        }
        else if (recovery) //When hitbox startup finishes
        {
            //End if Missed
            if (hitboxesInfo[currentHitBox].endIfMiss && !hasLanded)
            {
                currentHitBox = hitboxesInfo.Length;
            }
            else { 
            currentHitBox += 1;
            
            }
            recovery = false;
            attackTimer = 0;
            
            //if not last hitbox then add to currenthitbox and go next active
            if (currentHitBox == hitboxesInfo.Length)
            {
                gameObject.SetActive(false);
            } else
            {
                startup = true;
                //Add player stun to next attack
                if (hitboxesInfo[currentHitBox].lockPlayerMovement)
                {
                    player.stunTime = hitboxesInfo[currentHitBox].startupTime;
                    player.stunTime += hitboxesInfo[currentHitBox].activeTime;
                    player.stunTime += hitboxesInfo[currentHitBox].recoveryTime;
                }
            }
            
        }
        #endregion

    }

    private void FixedUpdate()
    {
        if (hitboxesInfo[currentHitBox].moveForce > 0)
        {
            Vector3 moveDirection = Vector3.zero;

            if (Mathf.Sign(gameObject.transform.localScale.x) > 0)
            {
                moveDirection += (-kart.transform.right * hitboxesInfo[currentHitBox].moveDirection.normalized.x).normalized;
            }
            else //If Right
            {
                moveDirection += (kart.transform.right * hitboxesInfo[currentHitBox].moveDirection.normalized.x).normalized;
            }

            //Forwards/Back
            moveDirection += (kart.transform.forward * hitboxesInfo[currentHitBox].moveDirection.normalized.z).normalized;

            player.ballDriving.rb.AddForce(moveDirection.normalized * hitboxesInfo[currentHitBox].moveForce, ForceMode.Force);
        }
    }
}

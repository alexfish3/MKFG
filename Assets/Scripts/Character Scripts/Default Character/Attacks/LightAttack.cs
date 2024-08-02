using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackStatus
{
    none,
    startup,
    active,
    recovery
};
public class LightAttack : MonoBehaviour
{
    //each attack derives from lightattack
    [Header("References")]
    [SerializeField] PlayerMain player;
    [SerializeField] GameObject kart;
    [SerializeField] PlacementHandler placement;

    [Header("VFX")]
    [SerializeField] Animator animator;
    public string animationTrigger;
    [SerializeField] Vector3 sideVfxSpawnPosition;
    [Tooltip("The rotation in the y for how much the vfx should rotate when flipped to the other side of the kart")]
    [SerializeField] float sideVfxRotation;

    [Header("Hitboxes")]
    [SerializeField] GameObject[] hitboxes;
    public HitBoxInfo[] hitboxesInfo;
    [HideInInspector] public bool startup, active, recovery;
    public float activeTimeRemaining = 0;
    int currentHitBox = 0;
    [HideInInspector] public float attackTimer = 0;
    public bool hasLanded = false;
    public bool flipped = false;

    [Header("Specials")]
    public bool isUtility = false;
    [SerializeField] public float specialRecoveryTime = 0;
    public enum SpecialInput
    {
        side,
        forward,
        back,
        neutral
    }
    [SerializeField] public SpecialInput special;
    [SerializeField] public bool specialCooldownByPlacement = false;
    [SerializeField] public float specialCooldownMultiplier = 0.25f;

    // audio
    [Header("Audio")]
    [SerializeField] private SoundPool soundPool;
    [SerializeField] private bool playAudio; 
    [SerializeField] private AttackStatus sfxType;
    [SerializeField] private string sfxKey;
    private bool dirtyAudio = false;

    private bool cutShort;


    void OnEnable()
    {

        //set default values
        startup = true;

        active = false;
        recovery = false;
        currentHitBox = 0;
        attackTimer = 0;
        hasLanded = false;
        dirtyAudio = false;
        cutShort = false;

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
            cutShort = true;
            this.gameObject.SetActive(false);
        }
        else
        {
            if (animator != null) animator.SetBool(animationTrigger, true);
        }

        //Reads if the hitbox has been flipped so it can flip the vfx objects
        if (hitboxesInfo[0].vfx != null)
        {
            Transform vfxTran = hitboxesInfo[0].vfx.transform;
            switch (animationTrigger)
            {

                case "LeftLightAttack":
                    if (vfxTran.localPosition != sideVfxSpawnPosition) {
                        vfxTran.localPosition = new Vector3(vfxTran.localPosition.x * -1, vfxTran.localPosition.y, vfxTran.localPosition.z);
                        vfxTran.Rotate(0, 0, -sideVfxRotation, Space.Self);
                    }
                    break;

                case "RightLightAttack":
                    if (vfxTran.localPosition == sideVfxSpawnPosition)
                    {
                        vfxTran.localPosition = new Vector3(vfxTran.localPosition.x * -1, vfxTran.localPosition.y, vfxTran.localPosition.z);
                        vfxTran.Rotate(0, 0, sideVfxRotation, Space.Self);
                    }
                    break;
            }
        }

        //Initialize Active Time 
        activeTimeRemaining = hitboxesInfo[0].activeTime + hitboxesInfo[0].startupTime;

        //VFX first hitbox
        if (hitboxesInfo[currentHitBox].vfx != null && hitboxesInfo[currentHitBox].vfxState == HitBoxInfo.vfxPlayState.startup)
        {
            hitboxesInfo[currentHitBox].vfx.Play();
        }
    }

    private void OnDisable()
    {
        float oldCooldown = specialRecoveryTime;

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
        
        //Special Cooldown If By Placement
        if (specialCooldownByPlacement)
        {
            specialRecoveryTime *= 1 - (specialCooldownMultiplier * placement.Placement);

            if (placement.Placement == 1 && isUtility)
            {
                specialRecoveryTime = 0;
            }
        }

        //Set Recovery Times
        if (special == SpecialInput.forward)
        {
            player.forwardSpecialCooldownTimer = specialRecoveryTime;
        } else if (special == SpecialInput.side)
        {
            player.sideSpecialCooldownTimer = specialRecoveryTime;
        }
        else if (special == SpecialInput.back)
        {
            player.backSpecialCooldownTimer = specialRecoveryTime;
        }
        else if (special == SpecialInput.neutral)
        {
            player.neutralSpecialCooldownTimer = specialRecoveryTime;
        }
        specialRecoveryTime = oldCooldown;

        if (animator != null && !cutShort) animator.SetBool(animationTrigger, false);


        currentHitBox = 0;

        //Disable All Hitboxes VFX
        for (int i = 0; i < hitboxes.Length; i++)
        {
           if ( hitboxesInfo[i].vfx != null && hitboxesInfo[i].vfx.gameObject.activeInHierarchy)
            {
                hitboxesInfo[i].vfx.Stop();
            }
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

        #region Sound
        if (!dirtyAudio && playAudio)
        {
            switch(sfxType)
            {
                case AttackStatus.startup:
                    if(startup)
                    {
                        PlayAttackSound(sfxKey);
                        dirtyAudio = true;
                    }
                    break;
                case AttackStatus.active:
                    if(active)
                    {
                        PlayAttackSound(sfxKey);
                        dirtyAudio = true;
                    }
                    break;
                case AttackStatus.recovery:
                    if(recovery)
                    {
                        PlayAttackSound(sfxKey);
                        dirtyAudio = true;
                    }
                    break;
                case AttackStatus.none:
                    break;
                default:
                    break;
            }
        }
        #endregion

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

            //Enable Active VFX
            if (hitboxesInfo[currentHitBox].vfx != null && hitboxesInfo[currentHitBox].vfxState == HitBoxInfo.vfxPlayState.active)
            {
                hitboxesInfo[currentHitBox].vfx.Play();
            }
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

            //Recovery VFX
            if (hitboxesInfo[currentHitBox].vfx != null && hitboxesInfo[currentHitBox].vfxState == HitBoxInfo.vfxPlayState.recovery)
            {
                hitboxesInfo[currentHitBox].vfx.Play();
            }
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
            if (hitboxesInfo[currentHitBox].vfx != null)
            {
                hitboxesInfo[currentHitBox].vfx.Stop();
            }

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

                //Startup VFX
                if (hitboxesInfo[currentHitBox].vfx != null && hitboxesInfo[currentHitBox].vfxState == HitBoxInfo.vfxPlayState.startup)
                {
                    hitboxesInfo[currentHitBox].vfx.Play();
                }
            }

        }
        #endregion
    }

    private void FixedUpdate()
    {
        if (hitboxesInfo[currentHitBox] != null)
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

    private void PlayAttackSound(string key)
    {
        Debug.Log("playing fw special");
        soundPool.PlaySound(key, kart.transform.position);
    }
}

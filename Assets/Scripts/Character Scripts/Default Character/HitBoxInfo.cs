using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class HitBoxInfo : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject kart;
    [SerializeField] public GameObject ball;
    [SerializeField] public LightAttack attack;

    [Header("VFX")]
    [SerializeField] public VisualEffect vfx;
    public enum vfxPlayState
    {
        none,
        startup,
        active,
        recovery,
        onhit
    }
    [SerializeField] public vfxPlayState vfxState = vfxPlayState.none;
    [SerializeField] public bool disableVFXOnDisable = false;


    [Header("Info")]
    [SerializeField] public bool isSpecial = false;
    [SerializeField] public GameObject[] specials;
    [SerializeField] public Vector3 dir;
    Vector3 originalDir;
    [SerializeField] public float stun;
    [SerializeField] public float damage;
    [SerializeField] public bool attackLanded = false;

    [Header("Chargeable")]
    [SerializeField] public bool chargeAble = false;
    [SerializeField] public bool infiniteCharge = false;
    [SerializeField] public float extraChargeLength = 0;
    public float chargeTime = 0;
    //Not Working
    [SerializeField] public float forceMultiplier = 0;
    [SerializeField] public float damageMultiplier = 0;


    [Header("Force")]
    [SerializeField] public float fixedForce;
    [SerializeField] public float dynamicForce = 0;
    [SerializeField] public float dynamicForceMultiplier = 1;
    [SerializeField] public float pullForce = 1;
    [SerializeField] public float pullVelocity = 0;
    [SerializeField] public float defaultConstForce = 0;
    [SerializeField] public float constantFixedForce = 0;

    [Header("Active Input")]
    [SerializeField] public bool activeVerticalInput = false;
    [SerializeField] public bool activeHorizontalInput = false;
    [SerializeField] public Vector3 activeAddDir = Vector3.zero;
    [SerializeField] public bool horOnly = false;
    [SerializeField] public bool vertOnly = false;
    [SerializeField] public float activefixedForce = 0;
    [SerializeField] public float activedynamicForce = 0;

    [Header("Frame Data")]
    [SerializeField] public float startupTime = 0;
    [SerializeField] public float activeTime = 1;
    [SerializeField] public float recoveryTime = 0;
    [SerializeField] public bool endIfMiss = false;

    [Header("Animated Hitbox")]
    [SerializeField] public Vector3 hitMoveDir = Vector3.zero;
    [SerializeField] public float hitMoveSpeed = 0;
    Vector3 originalPosition = Vector3.zero;

    [Header("Player Movement")]
    [SerializeField] public bool lockPlayerMovement = false;
    [SerializeField] public float steerMultiplier = 1f;
    [SerializeField] public Vector3 moveDirection = Vector3.zero;
    [SerializeField] public float moveForce = 0;

    [Header("Opponent")]
    [SerializeField] public bool lockOpponentWhileActive = false;
    [SerializeField] public bool godProperty = false;
    [SerializeField] public bool pullToKart = false;
    [SerializeField] public Vector3 lockPosition = Vector3.zero;
    [SerializeField] public bool rotateToPlayer = false;

    [Header("Audio")]
    [SerializeField] private bool playAudio = true;
    [SerializeField] private string sfxKey;
    private SoundPool soundPool;
    
    [Space(10)]
    
    //add more options over time then reference in light attack
    public PlayerMain playerBody;

    private void Start()
    {
        //hitboxCollider = GetComponent<Collider>();

    }
    private void OnEnable()
    {
        chargeTime = 0;
        originalDir = dir;
        playerBody = player.GetComponent<PlayerMain>();
        soundPool = player.GetComponentInChildren<SoundPool>();
        originalPosition = transform.localPosition;
        attackLanded = false;

        //Charge Force Multiplier
        /*
        if (forceMultiplier != 0) {
            fixedForce *= (attack.chargePercent * forceMultiplier);
            dynamicForce *= (attack.chargePercent * forceMultiplier);
            defaultConstForce *= (attack.chargePercent * forceMultiplier);
            constantFixedForce *= (attack.chargePercent * forceMultiplier);
        }
        //Charge Damage Multiplier
        if (damageMultiplier != 0)
        {
            damage *= (attack.chargePercent * damageMultiplier);
        }*/


        if (isSpecial)
        {
            for (int i = 0; i < specials.Length; i++)
            {
                specials[i].SetActive(true);
            }
        }

        //active input force bug
        if (activeVerticalInput && (playerBody.ballDriving.up || playerBody.ballDriving.down))
        {
            dir.x += activeAddDir.x;
            dir.z += activeAddDir.z;
            if (playerBody.ballDriving.up)
            {
            }
            else if (playerBody.ballDriving.down)
            {
                dir.z *= -1;
            }
            if (vertOnly)
            {
                dir.x -= activeAddDir.x;
            }
        }
        if (activeHorizontalInput && (playerBody.ballDriving.left || playerBody.ballDriving.right))
        {
            dir.x += activeAddDir.x;
            dir.z += activeAddDir.z;
            if (playerBody.ballDriving.right)
            {
            }
            else if (playerBody.ballDriving.left)
            {
                dir.x *= -1;
            }
            if (horOnly)
            {
                dir.z -= activeAddDir.z;
            }
        }
    }

    private void Update()
    {
        if (playerBody.attackLanded)
        {
            attackLanded = true;
        }

        if (hitMoveSpeed != 0)
        {
            transform.transform.localPosition += hitMoveDir.normalized * hitMoveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        HitCollisionCheck(col);
    }
    private void OnDisable()
    {
        attackLanded = false;
        playerBody.attackLanded = false;

        if (isSpecial)
        {
            for (int i = 0; i < specials.Length; i++)
            {
                specials[i].SetActive(false);
            }
        }

        dir = originalDir;

        transform.localPosition = originalPosition;

        if (disableVFXOnDisable && vfx != null)
        {
            vfx.Stop();
        }

        /*
        //Set Original Forces
        //Charge Force Multiplier
        //Charge Force Multiplier
        if (forceMultiplier != 0)
        {
            fixedForce /= (attack.chargePercent * forceMultiplier);
            dynamicForce /= (attack.chargePercent * forceMultiplier);
            defaultConstForce /= (attack.chargePercent * forceMultiplier);
            constantFixedForce /= (attack.chargePercent * forceMultiplier);
        }
        //Charge Damage Multiplier
        if (damageMultiplier != 0)
        {
            damage /= (attack.chargePercent * damageMultiplier);
        }*/
    }

    public void HitCollisionCheck(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (col.gameObject != kart && col.gameObject != player && col.gameObject != ball)
            {
                playerBody.OnLanded(damage);
                if (vfxState == vfxPlayState.onhit && !vfx.gameObject.activeInHierarchy)
                {
                    vfx.Stop();
                }

                if(playAudio)
                    soundPool.PlaySound(sfxKey, this.transform.position);
            }
        }
    }
}

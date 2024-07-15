using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxInfo : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject kart;
    [SerializeField] public GameObject ball;
    [SerializeField] public LightAttack attack;

    [Header("Info")]
    [SerializeField] public bool isSpecial = false;
    [SerializeField] public GameObject[] specials;
    [SerializeField] public Vector3 dir;
    Vector3 originalDir;
    [SerializeField] public float stun;
    [SerializeField] public float damage;
    [SerializeField] public bool attackLanded = false;

    [Header("Force")]
    [SerializeField] public float fixedForce;
    [SerializeField] public float dynamicForce = 0;
    [SerializeField] public float dynamicForceMultiplier = 1;
    [SerializeField] public float pullForce = 1;
    [SerializeField] public bool pullForceToAttacker = false;
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

    [Header("Player Movement")]
    [SerializeField] public bool lockPlayerMovement = false;
    [SerializeField] public float steerMultiplier = 1f;
    [SerializeField] public Vector3 moveDirection = Vector3.zero;
    [SerializeField] public float moveForce = 0;

    [Header("Opponent")]
    [SerializeField] public bool lockOpponentWhileActive = false;
    [SerializeField] public bool godProperty = false;
    [SerializeField] public Vector3 lockPosition = Vector3.zero;

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
        originalDir = dir;
        playerBody = player.GetComponent<PlayerMain>();
        soundPool = player.GetComponentInChildren<SoundPool>();

        attackLanded = false;

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
    }

    public void HitCollisionCheck(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (col.gameObject != kart && col.gameObject != player && col.gameObject != ball)
            {
                playerBody.OnLanded(damage);
                if(playAudio)
                    soundPool.PlaySound(sfxKey, this.transform.position);
            }
        }
    }
}

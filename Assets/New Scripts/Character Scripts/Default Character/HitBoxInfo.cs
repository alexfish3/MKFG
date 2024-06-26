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
    [SerializeField] public Vector3 dir;
    [SerializeField] public float stun;
    [SerializeField] public float damage;
    [SerializeField] public bool attackLanded = false;

    [Header("Force")]
    [SerializeField] public float fixedForce;
    [SerializeField] public float pullForce = 1;

    [Header("Frame Data")]
    [SerializeField] public float startupTime = 0;
    [SerializeField] public float activeTime = 1;
    [SerializeField] public float recoveryTime = 0;

    [Header("Movement")]
    [SerializeField] public bool lockPlayerMovement = false;
    [SerializeField] public Vector3 lockPosition = Vector3.zero;
    [SerializeField] public bool godProperty = false;
    [SerializeField] public float steerMultiplier = 1f;
    [SerializeField] public bool lockOpponentWhileActive = false;
    [SerializeField] public Vector3 moveDirection = Vector3.zero;
    [SerializeField] public float moveForce = 0;

    //add more options over time then reference in light attack
    public PlayerMain playerBody;

    private void Start()
    {
        //hitboxCollider = GetComponent<Collider>();
        playerBody = player.GetComponent<PlayerMain>();
    }
    private void OnEnable()
    {
        attackLanded = false;
    }
    private void OnTriggerEnter(Collider col)
    {
        HitCollisionCheck(col);
    }
    private void OnDisable()
    {
        attackLanded = false;
        playerBody.attackLanded = false;
    }

    public void HitCollisionCheck(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (col.gameObject != kart && col.gameObject != player && col.gameObject != ball)
            {
                attackLanded = true;
                playerBody.attackLanded = true;
                playerBody.OnLanded(damage);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxInfo : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject kart;
    [SerializeField] public GameObject ball;

    [Header("Info")]
    [SerializeField] public Vector3 dir;
    [SerializeField] public float force;
    [SerializeField] public float stun;
    [SerializeField] public float damage;
    [Header("Frame Data")]
    [SerializeField] public float startupTime = 0;
    [SerializeField] public float activeTime = 1;
    [SerializeField] public float recoveryTime = 0;
    [Header("Movement")]
    [SerializeField] public bool lockPlayerMovement = false;
    [SerializeField] public float steerMultiplier = 1f;
    [SerializeField] public bool lockOpponentWhileStunned = false;
    //add more options over time then reference in light attack


    [SerializeField] public bool attackLanded = false;
    PlayerMain playerBody;

    private void Start()
    {
        //hitboxCollider = GetComponent<Collider>();
        playerBody = player.GetComponent<PlayerMain>();
    }
    private void OnEnable()
    {
        attackLanded = false;

        if (transform.parent.localScale.x < 0)
        {
            dir.x = -dir.x;
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

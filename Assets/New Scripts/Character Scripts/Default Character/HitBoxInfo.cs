using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxInfo : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject kart;
    [SerializeField] public GameObject ball;

    [SerializeField] public Vector3 dir;
    [SerializeField] public float force;
    [SerializeField] public float stun;
    [SerializeField] public float damage;
    [SerializeField] public float startupTime = 0;
    [SerializeField] public float activeTime = 1;
    [SerializeField] public float recoveryTime = 0;
    [SerializeField] public bool lockPlayerMovement = false;
    [SerializeField] public int driftPercentage = 0;
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
        if (col.gameObject.tag == "Player")
        {
            if (col.gameObject != kart && col.gameObject != player && col.gameObject != ball)
            {
                attackLanded = true;
                playerBody.attackLanded = true;
            }
        }
    }
    private void OnDisable()
    {
        attackLanded = false;
        playerBody.attackLanded = false;
    }
}

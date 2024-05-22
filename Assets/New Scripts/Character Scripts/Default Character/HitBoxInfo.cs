using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxInfo : MonoBehaviour
{
    [SerializeField] public Vector3 dir;
    [SerializeField] public float force;
    [SerializeField] public float stun;
    [SerializeField] public float damage;
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject kart;
    [SerializeField] public GameObject ball;
    [SerializeField] public bool attackLanded = false;

    private void Start()
    {
        //hitboxCollider = GetComponent<Collider>();
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
            }
        }
    }
    private void OnDisable()
    {
        attackLanded = false;
    }
}

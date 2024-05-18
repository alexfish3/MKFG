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
    [SerializeField] public Collider hitboxCollider;
    [SerializeField] public bool attackLanded;

    private void OnEnable()
    {
        Physics.IgnoreCollision(hitboxCollider, kart.GetComponent<Collider>());
        Physics.IgnoreCollision(hitboxCollider, ball.GetComponent<Collider>());

        attackLanded = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject != player.gameObject)
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

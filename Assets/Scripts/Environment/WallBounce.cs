using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBounce : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float stunTime = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            GetHit hurtbox = other.gameObject.GetComponent<GetHit>();

            Vector3 collisionPoint = other.ClosestPoint(transform.position);
            Vector3 collisionNormal = other.gameObject.transform.position - collisionPoint;
            if (hurtbox.player.isStunned)
            {
                hurtbox.player.ballDriving.rb.velocity = Vector3.Reflect(hurtbox.player.ballDriving.rb.velocity, collisionNormal);
                hurtbox.player.stunTime += stunTime;
            }
        }
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GetHit hurtbox = collision.gameObject.GetComponent<GetHit>();
            if (hurtbox.player.isStunned)
            {
                hurtbox.player.ballDriving.rb.velocity = Vector3.Reflect(hurtbox.player.ballDriving.rb.velocity, collision.contacts[0].normal);
                hurtbox.player.stunTime += stunTime;
            }
        }
    }*/
}

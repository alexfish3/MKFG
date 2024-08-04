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

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            GetHit hurtbox = other.gameObject.GetComponent<GetHit>();

            Vector3 collisionPoint = other.ClosestPoint(other.gameObject.transform.position);
            Vector3 collisionNormal = hurtbox.player.ballDriving.rb.velocity - collisionPoint.;
            //if (hurtbox.player.isStunned)
            //{
                hurtbox.player.ballDriving.rb.velocity = Vector3.Reflect(hurtbox.player.ballDriving.rb.velocity, collisionNormal);
                hurtbox.player.stunTime += stunTime;
            //}
        }
    }*/

    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && collision.gameObject.layer == 3)
        {
            PlacementHandler placement = collision.gameObject.GetComponent<PlacementHandler>();
            PlayerMain player = placement.PlayerMain;
            //if (player.isStunned)
            //{
                player.ballDriving.rb.velocity = Vector3.Reflect(player.ballDriving.rb.velocity, collision.contacts[0].normal);
                player.stunTime += stunTime;
            //}
        }
    }
}

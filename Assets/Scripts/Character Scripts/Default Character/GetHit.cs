using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetHit : MonoBehaviour
{
    //Change for each character
    [SerializeField] public PlayerMain player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hitbox") 
        {
            Debug.Log("Hit Player");
            
            //Get info from what is hitting the player.
            //Don't keep get component forever
            HitBoxInfo info = other.gameObject.GetComponent<HitBoxInfo>();

            //Check for sub hitboxes
            if (info == null)
            {
                info = other.transform.parent.gameObject.GetComponent<HitBoxInfo>();
            }
            //Ignore the players attacks hitting himself
            if (info != null)
            {
                if (info.player != player.gameObject && info.playerBody.GetBodyTeamID() != player.GetBodyTeamID())
                {
                    //Check if it's a clash and add clash mechanic
                    player.OnHit(info); //simplify and dynamic
                }
            }
        }

    }
}

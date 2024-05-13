using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetHitOrion : MonoBehaviour
{
    //Change for each character
    [SerializeField] PlayerMain player;

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
        if (other.tag == "Hitbox") 
        {
            HitBoxInfo info = other.gameObject.GetComponent<HitBoxInfo>();
            //Ignore the players attacks hitting himself
            if (info.player != player.gameObject)
            {
                player.OnHit(info.dir, info.force, info.stun, info.damage);
            }
        }

    }
}

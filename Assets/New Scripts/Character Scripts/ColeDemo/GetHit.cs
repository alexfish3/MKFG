using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetHit : MonoBehaviour
{
    [SerializeField] GameObject playerBody;
    PlayerOrion player;
    // Start is called before the first frame update
    void Start()
    {
        player = playerBody.GetComponent<PlayerOrion>();
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
            player.OnHit(info.dir, info.force, info.stun, info.damage);
        }

    }
}

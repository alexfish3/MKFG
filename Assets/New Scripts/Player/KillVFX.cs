using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillVFX : MonoBehaviour
{
    [SerializeField] private float lifetime;

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime;
        if(lifetime < 0 )
        {
            Destroy(this.gameObject);
        }    
    }
}

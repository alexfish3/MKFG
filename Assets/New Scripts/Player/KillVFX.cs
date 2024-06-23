using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private float lifetime;

    private void Start()
    {
        particles.Play();
    }
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

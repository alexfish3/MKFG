using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherHitbox : MonoBehaviour
{
    [SerializeField] HitBoxInfo parentHitbox;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider col)
    {
        parentHitbox.HitCollisionCheck(col);
    }
}

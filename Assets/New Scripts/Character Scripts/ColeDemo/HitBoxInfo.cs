using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxInfo : MonoBehaviour
{

    [SerializeField] public Vector3 dir;
    [SerializeField] public float force;
    [SerializeField] public float stun;
    [SerializeField] public float damage;
    // Start is called before the first frame update

    private void OnEnable()
    {
    }

}

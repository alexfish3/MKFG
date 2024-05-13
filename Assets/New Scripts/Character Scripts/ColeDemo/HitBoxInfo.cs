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
}

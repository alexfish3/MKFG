using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private PlayerMain player;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private float smoothrotation;
    private bool collisionDetected = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target == null)
            return;

        //transform.LookAt(player.kart.transform);
        transform.rotation = Quaternion.Slerp(transform.rotation, target.transform.rotation, smoothSpeed);
        transform.position = Vector3.Slerp(transform.position, target.transform.position, smoothSpeed);

    }
}

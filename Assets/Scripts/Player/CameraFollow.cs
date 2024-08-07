using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private PlayerMain player;
    private float smoothSpeed = 10;
    private float smoothrotation = 10;
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

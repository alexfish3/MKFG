using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private Camera cam;
    [SerializeField] private PlayerMain player;
    private float smoothSpeed = 50;
    private float smoothrotation = 50;
    private bool collisionDetected = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target == null)
            return;

        Vector3 oldPos = transform.position;
        Quaternion oldRot = transform.rotation;

        transform.rotation = Quaternion.Lerp(transform.rotation, target.transform.rotation, smoothSpeed * Time.fixedDeltaTime);
        transform.position = Vector3.Lerp(transform.position, target.transform.position, smoothSpeed * Time.fixedDeltaTime);


        //Collider[] hitcollider;
        //hitcollider = Physics.OverlapSphere(cam.transform.position, 0.25f);

        if (Physics.CheckSphere(cam.transform.position, 1f))
        {
            transform.position = oldPos;
            transform.rotation = oldRot;
            return;
        }

        //transform.LookAt(player.kart.transform);

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(cam.transform.position, 1);
    }
}

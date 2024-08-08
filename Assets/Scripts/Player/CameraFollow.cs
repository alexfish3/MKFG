using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private Camera cam;
    [SerializeField] private PlayerMain player;
    private float smoothSpeed = 30;
    private float zoomSpeed = 3f;
    private float smoothrotation = 30;
    private bool collisionDetected = false;
    [SerializeField] Vector3 oldCamPos;

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

            if (Physics.CheckSphere(transform.position, 1f)) 
            { 
                transform.position += -cam.transform.forward * zoomSpeed * Time.fixedDeltaTime;

                //transform.rotation = Quaternion.Lerp(transform.rotation, cam.transform.rotation, smoothSpeed * Time.fixedDeltaTime);
            } else
            {
                return;
            }
            //cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, Vector3.zero, zoomSpeed * Time.fixedDeltaTime);
            return;
        } 
        //cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, oldCamPos, zoomSpeed * Time.fixedDeltaTime);

        //transform.LookAt(player.kart.transform);

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(cam.transform.position, 1);
    }
}

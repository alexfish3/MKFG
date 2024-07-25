using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private Vector3 offset;

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPos = target.position + offset;

        Vector3 smoothPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);

        transform.position = smoothPos;

        transform.LookAt(target);
    }
}

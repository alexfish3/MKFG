using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollow : MonoBehaviour
{
    [Header("Tracking Information")]
    [SerializeField] public Transform lookAt;
    [SerializeField] public Vector3 offset;

    [Header("Logic")]
    [SerializeField] Camera cam;

    private void Update()
    {
        Vector3 pos = cam.WorldToScreenPoint(lookAt.position + offset);

        if(transform.position != pos)
        {
            transform.position = pos;
        }
    }
}

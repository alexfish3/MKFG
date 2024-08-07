using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipObject : MonoBehaviour
{
    [SerializeField] PlayerMain player;
    float originalX;
    // Start is called before the first frame update
    void OnEnable()
    {
        originalX = transform.localPosition.x;
    }

    // Update is called once per frame
    void Update()
    {
        originalX = Mathf.Abs(transform.localPosition.x);
        if (player.facingRight)
        {
            transform.localPosition = new Vector3(Mathf.Abs(originalX), transform.localPosition.y, transform.localPosition.z);
        } else
        {
            transform.localPosition = new Vector3(-originalX, transform.localPosition.y, transform.localPosition.z);
        }
    }
}

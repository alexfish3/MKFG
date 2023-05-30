using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject player;
    public int playerSpeed = 1;
    public bool down = false;

    private Rigidbody r;
    private Vector3 speed;
    // Start is called before the first frame update
    void Start()
    {
        r.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            down = true;
            r.velocity = new Vector3(r.velocity.x + playerSpeed, r.velocity.y, r.velocity.z);
        } else
        {
            down = false;
        }
    }
}

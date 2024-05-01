using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDrivingVersion1 : MonoBehaviour
{
    [SerializeField] GameObject kart;
    [Header("Speed")]
    [SerializeField] float forwardSpeed;
    [SerializeField] float backwardsSpeed;
    [SerializeField] float smoothstepFriction;
    [SerializeField] float defaultSpeed;


    float speed = 0;
    public float currentSpeed;

    [Header("Steering")]
    public float rotate;
    public float currentRotate;
    [SerializeField] float steering = 1;
    [SerializeField] float steeringPower;
    [SerializeField] float steeringFriction = 1;


    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = kart.GetComponent<Rigidbody>();
    }

    // Update is called once per frame

    private void Update()
    {
        //Forward
        if (Input.GetKey(KeyCode.W))
        {
            speed = forwardSpeed;
        }
        //Back
        else if (Input.GetKey(KeyCode.S))
        {
            speed = backwardsSpeed;
        }

        //Left
        if (Input.GetKey(KeyCode.A))
        {
            Steer(-1, steeringPower);
        }
        //Right
        else if (Input.GetKey(KeyCode.D))
        {
            Steer(1, steeringPower);
        }

        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * smoothstepFriction);
        speed = defaultSpeed;
        // Debug.Log("Rotate:" + rotate);
        // currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * steeringFriction - 1);
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * (steeringFriction - 1));
        Debug.Log("Current Rotate:" + currentRotate);
        rotate = 0;
    }
    void FixedUpdate()
    {
        rb.AddForce(kart.transform.forward * currentSpeed, ForceMode.Acceleration);

        kart.transform.eulerAngles = Vector3.Lerp(kart.transform.eulerAngles, new Vector3(0, kart.transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * steeringFriction);
    }

    public void Steer(int direction, float amount)
    {
        rotate = (steering * direction) * amount;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDrivingVersion1 : MonoBehaviour
{
    [SerializeField] GameObject kart;
    [SerializeField] GameObject kartParent;
    [SerializeField] GameObject ball;

    [Header("Speed")]
    [SerializeField] float forwardSpeed;
    [SerializeField] float backwardsSpeed;
    [SerializeField] float smoothstepFriction;
    [SerializeField] float defaultSpeed;
    [SerializeField] float drag = 1;
    [SerializeField] float gravity = 10;
    [SerializeField] float gravityChangeFriction = 8;
    float speed = 0;
    float currentSpeed;

    [Header("Steering")]
    [SerializeField] float steeringPower;
    [SerializeField] float steeringFriction = 1;
    float rotate;
    float currentRotate;

    [Header("Other Stats")]
    [SerializeField] float groundNearRayDistance = 2;
    [SerializeField] float groundCheckDistance = 1.1f;
    [SerializeField] bool grounded;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = ball.GetComponent<Rigidbody>();
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

        //Turning Only If Moving
        if (Input.GetKey(KeyCode.A) && speed != defaultSpeed)
        {
            Steer(-1, steeringPower);
        }
        else if (Input.GetKey(KeyCode.D) && speed != defaultSpeed)
        {
            Steer(1, steeringPower);
        } else
        {
            rb.drag = drag;
        }

        //Set Speed & Rotate Values
        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * smoothstepFriction);
        speed = defaultSpeed;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * (steeringFriction - 1));
        rotate = 0;

    }
    void FixedUpdate()
    {
        if (grounded)
        {
            //Forward Acceleration
            rb.AddForce(kart.transform.forward * currentSpeed, ForceMode.Acceleration);

            //Steering
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * steeringFriction);
        }

        //Gravity
        //Adding down relative to the kart so the kart can drive on anything
        rb.AddForce(-kart.transform.up * gravity, ForceMode.Acceleration);

        //Rotate Body
        RaycastHit hitNear;
        RaycastHit hitGround;
        //Grounded Check
        if (Physics.Raycast(kart.transform.position, Vector3.down, out hitGround, groundCheckDistance))
        {
            grounded = true;
        } else
        {
            grounded = false;
        }
        //Rotate Kart To Match Ground
        Physics.Raycast(kart.transform.position, Vector3.down, out hitNear, groundNearRayDistance);
        kartParent.transform.up = Vector3.Lerp(kartParent.transform.up, hitNear.normal, Time.deltaTime * gravityChangeFriction);
        kartParent.transform.Rotate(0, transform.eulerAngles.y, 0);

        //Controller Follows Ball
        transform.position = ball.transform.position;

    }

    public void Steer(int direction, float amount)
    {
        rotate = direction * amount;
    }
}

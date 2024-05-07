using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class CylinderDriving : MonoBehaviour
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
    int dir;

    [Header("Drifting")]
    [SerializeField] float spotDodgeForce = 10f;
    [SerializeField] float defaultDriftForce = 50f;
    [SerializeField] float innerDriftForce = 100f;
    [SerializeField] float outerDriftForce = 10f;
    private float currDriftForce;
    private bool drifting = false;
    private bool hopping = false;
    private int driftDir;
    private float driftTimer = 0;
    private float driftMultiplier;
    [SerializeField] private float totalDriftTime = 5f;
    [SerializeField] private float driftBoostForce = 200f;
    [SerializeField] private ParticleSystem[] driftSparks;

    [Header("Other Stats")]
    [SerializeField] float groundNearRayDistance = 2;
    [SerializeField] float groundCheckDistance = 1.1f;
    [SerializeField] bool grounded;

    [Header("Cooldowns")]
    [SerializeField] private float driftCooldownTime = 3;
    private float driftCooldown = 0;
    [SerializeField] private float spotDodgeCooldownTime = 1f;
    private float spotDodgeCooldown = 0;

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
            dir = -1;
            Steer(-1, steeringPower);
        }
        else if (Input.GetKey(KeyCode.D) && speed != defaultSpeed)
        {
            dir = 1;
            Steer(1, steeringPower);
        }
        else
        {
            dir = 0;
            rb.drag = drag;
        }

        //Set Speed & Rotate Values
        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * smoothstepFriction);
        speed = defaultSpeed;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * (steeringFriction - 1));
        rotate = 0;

        // timers
        driftCooldown -= driftCooldown > 0 ? Time.deltaTime : 0;
        spotDodgeCooldown -= spotDodgeCooldown > 0 ? Time.deltaTime : 0;

        // various checks
        Drift();

    }
    void FixedUpdate()
    {
        if (grounded || drifting)
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
        }
        else
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
        if (!drifting)
        {
            rotate = direction * amount;
        }
    }

    public void Drift()
    {
        if (Input.GetKey(KeyCode.L))
        {
            if (driftCooldown > 0)
                return;

            if (spotDodgeCooldown <= 0 && !hopping)
            {
                spotDodgeCooldown = spotDodgeCooldownTime;
                hopping = true;
                rb.AddForce(kart.transform.up * spotDodgeForce, ForceMode.Acceleration); // make kart "jump" for spotdodge
            }
            
            if (grounded && spotDodgeCooldown <= 0 && !drifting)
            {
                if (dir == 0)
                {
                    StopDrift();
                    return;
                }
                drifting = true;
                driftDir = dir;
            }

            if (drifting)
            {
                if (driftDir == dir)
                {
                    currDriftForce = innerDriftForce;
                    driftMultiplier = 1.2f;
                }
                else if (driftDir == 0)
                {
                    currDriftForce = defaultDriftForce;
                    driftMultiplier = 1f;
                }
                else
                {
                    currDriftForce = outerDriftForce;
                    driftMultiplier = 0.8f;
                }

                rotate += currDriftForce * driftDir;
                driftTimer += Time.deltaTime * driftMultiplier;
                
                foreach (ParticleSystem ps in driftSparks)
                {
                    var main = ps.main;
                    ps.gameObject.SetActive(true);
                    main.startColor = Color.yellow;
                    ps.transform.localScale = Vector3.one;
                }
                
                if (driftTimer > totalDriftTime)
                {
                    foreach(ParticleSystem ps in driftSparks)
                    {
                        var main = ps.main;
                        main.startColor = Color.blue;
                        ps.transform.localScale = Vector3.one * 2;
                    }
                }
            }
        }
        else
        {
            StopDrift();
        }
    }

    private void StopDrift()
    {
        if (drifting)
        {
            drifting = false;
            hopping = false;

            if(driftTimer >= totalDriftTime)
            {
                BoostPlayer(driftBoostForce);
            }

            foreach (ParticleSystem ps in driftSparks)
            {
                ps.gameObject.SetActive(false);
            }

            driftTimer = 0;
        }
    }

    private void BoostPlayer(float amount)
    {
        rb.AddForce(transform.forward * amount, ForceMode.VelocityChange);
    }
}

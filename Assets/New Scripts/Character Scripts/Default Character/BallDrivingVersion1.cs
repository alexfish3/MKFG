using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDrivingVersion1 : MonoBehaviour
{
    [Header("Player Info")]
    [SerializeField] PlayerMain playerMain;

    [Header("GameObjects")]
    [SerializeField] GameObject kart;
    [SerializeField] GameObject kartParent;
    [SerializeField] GameObject ball;

    [Header("TESTING MATERIALS")]
    [SerializeField] Material defaultColour;
    [SerializeField] Material dodgeColour;
    [SerializeField] Material driftColour;
    MeshRenderer kartMaterial;

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

    [Header("Ground Checks")]
    [SerializeField] float groundNearRayDistance = 2;
    [SerializeField] float groundCheckDistance = 1.1f;
    [SerializeField] bool grounded;
    public bool Grounded { get { return grounded; } }

    [Header("Dash")]
    [SerializeField] float dashPower;
    [SerializeField] bool isDashing;
    [SerializeField] float dashCooldownTime = 0.5f;
    float dash;
    float currentDash;
    float dashTimer = 0;

    [Header("Dodge")]
    [SerializeField] bool isDodging;
    [SerializeField] float dodgeLength = 0.5f;
    [SerializeField] float dodgeCooldownLength = 3f;
    float dodgeTimer = 0;
    float dodgeCooldownTimer = 0;

    [Header("Drift")]
    [SerializeField] bool isDrifting;
    [SerializeField] float driftSteerPower = 1.2f;
    [SerializeField] float driftOppositeSteerPower = 1.2f;
    [SerializeField] float driftLengthToBoost = 2f;
    [SerializeField] float driftBoostPower = 50;
    float driftFloat;
    float driftTimer = 0;
    float driftDirection;

    public Rigidbody rb;

    public bool up = false;
    public bool down = false;
    public bool left = false;
    public bool right = false;
    public bool drift = false;
    public bool driftTap = false;
    public bool lastdriftInput = false;
    public bool drive = false;
    public bool reverse = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = ball.GetComponent<Rigidbody>();
        rb.drag = drag;
        kartMaterial = kart.GetComponent<MeshRenderer>();

        //ignore physics between ball and kart
        Physics.IgnoreCollision(ball.GetComponent<Collider>(), kart.GetComponent<Collider>());
    }

    // Update is called once per frame
    private void Update()
    {

        //Forward
        if (drive)
        {
            speed += forwardSpeed;
        }
        //Back
        if (reverse)
        {
            speed += backwardsSpeed;
        }
        //Stop Drifting if speed is zero or below
        if (speed <= 0 && isDrifting)
        {
            isDrifting = false;
        }

        //Turning Only If Moving
        if (left && speed != defaultSpeed)
        {
            Steer(-1, steeringPower);
        }
        else if (right && speed != defaultSpeed)
        {
            Steer(1, steeringPower);
        } //Drift & Dodge
        else if (driftTap && dodgeCooldownTimer >= dodgeCooldownLength)
        {
            isDodging = true;
            //isDrifting = false;
        }

        //End Drift
        
        if (isDrifting && !drift)
        {
            isDrifting = false;
        }

        //Default Drift Amount
        if (isDrifting && rotate == 0)
        {
            rotate = driftDirection * steeringPower;
        }

        //Dash & Reset Cooldown
        if (dash != 0)
        {
            isDodging = false;
            dashTimer = 0;
        }
        if (dashCooldownTime > dashTimer)
        {
            isDashing = true;
            dashTimer += Time.deltaTime;
        }
        else
        {
            isDashing = false;
        }

        //Dodge
        if (isDodging)
        {
            dodgeTimer += Time.deltaTime;
        }
        else
        {
            dodgeTimer = 0;
            if (dodgeCooldownTimer < dodgeCooldownLength)
            {
                dodgeCooldownTimer += Time.deltaTime;
            }
        }
        //End dodge & Start Drift
        if ((dodgeTimer >= dodgeLength || rotate != 0) && isDodging)
        {
            isDodging = false;
            dodgeCooldownTimer = 0;

            //check for drift and direction
            if (rotate > 0)
            {
                driftDirection = 1;
                isDrifting = true;
            }
            else if (rotate < 0)
            {
                driftDirection = -1;
                isDrifting = true;
            }
        }

        //Materials
        if (isDodging)
        {
            kartMaterial.material = dodgeColour;
        }
        else if (isDrifting)
        {
            kartMaterial.material = driftColour;
        }
        else
        {
            kartMaterial.material = defaultColour;
        }

        //Set Values If Not In Stun
        if (!playerMain.isStunned && !playerMain.isPlayerAttacking())
        {
            //sets player speed if not stunned
            speed *= playerMain.GetHealthMultiplier();
            currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * smoothstepFriction);
            speed = 0;
            currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * (steeringFriction - 1));
            rotate = 0;
            if (currentDash == 0)
            {
                currentDash = dash;
            }
            dash = 0;
        } //Set Values If In Stun
        else
        {
            isDashing = false;
            isDodging = false;
            isDrifting = false;
            speed = 0;
            currentSpeed = Mathf.SmoothStep(currentSpeed, currentSpeed, Time.deltaTime * smoothstepFriction);
            currentRotate = 0;
            rotate = 0;
            dash = 0;
        }

        //Turn off drift button
        //drift = false;
    }
    void FixedUpdate()
    {
        //Check is drift was pressed by saving the last input
        if (!lastdriftInput && drift && !driftTap)
        {
            driftTap = true;
        } else if (driftTap)
        {
            driftTap = false;
        }
        lastdriftInput = drift;

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

        //Dash Force
        rb.AddForce(kart.transform.right * currentDash, ForceMode.Impulse);
        currentDash = 0;

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
        rotate = direction * amount;

        //Set Dash Values
        if (driftTap && dashTimer >= dashCooldownTime && !isDodging && !isDrifting)
        {
            dash = dashPower * direction;
        }

        //If opposite direction end drift
        if (direction != driftDirection && isDrifting)
        {
            rotate *= -driftOppositeSteerPower;
        }
        //Drift
        else if (isDrifting)
        {
            rotate *= driftSteerPower;
        }
    }

    /// <summary>
    /// Set rotation of kart, only in Y axis since that's most applicable.
    /// </summary>
    /// <param name="newRot">New rotation for kart</param>
    public void SetKartRotation(Vector3 newRot)
    {
        transform.rotation = Quaternion.Euler(new Vector3(0,newRot.y,0));
    }

    /// <summary>
    /// Get rotation of the kart.
    /// </summary>
    public Vector3 GetKartRotation()
    {
        return transform.rotation.eulerAngles;
    }

}

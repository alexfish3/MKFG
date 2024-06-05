using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

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
    [SerializeField] Material stunColour;
    MeshRenderer kartMaterial;

    [Header("Speed")]
    [SerializeField] float forwardSpeed;
    [SerializeField] float backwardsSpeed;
    [SerializeField] float smoothstepFriction;
    [SerializeField] float defaultSpeed;
    [SerializeField] float drag = 1;
    [SerializeField] float defaultGravity = 10;
    [SerializeField] float gravityChangeFriction = 8;
    float gravity;
    float speed = 0;
    float currentSpeed;

    [Header("Steering")]
    [SerializeField] float steeringPower;
    [SerializeField] float steeringFriction = 1;
    float rotate;
    float currentRotate;
    float steerTimer = 0;
    [SerializeField] float steerTime = 0.2f;

    [Header("Ground Checks")]
    [SerializeField] float groundNearRayDistance = 2;
    [SerializeField] float groundCheckDistance = 1.1f;
    [SerializeField] bool grounded;
    public bool Grounded { get { return grounded; } }

    [Header("Dash")]
    [SerializeField] float dashPower;
    [SerializeField] bool isDashing;
    [SerializeField] float dashTime = 0.5f;
    float dash;
    float currentDash;
    float dashTimer = 0;
    int dashDirection;
    [SerializeField] float dashCooldown = 0.5f;
    [SerializeField] float chaseDashPower = 0.5f;
    bool isChaseDashing = false;
    Vector2 chaseDashDirection = Vector2.zero;
    int chaseDashVerticalDirection;

    [Header("Dodge")]
    [SerializeField] public bool isDodging;
    [SerializeField] float dodgeLength = 0.5f;
    [SerializeField] float dodgeCooldownLength = 3f;
    float dodgeTimer = 0;
    float dodgeCooldownTimer = 0;

    [Header("Drift")]
    [SerializeField] bool isDrifting;
    [SerializeField] float driftSteerPower = 1.2f;
    [SerializeField] float driftOppositeSteerPower = 1.2f;
    [SerializeField] float[] driftTimeInterval;
    [SerializeField] float[] driftBoostPower;
    [SerializeField] PlayerSparkHandler driftSparkHandler;
    float driftFloat;
    [SerializeField] float driftTapTime = 0.1f;
    float driftTimer = 0;
    float driftDirection;
    int nextDriftIndex;
    int driftType = -1;
    float driftTapTimer = 0;

    [Header("Taunt")]
    [SerializeField] float rampBoost = 25f;
    [SerializeField] float groundBoost = 100f;
    [SerializeField] float tauntTime = 1f;
    private TauntHandler tauntHandler;

    public Rigidbody rb;

    public Vector2 leftStick;
    public Vector2 rightStick;

    public bool up = false;
    public bool down = false;
    public bool left = false;
    public bool right = false;
    public bool steerTap = false;
    public bool drift = false;
    public bool driftTap = false;
    public bool lastdriftInput = false;
    public bool drive = false;
    public bool reverse = false;

    public float CurrentSpeed { get { return currentSpeed; } }

    // Start is called before the first frame update
    void Start()
    {
        rb = ball.GetComponent<Rigidbody>();
        rb.drag = drag;
        gravity = defaultGravity;
        kartMaterial = kart.GetComponent<MeshRenderer>();
        tauntHandler = GetComponent<TauntHandler>();

        //ignore physics between ball and kart
        Physics.IgnoreCollision(ball.GetComponent<Collider>(), kart.GetComponentInChildren<Collider>());
    }

    // Update is called once per frame
    private void Update()
    {
        //Check Drift If It Was Tapped
        #region DriftTap
        if (!lastdriftInput && drift && !driftTap)
        {
            driftTap = true;
            driftTapTimer = 0;
        }
        if (driftTap && driftTapTime > driftTapTimer)
        {
            driftTapTimer += Time.deltaTime;
        } else
        {
            driftTap = false;
        }
        lastdriftInput = drift;
        #endregion

        //Check If Steer Was Tapped
        #region SteerTap
        if ((left || right))
        {
            steerTimer += Time.deltaTime;
        } else
        {
            steerTimer = 0;
            steerTap = false;
        }
        if (steerTimer > steerTime)
        {
            steerTap = false;
        } else
        {
            steerTap = true;
        }
        #endregion

        //Forward
        #region Forward&Reverse
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
        #endregion

        //Turning Only If Moving
        if (left && speed != defaultSpeed)
        {
            Steer(-1, steeringPower);
        }
        else if (right && speed != defaultSpeed)
        {
            Steer(1, steeringPower);
        } //Set Dodge
        else if (driftTap && dodgeCooldownTimer >= dodgeCooldownLength && !isDrifting)
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

        //If not pressing dash button, stop dashing
        if (isDashing && !drift)
        {
            isDashing = false; 
        }

        //Dash & Reset Cooldown
        if (dashTime > dashTimer && isDashing)
        {
            isDashing = true;
            dashTimer += Time.deltaTime;
        }
        else
        {
            isDashing = false;
            if (dash != 0)
            {
                dash = 0;
                //Apply cooldown stun when dash is finished
                playerMain.stunTime = dashCooldown;
            }
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
        if ((dodgeTimer >= dodgeLength) && isDodging)
        {
            isDodging = false;
            dodgeCooldownTimer = 0;
        }
        //Start Drift After Dodge
        if (driftTap && rotate != 0 && isDodging && !tauntHandler.CanTaunt)
        {
            isDodging = false;
            isDrifting = true;
            if (rotate > 0)
            {

                driftDirection = 1;
            }
            else
            {
                driftDirection = -1;
            }
        }

        if(driftTap && tauntHandler.CanTaunt)
        {
            playerMain.stunTime = tauntHandler.TauntTime;
            tauntHandler.Taunt();
            
        }

        //Chase Dash
        //Chase Dash (if attack lands)
        /*
        if (playerMain.isPlayerAttacking() && drift)
        {
            isDashing = true;
            isDrifting = false;

            //Horizontal Direction
            int direction = 0;
            direction += left ? -1 : 0;
            direction += right ? 1 : 0;
            dash = dashPower * direction;
            dashTimer = 0;
            dashDirection = direction;

            //Exit attack early to chase (for now)
            playerMain.disablePlayerAttacking();
        }*/


        //Material Changes
        if (playerMain.isStunned)
        {
            kartMaterial.material = stunColour;
        }
        else if (isDodging || isChaseDashing)
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
        if ((!playerMain.isStunned && !playerMain.isPlayerAttacking()))
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

        // drift spark handling
        driftSparkHandler.ToggleDriftSparks(isDrifting, driftType+1);

        if(isDrifting)
        {
            driftTimer += Time.deltaTime;
            for(int i=driftTimeInterval.Length-1;i>=0;i--)
            {
                if(driftTimer > driftTimeInterval[i])
                {
                    driftType = i;
                    break;
                }
            }
        }
        else
        {
            BoostPlayer();
        }
        //Turn off drift button
        //drift = false;
    }
    void FixedUpdate()
    {

        if (grounded)
        {
            //Forward Acceleration
            rb.AddForce(kart.transform.forward * currentSpeed, ForceMode.Acceleration);

            //Steering
            if (!isDashing)
                transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * steeringFriction);
        }

        //Gravity
        //Adding down relative to the kart so the kart can drive on anything
        rb.AddForce(-kart.transform.up * gravity, ForceMode.Acceleration);

        //Dash Force
        rb.AddForce(kart.transform.right * currentDash, ForceMode.Impulse);
        currentDash = 0;

        //ADD Chase Dash Force


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
        if (drift && !driftTap && !steerTap && !isDashing && !isDodging && !isDrifting)
        {
            isDashing = true;
            dash = dashPower * direction;
            dashTimer = 0;
            dashDirection = direction;
        }
        //Stop Dashing If Opposite Direction Is Pressed
        if (isDashing && direction != dashDirection)
        {
            isDashing = false;
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

        //If turning and drift is pressed, start drifting
       /* if (!steerTap && driftTap && !isDrifting && !isDashing && !isDodging && !tauntHandler.CanTaunt)
        {
            //Make function to set isdrifting
            isDrifting = true;
            driftDirection = direction;
        } */
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

    /// <summary>
    /// Gives player a speed boost.
    /// </summary>
    private void BoostPlayer(bool forDrift = true, float overrideAmount = 0f)
    {
        float boostPower;

        if (driftType < 0 && forDrift)
            return;
        else if(forDrift)
        {
            boostPower = driftBoostPower[driftType];
        }
        else
        {
            boostPower = overrideAmount;
        }

        rb.AddForce(kartParent.transform.forward * boostPower, ForceMode.VelocityChange);
        driftSparkHandler.ToggleBoostSparks(driftType+1);

        if (forDrift)
        {
            driftTimer = 0;
            driftType = -1;
        }
    }

    /// <summary>
    /// Adjust gravity on player.
    /// </summary>
    /// <param name="setDefault">False will set gravity to default value, true will set to inGravity value.</param>
    /// <param name="inGravity">New value of gravity with setDefault to false.</param>
    public void ToggleGravity(bool setDefault=true, float inGravity=1)
    {
        if (setDefault)
        {
            gravity = defaultGravity;
            return;
        }
        gravity = inGravity;
    }

    /// <summary>
    /// Starts the wait for boost coroutine. Both of these are super simple and are just for testing the taunts.
    /// </summary>
    public void StartWaitForBoost()
    {
        BoostPlayer(false, rampBoost);
        StartCoroutine(WaitForBoost());
    }

    public IEnumerator WaitForBoost()
    {
        yield return new WaitForSeconds(tauntTime);
        while(!grounded)
        {
            yield return null;
        }
        BoostPlayer(false, groundBoost);
    }
}

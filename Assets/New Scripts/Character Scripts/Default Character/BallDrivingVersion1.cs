using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.VFX;

public class BallDrivingVersion1 : MonoBehaviour
{
    [Header("Player Info")]
    [SerializeField] PlayerMain playerMain;
    private Respawn respawn;
    [Header("GameObjects")]
    [SerializeField] GameObject kart;
    [SerializeField] GameObject kartParent;
    [SerializeField] GameObject ball;
    [SerializeField] GameObject dodgeBubbleVFX;
    [SerializeField] GameObject walkingRing;
    [SerializeField] GameObject walkingRingVFX;

    [Header("TESTING MATERIALS")]
    [SerializeField] Material defaultColour;
    [SerializeField] Material dodgeColour;
    [SerializeField] Material driftColour;
    [SerializeField] Material stunColour;
    [SerializeField] Material dashColour;
    [SerializeField] Material chaseDashColour;
    [SerializeField] Material tauntColour;
    [Space(10)]
    [SerializeField] MeshRenderer kartMaterial;

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
    float tauntSpeed;
    bool isBoosting;
    float boostTimer = 0f;

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
    [SerializeField] float dashSteerMultiplier = 1f;
    [SerializeField] public bool isDashing;
    [SerializeField] float dashTime = 0.5f;
    float dash;
    float currentDash;
    float dashTimer = 0;
    int dashDirection;
    [SerializeField] float dashCooldown = 0.5f;

    [Header("Chase Dash")]
    [SerializeField] float chaseDashPower = 0.5f;
    public bool isChaseDashing = false;
    Vector2 chaseDashDirection = Vector2.zero;
    [SerializeField] float chaseDashTime = 1f;
    [SerializeField] float chaseInputDashTime = 0.5f;
    float chaseDashTimer = 0;
    public float chaseDashInputTimer = 100;
    [SerializeField] float chaseDashSteerMultiplier = 1f;

    [Header("Dodge")]
    [SerializeField] public bool isDodging;
    [SerializeField] float dodgeLength = 0.5f;
    [SerializeField] float dodgeCooldownLength = 3f;
    float dodgeTimer = 0;
    float dodgeCooldownTimer = 0;

    [Header("Drift")]
    [SerializeField] public bool isDrifting;
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
    [SerializeField] float rampBoostTime = 0.5f;
    [SerializeField] float groundBoostTime = 1f;
    [SerializeField] float tauntSpeedMultiplier = 1f;
    [SerializeField] float liftGravity = 50f;
    [SerializeField] float fallGravity = 150f;
    [SerializeField] float liftTime = 0.3f;
    [SerializeField] float suspendedTime = 0.4f;
    private TauntHandler tauntHandler;

    [Space(10)]

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
        tauntHandler = GetComponent<TauntHandler>();
        respawn = ball.GetComponent<Respawn>();

        //Disable dodgeBubbleVFX
        dodgeBubbleVFX.SetActive(false);

        //Match bubble lifetime with dodge lifetime
        VisualEffect bubble = dodgeBubbleVFX.GetComponent<VisualEffect>();
        bubble.SetFloat("BubbleLifetime", dodgeLength);

        //Disable VFX stuff
        walkingRing.SetActive(false);
        walkingRingVFX.SetActive(false);

        //ignore physics between ball and kart
        Physics.IgnoreCollision(ball.GetComponent<Collider>(), kart.GetComponentInChildren<Collider>());
    }

    // Update is called once per frame
    private void Update()
    {
        // super fast debug
        Debug.DrawLine(kart.transform.position, kart.transform.position + (Vector3.down * groundCheckDistance), Color.cyan);
        Debug.DrawLine(kart.transform.position, kart.transform.position + (kart.transform.forward * 5f), Color.magenta);
        //Debug.Log($"Taunt {grounded}");

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
            steerTap = true;
        }
        if (steerTimer > steerTime)
        {
            steerTap = false;
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
        } // check dodge
        else if (driftTap && dodgeCooldownTimer >= dodgeCooldownLength && !isDrifting && !isDashing && !isChaseDashing && !playerMain.isPlayerAttacking() && !tauntHandler.CanTaunt && !tauntHandler.IsTaunting)
        {
            isDodging = true;
            //isDrifting = false;
        } // check taunt
        else if (driftTap && tauntHandler.CanTaunt && !isDodging && !isDrifting && !isChaseDashing && !playerMain.isPlayerAttacking())
        {
            tauntHandler.Taunt();

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

        //Stay In Dash & Reset Cooldown
        if (dashTime > dashTimer && isDashing)
        {
            isDashing = true;
            dashTimer += Time.deltaTime;
        }
        else //If not then end dash
        {
            if (dash != 0)
            {
                dash = 0;
                //rotate = 0;
                //Apply cooldown stun when dash is finished
                playerMain.stunTime = dashCooldown;
            }
        }

        //Dodge
        if (isDodging)
        {
            dodgeCooldownTimer = 0;
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
        }
        //Start Drift After Dodge
        if (drift && !driftTap && rotate != 0 && isDodging && !tauntHandler.CanTaunt)
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

        //Chase Dash
        //Chase Dash (if attack lands)
        if (chaseDashInputTimer < chaseInputDashTime && drift && !isChaseDashing)
        {
            //Initialize Values
            chaseDashDirection = new Vector2();
            chaseDashDirection.x += left ? -1 : 0;
            chaseDashDirection.x += right ? 1 : 0;
            chaseDashDirection.y += up ? 1 : 0;
            chaseDashDirection.y += down ? -1 : 0;

            //If direction is pressed then chase dash
            if (chaseDashDirection.magnitude != 0)
            {
                chaseDashTimer = 0;

                isChaseDashing = true;
                isDashing = false;
                isDodging = false;
                isDrifting = false;

                //Exit attack early to chase (for now)
                playerMain.disablePlayerAttacking();
                playerMain.stunTime = 0;
            }
        }
        //Add To Timer
        if (isChaseDashing)
        {
            chaseDashTimer += Time.deltaTime;

            chaseDashDirection = new Vector2();
            chaseDashDirection.x += left ? -1 : 0;
            chaseDashDirection.x += right ? 1 : 0;
            chaseDashDirection.y += up ? 1 : 0;
            chaseDashDirection.y += down ? -1 : 0;
        }
        //Stop Chase Dashing
        if ((isChaseDashing && !drift) || chaseDashTimer >= chaseDashTime)
        {
            isChaseDashing = false;
            chaseDashTimer = 0;
        }
        //Chase Dash Input Timer
        if (chaseDashInputTimer < chaseInputDashTime)
        {
            chaseDashInputTimer += Time.deltaTime;
        }

        isBoosting = boostTimer > 0;

        // boost
        if(isBoosting)
        {
            boostTimer -= Time.deltaTime;
            speed *= playerMain.BoostMultiplier;
        }

        //Material Changes
        if (playerMain.isStunned)
        {
            kartMaterial.material = stunColour;

            DisableDodgeVFX();
            DisableDashVFX();
        }
        else if (isDodging)
        {
            kartMaterial.material = dodgeColour;
            dodgeBubbleVFX.SetActive(true);
            DisableDashVFX();
        }
        else if (isDashing)
        {
            kartMaterial.material = dashColour;

            DisableDodgeVFX();

            walkingRing.SetActive(true);
            walkingRingVFX.SetActive(true);
        }
        else if (isDrifting)
        {
            kartMaterial.material = driftColour;

            DisableDodgeVFX();
            DisableDashVFX();
        }
        else if (isChaseDashing)
        {
            kartMaterial.material = chaseDashColour;

            DisableDodgeVFX();
            DisableDashVFX();
        }
        else if(tauntHandler.IsTaunting)
        {
            kartMaterial.material = tauntColour;
        }
        else
        {
            kartMaterial.material = defaultColour;

            DisableDodgeVFX();
            DisableDashVFX();
        }

        //Set Values If Not In Stun
        if (!playerMain.isStunned)
        {
            //sets player speed if not stunned
            speed *= playerMain.GetHealthMultiplier();
            currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * smoothstepFriction);
            speed = 0;

            //if hitbox sets your steerMultiplier
            rotate *= playerMain.steerMultiplier;
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
            if (driftType >= 0)
            {
                SetBoost(driftBoostPower[driftType], 1f);
            }
            driftTimer = 0;
            driftType = -1;
        }
        //Turn off drift button
        //drift = false;

        // burst kill
        if(playerMain.GetHealthMultiplier() <= 0 && !respawn.IsRespawning)
        {
            respawn.StartRespawnCoroutine();
        }
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
        
        if(tauntHandler.IsTaunting)
        {
            rb.AddForce(new Vector3(kart.transform.forward.x, 0f, kart.transform.forward.z) * tauntSpeed, ForceMode.Force);
            //rb.AddForce(kart.transform.up * 10f, ForceMode.Acceleration);
            //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * steeringFriction);
        }

        //Gravity
        //Adding down relative to the kart so the kart can drive on anything
        rb.AddForce(-kart.transform.up * gravity, ForceMode.Acceleration);

        //Dash Force
        rb.AddForce(kart.transform.right * currentDash, ForceMode.Impulse);
        currentDash = 0;

        //ADD Chase Dash Force
        if (isChaseDashing)
        {
            //Horizontal
            rb.AddForce(kart.transform.right * chaseDashDirection.x * chaseDashPower, ForceMode.Acceleration);

            //Vertical
            rb.AddForce(kart.transform.forward * chaseDashDirection.y * chaseDashPower, ForceMode.Acceleration);
        }

        //Rotate Body
        RaycastHit hitNear;
        RaycastHit hitGround;

        // layermask to avoid player ball layer
        int lm = ~(1 << 3);

        //Grounded Check
        if (Physics.Raycast(kart.transform.position, Vector3.down, out hitGround, groundCheckDistance, lm))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        //Rotate Kart To Match Ground
        Physics.Raycast(kart.transform.position, Vector3.down, out hitNear, groundNearRayDistance, lm);
        kartParent.transform.up = Vector3.Lerp(kartParent.transform.up, hitNear.normal, Time.deltaTime * gravityChangeFriction);

        kartParent.transform.Rotate(0, transform.eulerAngles.y, 0);

        //Controller Follows Ball
        transform.position = ball.transform.position;

    }

    public void Steer(int direction, float amount)
    {
        //Turn if not dashing and not stunned
        if (!playerMain.isStunned) 
        { 
            rotate = direction * amount;
        }
        //If is dashing then rotate by multiplier
        if (isDashing)
        {
            rotate = direction * dashSteerMultiplier * amount;
        } else if (isChaseDashing)
        {
            rotate = direction * chaseDashSteerMultiplier * amount;
        }

        //Set Tap To Drift
        if (driftTap && steerTap && !isDashing && !isChaseDashing && !playerMain.isPlayerAttacking())
        {
            isDrifting = true;
            driftDirection = direction;
        }

        //Set Dash Values
        if (drift && driftTap && !isDashing && !isDodging && !isDrifting && !isChaseDashing && !playerMain.isPlayerAttacking())
        {
            isDashing = true;
            dashTimer = 0;
            dashDirection = direction;
            dash = dashPower * dashDirection;
        } 
        //Stop Dashing If Opposite Direction Is Pressed?
        //Turn directions while dashing
        if (isDashing && direction != dashDirection)
        {
            dashDirection = direction;
            dash = dashPower * dashDirection;
            //isDashing = false;
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

        //if trap and release while turning, then drift?
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
    /// Boosts the player.
    /// </summary>
    /// <param name="multiplier">Boost multiplier amount</param>
    /// <param name="time">Duration of the boost</param>
    public void SetBoost(float multiplier, float time)
    {
        playerMain.BoostMultiplier = multiplier;
        boostTimer = time;
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
        SetBoost(rampBoost, rampBoostTime);
        tauntSpeed = currentSpeed * tauntSpeedMultiplier;
        StartCoroutine(WaitForBoost());
    }

    public IEnumerator WaitForBoost()
    {
        // lift player off the ramp
        ToggleGravity(false, liftGravity);
        yield return new WaitForSeconds(liftTime);
        
        // suspend the player in the air
        ToggleGravity(false, 0);
        yield return new WaitForSeconds(suspendedTime);

        // fall to the ground
        ToggleGravity(false, fallGravity);
        yield return new WaitUntil(() => grounded == true);
        
        // set normal gravity, end taunt and give player ground boost
        ToggleGravity();
        tauntHandler.IsTaunting = false;
        SetBoost(groundBoost, groundBoostTime);
    }

    //Disables the gameobjects relating to the Invinibility Dodge
    private void DisableDodgeVFX()
    {
        if (dodgeBubbleVFX.activeSelf)
            dodgeBubbleVFX.SetActive(false);
    }

    //Disables the gameobjects relating to the Dash
    private void DisableDashVFX()
    {
        walkingRing.SetActive(false);
        walkingRingVFX.SetActive(false);
    }

    /// <summary>
    /// Stuns the player and can be used from external scripts.
    /// </summary>
    /// <param name="inTime">New stun time for the player</param>
    public void StunPlayer(float inTime)
    {
        playerMain.stunTime = inTime;
    }
}

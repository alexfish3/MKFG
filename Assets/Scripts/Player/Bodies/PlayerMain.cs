///
/// Created by Alex Fischer | May 2024
/// 

using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The base player class that stores information all players need
/// </summary>
public abstract class PlayerMain : MonoBehaviour
{
    public string playerName;

    [Header("Info")]
    [SerializeField] int playerID = 0;
        public void SetBodyPlayerID(int PlayerID) { playerID = PlayerID; } // Sets the player ID of the body
        public int GetBodyPlayerID() { return playerID; } // Returns the player ID of the body

    [SerializeField] int deviceId = 0;
        public void SetBodyDeviceID(int DeviceID) { deviceId = DeviceID; } // Sets the device ID of the body
        public int GetBodyDeviceID() { return deviceId; } // Returns the device ID of the body

    [SerializeField] int teamID = 0;
        public void SetBodyTeamID(int TeamID) { teamID = TeamID; } // Sets the team ID of the body
        public int GetBodyTeamID() { return teamID; } // Returns the team ID of the body

    [SerializeField] string playerUsername = "";
        public string GetPlayerUsername() { return playerUsername; } // Returns the player username
        public void SetPlayerUsername(string PlayerUsername) { playerUsername = PlayerUsername; } // Sets the player username

    [SerializeField] Color teamColor = Color.white;
        public void SetBodyTeamColor(Color TeamColor) { teamColor = TeamColor; playerStatusIndicators.SetColorOfPlayerArrows(TeamColor); } // Sets the team color of the body
        public Color GetBodyTeamColor() { return teamColor; } // Returns the team color of the body

    [SerializeField] private Canvas playerDisplayUI;
        public void SetPlayerCanvas(Canvas newDisplayUI) { playerDisplayUI = newDisplayUI; }
        public Canvas GetPlayerCanvas() { return playerDisplayUI; }

    [SerializeField] public BallDrivingVersion1 ballDriving;
    [SerializeField] public GameObject kart;
    [SerializeField] public GameObject playerBodyBall;
    [SerializeField] protected SoundPool soundPool;

    PlacementHandler placementHandler;
    Collider playerHurtbox;

    [Header("UI")]
    public PlayerStatusIndicators playerStatusIndicators;

    [Header("Attacks")]
    [SerializeField] public GameObject[] attacks;
    [SerializeField] public GameObject[] specials;
    public LightAttack[] attacksInfo;
    public LightAttack[] specialsInfo;
    public bool attackLanded;

    [Header("Cameras")]
    [SerializeField] public Camera playerCamera;
    [SerializeField] public Camera uiCamera;
    [SerializeField] public Transform forwardTransform;
    [SerializeField] public Transform backwardTransform;

    [Header("Player Stats")]
    [SerializeField] public PlayerMatchStats playerMatchStats;
    [SerializeField] public float deathDamage = 0.2f;
    [SerializeField] public float respawnDodgeTime = 3;
    public float respawnDodgeTimer = 0;
    //Health should be a set value?
    [SerializeField] float healthMultiplier = 1f;
    [SerializeField] float healthRecoveryRate = 0.5f;
    [SerializeField] float boostMultiplier = 1f;
    int healthPercent = 100;
    int projectedHealthPercent = 100;
    Vector3 currentVelocity = Vector3.zero;
    float totalVelocity = 0;
    public float GetHealthMultiplier() { return healthMultiplier; }
    public void SetHealthMultiplier(float newHealth) { healthMultiplier = newHealth; }
    public float healthDifference = 0.30f;
    //Your Damage Health That Lasts All Game
    public float damageHealthMultiplier = 1f;
    [SerializeField] public float damageHealthMultiplierRate = 0.025f;
    public bool isStunned;
    public float stunTime;
    public float steerMultiplier = 1f;
    public float movementStunTime = 0;
    public HitBoxInfo lastHitboxThatHit;
    Vector3 lastHitboxFixedForce = Vector3.zero;
    Vector3 lastHitboxDynamicForce = Vector3.zero;
    Vector3 forceDirection = Vector3.zero;
    Vector3 velocityOnHit = Vector3.zero;
    public float onHitStunTimer = 0;
    public float sameAttackTimer = 0;
    [SerializeField] public float sameAttackTime = 0;
    public GameObject lastAttack;

    float projectedHealth;

    public float sideSpecialCooldownTimer = 0;
    public float forwardSpecialCooldownTimer = 0;
    public float neutralSpecialCooldownTimer = 0;
    public float backSpecialCooldownTimer = 0;

    // getters and setters
    public float BoostMultiplier {  get { return boostMultiplier; } set {  boostMultiplier = value; } }

    void Start()
    {
        placementHandler = playerBodyBall.GetComponent<PlacementHandler>();
        playerHurtbox = kart.GetComponent<Collider>();

        attacksInfo = new LightAttack[attacks.Length];
        specialsInfo = new LightAttack[specials.Length];

        for (int i = 0; i < attacks.Length; i++)
        {
            attacksInfo[i] = attacks[i].GetComponent<LightAttack>();
        }
        for (int i = 0; i < specials.Length; i++)
        {
            specialsInfo[i] = specials[i].GetComponent<LightAttack>();
        }
    }

    /// <summary>
    /// The generic up method for when up is pressed
    /// </summary>
    public virtual void Up(bool status)
    {
        ballDriving.up = status;
    }

    /// <summary>
    /// The generic left method for when left is pressed
    /// </summary>
    public virtual void Left(bool status)
    {
        ballDriving.left = status;
    }

    /// <summary>
    /// The generic down method for when down is pressed
    /// </summary>
    public virtual void Down(bool status)
    {
        ballDriving.down = status;
    }

    /// <summary>
    /// The generic right method for when right is pressed
    /// </summary>
    public virtual void Right(bool status)
    {
        ballDriving.right = status;
    }

    /// <summary>
    /// The generic drift method for when drift is pressed
    /// </summary>
    public virtual void Drift(bool status)
    {
        ballDriving.drift = status;
    }

    /// <summary>
    /// The generic attack method for when attack is pressed
    /// </summary>
    public virtual void Attack(bool status)
    {

    }

    /// <summary>
    /// The generic special method for when special is pressed
    /// </summary>
    public virtual void Special(bool status)
    {

    }

    /// <summary>
    /// The generic special method for when drive is pressed
    /// </summary>
    public virtual void Drive(bool status)
    {
        ballDriving.drive = status;
    }

    /// <summary>
    /// The generic special method for when reverse is pressed
    /// </summary>
    public virtual void Reverse(bool status)
    {
        ballDriving.reverse = status;
    }

    /// <summary>
    /// The generic special method for when reverse camera is pressed
    /// </summary>
    public virtual void ReflectCamera(bool status)
    {
        // If button is held, flip camera
        if (status)
        {
            playerCamera.transform.position = backwardTransform.position;
            playerCamera.transform.rotation = backwardTransform.rotation;

            uiCamera.transform.position = backwardTransform.position;
            uiCamera.transform.rotation = backwardTransform.rotation;
        }
        else
        {
            playerCamera.transform.position = forwardTransform.position;
            playerCamera.transform.rotation = forwardTransform.rotation;

            uiCamera.transform.position = forwardTransform.position;
            uiCamera.transform.rotation = forwardTransform.rotation;
        }
    }

    public virtual void Pause(bool status)
    {
        
    }

    public void LeftStick(Vector2 axis)
    {
        ballDriving.leftStick = axis;
    }

    public void RightStick(Vector2 axis)
    {
        ballDriving.rightStick = axis;
    }

    /// <summary>
    /// The generic OnHit method when the player is attacked
    /// </summary>
    public virtual void OnHit(HitBoxInfo landedHitbox)
    {
        //If Double Hit
        if (landedHitbox == lastHitboxThatHit && isStunned)
        {
            return;
        }

        //Stats
        playerMatchStats.AddDamageTaken(landedHitbox.damage);
        landedHitbox.playerBody.playerMatchStats.AddDamageDone(landedHitbox.damage);

        landedHitbox.playerBody.attackLanded = true;
        lastHitboxThatHit = landedHitbox;
        disablePlayerAttacking();
        stunTime = landedHitbox.stun;
        onHitStunTimer = landedHitbox.stun;

        //Clamp Health
        if (GetHealthMultiplier() > landedHitbox.damage) { 
            SetHealthMultiplier(GetHealthMultiplier() - landedHitbox.damage);
            damageHealthMultiplier -= landedHitbox.damage * damageHealthMultiplierRate; //If 10% damage then remove 0.01% from damageHealth
        } else
        {
            //0 health
            SetHealthMultiplier(0);
            damageHealthMultiplier -= landedHitbox.damage * damageHealthMultiplierRate;
        }


        if (landedHitbox.lockOpponentWhileActive)
        {
            movementStunTime = landedHitbox.attack.activeTimeRemaining;
        } else
        {
            movementStunTime = -1;
            //Horizontal Force
            //If Left
            if (Mathf.Sign(landedHitbox.attack.gameObject.transform.localScale.x) > 0)
            {
                forceDirection = (-landedHitbox.kart.transform.right * landedHitbox.dir.x) + landedHitbox.transform.forward* landedHitbox.dir.z;
            } else //If Right
            {
                forceDirection = (landedHitbox.kart.transform.right * landedHitbox.dir.x) + landedHitbox.transform.forward * landedHitbox.dir.z;
            }
            //set kart to opponent velocity
            ballDriving.rb.velocity = landedHitbox.playerBody.ballDriving.rb.velocity;
            velocityOnHit = ballDriving.rb.velocity;

            //set kart rotation to opponent rotation
            if (lastHitboxThatHit.rotateToPlayer)
            {
                ballDriving.SetKartRotation(landedHitbox.playerBody.ballDriving.GetKartRotation());
            }

            //Add Force
            lastHitboxFixedForce = forceDirection.normalized * landedHitbox.fixedForce;
            lastHitboxDynamicForce = forceDirection.normalized * landedHitbox.dynamicForce * ((1 - healthMultiplier) + 1) * landedHitbox.dynamicForceMultiplier;
            ballDriving.rb.AddForce(lastHitboxFixedForce, ForceMode.Force);
            ballDriving.rb.AddForce(lastHitboxDynamicForce, ForceMode.Force);
        }
    }

    public virtual void OnLanded(float damage)
    {
        damageHealthMultiplier += damage * damageHealthMultiplierRate; //set player damage health multiplier
    }

    void FixedUpdate()
    {
        //Get Velocity Info & Set Velocity To Zero If Near Zero
        currentVelocity = ballDriving.rb.velocity;
        totalVelocity = currentVelocity.magnitude;
        //if (totalVelocity < 0.1f && totalVelocity > -0.1f)
        //{
        //ballDriving.rb.velocity = Vector3.zero;
        // }

        //Movement Stun Time
        if (movementStunTime > 0)
        {
            Vector3 moveTowardsPosition;
            //If Left
            if (Mathf.Sign(lastHitboxThatHit.attack.gameObject.transform.localScale.x) > 0)
            {
                moveTowardsPosition = lastHitboxThatHit.gameObject.transform.position + (lastHitboxThatHit.kart.transform.forward * lastHitboxThatHit.lockPosition.z) + (lastHitboxThatHit.kart.transform.right * lastHitboxThatHit.lockPosition.x);          
                if (lastHitboxThatHit.pullToKart)
                {
                    moveTowardsPosition = lastHitboxThatHit.kart.transform.position + (lastHitboxThatHit.kart.transform.forward * lastHitboxThatHit.lockPosition.z) + (lastHitboxThatHit.kart.transform.right * lastHitboxThatHit.lockPosition.x);
                }
            }
            else //If Right
            {
                moveTowardsPosition = lastHitboxThatHit.gameObject.transform.position + (lastHitboxThatHit.kart.transform.forward * lastHitboxThatHit.lockPosition.z) + (-lastHitboxThatHit.kart.transform.right * lastHitboxThatHit.lockPosition.x);
                if (lastHitboxThatHit.pullToKart)
                {
                    moveTowardsPosition = lastHitboxThatHit.kart.transform.position + (lastHitboxThatHit.kart.transform.forward * lastHitboxThatHit.lockPosition.z) + (-lastHitboxThatHit.kart.transform.right * lastHitboxThatHit.lockPosition.x);
                }
            }
            if (lastHitboxThatHit.godProperty)
            {
                ballDriving.rb.transform.position = moveTowardsPosition; //set to hitbox position
            }
            else
            {
                Vector3 moveDirection = (moveTowardsPosition - ballDriving.rb.transform.position).normalized;
                // pull towards x how far away it is
                ballDriving.rb.AddForce(moveDirection * lastHitboxThatHit.pullForce * (moveTowardsPosition - ballDriving.rb.transform.position).magnitude);
            }


            //ballDriving.rb.transform.position = new Vector3(hit)
            movementStunTime -= Time.fixedDeltaTime;
        }

        //Apply Force While Stunned
        if (onHitStunTimer > 0 && lastHitboxThatHit != null && lastHitboxThatHit.constantFixedForce != 0)
        {
            ballDriving.rb.velocity = lastHitboxThatHit.playerBody.ballDriving.rb.velocity;
            ballDriving.rb.velocity += velocityOnHit.magnitude * forceDirection.normalized * lastHitboxThatHit.constantFixedForce;
            if (ballDriving.rb.velocity.magnitude < lastHitboxThatHit.defaultConstForce)
            {
                ballDriving.rb.velocity = forceDirection.normalized * lastHitboxThatHit.defaultConstForce;
            }
        }

        #region Enable/Disable Hurtbox
        //Disable & Enable Hurtbox
        if (ballDriving.isDodging)
        {
            playerHurtbox.enabled = false;
        }
        else
        {
            playerHurtbox.enabled = true;
        }
        #endregion

        //Respawn i frames
        if (respawnDodgeTimer > 0)
        {
            respawnDodgeTimer -= Time.fixedDeltaTime;
            ballDriving.isDodging = true;

            if (isPlayerAttacking())
            {
                respawnDodgeTimer = 0;
            }
        }
    }

    void Update()
    {
        #region Movement & Combat Conditions
        //Disable drift while attacking
        if (ballDriving.isDrifting && isPlayerAttacking())
        {
            ballDriving.isDrifting = false;
        }
        //Disable chase dash while attacking
        if (ballDriving.isChaseDashing && isPlayerAttacking())
        {
            ballDriving.isChaseDashing = false;
        }
        //Disable dodge if attacking
        if (ballDriving.isDodging && isPlayerAttacking())
        {
            ballDriving.isDodging = false;
        }
        if (ballDriving.isDashing && isPlayerAttacking())
        {
            ballDriving.isDashing = false;
        }
            #endregion

        #region SetProjectedHealth
            //Set Health It Should Go To
            int numOfPlayers = PlayerSpawnSystem.Instance.GetActiveBrainCount();
        if (numOfPlayers > 1)
        {
            projectedHealth = 1 + (healthDifference / (numOfPlayers - 1)) * (placementHandler.Placement - 1);
            projectedHealth = Mathf.Round(projectedHealth * 100) * 0.01f;

            projectedHealth *= damageHealthMultiplier;
        }
        else
        {
            projectedHealth = 100;
        }
        //Set to percent out of 100
        #endregion

        // Handles player's stun time if the player is stunned
        #region StunHandler
        if (stunTime > 0)
        {
            isStunned = true;
            stunTime -= Time.deltaTime;
            onHitStunTimer -= Time.deltaTime;
        }
        else
        {
            stunTime = 0;
            onHitStunTimer = 0;
            isStunned = false;
        }
        #endregion

        //Reset Player. Each Placement is 10%
        #region RecoveryPlayerHealth
        healthPercent = Mathf.RoundToInt(healthMultiplier * 100);
        projectedHealthPercent = Mathf.RoundToInt(projectedHealth * 100);
        if (!isStunned && numOfPlayers > 1)
        {
            if (healthPercent < projectedHealthPercent)
            {
                healthMultiplier += healthRecoveryRate * Time.deltaTime;
            }
            else if (healthPercent > projectedHealthPercent)
            {
                healthMultiplier -= healthRecoveryRate * Time.deltaTime;
            }
        }
        #endregion

        //Same Attack Timer
        if (sameAttackTimer > 0)
        {
            sameAttackTimer -= Time.deltaTime;
        } else
        {
            sameAttackTimer = 0;
        }

        //Specials Cooldown
        if (sideSpecialCooldownTimer > 0)
        {
            sideSpecialCooldownTimer -= Time.deltaTime;
        }
        if (neutralSpecialCooldownTimer > 0)
        {
            neutralSpecialCooldownTimer -= Time.deltaTime;
        }
        if (backSpecialCooldownTimer > 0)
        {
            backSpecialCooldownTimer -= Time.deltaTime;
        }
        if (forwardSpecialCooldownTimer > 0)
        {
            forwardSpecialCooldownTimer -= Time.deltaTime;
        }
    }

    public bool isPlayerAttacking()
    {
        //check if a game object is active and if so then return false
        for (int i = 0; i < attacks.Length; i++)
        {
            if (attacks[i].activeInHierarchy)
            {
                if (!attacksInfo[i].isUtility)
                    return true;
            }
        }

        //check if a game object is active and if so then return false
        for (int i = 0; i < specials.Length; i++)
        {
            if (specials[i].activeInHierarchy)
            {
                //Check if special is utility then say it isn't active
                if (!specialsInfo[i].isUtility)
                    return true;
            }
        }

        return false;
    }

    public void disablePlayerAttacking()
    {
        //disable all non utility attacks

        //check if a game object is active and if so then return false
        for (int i = 0; i < attacks.Length; i++)
        {
            if (attacks[i].activeInHierarchy && !attacksInfo[i].isUtility)
            {
                //disable children hitboxes
                for (int j = 0; j < attacks[i].transform.childCount; j++)
                {
                    attacks[i].transform.GetChild(j).gameObject.SetActive(false);
                }
                //disable parent after
                attacks[i].SetActive(false);
            }
        }
        for (int i = 0; i < specials.Length; i++)
        {
            if (specials[i].activeInHierarchy && !specialsInfo[i].isUtility)
            {
                //disable children hitboxes
                for (int j = 0; j < specials[i].transform.childCount; j++)
                {
                    specials[i].transform.GetChild(j).gameObject.SetActive(false);
                }
                //disable parent after
                specials[i].SetActive(false);
            }
        }

    }

    public void StopDriving()
    {
        ballDriving.rb.velocity = Vector3.zero;

        ballDriving.up = false;
        ballDriving.down = false;
        ballDriving.left = false;
        ballDriving.right = false;
        ballDriving.steerTap = false;
        ballDriving.drift = false;
        ballDriving.driftTap = false;
        ballDriving.lastdriftInput = false;
        ballDriving.drive = false;
        ballDriving.reverse = false;
    }
}

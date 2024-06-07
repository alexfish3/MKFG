///
/// Created by Alex Fischer | May 2024
/// 

using UnityEngine;

/// <summary>
/// The base player class that stores information all players need
/// </summary>
public abstract class PlayerMain : MonoBehaviour, IPlayer
{
    public string playerName;

    [Header("Info")]
    [SerializeField] int deviceId = 0;
    public void SetBodyDeviceID(int DeviceID) { deviceId = DeviceID; } // Sets the device ID of the body
    public int GetBodyDeviceID() { return deviceId; } // Returns the device ID of the body

    [SerializeField] public Camera playerCamera;
    [SerializeField] private Canvas playerDisplayUI;
    public void SetPlayerCanvas(Canvas newDisplayUI) { playerDisplayUI = newDisplayUI; }
    public Canvas GetPlayerCanvas() { return playerDisplayUI; }
    [SerializeField] public BallDrivingVersion1 ballDriving;
    [SerializeField] public GameObject kart;
    [SerializeField] GameObject playerBodyBall;
    PlacementHandler placementHandler;
    Collider playerHurtbox;
    public bool attackLanded = false;

    /*
    0 = Side Attack
    1 = Forward Attack
    2 = Back Attack
    3 = Neutral Attack
     */
    [SerializeField] public GameObject[] attacks;
    [SerializeField] public GameObject[] specials;

    [Header("Player Stats")]
    //Health should be a set value?
    [SerializeField] float healthMultiplier = 1f;
    [SerializeField] float healthRecoveryRate = 0.5f;
    int healthPercent = 100;
    int projectedHealthPercent = 100;
    public float GetHealthMultiplier() { return healthMultiplier; }
    public void SetHealthMultiplier(float newHealth) { healthMultiplier = newHealth; }
    public float healthDifference = 0.30f;
    [SerializeField] public bool isStunned;
    [SerializeField] public float stunTime;

    float projectedHealth;

    void Start()
    {
        placementHandler = playerBodyBall.GetComponent<PlacementHandler>();
        playerHurtbox = kart.GetComponent<Collider>();
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

    public virtual void Drive(bool status)
    {
        ballDriving.drive = status;
    }

    public virtual void Reverse(bool status)
    {
        ballDriving.reverse = status;
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
    public virtual void OnHit(Vector3 dir, float force, float stun, float damage, GameObject attacker)
    {
        stunTime = stun;
        ballDriving.rb.AddForce(attacker.transform.TransformVector(dir) * force, ForceMode.Force);
        SetHealthMultiplier(GetHealthMultiplier() - damage);
    }
    void Update()
    {
    }

    public virtual void FixedUpdate()
    {

        //Disable & Enable Hurtbox
        if (ballDriving.isDodging)
        {
            playerHurtbox.enabled = false;
        } else
        {
            playerHurtbox.enabled = true;
        }

        //No attacking while stunned
        if (isStunned)
        {
            disablePlayerAttacking();
        }

        //Set Health It Should Go To
        int numOfPlayers = PlayerSpawnSystem.Instance.GetPlayerCount();
        if (numOfPlayers > 1)
        {
            projectedHealth = 1 + (healthDifference / (numOfPlayers - 1)) * (placementHandler.Placement - 1);
            projectedHealth = Mathf.Round(projectedHealth * 100) * 0.01f;
        } else
        {
            projectedHealth = 100;
        }
        //Set to percent out of 100

        // Handles player's stun time if the player is stunned
        #region StunHandler
        if (stunTime > 0)
        {
            isStunned = true;
            stunTime -= Time.fixedDeltaTime;
        }
        else
        {
            stunTime = 0;
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
                healthMultiplier += healthRecoveryRate * Time.fixedDeltaTime;
            }
            else if (healthPercent > projectedHealthPercent)
            {
                healthMultiplier -= healthRecoveryRate * Time.fixedDeltaTime;
            }
        }
        #endregion
    }

    public bool isPlayerAttacking()
    {
        //check if a game object is active and if so then return false
        for (int i = 0; i < attacks.Length; i++)
        {
            if (attacks[i].activeInHierarchy)
            {
                return true;
            }
        }

        return false;
    }

    public void disablePlayerAttacking()
    {
        //check if a game object is active and if so then return false
        for (int i = 0; i < attacks.Length; i++)
        {
            if (attacks[i].activeInHierarchy)
            {
                attacks[i].SetActive(false);
            }
        }

    }
}

/// <summary>
/// Player Interface for all inputs the player requires
/// </summary>
public interface IPlayer
{
    public void Up(bool status);
    public void Left(bool status);
    public void Down(bool status);
    public void Right(bool status);
    public void Drift(bool status);
    public void Attack(bool status);
    public void Special(bool status);
    public void Drive(bool status);
    public void Reverse(bool status);
    public void LeftStick(Vector2 axis);
    public void RightStick(Vector2 axis);

}

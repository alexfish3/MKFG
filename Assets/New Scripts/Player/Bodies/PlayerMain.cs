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
    [SerializeField] public BallDrivingVersion1 ballDriving;
    [SerializeField] public GameObject kart;
    [SerializeField] public GameObject sideAttack;
    [SerializeField] public GameObject forwardAttack;
    [SerializeField] public GameObject backAttack;
    [SerializeField] public GameObject neutralAttack;

    [Header("Player Stats")]
    //Health should be a set value?
    [SerializeField] float healthMultiplier = 1f;
    public float GetHealthMultiplier() { return healthMultiplier; }
    public void SetHealthMultiplier(float newHealth) { healthMultiplier = newHealth; }

    [SerializeField] public bool isStunned;
    [SerializeField] public float stunTime;

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

    /// <summary>
    /// The generic OnHit method when the player is attacked
    /// </summary>
    public void OnHit(Vector3 dir, float force, float stun, float damage, GameObject attacker)
    {
        stunTime = stun;
        ballDriving.rb.AddForce(attacker.transform.TransformVector(dir) * force, ForceMode.Force);
        SetHealthMultiplier(GetHealthMultiplier() - damage);
    }

    public void FixedUpdate()
    {
        // Handles player's stun time if the player is stunned
        if (stunTime > 0)
        {
            isStunned = true;
            stunTime -= Time.deltaTime;
        } 
        else
        {
            stunTime = 0;
            isStunned = false;
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

}

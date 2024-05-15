using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour, IPlayer
{
    public string playerName;

    [Header("Info")]
    [SerializeField] int deviceId = 0;
    public void SetBodyDeviceID(int DeviceID) { deviceId = DeviceID; }
    public int GetBodyDeviceID() { return deviceId; }

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

    public virtual void Up(bool status)
    {
        ballDriving.up = status;
    }

    public virtual void Left(bool status)
    {
        ballDriving.left = status;
    }

    public virtual void Down(bool status)
    {
        ballDriving.down = status;
    }

    public virtual void Right(bool status)
    {
        ballDriving.right = status;
    }

    public virtual void Drift(bool status)
    {
        ballDriving.drift = status;
    }
    
    public virtual void Attack(bool status)
    {

    }

    public virtual void Special(bool status)
    {

    }

    public void OnHit(Vector3 dir, float force, float stun, float damage)
    {
        stunTime = stun;
        ballDriving.rb.AddForce((dir + kart.transform.position.normalized) * force, ForceMode.Force);
        SetHealthMultiplier(GetHealthMultiplier() - damage);
    }

    public void FixedUpdate()
    {
        if (stunTime > 0)
        {
            isStunned = true;
            stunTime -= Time.deltaTime;
        } else
        {
            stunTime = 0;
            isStunned = false;
        }

    }
}

public interface IPlayer
{
    public void Up(bool status);
    public void Left(bool status);
    public void Down(bool status);
    public void Right(bool status);
    public void Drift(bool status);
}

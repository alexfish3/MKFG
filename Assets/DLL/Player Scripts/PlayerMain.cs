using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour, IPlayer
{
    public string playerName;

    [SerializeField] int deviceId = 0;
    [SerializeField] float healthMultiplier = 1f;
    [SerializeField] Camera playerCamera;
    public float GetHealthMultiplier() { return healthMultiplier; }
    public void SetBodyDeviceID(int DeviceID) { deviceId = DeviceID; }
    public int GetBodyDeviceID() { return deviceId; }

    [SerializeField] BallDrivingVersion1 ballDriving;

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
        /*  
  if(status == false)
  {
      ballDriving.drifted = !status; 
  }*/
    }

    public void ResetMovement(bool status)
    {

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

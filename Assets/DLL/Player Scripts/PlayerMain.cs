using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour, IPlayer
{
    public string playerName;

    [SerializeField] int deviceId = 0;
    public void SetBodyDeviceID(int DeviceID) { deviceId = DeviceID; }
    public int GetBodyDeviceID() { return deviceId; }

    Vector3 direction;
    public int speed;

    public void Update()
    {
        transform.position = transform.position += (direction * speed);
    }

    public virtual void Attack()
    {

    }

    public virtual void MoveDown()
    {

    }

    public virtual void MoveLeft()
    {
        direction = new Vector3(-1, 0, 0);
    }

    public virtual void MoveRight()
    {
        direction = new Vector3(1, 0, 0);
    }

    public virtual void MoveUp()
    {

    }

    public void ResetMovement()
    {
        direction = Vector3.zero;
    }
}

public interface IPlayer{
    public void MoveLeft();
    public void MoveRight();
    public void MoveUp();
    public void MoveDown();

    public void Attack();
}

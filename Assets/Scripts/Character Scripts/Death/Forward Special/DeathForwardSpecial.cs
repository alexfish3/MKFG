using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathForwardSpecial : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] BallDrivingVersion1 playerController;
    [SerializeField] GameObject kart;
    [SerializeField] int recoveryForce = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        playerController.rb.AddForce(kart.transform.forward * recoveryForce, ForceMode.Impulse);
    }
}

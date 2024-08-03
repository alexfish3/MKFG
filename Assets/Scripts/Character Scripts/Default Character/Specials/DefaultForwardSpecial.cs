using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultForwardSpecial : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] BallDrivingVersion1 playerController;
    [SerializeField] GameObject kart;
    [SerializeField] float recoveryForce = 0;
    [SerializeField] PlacementHandler placement;
    [SerializeField] GameObject special;
    [SerializeField] LightAttack specialInfo;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (placement.Placement != 1 && !specialInfo.hasLanded)
        {
            playerController.rb.AddForce(kart.transform.forward * recoveryForce * placement.Placement * (specialInfo.chargePercent + 1), ForceMode.Impulse);
        }
    }
}

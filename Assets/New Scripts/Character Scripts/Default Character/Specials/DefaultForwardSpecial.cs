using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultForwardSpecial : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] BallDrivingVersion1 playerController;
    [SerializeField] GameObject kart;
    [SerializeField] int recoveryForce = 0;
    [SerializeField] float coolDown = 0;
    float coolDownTimer = 0;
    void OnEnable()
    {
        playerController.rb.AddForce(kart.transform.forward * recoveryForce, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        coolDownTimer += Time.deltaTime;
        if (coolDownTimer > coolDown)
        {
            coolDownTimer = 0;
            gameObject.SetActive(false);
        }
    }
}

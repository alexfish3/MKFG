using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSparkHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] driftSparks;
    [SerializeField] private GameObject boostSparks;
    private bool boosting = false;
    private float boostTime;

    private void Start()
    {
        ToggleDriftSparks(false);
        boostSparks.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (!boosting)
            return;

        boostTime -= Time.deltaTime;
        boostSparks.transform.localScale = Vector3.one * boostTime;
        if(boostTime <= 0)
        {
            boostSparks.transform.localScale = Vector3.zero;
            boosting = false;
        }
    }

    /// <summary>
    /// Sets drift sparks to desired activation and scale.
    /// </summary>
    /// <param name="enabled"></param>
    /// <param name="scale"></param>
    public void ToggleDriftSparks(bool enabled=true, float scale=1.0f)
    {
        foreach(GameObject drift in driftSparks)
        {
            drift.SetActive(enabled);
            drift.transform.localScale = Vector3.one * scale;
        }
    }

    public void ToggleBoostSparks(float inBoostTime=1f)
    {
        boostTime = inBoostTime;
        boosting = true;
    }
}

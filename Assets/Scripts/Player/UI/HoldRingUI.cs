using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldRingUI : MonoBehaviour
{
    [SerializeField] float maxTimeToHold;
    float timer;
    [SerializeField] float fillPercent;

    [SerializeField] Image fillRing;
    [SerializeField] Image backgroundRing;

    public event Action OnFillRing;

    /// <summary>
    /// Updates the ring every tick, filling the ring
    /// </summary>
    public void TickFill()
    {
        if (timer >= maxTimeToHold)
        {
            timer = 0;
            fillRing.enabled = false;
            backgroundRing.enabled = false;

            SetFill();
            OnFillRing?.Invoke();
            return;
        }

        timer += Time.deltaTime;
        SetFill();
    }

    /// <summary>
    /// Sets the ring's fill to a calculated value
    /// </summary>
    private void SetFill()
    {
        if(timer > 0) // Timer is not zero, want to show ring
        {
            fillRing.enabled = true;
            backgroundRing.enabled = true;
        }
        else // Timer is zero, want to hide ring
        {
            fillRing.enabled = false;
            backgroundRing.enabled = false;
        }

        fillPercent = timer / maxTimeToHold;
        fillRing.fillAmount = fillPercent;
    }

    public void SetFillZero()
    {
        timer = 0;
        fillPercent = 0;
        fillRing.fillAmount = 0;

        fillRing.enabled = false;
        backgroundRing.enabled = false;
    }
}

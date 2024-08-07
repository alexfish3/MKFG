using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusIndicatorInstance : MonoBehaviour
{
    [Header("Status Indicator Sections")]
    [SerializeField] GameObject abovePlayer;
    [SerializeField] GameObject belowPlayer;
    Vector3 scaleValue;

    [Header("Team Color")]
    [SerializeField] Color teamColor;
    [SerializeField] SpriteRenderer[] teamColorSprites;

    [Header("Speed Color")]
    [SerializeField] Gradient speedGradient;
    [SerializeField] float maxHealthSpeedValue;
    [SerializeField] SpriteRenderer[] speedColorSprites;

    [Header("Cooldowns")]
    [SerializeField] GameObject[] specialCooldowns;
    [SerializeField] float[] specialCooldownTimes;

    public void SetLocalScale(float newScaleFloat)
    {
        scaleValue = new Vector3 (newScaleFloat, newScaleFloat, newScaleFloat);
        abovePlayer.transform.localScale = scaleValue;
        belowPlayer.transform.localScale = scaleValue;
    }

    public void SetTeamColor(Color newTeamColor)
    {
        teamColor = newTeamColor;

        foreach (SpriteRenderer sprite in teamColorSprites)
        {
            sprite.color = newTeamColor;
        }
    }

    public void SetSpeedColorHealth(float currentHealth)
    { 
        float sampledHealth = Mathf.Clamp01(currentHealth / maxHealthSpeedValue);

        Color speedColor = speedGradient.Evaluate(sampledHealth);

        foreach (SpriteRenderer sprite in speedColorSprites)
        {
            sprite.color = speedColor;
        }
    }

    public void SetSpecialCooldownStatus()
    {

    }
}

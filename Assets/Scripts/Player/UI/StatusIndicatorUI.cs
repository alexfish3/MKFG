///
/// Created by Alex Fischer | August 2024
/// 

using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class StatusIndicatorUI : MonoBehaviour
{
    [Header("Status Indicator Sections")]
    int trackedPlayerId = -1;
    public void SetTrackedPlayerId(int TrackedPlayerId) { trackedPlayerId = TrackedPlayerId; }
    public int GetTrackedPlayerId() { return trackedPlayerId; }

    [SerializeField] GameObject abovePlayer;
    [SerializeField] GameObject belowPlayer;

    Vector3 scaleValue;

    [Header("Team Color")]
    [SerializeField] Color teamColor;
    [SerializeField] Image[] teamColorSprites;

    [Header("Speed Color")]
    [SerializeField] Gradient speedGradient;
    [SerializeField] float maxHealthSpeedValue;
    [SerializeField] Image[] speedColorSprites;

    [Header("Cooldowns")]
    [SerializeField] SpecialCooldownUI[] specialCooldowns;

    /// <summary>
    /// Sets the local scale of the status indicator
    /// </summary>
    /// <param name="newScaleFloat"></param>
    public void SetLocalScale(float newScaleFloat)
    {
        scaleValue = new Vector3(newScaleFloat, newScaleFloat, newScaleFloat);
        abovePlayer.transform.localScale = scaleValue;
        belowPlayer.transform.localScale = scaleValue;
    }

    public void SetTeamColor(Color newTeamColor)
    {
        teamColor = newTeamColor;

        foreach (Image sprite in teamColorSprites)
        {
            sprite.color = newTeamColor;
        }
    }

    public void SetSpeedColorHealth(float currentHealth)
    {
        float sampledHealth = Mathf.Clamp01(currentHealth / maxHealthSpeedValue);

        Color speedColor = speedGradient.Evaluate(sampledHealth);

        foreach (Image sprite in speedColorSprites)
        {
            sprite.color = speedColor;
        }
    }

    /// <summary>
    /// Initalizes all the special cooldowns in the status ui
    /// </summary>
    /// <param name="specialAttacks">The list of special attacks on the player who's ui is being initalized</param>
    public void InitalizeCooldowns(List<LightAttack> specialAttacks, Sprite[] cooldownImages)
    {
        // Loops through passed in special attacks and initalizes a cooldown for each
        for(int i = 0; i < specialAttacks.Count; i++)
        {
            Debug.Log("Initalizing cooldown for time " + specialAttacks[i].specialRecoveryTime);
            specialCooldowns[i].InitalizeCooldown(specialAttacks[i].specialRecoveryTime, cooldownImages[i]);
        }
    }

    /// <summary>
    /// Sets the special's cooldown to begin
    /// </summary>
    /// <param name="cooldownToSet">The cooldown to start</param>
    public void SetSpecialCooldownStatus(int cooldownToSet)
    {
        specialCooldowns[cooldownToSet].gameObject.SetActive(true);
        specialCooldowns[cooldownToSet].StartCooldown();
    }
}

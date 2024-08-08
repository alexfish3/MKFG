using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultsPodium : MonoBehaviour
{
    [SerializeField] TMP_Text nameTaxt;
    [SerializeField] TMP_Text placementText;
    [SerializeField] GameObject crownIcon;
    [SerializeField] TMP_Text[] statTexts;

    /// <summary>
    /// Initalizes the results podium to match a set player's stats
    /// </summary>
    /// <param name="placement">The placment the player got</param>
    /// <param name="playerPodiumStats">The class of stats the player had on race completed</param>
    public void InitalizeResultsPodium(int placement, PodiumStats playerPodiumStats)
    {
        // Set the placement Text!
        string tempPlacementString = placement.ToString();
        switch (placement)
        {
            case 1:
                tempPlacementString += "st";
                crownIcon.SetActive(true);
                break;
            case 2:
                tempPlacementString += "nd";
                break;
            case 3:
                tempPlacementString += "rd";
                break;
            case 4:
            default:
                tempPlacementString += "th";
                break;
        }
        placementText.text = tempPlacementString;

        // Set the name
        nameTaxt.text = playerPodiumStats.PlayerUserame;

        statTexts[0].text = Mathf.Round(playerPodiumStats.PlayerMain.damageHealthMultiplier * 100.0f).ToString() + "%";
        statTexts[1].text = Mathf.Round(playerPodiumStats.damageDone * 100.0f).ToString() + "%";
        statTexts[2].text = Mathf.Round(playerPodiumStats.damageTaken * 100.0f).ToString() + "%";
        statTexts[3].text = playerPodiumStats.kills.ToString();
        statTexts[4].text = playerPodiumStats.deaths.ToString();
        statTexts[5].text = playerPodiumStats.tricks.ToString();
    }
}

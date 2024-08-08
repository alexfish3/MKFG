///
/// Created by Alex Fischer | July 2024
/// 

using System;
using UnityEngine;

public class PlayerMatchStats : MonoBehaviour
{
    [Header("Match Stats")]
    [SerializeField] PlayerMain playerMain;
    [SerializeField] private PodiumStats stats;

    public PodiumStats Stats => stats;
    bool initalized = false;

    public void InitalizePodiumStats()
    {
        if (initalized == false)
        {
            initalized = true;
            Debug.Log("Creating New Podium Stats");

            // Set the old stats object to null to allow for garbage collection
            stats = null;

            stats = new PodiumStats
            {
                PlayerID = playerMain.GetBodyPlayerID(),
                PlayerUserame = playerMain.GetPlayerUsername(),
                PlayerMain = playerMain,
            };
        }
    }

    public void SetKartHealth(float kartHealth) { InitalizePodiumStats(); stats.kartPercent = kartHealth; }
    public void AddDamageDone(float addToDamageDone) { InitalizePodiumStats(); stats.damageDone += addToDamageDone; }
    public void AddDamageTaken(float addToDamageTaken) { InitalizePodiumStats(); stats.damageTaken += addToDamageTaken; }
    public void AddKill() { InitalizePodiumStats(); stats.kills++; }
    public void AddDeath() { InitalizePodiumStats(); stats.deaths++; }
    public void AddTrick() { InitalizePodiumStats(); stats.tricks++; }
}

[System.Serializable]
public class PodiumStats
{
    public int PlayerID;
    public string PlayerUserame;
    public PlayerMain PlayerMain;
    public float kartPercent;
    public float damageDone;
    public float damageTaken;
    public float kills;
    public float deaths;
    public float tricks;
}

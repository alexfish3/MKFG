using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMatchStats : MonoBehaviour
{
    [Header("Match Stats")]
    [SerializeField] private float damageDone;
    [SerializeField] private float damageTaken;
    [SerializeField] private float kills;
    [SerializeField] private float deaths;

    public void AddDamageDone(float addToDamageDone) { damageDone += addToDamageDone; }
    public void AddDamageTaken(float addToDamageTaken) { damageTaken += addToDamageTaken; }
    public void AddKill() { kills++; }
    public void AddDeath() { deaths++; }
}

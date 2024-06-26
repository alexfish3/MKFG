using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TauntHandler : MonoBehaviour
{
    [SerializeField] private Transform frontCheck, backCheck;
    [SerializeField] private float minSpeed = 5f;
    [SerializeField] private float tauntTime = 3f;
    [SerializeField] private BallDrivingVersion1 ball; // for speed checks
    
    private bool canTaunt = false, isTaunting = false;
    private IEnumerator cooldownRoutine;

    public Action TauntPerformed; // subscribe to this event in other scripts to control specific taunts

    // getters
    public bool CanTaunt { get { return canTaunt; } }
    public float TauntTime { get { return tauntTime; } }

    // Start is called before the first frame update
    void Start()
    {
        TauntPerformed += ball.StartWaitForBoost;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(frontCheck.position, frontCheck.position - frontCheck.up * 1f, Color.yellow);
        Debug.DrawLine(backCheck.position, backCheck.position - backCheck.up * 1f, Color.yellow);

        // if the rear raycast check is hitting something and the front isn't, and the player isn't currently tricking then this bool will be set to true
        canTaunt = !Physics.Raycast(frontCheck.position, -frontCheck.up, 1f) && Physics.Raycast(backCheck.position, -backCheck.up, 1f) && !isTaunting;
    }

    /// <summary>
    /// Method for player to taunt.
    /// </summary>
    public void Taunt()
    {
        if (!canTaunt)
            return;

        isTaunting = true;
        TauntPerformed?.Invoke();
        cooldownRoutine = TauntCooldown();
        StartCoroutine(cooldownRoutine);
    }

    private IEnumerator TauntCooldown()
    {
        isTaunting = true;
        yield return new WaitForSeconds(tauntTime);
        isTaunting = false;
    }
}

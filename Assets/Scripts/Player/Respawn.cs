using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.VFX;

/// <summary>
/// This class will respawn the player if they fall into the water.
/// </summary>
public class Respawn : MonoBehaviour
{
    IEnumerator respawnCoroutine;
    [Header("Player Information")]
    [SerializeField] private GameObject playerController;
    [SerializeField] private GameObject kartParent;

    [Header("Respawn Stats")]
    [SerializeField] private float respawnTime = 2f;

    [Header("VFX")]
    [SerializeField] private GameObject killVFX;
    [SerializeField] private float viewTime; // time after vfx is shown where nothing happens

    private Vector3 lastGroundedPos; // last position the player was grounded at

    private bool isRespawning;
    public bool IsRespawning { get { return isRespawning; } }

    private Rigidbody rb;
    private SphereCollider sc;
    private PlacementHandler ph;

    // new player information MKFG
    BallDrivingVersion1 player;
    SoundPool soundPool;

    // events
    public Action OnRespawnStart;
    public Action OnRespawnEnd;

    private void OnEnable()
    {
        // game object intialization
        player = playerController.GetComponent<BallDrivingVersion1>();
        ph = GetComponent<PlacementHandler>();

        // event subscriptions
        OnRespawnStart += () => kartParent.SetActive(false);
        OnRespawnEnd += () => kartParent.SetActive(true);
        OnRespawnStart += () => player.StunPlayer(respawnTime + viewTime);

        // object refs
        soundPool = playerController.GetComponent<SoundPool>();
    }

    private void Update()
    {
        // setting last grounded
        if(player.Grounded)
        {
            lastGroundedPos = transform.position;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sc = GetComponent<SphereCollider>();
        lastGroundedPos = transform.position;
        player = playerController.GetComponent<BallDrivingVersion1>();
    }

    /// <summary>
    /// The new respawning player coroutine. Simplified from the old version, rotates the player 180 degrees and positions them at their respawn point. Also creates a tombstone.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RespawnPlayer()
    {
        // disable ball function
        player.rb.velocity = Vector3.zero;

        RespawnPoint rsp = RespawnManager.Instance.GetRespawnPoint(lastGroundedPos);

        // vfx and sounds for killing
        Instantiate(killVFX, this.transform.position, new Quaternion(rsp.Facing.x, rsp.Facing.y + 90f, rsp.Facing.z, rsp.Facing.w));
        soundPool.PlaySound("orion_death", this.transform.position);

        float elapsedTime = 0;
        Vector3 deathPos = transform.position;
        Vector3 deathRot = transform.localRotation.eulerAngles;

        // wait while the player "views" their kill vfx
        while(elapsedTime < viewTime)
        {
            elapsedTime += Time.deltaTime;
            transform.position = deathPos;
            yield return null;
        }
        
        elapsedTime = 0;

        // move player to rsp
        while (elapsedTime < respawnTime)
        {
            transform.position = Vector3.Lerp(deathPos, rsp.transform.position, elapsedTime/respawnTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // match player rotation to rsp
        player.SetKartRotation(rsp.Facing.eulerAngles);
        StopRespawnCoroutine();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Killbox")
        {
            StartRespawnCoroutine();
        }
    }

    public void StartRespawnCoroutine()
    {
        player.StopWaitForBoost();
        player.playerMain.disablePlayerAttacking();

        player.playerMain.damageHealthMultiplier -= player.playerMain.deathDamage * player.playerMain.damageHealthMultiplierRate;
        if (player.playerMain.damageHealthMultiplier < 0)
        {
            player.playerMain.damageHealthMultiplier = 0;
        }

        if(player.playerMain.lastHitboxThatHit != null)
            player.playerMain.lastHitboxThatHit.playerBody.damageHealthMultiplier += player.playerMain.deathDamage * player.playerMain.lastHitboxThatHit.playerBody.damageHealthMultiplierRate;
        
        //Stats
        player.playerMain.playerMatchStats.AddDeath();

        if(player.playerMain.lastHitboxThatHit != null)
            player.playerMain.lastHitboxThatHit.playerBody.playerMatchStats.AddKill();

        player.playerMain.stunTime = 0;
        OnRespawnStart?.Invoke();
        isRespawning = true;

        // Turning these off fixes camera jittering on respawn
        player.StopBall();
        rb.useGravity = false;
        //sc.enabled = false;

        if (respawnCoroutine == null)
        {
            respawnCoroutine = RespawnPlayer();
        }

        //player.CanSteer = false;

        StartCoroutine(respawnCoroutine);
    }

    private void StopRespawnCoroutine()
    {
        OnRespawnEnd?.Invoke();
        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
        }

        isRespawning = false;
        respawnCoroutine = null;
        rb.useGravity = true;
        sc.enabled = true;
        player.playerMain.SetHealthMultiplier(1f);
        player.playerMain.respawnDodgeTimer = player.playerMain.respawnDodgeTime;
        player.CanSteer = true;
    }
}

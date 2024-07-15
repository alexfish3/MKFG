using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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

    // new player information MKFG
    BallDrivingVersion1 player;
    SoundPool soundPool;

    private RespawnPoint[] legalRSPs;

    public RespawnPoint[] LegalRSPs { get { return legalRSPs; } set { legalRSPs = value; } }

    // events
    public Action OnRespawnStart;
    public Action OnRespawnEnd;

    private void OnEnable()
    {
        // game object intialization
        player = playerController.GetComponent<BallDrivingVersion1>();

        // event subscriptions
        OnRespawnStart += () => kartParent.SetActive(false);
        OnRespawnEnd += () => kartParent.SetActive(true);

        OnRespawnStart += () => player.StunPlayer(respawnTime + viewTime);

        // object refs
        soundPool = playerController.GetComponent<SoundPool>();
    }

    private void OnDisable()
    {
        OnRespawnStart -= () => kartParent.SetActive(false);
        OnRespawnEnd -= () => kartParent.SetActive(true);

        OnRespawnStart -= () => player.StunPlayer(respawnTime + viewTime);
    }

    private void Update()
    {
        // hotkey functionality
        if(Input.GetKeyDown(KeyCode.R))
        {
            StartRespawnCoroutine();
        }

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
        RespawnPoint rsp = GetLegalRSP(lastGroundedPos); // get the RSP
        player.SetKartRotation(rsp.Facing);
        Instantiate(killVFX, this.transform.position, Quaternion.Euler(rsp.Facing.x, rsp.Facing.y + 90f, rsp.Facing.z));
        soundPool.PlaySound("orion_death", this.transform.position);

        float elapsedTime = 0;
        Vector3 deathPos = transform.position;

        // wait while the player "views" their kill vfx
        while(elapsedTime < viewTime)
        {
            elapsedTime += Time.deltaTime;
            transform.position = deathPos;
            yield return null;
        }
        
        sc.enabled = false;
        elapsedTime = 0;
        // raise the wisp above the water
        while (elapsedTime < respawnTime)
        {
            transform.position = Vector3.Lerp(deathPos, rsp.transform.position, elapsedTime/respawnTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rsp.InitPoint();

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
        OnRespawnStart?.Invoke();
        isRespawning = true;

        // Turning these off fixes camera jittering on respawn
        rb.velocity = Vector3.zero; // set velocity to 0 on respawn
        rb.useGravity = false;
        //sc.enabled = false;

        if (respawnCoroutine == null)
        {
            respawnCoroutine = RespawnPlayer();
        }

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
    }

    private RespawnPoint GetLegalRSP(Vector3 lastGrounded)
    {
        if (legalRSPs.Length == 0)
        {
            return null;
        }

        float minDist = Vector3.Distance(lastGrounded, legalRSPs[0].PlayerSpawn);
        int rspIndex = 0;
        for (int i = 1; i < legalRSPs.Length; i++)
        {
            if (legalRSPs[i].InUse) { continue; }
            float newDist = Vector3.Distance(lastGrounded, legalRSPs[i].PlayerSpawn);
            if (newDist < minDist)
            {
                rspIndex = i;
                minDist = newDist;
            }
        }
        return legalRSPs[rspIndex];
    }

    public void AssignRSPs(RespawnPoint[] inRSPs)
    {
        legalRSPs = inRSPs;
    }
}

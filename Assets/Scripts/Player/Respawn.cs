using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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

    [Header("Respawn Stats")]
    [SerializeField] private float respawnTime = 2f;

    private Vector3 lastGroundedPos; // last position the player was grounded at

    private bool isRespawning;
    public bool IsRespawning { get { return isRespawning; } }

    private Rigidbody rb;
    private SphereCollider sc;

    // new player information MKFG
    BallDrivingVersion1 player;

    private RespawnPoint[] legalRSPs;

    public RespawnPoint[] LegalRSPs { get { return legalRSPs; } set { legalRSPs = value; } }

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
        //rsp.InUse = true;
        float elapsedTime = 0;
        Vector3 deathPos = transform.position;

        // raise the wisp above the water
        while (elapsedTime < respawnTime)
        {
            transform.position = Vector3.Lerp(deathPos, rsp.transform.position, elapsedTime/respawnTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.SetKartRotation(rsp.Facing);
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
        isRespawning = true;

        // Turning these off fixes camera jittering on respawn
        rb.velocity = Vector3.zero; // set velocity to 0 on respawn
        rb.useGravity = false;
        sc.enabled = false;

        if (respawnCoroutine == null)
        {
            respawnCoroutine = RespawnPlayer();
        }

        StartCoroutine(respawnCoroutine);
    }

    private void StopRespawnCoroutine()
    {
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

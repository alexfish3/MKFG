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

    [Header("Respawn Stats")]
    [SerializeField] private float respawnTime = 2f;

    private Vector3 lastGroundedPos; // last position the player was grounded at
    public Vector3 LastGroundedPos { get { return lastGroundedPos; } set { lastGroundedPos = value; } }

    private bool isRespawning;
    public bool IsRespawning { get { return isRespawning; } }

    private Rigidbody rb;
    private SphereCollider sc;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            StartRespawnCoroutine();
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sc = GetComponent<SphereCollider>();
        lastGroundedPos = transform.position;
    }

    /// <summary>
    /// The new respawning player coroutine. Simplified from the old version, rotates the player 180 degrees and positions them at their respawn point. Also creates a tombstone.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RespawnPlayer()
    {
        RespawnPoint rsp = RespawnManager.Instance.GetRespawnPoint(lastGroundedPos); // get the RSP
        rsp.InUse = true;
        float elapsedTime = 0;
        Vector3 deathPos = transform.position;

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
}

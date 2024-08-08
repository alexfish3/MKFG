///
/// Created by Alex Fischer and Max Moverley | August 2024
/// 

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
    [Header("Important Information")]
    [SerializeField] private PlayerMain player;
    [SerializeField] GameObject drivingMenu;
    [SerializeField] private PlacementHandler placement;
    [SerializeField] private Camera playerCam;
    private Canvas mainCanvas;

    [Header("Status Indicator Information")]
    [SerializeField] List<PlayerMain> playersToKeepTrackOf;
    [SerializeField] List<StatusIndicatorUI> indicatorsForPlayers;
    [SerializeField] GameObject indicatorParent;
    [SerializeField] GameObject indicatorPrefab;

    [Space(10)]
    [SerializeField] Sprite[] cooldownImages;

    [Header("Distance")]
    [SerializeField] float statusLerpSpeed;
    [SerializeField] float distanceScale = 15f;
    [SerializeField] float changeRate = 0.5f;
    [SerializeField] Vector2 sizeValues;
    [SerializeField] float cutoffValue;

    Plane[] cameraFrustum;
    Collider playersCollider;
    MeshRenderer playerRenderer;

    [Header("Placement")]
    [SerializeField] private TextMeshProUGUI lap;
    [SerializeField] private TextMeshProUGUI dir;
    [SerializeField] private TextMeshProUGUI place;

    [Header("Health")]
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI realHealth;
    [SerializeField] private TextMeshProUGUI fwdSpeed;

    [Header("Cooldowns")]
    [SerializeField] private TextMeshProUGUI neutral;
    [SerializeField] private TextMeshProUGUI forward;
    [SerializeField] private TextMeshProUGUI back;
    [SerializeField] private TextMeshProUGUI side;

    [Header("Pause")]
    [SerializeField] bool tryingToPause;
    bool tryingToPauseCache;
    public bool TryingToPause {  get { return tryingToPause; } set { tryingToPause = value; } }
    [SerializeField] HoldRingUI holdRing;

    public TextMeshProUGUI Lap { get { return lap; } }
    public TextMeshProUGUI Dir { get {  return dir; } }
    public TextMeshProUGUI Place { get { return place; } }
    public TextMeshProUGUI Health { get {  return health; } }
    public TextMeshProUGUI FwdSpeed { get { return fwdSpeed; } }

    private void OnEnable()
    {
        mainCanvas = GetComponent<Canvas>();

        holdRing.OnFillRing += () => { holdRing.SetFillZero(); player.TriggerPause(); drivingMenu.SetActive(false); };

        GameManagerNew.Instance.OnSwapMainLoop += InitalizeStatusIndicators;

    }
    private void OnDisable()
    {
        holdRing.OnFillRing -= () => { holdRing.SetFillZero(); player.TriggerPause(); drivingMenu.SetActive(false); };

        GameManagerNew.Instance.OnSwapMainLoop -= InitalizeStatusIndicators;
    }

    private void FixedUpdate()
    {
        if (tryingToPause == true)
        {
            tryingToPauseCache = true;
            holdRing.TickFill();
        }
        else if (tryingToPause == false && tryingToPauseCache == true)
        {
            tryingToPauseCache = false;
            holdRing.SetFillZero();
        }

        #region Text
        if (!placement.IsFinished)
        {
            lap.text = $"Lap: {placement.Lap}/{CheckpointManager.Instance.TotalLaps}";
            place.text = $"Placement: {placement.Placement}";
        }
        else
        {
            lap.text = "";
            place.text = $"Finished In: {placement.Placement}";
        }

        health.text = $"Health: {Mathf.RoundToInt(player.GetHealthMultiplier()*100)}%";
        realHealth.text = $"Real Health: {Mathf.RoundToInt(player.damageHealthMultiplier * 100)}%";

        //neutral.text = player.neutralSpecialCooldownTimer > 0 ? "NC: " + player.neutralSpecialCooldownTimer.ToString() : "";
        //forward.text = player.forwardSpecialCooldownTimer > 0 ? "FC: " + player.forwardSpecialCooldownTimer.ToString() : "";
        //back.text = player.backSpecialCooldownTimer > 0 ? "BC: " + player.backSpecialCooldownTimer.ToString() : "";
        //side.text = player.sideSpecialCooldownTimer > 0 ? "SC: " + player.sideSpecialCooldownTimer.ToString() : "";

        #endregion

        // Loop for ui objects
        for (int i = 0; i < playersToKeepTrackOf.Count; i++)
        {
            PlayerMain currentPlayerBeingChecked = playersToKeepTrackOf[i];
            StatusIndicatorUI currentStatusIndicator = indicatorsForPlayers[i].GetComponent<StatusIndicatorUI>();

            // Only calculates if not self ui
            if (i != player.GetBodyPlayerID() | i == player.GetBodyPlayerID())
            {
                playersCollider = currentPlayerBeingChecked.kart.GetComponent<Collider>();
                cameraFrustum = GeometryUtility.CalculateFrustumPlanes(playerCam);
                 
                // If player is on screen, render the UI
                if (GeometryUtility.TestPlanesAABB(cameraFrustum, playersCollider.bounds))
                {
                    currentStatusIndicator.gameObject.SetActive(true);
                    // Track players and transform
                    Vector3 screenPoint = playerCam.WorldToScreenPoint(currentPlayerBeingChecked.kart.transform.position);
                    Vector2 result;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.GetComponent<RectTransform>(), screenPoint, playerCam, out result);

                    Vector2 currentPosition = indicatorsForPlayers[i].GetComponent<RectTransform>().anchoredPosition;
                    indicatorsForPlayers[i].GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(currentPosition, result, statusLerpSpeed);

                    // Handles scale
                    float distance = Vector3.Distance(player.kart.transform.position, currentPlayerBeingChecked.kart.transform.position);
                   
                    if (distance > cutoffValue)
                    {
                        currentStatusIndicator.SetLocalScale(0);
                    }
                    else
                    {
                        float sizeValue = RangeMutations.Map_Linear((distance / distanceScale), sizeValues.x, sizeValues.y, 1, cutoffValue);

                        //float sizeValue = Mathf.Clamp((distance / distanceScale) * changeRate, sizeValues.x, sizeValues.y);
                        currentStatusIndicator.SetLocalScale(sizeValue);
                    }

                }
                // Hide UI if player is off screen
                else
                {
                    currentStatusIndicator.gameObject.SetActive(false);
                }
            }

            // Update Visuals
            SetSpeedColorOnInstances(Mathf.RoundToInt(currentPlayerBeingChecked.GetHealthMultiplier() * 100), currentPlayerBeingChecked);

            SetEnableOnInstances(!currentPlayerBeingChecked.placementHandler.gameObject.GetComponent<Respawn>().IsRespawning, currentPlayerBeingChecked);

            if (currentPlayerBeingChecked.sideSpecialCooldownTimer > 0)
            {
                SetCooldownCounterOnInstances(0, currentPlayerBeingChecked);
            }

            if (currentPlayerBeingChecked.forwardSpecialCooldownTimer > 0)
            {
                SetCooldownCounterOnInstances(1, currentPlayerBeingChecked);
            }

            if (currentPlayerBeingChecked.backSpecialCooldownTimer > 0)
            {
                SetCooldownCounterOnInstances(2, currentPlayerBeingChecked);
            }

            if (currentPlayerBeingChecked.neutralSpecialCooldownTimer > 0)
            {
                SetCooldownCounterOnInstances(3, currentPlayerBeingChecked);
            }
        }
    }

    /// <summary>
    /// Initalizes status indcators for all players in game
    /// </summary>
    public void InitalizeStatusIndicators()
    {
        playersToKeepTrackOf = PlayerSpawnSystem.Instance.GetSpawnedBodies();

        // Loops through and initalizes ui for each player
        foreach (PlayerMain currentCheckedPlayer in playersToKeepTrackOf)
        {
            StatusIndicatorUI tempIndicator = Instantiate(indicatorPrefab, indicatorParent.transform).GetComponent<StatusIndicatorUI>();

            tempIndicator.SetTrackedPlayerId(currentCheckedPlayer.GetBodyPlayerID());

            List<LightAttack> specialsInfo = new List<LightAttack>(currentCheckedPlayer.specialsInfo);

            Debug.Log("Initalizing player " + currentCheckedPlayer.specialsInfo.Length);

            tempIndicator.InitalizeCooldowns(specialsInfo, cooldownImages);
            tempIndicator.SetTeamColor(currentCheckedPlayer.GetBodyTeamColor());

            indicatorsForPlayers.Add(tempIndicator);
        }

        // Loops and finds ui for main player and sets that as last sibling to render ontop
        foreach (StatusIndicatorUI indicators in indicatorsForPlayers)
        {
            if(indicators.GetTrackedPlayerId() == player.GetBodyPlayerID())
            {
                indicators.gameObject.transform.SetAsLastSibling();
                indicators.gameObject.transform.position += new Vector3(0, 10, 0);
            }
        }
    }

    /// <summary>
    /// Updates the cooldown counters on set status indicators
    /// </summary>
    /// <param name="specialType">The type of special attack</param>
    /// <param name="playerToUpdate">Used to upadate spesific countdowns</param>
    public void SetCooldownCounterOnInstances(int specialType, PlayerMain playerToUpdate)
    {
        // Loops and finds ui for main player and sets that as last sibling to render ontop
        foreach (StatusIndicatorUI indicators in indicatorsForPlayers)
        {
            if (indicators.GetTrackedPlayerId() == playerToUpdate.GetBodyPlayerID())
            {
                indicators.SetSpecialCooldownStatus(specialType);
            }
        }
    }

    /// <summary>
    /// Updates the speed color on set status indicators
    /// </summary>
    /// <param name="speed">The speed of the player</param>
    /// <param name="playerToUpdate">Used to upadate spesific countdowns</param>
    public void SetSpeedColorOnInstances(float speed, PlayerMain playerToUpdate)
    {
        foreach (StatusIndicatorUI instance in indicatorsForPlayers)
        {
            if (instance.GetTrackedPlayerId() == playerToUpdate.GetBodyPlayerID())
            {
                instance.SetSpeedColorHealth(speed);
            }
        }
    }

    public void SetEnableOnInstances(bool enableStatus, PlayerMain playerToUpdate)
    {
        foreach (StatusIndicatorUI instance in indicatorsForPlayers)
        {
            if (instance.GetTrackedPlayerId() == playerToUpdate.GetBodyPlayerID())
            {
                instance.SetEnable(enableStatus);
            }
        }
    }
}

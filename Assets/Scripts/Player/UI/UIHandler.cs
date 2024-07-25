using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
    [Header("Important Information")]
    [SerializeField] private PlayerMain player;
    [SerializeField] private PlacementHandler placement;
    [SerializeField] private Camera playerCam;
    private Canvas mainCanvas;

    [Header("Placement")]
    [SerializeField] private TextMeshProUGUI lap;
    [SerializeField] private TextMeshProUGUI dir;
    [SerializeField] private TextMeshProUGUI place;

    [Header("Health")]
    [SerializeField] private TextMeshProUGUI health;
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

        holdRing.OnFillRing += () => { holdRing.SetFillZero(); player.TriggerPause(); };
    }

    private void OnDisable()
    {
        holdRing.OnFillRing -= () => { holdRing.SetFillZero(); player.TriggerPause(); };
    }

    private void Update()
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

        neutral.text = player.neutralSpecialCooldownTimer > 0 ? "NC: " + player.neutralSpecialCooldownTimer.ToString() : "";
        forward.text = player.forwardSpecialCooldownTimer > 0 ? "FC: " + player.forwardSpecialCooldownTimer.ToString() : "";
        back.text = player.backSpecialCooldownTimer > 0 ? "BC: " + player.backSpecialCooldownTimer.ToString() : "";
        side.text = player.sideSpecialCooldownTimer > 0 ? "SC: " + player.sideSpecialCooldownTimer.ToString() : "";
        #endregion
    }
}

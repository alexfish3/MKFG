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

    [Header("Arrow Stuff")]
    [SerializeField] private Camera uiCam;
    [SerializeField] private Image[] arrows;
    private Transform[] playerArrowPositions;// = new List<Transform>();
    private bool trackArrows;

    [Header("Text Refs")]
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

    public TextMeshProUGUI Lap { get { return lap; } }
    public TextMeshProUGUI Dir { get {  return dir; } }
    public TextMeshProUGUI Place { get { return place; } }
    public TextMeshProUGUI Health { get {  return health; } }
    public TextMeshProUGUI FwdSpeed { get { return fwdSpeed; } }

    private void OnEnable()
    {
        GameManagerNew.Instance.OnSwapLoadMatch += InitArrows;
        mainCanvas = GetComponent<Canvas>();
    }

    private void OnDisable()
    {
        GameManagerNew.Instance.OnSwapLoadMatch -= InitArrows;
    }

    private void Update()
    {
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

  /*      if(trackArrows)
        {
            for(int i=0;i<PlayerList.Instance.spawnedBodyCount;i++)
            {
                //Vector3 arrowScreenPoint = RectTransformUtility.WorldToScreenPoint(uiCam, playerArrowPositions[i].position);

                Vector3 arrowScreenPoint = uiCam.WorldToScreenPoint(playerArrowPositions[i].position);

                RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvas.GetComponent<RectTransform>(), arrowScreenPoint, uiCam, out Vector2 arrowScreenPosition);

                arrows[i].rectTransform.anchoredPosition3D = arrowScreenPosition;

                // yikes
                *//*Vector3 position = uiCam.WorldToScreenPoint(arrows[i].transform.position);
                position.z = (mainCanvas.transform.position - uiCam.transform.position).magnitude;
                arrows[i].transform.position = playerCam.ScreenToWorldPoint(position);*/

                /*Vector3 viewport = uiCam.WorldToViewportPoint(playerArrowPositions[i].position);
                Ray canvasRay = mainCanvas.worldCamera.ViewportPointToRay(viewport);

                arrows[i].transform.position = canvasRay.GetPoint(mainCanvas.planeDistance);*/

                /*Canvas c;
                arrows[i].transform.position = c.(playerArrowPositions[i].position);*/

                /*Vector3 screen = playerCam.WorldToScreenPoint(playerArrowPositions[i].transform.position);
                screen.z = (mainCanvas.transform.position - uiCam.transform.position).magnitude;
                Vector3 position = uiCam.ScreenToWorldPoint(screen);*//*
                arrows[i].transform.position = RectTransformUtility.WorldToScreenPoint(uiCam, playerArrowPositions[i].position);//position; // element is the Text show in the UI.*//*
            }
        }*/
    }

    private void InitArrows()
    {
        return; 
        //playerArrowPositions = PlayerList.Instance.UIArrows;
        //playerArrowPositions.Remove(player.GetArrowPosition());

        /*for(int i=0;i<PlayerList.Instance.spawnedBodyCount;i++)
        {
            if (playerArrowPositions[i] != player.GetArrowPosition())
                arrows[i].enabled = true;
        }

        trackArrows = true;*/
    }
}

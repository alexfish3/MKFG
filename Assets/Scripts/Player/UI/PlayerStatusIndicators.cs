using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusIndicators : MonoBehaviour
{
    [SerializeField] GameObject thisPlayerGameobject;
    [SerializeField] GameObject[] playersToKeepTrackOf;
    [SerializeField] Transform[] playerCameraTransforms;
    [SerializeField] GameObject[] playersRotationObjects;

    [Header("Rotation Sprite Options")]
    [SerializeField] SpriteRenderer[] rotationSprites;
    [SerializeField] Sprite[] spriteOptions;

    [Header("Distance")]
    [SerializeField] float distanceScale = 15f;
    [SerializeField] Vector2 sizeValues;

    // Update is called once per frame
    void Update()
    {
        // Loops for all players in the game
        for (int i = 0; i <= playersToKeepTrackOf.Length - 1; i++)
        {
            if (playersToKeepTrackOf[i] == null)
                continue;

            // Handles rotation
            playersRotationObjects[i].transform.rotation = Quaternion.Euler(new Vector3(
                playerCameraTransforms[i].eulerAngles.x,
                playerCameraTransforms[i].eulerAngles.y,
                playerCameraTransforms[i].eulerAngles.z));

            // Handles scale
            float distance = Vector3.Distance(thisPlayerGameobject.transform.position, playersToKeepTrackOf[i].transform.position);
            float sizeValue = Mathf.Clamp(distance / distanceScale, sizeValues.x, sizeValues.y);
            playersRotationObjects[i].transform.localScale = new Vector3(sizeValue, sizeValue, sizeValue);
        }
    }

    public void UpdatePlayerReferencesForObjects()
    {
        // Sets all to false
        foreach (GameObject rotationObject in playersRotationObjects)
        {
            rotationObject.SetActive(false);
        }

        // Loops and adds player references
        playersToKeepTrackOf = new GameObject[4];
        playerCameraTransforms = new Transform[4];

        //for (int i = 0; i < playerInstantiate.PlayerInputs.Length; i++)
        //{
        //    PlayerInput playerInput = playerInstantiate.PlayerInputs[i];

        //    if (playerInput != null && playerInput != thisPlayer)
        //    {
        //        playersToKeepTrackOf[i] = playerInput.gameObject.GetComponentInChildren<BallDriving>().gameObject;

        //        playerCameraTransforms[i] = playerInput.gameObject.GetComponent<PlayerCameraResizer>().PlayerReferenceCamera.transform;

        //        playersRotationObjects[i].SetActive(true);

        //        List<GameObject> needToSwitch = new List<GameObject>
        //        {
        //            playersRotationObjects[i],
        //            playersRotationObjects[i].transform.GetChild(0).gameObject
        //        };

        //        PlayerCameraResizer.UpdatePlayerObjectLayer(needToSwitch, i, iconCamera);
        //    }
        //}
    }

    //public void SetRotationSprites(IndicatorStatus indicatorStatus)
    //{
    //    switch (indicatorStatus)
    //    {
    //        case IndicatorStatus.HoldingNothing:
    //            SetRotationSpritesSprite(spriteOptions[0]);
    //            break;
    //        case IndicatorStatus.HoldingEasy:
    //            SetRotationSpritesSprite(spriteOptions[1]);
    //            break;
    //        case IndicatorStatus.HoldingMedium:
    //            SetRotationSpritesSprite(spriteOptions[2]);
    //            break;
    //        case IndicatorStatus.HoldingHard:
    //            SetRotationSpritesSprite(spriteOptions[3]);
    //            break;
    //        case IndicatorStatus.HoldingGolden:
    //            SetRotationSpritesSprite(spriteOptions[4]);
    //            break;
    //        default:
    //            break;
    //    }
    //}

    public void SetRotationSpritesSprite(Sprite spriteToSet)
    {
        foreach (SpriteRenderer renderer in rotationSprites)
        {
            renderer.sprite = spriteToSet;
        }
    }
}

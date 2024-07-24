using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusIndicators : MonoBehaviour
{
    [SerializeField] GameObject thisPlayerGameobject;
    [SerializeField] Transform[] playerCameraTransforms;
    [SerializeField] GameObject[] playersRotationObjects;

    [Header("Rotation Sprite Options")]
    [SerializeField] SpriteRenderer[] rotationSprites;
    [SerializeField] Sprite[] spriteOptions;

    [Header("Distance")]
    [SerializeField] float distanceScale = 15f;
    [SerializeField] Vector2 sizeValues;

    int counter = 0;
    // Update is called once per frame
    void Update()
    {
        counter = 0;
        foreach(KeyValuePair<GenericBrain, PlayerMain> spawnedPlayers in PlayerSpawnSystem.Instance.GetSpawnedBodies())
        {
            GameObject currentPlayerBeingChecked = spawnedPlayers.Value.ballDriving.gameObject;

            // Handles rotation
            playersRotationObjects[counter].transform.rotation = Quaternion.Euler(new Vector3(
                currentPlayerBeingChecked.transform.eulerAngles.x,
                currentPlayerBeingChecked.transform.eulerAngles.y,
                currentPlayerBeingChecked.transform.eulerAngles.z));

            // Handles scale
            float distance = Vector3.Distance(thisPlayerGameobject.transform.position, currentPlayerBeingChecked.transform.position);
            float sizeValue = Mathf.Clamp(distance / distanceScale, sizeValues.x, sizeValues.y);
            playersRotationObjects[counter].transform.localScale = new Vector3(sizeValue, sizeValue, sizeValue);

            counter++;
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
        //playersToKeepTrackOf = new GameObject[4];
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

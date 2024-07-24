using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

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
    [SerializeField] float cutoffValue;

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

            if(distance > cutoffValue)
            {
                playersRotationObjects[counter].transform.localScale = new Vector3(0, 0, 0);
            }
            else
            {
                float sizeValue = Mathf.Clamp(distance / distanceScale, sizeValues.x, sizeValues.y);
                playersRotationObjects[counter].transform.localScale = new Vector3(sizeValue, sizeValue, sizeValue);
            }

            counter++;
        }
    }

    public void SetColorOfPlayerArrows(Color teamColor)
    {
        foreach (GameObject rotationParent in playersRotationObjects)
        {
            rotationParent.SetActive(true);
        }

        foreach (SpriteRenderer renderer in rotationSprites)
        {
            renderer.color = teamColor;
        }
    }
}

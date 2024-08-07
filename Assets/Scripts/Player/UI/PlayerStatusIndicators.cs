using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerStatusIndicators : MonoBehaviour
{
    [SerializeField] GameObject thisPlayerGameobject;
    [SerializeField] PlayerMain playerMain;
    [SerializeField] GameObject[] playersRotationObjects;
    [SerializeField] StatusIndicatorInstance[] statusIndicatorInstances;

    [Header("Distance")]
    [SerializeField] float distanceScale = 15f;
    [SerializeField] Vector2 sizeValues;
    [SerializeField] float cutoffValue;

    int counter = 0;
    // Update is called once per frame
    void Update()
    {
        counter = 0;

        SetSpeedColorOnInstances(Mathf.RoundToInt(playerMain.GetHealthMultiplier() * 100));

        foreach (PlayerMain spawnedPlayers in PlayerSpawnSystem.Instance.GetSpawnedBodies())
        {
            GameObject currentPlayerBeingChecked = spawnedPlayers.ballDriving.gameObject;
            StatusIndicatorInstance currentStatusIndicator = statusIndicatorInstances[counter];

            // Handles rotation
            playersRotationObjects[counter].transform.rotation = Quaternion.Euler(new Vector3(
                currentPlayerBeingChecked.transform.eulerAngles.x,
                currentPlayerBeingChecked.transform.eulerAngles.y,
                currentPlayerBeingChecked.transform.eulerAngles.z));

            // Handles scale
            float distance = Vector3.Distance(thisPlayerGameobject.transform.position, currentPlayerBeingChecked.transform.position);

            if(distance > cutoffValue)
            {
                currentStatusIndicator.SetLocalScale(0);
            }
            else
            {
                float sizeValue = Mathf.Clamp(distance / distanceScale, sizeValues.x, sizeValues.y);
                currentStatusIndicator.SetLocalScale(sizeValue);
            }

            counter++;
        }
    }

    public void SetSpeedColorOnInstances(float speed)
    {
        foreach (StatusIndicatorInstance instance in statusIndicatorInstances)
        {
            instance.SetSpeedColorHealth(speed);
        }
    }

    public void SetTeamColorOnInstances(Color teamColor)
    {
        foreach (StatusIndicatorInstance instance in statusIndicatorInstances)
        {
            instance.SetTeamColor(teamColor);
        }
    }
}

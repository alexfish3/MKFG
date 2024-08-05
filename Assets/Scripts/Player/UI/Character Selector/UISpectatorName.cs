using UnityEngine;
using TMPro;

public class UISpectatorName : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] GenericBrain playerBrain;

    public GenericBrain PlayerBrain { get { return playerBrain; } }

    public void InitalizeUISpectatorName(GenericBrain genericBrain)
    {
        playerBrain = genericBrain;
        nameText.text = genericBrain.GetPlayerUsername();
    }
}

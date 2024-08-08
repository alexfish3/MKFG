using UnityEngine;
using UnityEngine.UI;

public class HelperButtonUI : MonoBehaviour
{
    [SerializeField] bool displayHold;
    [SerializeField] GameObject helperText;
    [SerializeField] GameObject holdText;
    [SerializeField] Image buttonImage;
    [SerializeField] Sprite controllerFace;
    [SerializeField] Sprite keyboardFace;

    public void SetUIIconToMatchBrainType(InputType inputType)
    {
        helperText.SetActive(true);
        buttonImage.gameObject.SetActive(true);

        if(displayHold)
            holdText.SetActive(true);

        switch (inputType)
        {
            case InputType.DLLKeyboard:
                buttonImage.sprite = keyboardFace;
                return;
            case InputType.UnityKeyboard:
                buttonImage.sprite = keyboardFace;
                return;
            case InputType.UnityController:
                buttonImage.sprite = controllerFace;
                return;
        }
    }
}

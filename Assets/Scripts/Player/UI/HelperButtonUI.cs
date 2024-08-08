using UnityEngine;
using UnityEngine.UI;

public class HelperButtonUI : MonoBehaviour
{
    [SerializeField] GameObject Text;
    [SerializeField] Image buttonImage;
    [SerializeField] Sprite controllerFace;
    [SerializeField] Sprite keyboardFace;

    public void SetUIIconToMatchBrainType(InputType inputType)
    {
        Text.SetActive(true);
        buttonImage.gameObject.SetActive(true);

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

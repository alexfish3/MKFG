
using UnityEngine;
using UnityEngine.SceneManagement;
using Udar.SceneManager;

public class DebugControlSwitcher : MonoBehaviour
{
    GenericBrain genericBrain;
    [SerializeField] ControlProfile newControlProfile;
    [SerializeField] string sceneToSwapToDriving;

    bool initalized = false;

    // Start is called before the first frame update
    void Update()
    {
        if (initalized)
            return;

        genericBrain = GetComponent<GenericBrain>();
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == sceneToSwapToDriving)
        {
            initalized = true;
            genericBrain.controlProfileSerialize = newControlProfile;
        }
    }
}

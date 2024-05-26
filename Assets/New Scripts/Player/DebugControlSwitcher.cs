using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using Udar.SceneManager;

public class DebugControlSwitcher : MonoBehaviour
{
    GenericBrain genericBrain;
    [SerializeField] ControlProfile newControlProfile;
    [SerializeField] SceneField sceneToSwapToDriving;

    // Start is called before the first frame update
    void Start()
    {
        genericBrain = GetComponent<GenericBrain>();
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == sceneToSwapToDriving.Name)
        {
            genericBrain.controlProfileSerialize = newControlProfile;
        }
    }
}

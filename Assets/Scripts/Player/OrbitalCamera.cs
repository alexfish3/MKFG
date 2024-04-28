using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OrbitalCamera : MonoBehaviour
{
    [SerializeField] InputManager inputManager;

    [Header("Horizontal Movement")]
    [SerializeField] CinemachineVirtualCamera virtualCameraMain;
    [SerializeField] CinemachineVirtualCamera virtualCameraIcon;
    [SerializeField] CinemachineOrbitalTransposer mainOrb;
    [SerializeField] CinemachineOrbitalTransposer iconOrb;
    [SerializeField] float maxXAngle = 180;
    [SerializeField] float smoothSpeedValue = 0.1f;
    [SerializeField] float realXAxis;
    public float smoothXAxis;

    [Header("Vertical Movement")]
    [SerializeField] GameObject CameraFocus;
    [SerializeField] Vector2 yAngleMinMax;
    [SerializeField] float realYAxis;
    public float smoothYAxis;

    [Header("FOV")]
    [SerializeField] float fovValue = 60f;
    public float passInFOV;
    public float maxFOV = 80;
    [SerializeField] float changeSpeed;

    bool reverseCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainOrb = virtualCameraMain.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        iconOrb = virtualCameraIcon.GetCinemachineComponent<CinemachineOrbitalTransposer>();
    }

    // Update is called once per frame
    void Update()
    {
        fovValue = Mathf.Lerp(fovValue, passInFOV, changeSpeed);

        // Custom joystick camera aim
        if (!inputManager.RightStickValue && reverseCamera == false)
        {
            realXAxis = RangeMutations.Map_Linear(inputManager.RightStickXValue, -1, 1, -maxXAngle, maxXAngle);
            realYAxis = RangeMutations.Map_Linear(inputManager.RightStickYValue, -1, 1, yAngleMinMax.x, yAngleMinMax.y);

            smoothXAxis = Mathf.Lerp(smoothXAxis, realXAxis, smoothSpeedValue);
            smoothYAxis = Mathf.Lerp(smoothYAxis, realYAxis, smoothSpeedValue);

            mainOrb.m_XAxis.Value = smoothXAxis;
            iconOrb.m_XAxis.Value = smoothXAxis;

            CameraFocus.transform.localPosition = new Vector3(CameraFocus.transform.localPosition.x, smoothYAxis, CameraFocus.transform.localPosition.z);

            virtualCameraMain.m_Lens.FieldOfView = fovValue;
            virtualCameraIcon.m_Lens.FieldOfView = fovValue;

        }
        // Reset look behind
        if (!inputManager.RightStickValue && reverseCamera == true)
        {
            reverseCamera = false;
            smoothXAxis = 0f;
        }
        // Static look behind
        else if(inputManager.RightStickValue)
        {
            reverseCamera = true;

            mainOrb.m_XAxis.Value = -180;
            iconOrb.m_XAxis.Value = -180;

            realXAxis = 180;
            realYAxis = 0;
            
            smoothYAxis = 0;
            smoothXAxis = 180f;

            virtualCameraMain.m_Lens.FieldOfView = 60;
            virtualCameraIcon.m_Lens.FieldOfView = 60;

            CameraFocus.transform.localPosition = new Vector3(CameraFocus.transform.localPosition.x, smoothYAxis, CameraFocus.transform.localPosition.z);
        }
    }

    public IEnumerator SetFOVAfterTime(float newFOVValue, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        passInFOV = newFOVValue;
    } 
}

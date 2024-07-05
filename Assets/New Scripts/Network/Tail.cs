using UnityEngine;

public class Tail : MonoBehaviour
{
    public Transform networkedOwner;
    public Transform followTransform;
    [SerializeField] float delayTime = 0.1f;
    [SerializeField] float distance = 0.3f;
    [SerializeField] float moveStep = 10f;

    private Vector3 targetPosition;

    private void Update()
    {
        targetPosition = followTransform.position - followTransform.forward * distance;
        targetPosition += (transform.position - targetPosition) * delayTime;
        targetPosition.z = 0f;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveStep);
    }
}

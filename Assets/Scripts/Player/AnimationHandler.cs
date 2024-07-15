using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    // animation refs
    [SerializeField] private Animator animator;
    
    // other GO refs
    private BallDrivingVersion1 ballDriving;
    private TauntHandler tauntHandler;

    private void OnEnable()
    {
        ballDriving = GetComponent<BallDrivingVersion1>();
        tauntHandler = GetComponent<TauntHandler>();

        tauntHandler.TauntPerformed += StartTauntAnimation;
    }

    private void OnDisable()
    {
        tauntHandler.TauntPerformed += StartTauntAnimation;
    }

    private void StartTauntAnimation()
    {
        Debug.Log("SPIN");
        animator.SetTrigger("Taunt");
    }
}

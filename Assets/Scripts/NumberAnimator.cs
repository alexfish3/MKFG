using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberAnimator : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] Image numberImage;
    [SerializeField] Animator animator;

    public void AnimateNumberIn(int number)
    {
        // Return if the number to set is greater then bounds
        if (number < 0 | number > 9)
            return;

        numberImage.sprite = sprites[number];

        animator.SetTrigger("Grow");
    }

    public void AnimateNumberOut()
    {
        animator.SetTrigger("Shrink");
    }
}

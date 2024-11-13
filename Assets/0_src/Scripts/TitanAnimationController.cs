using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TitanAnimationController : MonoBehaviour
{
    public Animator animator; // Reference to the Animator component controlling the character's animations
    public string defeatTrigger; // Name of the trigger for the defeat animation
    public string finalAttackTrigger; // Name of the trigger for the final attack animation
    public string outofViewTrigger;
    public string entranceTrigger; // Name of the trigger for the entrance animation
    public string entranceFinishedState; // Name of the state representing the end of the entrance animation
    public UnityEvent OnEntranceFinished; // Action to invoke when the entrance animation completes

    public void AnimateDefeat()
    {
        // Set the trigger for the defeat animation
        animator.SetTrigger(defeatTrigger);
    }

    public void AnimateFinalAttack()
    {
        // Set the trigger for the final attack animation
        animator.SetTrigger(finalAttackTrigger);
    }

    public void SetOutOfView(){
        animator.SetTrigger(outofViewTrigger);
    }

    public void AnimateEntrance()
    {
        // Set the trigger for the entrance animation and start a coroutine to monitor its progress
        animator.SetTrigger(entranceTrigger);
        StartCoroutine(WaitForEntranceAnimation());
    }

    private IEnumerator WaitForEntranceAnimation()
    {
        // Wait until the Animator enters the specific exit state for the entrance
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(entranceFinishedState))
        {
            yield return null; // Wait for the next frame before checking again
        }

        // Wait for the exit state animation to complete (normalizedTime reaches 1.0 when the animation finishes)
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null; // Continue waiting for the next frame until the animation is done
        }

        // Invoke the action to signal that the entrance animation has finished
        OnEntranceFinished?.Invoke();
    }
}

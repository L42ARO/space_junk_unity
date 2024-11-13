using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitIndicatorController : MonoBehaviour
{

    public Animator animator;
    public string hitTrigger = "TrBlind";
    public string bigHitTrigger = "TrBlind";
    public void IndicateHit(){
        animator.SetTrigger(hitTrigger);
        WaitAndDestroy();
    }
    public void IndicateBigHit(){
        animator.SetTrigger(bigHitTrigger);
        WaitAndDestroy();
    }
    //create functino to wait for animation to finish and then destroy the object
    IEnumerator WaitAndDestroy(){
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null; // Continue waiting for the next frame until the animation is done
        }
        Destroy(gameObject);
    }
}

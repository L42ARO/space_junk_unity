using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameStartScript : MonoBehaviour
{
    public Animator TitanAnimator;
    public string StartTrigger;

    public UnityAction ScriptEnded;

    public void StartScript(){
        TitanAnimator.SetTrigger(StartTrigger);
    }
}

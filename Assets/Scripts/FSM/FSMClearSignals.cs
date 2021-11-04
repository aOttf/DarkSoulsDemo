using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMClearSignals : StateMachineBehaviour
{
    public string[] clearAtEnter;   //Signals cleared when entering a state
    public string[] clearAtExit;    //Signals cleared when exiting a state

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var signal in clearAtEnter)
        {
            animator.ResetTrigger(signal);
            Debug.Log("Attack Reset");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var signal in clearAtExit)
        {
            animator.ResetTrigger(signal);
        }
    }
}
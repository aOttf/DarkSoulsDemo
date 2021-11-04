using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMSetNormalizedTime : StateMachineBehaviour
{
    public string targetParameter = "ntime";

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat(targetParameter, stateInfo.normalizedTime);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMOnEnterAndExit : StateMachineBehaviour
{
    public string[] OnEnterMsgs;
    public string[] OnExitMsgs;
    public string[] OnUpdateMsgs;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var msg in OnEnterMsgs)
        {
            animator.SendMessageUpwards(msg);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var msg in OnExitMsgs)
        {
            animator.SendMessageUpwards(msg);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var msg in OnUpdateMsgs)
        {
            animator.SendMessageUpwards(msg);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ExitState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("OnStateEnter - ExitState");
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
    {
        Debug.Log("OnStateEnter - ExitState - overload");
        base.OnStateEnter(animator, stateInfo, layerIndex, controller);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("FoundMerchantShip");
        Debug.Log("OnStateExit - ExitState");
        base.OnStateExit(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
    {
        Debug.Log("OnStateExit - ExitState - overload");
        base.OnStateExit(animator, stateInfo, layerIndex, controller);
    }
}

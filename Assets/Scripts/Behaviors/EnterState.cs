using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class EnterState : StateMachineBehaviour
{
    private float m_time = 2f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("OnStateEnter - EnterState");
        m_time = Random.Range(2f, 4f);
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
    {
        Debug.Log("OnStateEnter - EnterState - overload");
        base.OnStateEnter(animator, stateInfo, layerIndex, controller);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_time -= Time.deltaTime;
        if (m_time < 0)
        {
			Debug.Log("OnStateUpdate - FoundMerchantShip");
            animator.SetTrigger("FoundMerchantShip");
        }
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("OnStateExit - ExitState");
        base.OnStateExit(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
    {
        Debug.Log("OnStateExit - ExitState - overload");
        base.OnStateExit(animator, stateInfo, layerIndex, controller);
    }
}

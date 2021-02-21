	using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.AI;

public class GoToPoint : StateMachineBehaviour
{
    private Transform m_target;
    private Ship m_owner;
    private NavMeshAgent m_ownerNavMeshAgent;
    private float m_distanceThreshold;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator?.ResetTrigger("ReachedTarget");
        m_owner = animator.GetComponent<Ship>();

        if (m_owner == null)
        {
            Debug.LogError("Ship component not found");
        }

        m_ownerNavMeshAgent = m_owner.GetComponent<NavMeshAgent>();

        if (m_ownerNavMeshAgent == null)
        {
            Debug.LogError("Ship doesn't have nav mesh agent");
        }
        m_ownerNavMeshAgent.SetDestination(m_owner.m_targetDestination.position);
        m_distanceThreshold = m_owner.m_distanceToPointThreshold;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_ownerNavMeshAgent.remainingDistance <= m_distanceThreshold && !m_ownerNavMeshAgent.pathPending)
        {
			//Debug.Log(m_owner.name + " Has reached the current target and is continuing.");
            animator.SetTrigger("ReachedTarget");
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_owner.UpdatePath();
    }
}

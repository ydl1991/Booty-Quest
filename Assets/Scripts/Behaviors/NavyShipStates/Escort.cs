using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Escort : StateMachineBehaviour
{
	private Ship m_owner;
	private GameObject m_player;

	//OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Debug.Log("Navy Ship Begining Escort");
		m_owner = animator.GetComponent<Ship>();
		m_player = GameObject.FindGameObjectWithTag("Player");

		m_owner.GetComponent<NavMeshAgent>().isStopped = true;
	}

	//OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (Vector3.Distance(m_owner.transform.position, m_player.transform.position) < m_owner.m_detectionRadius)
		{
			animator.SetTrigger("PlayerIsSighted");
			return;
		}

		if (m_owner.m_escortee != null)
		{
			if (Vector3.Distance(m_owner.m_escortee.transform.position, m_owner.transform.position) >= m_owner.m_escortDistance)
			{
				// Formula for finding a point along a line between 2 points given a distance.
				Vector3 differenceVector = m_owner.transform.position - m_owner.m_escortee.transform.position;
				Vector3 unitVector = Vector3.Normalize(differenceVector);

				Vector3 newDestination = m_owner.m_escortee.transform.position + (m_owner.m_escortDistance) * unitVector;

				m_owner.GetComponent<NavMeshAgent>().isStopped = false;
				m_owner.GetComponent<NavMeshAgent>().SetDestination(newDestination);
				m_owner.m_targetDestination.position = newDestination;
			}
		}
		else
		{
			m_owner.GetComponent<NavMeshAgent>().isStopped = false;
			animator.SetTrigger("MerchantShipReachedDestination");
		}
	}

	//OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Debug.Log("Navy Ship Ending Escort");
	}
}

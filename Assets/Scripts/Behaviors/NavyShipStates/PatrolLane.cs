using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolLane : StateMachineBehaviour
{
	private Ship m_owner;

	private GameObject m_player;

	private int m_pathIncrementValue = 1;
	
	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Debug.Log("Navy Ship Begining Patrol");
		m_owner = animator.GetComponent<Ship>();
		m_player = GameObject.FindGameObjectWithTag("Player");

		m_owner.FindMerchantToEscort();

		// Find the nearest point from path to this ship and set it as the target.
		float shortestDistance = float.MaxValue;
		for (int i = 0; i < m_owner.m_pathToFollow.Length; ++i)
		{
			float dist = Vector3.Distance(m_owner.m_pathToFollow[i].position, m_owner.GetComponent<Transform>().position);
			if (dist < shortestDistance)
			{
				shortestDistance = dist;
				m_owner.m_targetDestination = m_owner.m_pathToFollow[i];
				m_owner.currentPathIndex = i;
			}
		}

		m_owner.GetComponent<NavMeshAgent>().SetDestination(m_owner.m_targetDestination.position);
	}

	//OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		float distanceToPirateIsland = Vector3.Distance(m_owner.transform.position, m_owner.m_pirateIslandTransform.position);
		if (distanceToPirateIsland > m_owner.m_distanceToPirateIslandThreshold)
		{
			animator.ResetTrigger("NearPirateIsland");
		}

		if (Vector3.Distance(m_owner.transform.position, m_player.transform.position) < m_owner.m_detectionRadius &&
			distanceToPirateIsland > m_owner.m_distanceToPirateIslandThreshold)
		{
			animator.SetTrigger("PlayerIsSighted");
			return;
		}

		// If the ship has reached it's destination, continue onwards to the next point.
		if (m_owner.GetComponent<NavMeshAgent>().remainingDistance <= m_owner.m_distanceToPointThreshold && !m_owner.GetComponent<NavMeshAgent>().pathPending)
		{
			Debug.Log("Navy Ship Reached First Point");

			// If this is the beginning of the path or the end of the path, then go backwards.
			if (m_owner.currentPathIndex + m_pathIncrementValue == m_owner.m_pathToFollow.Length || m_owner.currentPathIndex + m_pathIncrementValue == -1)
			{
				// Reverse the path increment value.
				m_pathIncrementValue *= -1;
			}

			m_owner.currentPathIndex += m_pathIncrementValue;
			m_owner.m_targetDestination = m_owner.m_pathToFollow[m_owner.currentPathIndex];
			m_owner.GetComponent<NavMeshAgent>().SetDestination(m_owner.m_targetDestination.position);
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Debug.Log("Navy Ship Ending Patrol");
	}
}
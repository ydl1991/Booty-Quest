using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Combat : StateMachineBehaviour
{
	private Ship m_owner;

	private GameObject m_player;
	
	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		m_owner = animator.GetComponent<Ship>();

		var players = GameObject.FindGameObjectsWithTag("Player");
		foreach(var player in players) 
		{
			if (player.GetComponent<HealthComponent>() != null)
			{
				m_player = player;
			}
		}
		
	}

	//OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		HealthComponent playerHealth = m_player.GetComponent<HealthComponent>();

		if (Vector3.Distance(m_owner.transform.position, m_owner.m_pirateIslandTransform.position) < m_owner.m_distanceToPirateIslandThreshold)
		{
			animator.SetTrigger("NearPirateIsland");
			animator.SetTrigger("CombatEnd");
		}

		if (playerHealth == null)
		{
			Debug.Log("Player health was null");
		}

		if ((playerHealth?.health ?? 0f) <= 0f)
		{
			animator.SetTrigger("CombatEnd");
			return;
		}

		if (Vector3.Distance(m_owner.transform.position, m_player.transform.position) > m_owner.m_combatDistance * 1.5f)
		{
			animator.SetTrigger("PlayerTooFarForCombat");
            return;
		}

		float cosAngle = Vector3.Dot(m_owner.transform.forward, m_player.transform.forward);
		var cannonComponent = m_owner.GetComponent<CannonComponent>();
		var navMeshAgent = m_owner.GetComponent<NavMeshAgent>();

		if (cannonComponent == null)
		{
			Debug.Log("Cannon component not found, nothing to fire");
			return;
		}

		float distance = Vector3.Distance(m_owner.transform.position, m_player.transform.position);
		if (distance > m_owner.m_combatDistance / 8) distance = m_owner.m_combatDistance / 8;

		var firstPoint = m_player.transform.position + m_player.transform.right * distance;
		var secondPoint = firstPoint - m_player.transform.right * distance;
		var closestPoint = (Vector3.SqrMagnitude(firstPoint - m_owner.transform.position) > Vector3.SqrMagnitude(secondPoint - m_owner.transform.position) ? secondPoint : firstPoint); 

		if (!cannonComponent.CanFire())
		{
			if  (navMeshAgent != null)
			{
				navMeshAgent.isStopped = false;
				navMeshAgent?.SetDestination(closestPoint);
			}

			return;
		}

		RaycastHit hit;
		if (cannonComponent.CanFireRight)
		{
			bool wasHit = Physics.Raycast(m_owner.transform.position, m_owner.transform.right, out hit, m_owner.m_combatDistance / 2, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);

			if (wasHit && hit.collider.gameObject.transform.parent.tag == "Player")
			{
				navMeshAgent?.ResetPath();
				navMeshAgent.isStopped = true;
				m_owner.GetComponent<CannonComponent>().FireRight(m_owner);
				return;
			}
		}

		if (cannonComponent.CanFireLeft)
		{
			bool wasHit = Physics.Raycast(m_owner.transform.position, -m_owner.transform.right, out hit, m_owner.m_combatDistance / 2, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
			if (wasHit && hit.collider.gameObject.transform.parent.tag == "Player")
			{
				navMeshAgent?.ResetPath();
				navMeshAgent.isStopped = true;
				m_owner.GetComponent<CannonComponent>().FireLeft(m_owner);
				return;
			}
		}

		navMeshAgent?.ResetPath();
		navMeshAgent.isStopped = false;
		navMeshAgent?.SetDestination(closestPoint);

		return;
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.ResetTrigger("CombatStart");
		animator.ResetTrigger("CombatEnd");
		m_owner.GetComponent<NavMeshAgent>().isStopped = false;
	}
}

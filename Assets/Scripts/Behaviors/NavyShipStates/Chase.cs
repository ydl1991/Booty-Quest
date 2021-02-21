using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : StateMachineBehaviour
{
	private Ship m_owner;

	private GameObject m_player;
	
	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Debug.Log("Navy Ship Begining Chase");
		m_owner = animator.GetComponent<Ship>();
		m_player = GameObject.FindGameObjectWithTag("Player");
		animator.ResetTrigger("PlayerTooFarForCombat");
	}

	//OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (Vector3.Distance(m_owner.transform.position, m_player.transform.position) <= m_owner.m_combatDistance)
		{
			animator.SetTrigger("CombatStart");
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Debug.Log("Navy Ship Ending Chase");
		animator.ResetTrigger("PlayerIsSighted");
	}
}

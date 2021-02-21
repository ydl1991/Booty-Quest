using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NMShipMovement : MonoBehaviour
{
	public Transform m_targetDestination;
	public float m_shipSpeed = 80;
	public float m_shipTurnSpeedDeg = 120;
	NavMeshAgent m_navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        m_navMeshAgent = this.GetComponent<NavMeshAgent>();

		if (m_navMeshAgent == null)
		{
			Debug.LogError("No NavMeshAgent Component found for " + gameObject.name);
		}
		else
		{
			SetShipStats();
			SetDestination(m_targetDestination);
		}
    }

	private void SetShipStats()
	{
		m_navMeshAgent.speed = m_shipSpeed;
		m_navMeshAgent.angularSpeed = m_shipTurnSpeedDeg;
	}

	public void SetDestination(Transform destination)
	{
		if(m_targetDestination != null)
		{
			m_targetDestination = destination;
			m_navMeshAgent.SetDestination(m_targetDestination.position);
		}
	}
}

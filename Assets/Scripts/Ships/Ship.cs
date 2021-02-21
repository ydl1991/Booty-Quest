using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : Vehicle
{
	public float m_shipSpeed = 20.0f;
	public float m_shipTurnSpeedDeg = 30.0f;
	public float m_distanceToPointThreshold = 200.0f;

	public float m_detectionRadius = 800.0f;
	public float m_combatDistance = 400.0f;
	public float m_escortDistance = 75.0f;
	public Transform[] m_pathToFollow;
	public GameObject m_lootPrefab; //Hold data for default loot class.

	// This should be set directly after instantiation by the ship system. This is not very clear and should be enforced better.
	// This is needed only for ship destruction so far. Maybe we can consider refactoring how the destruction works.
	public ShipSystem.ShipCategory m_category;

	private UnityEngine.AI.NavMeshAgent m_navMeshAgent;
	private Animator m_stateMachine;

	private Transform m_playerTransform;
	public GameObject m_escortee;
	public Transform m_pirateIslandTransform;
	public float m_distanceToPirateIslandThreshold = 300f;

	public Transform m_targetDestination;
	public int currentPathIndex = 0;
	public int minDropValue = 10;
	public int maxDropValue = 50;

	private Vector3? m_initialWaterParticleScale;

	// Start is called before the first frame update
	void Start()
	{
		m_stateMachine = GetComponent<Animator>();
		m_navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		m_playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
		m_pirateIslandTransform = GameObject.Find("PirateIsland").transform;
		GetComponent<CannonComponent>()?.SetupCannons(this);
		m_initialWaterParticleScale = gameObject.transform.Find("FX_BoatWater_Large")?.localScale;

		//FindMerchantToEscort();

		if (m_navMeshAgent == null)
		{
			Debug.LogError("No NavMeshAgent Component found for " + gameObject.name);
		}
		else
		{
			SetShipStats();
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (!m_navMeshAgent || m_navMeshAgent.speed == 0f)
			return;

		var particleSystem = gameObject.transform.Find("FX_BoatWater_Large");

		if (particleSystem != null)
		{
			particleSystem.localScale = m_initialWaterParticleScale != null ? m_initialWaterParticleScale.Value * (m_navMeshAgent.velocity.sqrMagnitude / (m_navMeshAgent.speed * m_navMeshAgent.speed)) : Vector3.zero;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		AnimatorStateInfo? info = m_stateMachine?.GetCurrentAnimatorStateInfo(0);
		if (info == null) return;
		
		if (Vector3.Distance(transform.position, m_pirateIslandTransform.position) < m_distanceToPirateIslandThreshold)
		{
			if (info.Value.IsName("Combat") || info.Value.IsName("Chase"))
				m_stateMachine.SetTrigger("NearPirateIsland");
		}
		else if (other.gameObject.GetComponent<Player>() != null)
		{
			m_stateMachine.SetTrigger("PlayerIsSighted");
		}

		else if (m_escortee != null && other.gameObject.GetComponent<Ship>() && other.gameObject.GetComponent<Ship>().m_category == ShipSystem.ShipCategory.Merchant)
		{
			if (!info.Value.IsName("Combat"))
			{
				m_stateMachine.SetTrigger("FoundMerchantShip");
				m_escortee = other.gameObject;
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.GetComponent<Player>() != null)
		{
			m_stateMachine.SetTrigger("PlayerIsLost");
		}
	}

	void OnDestroy()
	{
		// temporary, think how to do better? assign from the editor?
		// I don't think this has a bad impact on performance. It isn't called on update. - Erin
		GameObject navyShipSystem = GameObject.FindGameObjectWithTag("ShipSystem");
		ShipSystem owner = navyShipSystem?.GetComponent<ShipSystem>();
		
		owner?.DestroyShip(this);
	}
	public void SetShipStats()
	{
		m_navMeshAgent.speed = m_shipSpeed;
		m_navMeshAgent.angularSpeed = m_shipTurnSpeedDeg;

		Loot lootScript = m_lootPrefab.GetComponent<Loot>();
		lootScript.SetValues(minDropValue, maxDropValue);
	}

	// This function needs to be phased out.
	public void UpdatePath()
	{
		if (currentPathIndex < m_pathToFollow.Length)
		{
			m_targetDestination = m_pathToFollow[currentPathIndex];
			++currentPathIndex;
		}
		else
		{
			m_navMeshAgent.isStopped = true;
			m_stateMachine.SetTrigger("EndOfPath");
			Object.Destroy(this.gameObject);
		}
	}

	public override void TakeDamage(int damage)
	{
		Debug.Log(gameObject.name + " has taken " + damage + " damage.");
		GetComponent<HealthComponent>().ChangeHealth(-damage);

		// If the ship is destroyed (via combat)
		if(!GetComponent<HealthComponent>().alive)
		{
			// Spawn loot object tied to this ship and destroy the ship.
			Vector3 posToSpawnLoot = gameObject.transform.position;
			posToSpawnLoot.y += GameplayManager.kLootYOffset;
			Instantiate(m_lootPrefab, posToSpawnLoot, gameObject.transform.rotation);
			Object.Destroy(this.gameObject);
		}
	}

	public void FindMerchantToEscort()
	{
		Ship[] foundShips = GameObject.FindObjectsOfType<Ship>();

		foreach (Ship ship in foundShips)
		{
			if (ship.m_category != ShipSystem.ShipCategory.Merchant)
				continue;

			if (Vector3.Distance(ship.transform.position, transform.position) > m_detectionRadius)
				continue;

			m_stateMachine.SetTrigger("FoundMerchantShip");
			m_escortee = ship.gameObject;
		}
	}
}
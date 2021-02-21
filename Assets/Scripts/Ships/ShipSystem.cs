using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSystem : MonoBehaviour
{
	public enum ShipCategory
    {
		Merchant,
        Navy,
        Pirate,
        MaxCategory
    }

	public enum ShipType
    {
        MerchantPoor,
		MerchantWealthy,
		MerchantRoyalty,
        Navy,
        Pirate,
        MaxTypes
    }

	// List of spawnable ship types.
	private List<Ship>[] m_ships = new List<Ship>[(int)ShipCategory.MaxCategory];

	// Prefabs used to spawn different ships.
	[EnumArrayAttribute(typeof(ShipType))]
	public GameObject[] m_shipPrefabs = new GameObject[(int)ShipType.MaxTypes];
	
	// Total number of merchant and navy ships allowed. This can be adjusted in game as the player levels.
	public int m_maximumMerchantShipCount = 8;
	public int m_maximumNavyShipCount = 1;

	// The weight for the weighted random selection between the three types of merchants.
	// These values should be normalized. (Between 0 and 1)
	[SerializeField] private float m_wealthyMerchantSpawnWeight = 0.4f;
	[SerializeField] private float m_royaltyMerchantSpawnWeight = 0.05f;

	// Time between ship spawns. This is shared by all ship spawners.
	public float m_spawnTime = 2.0f;
	private float m_currentTime = 2.0f;

	// This prevents navy ships from spawning in the same places.
	// This is not going to work 100%, in the event a ship dies,
	// it will likely spawn in another lane to where it was.. which is okay.
	private int m_navyShipSpawnIndex = 0;

	//The list of spawn points controlled by this manager.
	public ShipSpawner[] m_spawners;

	public BankPanelController m_bank;
	[SerializeField] private int m_tierOne = 250; // tiers are the money required to spawn more navy ships.
	[SerializeField] private int m_tierTwo = 500;
	[SerializeField] private int m_tierThree = 1000;

	// timer used to cout the respawn time of the navy ships
	[SerializeField] private float m_respawnTimer = 15.0f; // This is also the initial spawn time of the first navy ship
	[SerializeField] private float m_respawnTime = 5.0f;

	// Start is called before the first frame update
	void Start()
	{
		m_navyShipSpawnIndex = Random.Range(0, m_spawners.Length);

		for (int i = 0; i < m_ships.Length; ++i)
		{
			m_ships[i] = new List<Ship>();
		}

		m_currentTime = m_spawnTime;
	}

	// Update is called once per frame
	void Update()
	{
		if (m_bank.lootInBank <= m_tierOne)
		{
			m_maximumNavyShipCount = 1;
		}
		else if (m_bank.lootInBank <= m_tierTwo)
		{
			m_maximumNavyShipCount = 3;
		}
		else if (m_bank.lootInBank <= m_tierThree)
		{
			m_maximumNavyShipCount = 7;
		}

		m_currentTime -= Time.deltaTime;
		m_respawnTimer -= Time.deltaTime;
		if (m_currentTime <= 0.0f)
		{
			if (m_ships[(int)ShipCategory.Merchant].Count < m_maximumMerchantShipCount)
			{
				SpawnMerchantShip();
				m_currentTime = m_spawnTime;
			}

			if (m_ships[(int)ShipCategory.Navy].Count < m_maximumNavyShipCount && m_respawnTimer <= 0.0f)
			{
				SpawnNavyShip();
				m_currentTime = m_spawnTime;
			}
		}


	}

	public void SpawnNavyShip()
	{
		Ship spawned = SpawnShip(ShipCategory.Navy, ShipType.Navy, m_navyShipSpawnIndex);

		spawned.m_category = ShipCategory.Navy;
		m_currentTime = m_spawnTime;
		++m_navyShipSpawnIndex;
		if (m_navyShipSpawnIndex >= m_spawners.Length)
			m_navyShipSpawnIndex = 0;
	}

	public void SpawnMerchantShip()
	{
		int randomIndex = Random.Range(0, m_spawners.Length);
		float choice = Random.Range(0.0f, 1.0f);

		//Debug.Log(choice);

		Ship spawned;
		if (choice <= m_royaltyMerchantSpawnWeight)
		{
			spawned = SpawnShip(ShipCategory.Merchant, ShipType.MerchantRoyalty, randomIndex);
		}
		else if (choice <= m_wealthyMerchantSpawnWeight)
		{
			spawned = SpawnShip(ShipCategory.Merchant, ShipType.MerchantWealthy, randomIndex);
		}
		else
		{
			spawned = SpawnShip(ShipCategory.Merchant, ShipType.MerchantPoor, randomIndex);
		}

		spawned.m_category = ShipCategory.Merchant;

		int destinationIndex = Random.Range(0, m_spawners.Length);
		while (destinationIndex == randomIndex)
		{
			destinationIndex = Random.Range(0,m_spawners.Length);
		}

		//spawned?.SetDestination(m_spawners[destinationIndex].transform);
	}

	public Ship SpawnShip(ShipCategory catagory, ShipType type, int portIndex)
	{
		GameObject spawned = m_spawners[portIndex].SpawnShip(m_shipPrefabs[(int)type]);
		Ship shipComp = spawned.GetComponent<Ship>();

		if (shipComp == null)
		{
			Debug.LogError("Prefab doesn't have Ship script attached :(");
		}
		else
		{
			m_ships[(int)catagory].Add(shipComp);
		}
		return shipComp;
	}

	public void DestroyShip(Ship shipToDestroy)
	{
		if (shipToDestroy.m_category == ShipCategory.Navy)
			m_respawnTimer = m_respawnTime; 
		m_ships[(int)shipToDestroy.m_category].Remove(shipToDestroy);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonComponent : MonoBehaviour
{
	[SerializeField] private Cannon m_cannonBallPrefab = null;  // Cannon ball prefab to shoot
	[SerializeField] private int m_maxCannonCount = 20;
	[SerializeField] private float m_fireCoolDown = 5.0f;       // Reload seconds for firing, the greater it is, the longer cool down it is

	[SerializeField] private List<GameObject> m_rightWeapons;
	public float m_rightCooldown{ get; private set; }
	[SerializeField] private List<GameObject> m_leftWeapons;
	public float m_leftCooldown { get; private set; }

	private int m_startMaxCannonCount = 0;
	public int cannonballCount { get; private set; }              // Current cannon balls
	public int bonusCannonDamage { get; private set; }

	public bool CanFire() => CanFireLeft || CanFireRight;
	public bool CanFireLeft => m_leftCooldown <= 0f;
	public bool CanFireRight => m_rightCooldown <= 0f;

	void Awake()
	{
		m_startMaxCannonCount = m_maxCannonCount;
		cannonballCount = m_maxCannonCount;
		Debug.Assert(m_cannonBallPrefab != null, "You haven't assign cannon prefab to " + gameObject.name);

		ResetUpgrades();
		m_leftCooldown = 0;
		m_rightCooldown = 0;
	}

	public void SetupCannons(Vehicle owner)
	{
		m_rightWeapons.Clear();
		m_leftWeapons.Clear();
		// Loop through the cannons prefab inside the vehicle
		foreach (Transform child in owner.transform)
		{
			if (child.CompareTag("Weapon"))
			{
				if (Vector3.Dot(child.transform.forward, owner.transform.right) > 0)
					m_rightWeapons.Add(child.gameObject);
				else
					m_leftWeapons.Add(child.gameObject);
			}
		}
	}

    private void Update()
    {
		m_leftCooldown -= Time.deltaTime;
		m_rightCooldown -= Time.deltaTime;
    }

    //----------------------------------------------------------------------------
    // Fire logic
    //----------------------------------------------------------------------------

	public void FireLeft(Vehicle vehicleToFire)
	{
		if (m_leftCooldown > 0f) return;

		// Loop through the cannons prefab inside the vehicle
		foreach (var weapon in m_leftWeapons)
		{
			// Check current cannon shots count
			if (cannonballCount <= 0)
				break;

			Cannon cannon = Instantiate(m_cannonBallPrefab, weapon.transform.position, weapon.transform.rotation);
			weapon.GetComponentInChildren<ParticleSystem>()?.Play(true);
			cannon.SetShooter(vehicleToFire);
			cannon.AddDmg(bonusCannonDamage);
			--cannonballCount;
		}

		m_leftCooldown = m_fireCoolDown;
	}
	public void FireRight(Vehicle vehicleToFire)
	{
		if (m_rightCooldown > 0f) return;

		// Loop through the cannons prefab inside the vehicle
		foreach (var weapon in m_rightWeapons)
		{
			// Check current cannon shots count
			if (cannonballCount <= 0)
				break;

			Cannon cannon = Instantiate(m_cannonBallPrefab, weapon.transform.position, weapon.transform.rotation);
			weapon.GetComponentInChildren<ParticleSystem>()?.Play(true);
			cannon.SetShooter(vehicleToFire);
			cannon.AddDmg(bonusCannonDamage);
			--cannonballCount;
		}

		m_rightCooldown = m_fireCoolDown;
	}

    public void Fire(Vehicle vehicleToFire)
	{
		FireLeft(vehicleToFire);
		FireRight(vehicleToFire);
	}

	//----------------------------------------------------------------------------
	// Accessors & mutators
	//----------------------------------------------------------------------------
	public void AddCannonDmg(int dmg) { bonusCannonDamage += dmg; }
	public void AddCannon(int count) 
	{
		if (cannonballCount + count > m_maxCannonCount)
			cannonballCount = m_maxCannonCount;
		else
			cannonballCount += count;
	}
	public int GetMaxCannonCount() { return m_maxCannonCount; }
	public void AddMaxCannonCount(int count) { m_maxCannonCount += count; }
	public void ResetUpgrades()
    {
		bonusCannonDamage = 0;
		m_maxCannonCount = m_startMaxCannonCount;
	}
}

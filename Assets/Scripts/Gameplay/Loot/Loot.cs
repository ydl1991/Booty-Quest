using System;
using System.Collections.Generic;
using UnityEngine;

//==========================================================================================================
// Represent loot object
//==========================================================================================================
public class Loot : MonoBehaviour
{
	public enum LootType
    {
        Treasure,
        CannonAmmo,
        MaxTypes
    }

    [SerializeField] private int m_value = 10;
	[SerializeField] private LootType m_LootType = LootType.Treasure;

    private void Awake()
    {
        Destroy(gameObject, GameplayManager.kDestroyDelay);
	}

	public void SetValues(int minValue, int maxValue)
    {
		// Set this loot objects stats.
		m_value = UnityEngine.Random.Range(minValue, maxValue); // Need to use UnityEngine.Random to solve ambiguity error.
	}

    //----------------------------------------------------------------------------
    // Collision detected enter trigger
    //----------------------------------------------------------------------------
    protected void OnTriggerEnter(Collider other) 
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null &&	// Collier is player
			(player.GetComponent<LootComponent>().GetLootCount() < player.GetComponent<LootComponent>().m_maxLoot))	// Player's current loot count has not hit max loot yet
        {
			if (m_LootType == LootType.Treasure)
			{
				LootComponent playerLootComponent = player.GetComponent<LootComponent>();
				if (playerLootComponent != null)
				{
					playerLootComponent.Loot(m_value);
					Destroy(this.gameObject);        // Kill this loot object
				}
			}
			else
			{
				CannonComponent playerCannonComponent = player.GetComponent<CannonComponent>();
				if (playerCannonComponent != null)
				{
					playerCannonComponent.AddCannon(m_value);
					Destroy(this.gameObject);        // Kill this loot object
				}
			}
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================================================================================
// Loot Component can be attached to any game object or its children object
//==========================================================================================================
public class LootComponent : MonoBehaviour
{
    [SerializeField] int m_lootCount = 5;     // The reason why I made this as float is I need to set moving speed according to loot count
    [SerializeField] BankPanelController m_bank;
    public int m_maxLoot { get; private set; }	// The max loot that this component can carry

    void Awake()
    {
        ResetUpgrades();
    }

    //----------------------------------------------------------------------------
    // Accessors & mutators
    //----------------------------------------------------------------------------
    public void Loot(int count)
    {
        if (m_lootCount + count > m_maxLoot)
            m_lootCount = m_maxLoot;
        else
            m_lootCount += count;

        // Player change move speed logic
        Player player = gameObject.GetComponent<Player>();
        if (player != null)
            player.UpdateShipMoveSpeed();
    }

    public void SpendLoot(int count) { m_bank.ChangeLootInBank(-count); }
    public void EarnSpendableLoot(int count) { m_bank.ChangeLootInBank(count); }
    public int GetSpendableLootCount() { return m_bank.lootInBank; }
    public void AddMaxLootCount(int bonusCount) { m_maxLoot += bonusCount; }
    public int GetLootCount() { return m_lootCount; }
    public int GetMaxLootCount() { return m_maxLoot; }
    public void Clear() { m_lootCount = 0; }
    public void ResetUpgrades() { m_maxLoot = GameplayManager.kMaxLootCount; }
}

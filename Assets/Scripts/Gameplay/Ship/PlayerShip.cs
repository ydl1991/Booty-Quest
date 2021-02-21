using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : Vehicle
{
    [SerializeField] private GameObject m_characterModel = null;
    [SerializeField] private GameObject m_respawnObject = null; // Used when the player dead
    private Player m_player = null;     // The player that is currently driving this ship
    private int m_armor = 100;          // This is used as a percentage that how much incoming dmg is going to take. i.e. When m_armor is 50, the enemy's cannon's dmg is 100. This ship takes 50 dmg

    private void Awake()
    {
        Debug.Assert(m_characterModel != null, "You haven't m_characterModel to PlayerShip");
        m_respawnObject = GameObject.Find("ParkingObject");
        Debug.Assert(m_respawnObject != null, "You haven't assign respawn object to the player ship.");
    }

    //--------------------------------------------------------------------
    // Call this when the player enters the ship's trigger
    //--------------------------------------------------------------------
    public void OnPlayerEnterShip(Player player)
    {
        m_player = player;
    }

    //--------------------------------------------------------------------
    // Call this when the player starts driving the ship
    //--------------------------------------------------------------------
    public void OnPlayerDriveShip()
    {
        GetComponent<MeshCollider>().enabled = false;
        m_characterModel.SetActive(true);
    }

    //--------------------------------------------------------------------
    // Call this when the player leaves the ship trigger
    //--------------------------------------------------------------------
    public void OnPlayerLeaveShip()
    {
        m_player = null;
    }

    //--------------------------------------------------------------------
    // Call this when the player stop driving the ship
    //--------------------------------------------------------------------
    public void OnPlayerGetOffShip()
    {
        GetComponent<MeshCollider>().enabled = true;
        m_characterModel.SetActive(false);
    }

    //--------------------------------------------------------------------
    // Return the player that is currently driving this ship
    //--------------------------------------------------------------------
    public Player GetPlayer()
    {
        return m_player;
    }

    //--------------------------------------------------------------------
    // Note that this is upgrading armor
    //--------------------------------------------------------------------
    public void AddArmor(int count)
    {
        m_armor -= count;
    }

    public void OnPlayerDead()
    {
        GetComponent<Transform>().SetParent(m_respawnObject.GetComponent<Transform>());
        GetComponent<Transform>().position = m_respawnObject.GetComponent<Transform>().position;
        OnPlayerGetOffShip();
        OnPlayerLeaveShip();
    }

    //--------------------------------------------------------------------
    // Take damage for player ship
    //--------------------------------------------------------------------
    public override void TakeDamage(int damage)
    {
        Debug.Log(gameObject.name + " has taken " + damage + " damage.");

        var hpComp = m_player.GetComponent<HealthComponent>();
        hpComp?.ChangeHealth(-damage);

        // If the ship is destroyed (via combat)
        if (hpComp && !hpComp.alive)
        {
            m_player.OnDead();
        }
    }
}

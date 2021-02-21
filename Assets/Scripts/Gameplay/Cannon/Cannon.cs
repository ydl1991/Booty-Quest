using System;
using System.Collections.Generic;
using UnityEngine;

//==========================================================================================================
// Represent Cannon ball object
//==========================================================================================================
public class Cannon : MonoBehaviour
{
    protected float m_velocity = GameplayManager.kCannonVelocity;       // Speed when moving towards the target
    protected float m_weight = GameplayManager.kCannonWeight;           // Used for slowing down ships with loots
    protected int m_damage = GameplayManager.kCannonDamage;             // How much damage does this cannon damage other
    protected float m_lifeTime = GameplayManager.kCannonLifeTime;       // How long does this cannon live
    protected Vector3 m_dir;                                   // Direction of this Cannon is moving when active
    protected Vehicle m_shooter;        // The shooter vehicle of this cannon

    //----------------------------------------------------------------------------
    // Init this cannon
    //----------------------------------------------------------------------------
    void Awake()
    {
        m_dir = transform.forward;
        Destroy(gameObject, m_lifeTime);
    }

    //----------------------------------------------------------------------------
    // Damage other collider's owner
    //----------------------------------------------------------------------------
    private void OnCollisionEnter(Collision collisionInfo)
    {
        Vehicle vehicle = collisionInfo.gameObject.GetComponent<Vehicle>() ?? collisionInfo.gameObject.GetComponentInParent<Vehicle>() ?? collisionInfo.gameObject.GetComponentInChildren<Vehicle>();
        if (vehicle != null && vehicle != m_shooter)
        {
            // Do damage to other ship
            vehicle.TakeDamage(m_damage);

            // Destroy this cannon
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        Vehicle vehicle = other.gameObject.GetComponent<Vehicle>() ?? other.gameObject.GetComponentInParent<Vehicle>() ?? other.gameObject.GetComponentInChildren<Vehicle>();
        if (vehicle != null && vehicle != m_shooter && vehicle.gameObject.tag == "Player")
        {
            // Do damage to other ship
            vehicle.TakeDamage(m_damage);

            // Destroy this cannon
            Destroy(gameObject);
        }
    }

    //----------------------------------------------------------------------------
    // Move towards to the target vector
    //----------------------------------------------------------------------------
    private void Update()
    {
        // Move towards to the target
        m_velocity += GameplayManager.kCannonVelocityBonus;
        transform.position += m_dir * m_velocity * Time.deltaTime;
    }

    //----------------------------------------------------------------------------
    // Accessors & Mutators
    //----------------------------------------------------------------------------
    public void AddDmg(int dmg) { m_damage += dmg; }
    public void SetShooter(Vehicle vehicle) { m_shooter = vehicle; }

    //----------------------------------------------------------------------------
    // Virtual function for cannon effects
    //----------------------------------------------------------------------------
    protected virtual void SpecialEffects()
    {
    }
}


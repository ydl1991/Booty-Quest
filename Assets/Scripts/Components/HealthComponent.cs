using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Health Component can be attached to any game object or its children object
// but note that one object tree should only contain one health component
public class HealthComponent : MonoBehaviour
{
    // properties
    public float health { get; private set; }
    public float healthPercentage { get; private set; }
    public bool alive { get{ return health > 0f; } }

    // member variables
    [SerializeField] float m_maxHealth = 100f;

    // Start is called before the first frame update
    void Start()
    {
        health = m_maxHealth;
        Refresh();
    }

    void Update()
    {  
    }

    // if taking damage, delta should be a negative number
    // if healing, delta should be positive
    public void ChangeHealth(float delta)
    {
        health += delta;

        if (health > m_maxHealth)
            health = m_maxHealth;
        else if (health <= 0f)
            health = 0f;

        Refresh();
    }

    public void RecoverToFullHealth()
    {
        ChangeHealth(m_maxHealth);
    }

    // refresh health scale
    private void Refresh()
    {
        healthPercentage = health / m_maxHealth;
    }
}

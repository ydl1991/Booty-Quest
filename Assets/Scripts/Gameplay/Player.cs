using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//==========================================================================================================
// Player component script
//==========================================================================================================
public class Player : MonoBehaviour
{
    // KeyCode for input
    [SerializeField] private KeyCode m_fireRightKey = KeyCode.E;     // Key to shoot right cannons
    [SerializeField] private KeyCode m_fireLeftKey = KeyCode.Q;     // Key to shoot left cannons
    [SerializeField] private KeyCode m_interactKey = KeyCode.F; // Key to interact with objects
    [SerializeField] private GameObject m_model = null;
    [SerializeField] private GameObject m_respawnObject = null; // Used when the player dead
    [SerializeField] private GameObject m_parkingObj = null; 

    // Upgrade benchmark
    public float m_speedUpgradeIndicator;
    public float m_maneuverabilityUpgradeIndicator;
    public int m_damageUpgradeIndicator;
    public int m_cannonCarryUpgradeIndicator;
    public int m_lootCarryUpgradeIndicator;
    public int m_armorUpgradeIndicator;

    // Gameplay data
    [SerializeField] private GameplayManager m_manager = null;
    private PlayerShip m_drivingShip = null;        // The ship that the player is driving
    private float m_characterSwimSpeed = 0.0f;
    private float m_currentShipMoveSpeed = 0.0f;
    private float m_stamina = 0.0f;

    // Capsule collider
    private Vector3 m_characterCenter = Vector3.zero;
    private float m_characterRadius = 0.0f;
    private float m_characterHeight = 0.0f;
    private float m_shipRadius = 0.0f;
    private float m_shipHeight = 0.0f;

    // Upgrades
    public float m_bonusShipForwardSpeed { get; private set; }   // The bonus for ship moving speed
    public float m_bonusShipManeuverability { get; private set; }   // The bonus for ship Maneuverability
    public int shipLevel { get; private set; }

    private int m_bonusArmorToUpgrade = 0;   // bonus armor for ship

    // Scores
    public int score { get; private set; }

    //----------------------------------------------------------------------------
    // Init player
    //----------------------------------------------------------------------------
    private void Awake()
    {
        // Assert required components
        LootComponent lootComponent = GetComponent<LootComponent>();
        Debug.Assert(lootComponent != null, "You haven't assign loot component to the player!");

        CannonComponent cannonComponent = GetComponent<CannonComponent>();
        Debug.Assert(cannonComponent != null, "You haven't assign cannon component to the player!");

        Debug.Assert(m_manager != null, "You haven't assign gameplay manager to the player.");

        Debug.Assert(m_respawnObject != null, "You haven't assign respawn object to the player.");

        // Upgrades
        ResetUpgrades();
        m_characterRadius = GetComponent<CapsuleCollider>().radius;
        m_characterHeight = GetComponent<CapsuleCollider>().height;
        m_characterCenter = GetComponent<CapsuleCollider>().center;

        // Init Data
        score = 0;
        m_stamina = GetComponent<Invector.vCharacterController.vActions.vSwimming>().stamina;
        m_characterSwimSpeed = GetComponent<Invector.vCharacterController.vActions.vSwimming>().swimForwardSpeed;
    }

    //----------------------------------------------------------------------------
    // Update player
    //----------------------------------------------------------------------------
    private void Update()
    {
        // Fire
        if (Input.GetKeyDown(m_fireRightKey) && m_drivingShip != null)
        {
            GetComponent<CannonComponent>().FireRight(m_drivingShip);
        }
        if (Input.GetKeyDown(m_fireLeftKey) && m_drivingShip != null)
        {
            GetComponent<CannonComponent>().FireLeft(m_drivingShip);
        }

        if (m_drivingShip != null && GetComponent<Transform>().position.y < GameplayManager.YOffset)
        {
            GetComponent<Transform>().position = m_parkingObj.transform.position;
        }
    }

    public void UpgradeShip()
    {
        UpgradeLevel();
        UpgradeSpeed(m_speedUpgradeIndicator);
        UpgradeManeuverability(m_maneuverabilityUpgradeIndicator);
        UpgradeDamage(m_damageUpgradeIndicator);
        UpgradeCannonMaxCount(m_cannonCarryUpgradeIndicator);
        UpgradeLootMaxCount(m_lootCarryUpgradeIndicator);
        UpgradeArmor(m_armorUpgradeIndicator);
    }

    //----------------------------------------------------------------------------
    // Reset upgrades to the begin stats of the game
    //----------------------------------------------------------------------------
    private void ResetUpgrades()
    {
        m_bonusArmorToUpgrade = 0;
        m_bonusShipForwardSpeed = GameplayManager.kShipBaseMoveSpeed;
        m_bonusShipManeuverability = GameplayManager.kShipBaseRotateSpeed;
        m_shipRadius = m_manager.kShipCapsuleRadiuess[0];
        m_shipHeight = m_manager.kShipCapsuleHeights[0];
        shipLevel = 1;
    }

    //----------------------------------------------------------------------------
    // Drive logic
    //----------------------------------------------------------------------------
    public void Drive(PlayerShip shipToDrive)
    {
        // Disable Rigibody gravity
        GetComponent<Rigidbody>().useGravity = false;

        // Disable shift, E, space input for swimming
        GetComponent<Invector.vCharacterController.vActions.vSwimming>().stamina = 0;
        GetComponent<Invector.vCharacterController.vActions.vSwimming>().swimUpInput = new Invector.vCharacterController.GenericInput("None", "X", "X");
        GetComponent<Invector.vCharacterController.vActions.vSwimming>().swimDownInput = new Invector.vCharacterController.GenericInput("None", "Y", "Y");
        GetComponent<Invector.vCharacterController.vActions.vLadderAction>().enterInput = new Invector.vCharacterController.GenericInput("None", "A", "A");

        // Set capsule collider to match the ship
        GetComponent<CapsuleCollider>().direction = 2;      // Z - axis
        GetComponent<CapsuleCollider>().center = new Vector3(0.0f, GameplayManager.kShipYOffset, 0.0f);
        GetComponent<CapsuleCollider>().radius = m_shipRadius;
        GetComponent<CapsuleCollider>().height = m_shipHeight;
        GetComponent<Invector.vCharacterController.vThirdPersonMotor>().UpdateCapsule();
        
        // Set driving ship object
        m_drivingShip = shipToDrive;
        m_model.SetActive(false);       // Hide player character model

        // Upgrade ship's armor
        m_drivingShip.AddArmor(m_bonusArmorToUpgrade);
        m_bonusArmorToUpgrade = 0;

        // Tune swimming speed to drive mode
        UpdateShipMoveSpeed();
        GetComponent<Invector.vCharacterController.vActions.vSwimming>().swimRotationSpeed += m_bonusShipManeuverability;

        // Setup cannons
        var cannonComponent = GetComponent<CannonComponent>();
        if (cannonComponent != null)
        {
            cannonComponent.enabled = true;
            cannonComponent.SetupCannons(shipToDrive);
        }
    }

    //----------------------------------------------------------------------------
    // Call this when the player stop driving the ship
    //----------------------------------------------------------------------------
    public void GetOffShip()
    {
        // Enable gravity
        GetComponent<Rigidbody>().useGravity = true;

        // Enable shift, E, space input for swimming
        GetComponent<Invector.vCharacterController.vActions.vSwimming>().stamina = m_stamina;
        GetComponent<Invector.vCharacterController.vActions.vSwimming>().swimUpInput = new Invector.vCharacterController.GenericInput("Space", "X", "X");
        GetComponent<Invector.vCharacterController.vActions.vSwimming>().swimDownInput = new Invector.vCharacterController.GenericInput("LeftShift", "Y", "Y");
        GetComponent<Invector.vCharacterController.vActions.vLadderAction>().enterInput = new Invector.vCharacterController.GenericInput("F", "A", "A");

        // Set capsule collider back to the player
        GetComponent<CapsuleCollider>().direction = 1;      // Y - axis
        GetComponent<CapsuleCollider>().center = m_characterCenter;
        GetComponent<CapsuleCollider>().radius = m_characterRadius;
        GetComponent<CapsuleCollider>().height = m_characterHeight;
        GetComponent<Invector.vCharacterController.vThirdPersonMotor>().UpdateCapsule();

        // Get model back
        m_drivingShip = null;
        m_model.SetActive(true);       // Show player character model

        // Tune swimming speed to drive mode
        GetComponent<Invector.vCharacterController.vActions.vSwimming>().swimForwardSpeed = m_characterSwimSpeed;
        GetComponent<Invector.vCharacterController.vActions.vSwimming>().swimRotationSpeed -= m_bonusShipManeuverability;
    
        // Deactivate cannons
        GetComponent<CannonComponent>().enabled = false;
    }

    //----------------------------------------------------------------------------
    // On dead
    //  - Lose all loot in ship’s hold
    //  - Lose all upgrades
    //  - Respawn at Pirate Island
    //----------------------------------------------------------------------------
    public void OnDead()
    {
        m_drivingShip.OnPlayerDead();
        GetOffShip();

        // Lose loot
        LootComponent lootComponent = GetComponent<LootComponent>();
        lootComponent.Clear();

        // Lose all upgrades
        ResetUpgrades();
        GetComponent<CannonComponent>().ResetUpgrades();
        GetComponent<LootComponent>().ResetUpgrades();
        m_manager.ResetUpgrades();

        // Respawn at pirate island
        GetComponent<Transform>().position = m_respawnObject.GetComponent<Transform>().position;
        GetComponent<HealthComponent>().RecoverToFullHealth();
    }

    //----------------------------------------------------------------------------
    // Call this function to update ship movement speed when spend or get loot
    //----------------------------------------------------------------------------
    public void UpdateShipMoveSpeed()
    {
        // Update ship movement speed by loot scale
        float scale = (1.0f - (GetComponent<LootComponent>().GetLootCount() / GetComponent<LootComponent>().GetMaxLootCount()));

        // ShipMoveSpeed = kShipBaseMoveSpeed + (ShipBonusMoveSpeed * lootCount%)
        m_currentShipMoveSpeed = (m_bonusShipForwardSpeed * scale) + m_bonusShipForwardSpeed;

        // Update ship movement speed right away if the player is currently driving a ship
        if (m_drivingShip != null)
            GetComponent<Invector.vCharacterController.vActions.vSwimming>().UpdateSwimSpeed(m_currentShipMoveSpeed);
    }

    //--------------------------------------------------------------------
    // When colliding with enemy ship
    //--------------------------------------------------------------------
    private void OnCollisionEnter(Collision collisionInfo)
    {
        // Exit if not driving a ship
        if (!m_drivingShip) return;

        // Got killed when hitting by an enemy ship
        Vehicle vehicle = collisionInfo.gameObject.GetComponent<Vehicle>();
        if (vehicle != null)
        {
            OnDead();
        }
    }

    //----------------------------------------------------------------------------
    // Accessors & mutators
    //----------------------------------------------------------------------------
    public int GetScore() { return score; }
    public void AddScore(int score) { this.score += score; }
    public int GetCurrentCannonShots() { return GetComponent<CannonComponent>().cannonballCount; }
    public int GetMaxCannonShots() { return GetComponent<CannonComponent>().GetMaxCannonCount(); }
    public int GetCurrentCarryingLoots() { return GetComponent<LootComponent>().GetLootCount(); }
    public int GetMaxCarryingLoots() { return GetComponent<LootComponent>().GetMaxLootCount(); }
    public int GetCurrentDamage() { return GetComponent<CannonComponent>().bonusCannonDamage; }
    public KeyCode GetInteractKey() { return m_interactKey; }
    public PlayerShip GetDrivingShip() { return m_drivingShip; }

    //----------------------------------------------------------------------------
    // Upgrades
    //----------------------------------------------------------------------------
    // Upgrade ship driving speed
    void UpgradeSpeed(float bonusSpeed) { m_bonusShipForwardSpeed += bonusSpeed; }
    void UpgradeManeuverability(float bonusSpeed) { m_bonusShipManeuverability += bonusSpeed; }
    void UpgradeDamage(int bonusDmg) { GetComponent<CannonComponent>().AddCannonDmg(bonusDmg); }
    void UpgradeCannonMaxCount(int bonusCount) { GetComponent<CannonComponent>().AddMaxCannonCount(bonusCount);  }
    void UpgradeLootMaxCount(int bonusCount) { GetComponent<LootComponent>().AddMaxLootCount(bonusCount); }
    void UpgradeArmor(int bonusArmor) { m_bonusArmorToUpgrade += bonusArmor;  }
    void UpgradeLevel()
    {
        ++shipLevel;
        m_manager.UpgradeShip(shipLevel - 1);
        m_shipRadius = m_manager.kShipCapsuleRadiuess[shipLevel - 1];
        m_shipHeight = m_manager.kShipCapsuleHeights[shipLevel - 1];
    }
}


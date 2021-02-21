using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enums
[System.Serializable]
public enum SpinDirection
{
    kHorizontal
}

// Constants and ship manager
public class GameplayManager : MonoBehaviour
{
    public const float YOffset = -2.0f;

    // Destroy object time
    public const float kDestroyDelay = 120.0f;     // Destroy objects after this time count

    // Cannon
    public const float kCannonVelocity = 70.0f;
    public const float kCannonVelocityBonus = 0.1f;    // Add speed to cannon every frame by this value
    public const float kCannonWeight = 20.0f;
    public const int kCannonDamage = 20;
    public const float kCannonLifeTime = 5.0f;
    public const float kFlameCannonWeight = 25.0f;
    public const int kFlameCannonDamage = 25;

    // Ship
    public const float kShipYOffset = 2.0f;
    public const float kShipBaseMoveSpeed = 10.0f;      // Guarantee ship has at least this move speed
    public const float kShipBaseRotateSpeed = -2.0f;
    public float[] kShipCapsuleRadiuess = { 2.5f, 2.5f, 3.0f };     
    public float[] kShipCapsuleHeights = { 15.0f, 15.0f, 17.0f };

    // Ships different levels
    [SerializeField] private GameObject[] m_ships = null;
    private int m_currentShipIndex = 0;

    // Loot
    public const int kMaxLootCount = 100;
    public const float kLootYOffset = 2;

    // Camera
    public const int kZoomOutShipDistance = 20;
    public const int kZoomOutShipHeight = 3;

    public void ResetUpgrades()
    {
        m_ships[m_currentShipIndex].SetActive(false);
        m_ships[0].SetActive(true);
        m_currentShipIndex = 0;
    }

    public void UpgradeShip(int index)
    {
        m_ships[m_currentShipIndex].SetActive(false);
        m_ships[index].SetActive(true);
        m_currentShipIndex = index;
    }
}

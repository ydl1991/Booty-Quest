using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//===================================================================================================
// Attach this script to a ship to allow the player get on
//===================================================================================================
public class OnShipTrigger : MonoBehaviour
{
    [SerializeField] private PlayerShip m_ownerShip = null;

    private void Awake()
    {
        Debug.Assert(m_ownerShip != null, "OwnerShip is not assigned to OnShipTrigger");
    }

    private void Update()
    {
        // Drive ship logic
        if (m_ownerShip.GetPlayer() != null && Input.GetKeyDown(m_ownerShip.GetPlayer().GetInteractKey()) && m_ownerShip.GetPlayer().GetDrivingShip() == null)
        {
            // Adjust player's rotation and owner ship's position
            m_ownerShip.GetPlayer().GetComponent<Transform>().rotation = m_ownerShip.GetComponent<Transform>().rotation;    // Rotation
            m_ownerShip.GetPlayer().GetComponent<Transform>().position = m_ownerShip.GetComponent<Transform>().position;    // Position
            m_ownerShip.GetComponent<Transform>().SetParent(m_ownerShip.GetPlayer().GetComponent<Transform>());             // Set the ship as a child of player

            // Player drive ship logic
            m_ownerShip.GetPlayer().Drive(m_ownerShip);
            m_ownerShip.OnPlayerDriveShip();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null && (player.GetDrivingShip() == null))
        {
            if (GameManager.s_instance.gameLanguage == Language.kEnglish)
                MessagePanelController.s_instance.SetText("Press " + player.GetInteractKey().ToString() + " to drive the ship");

            else if (GameManager.s_instance.gameLanguage == Language.kChinese)
                MessagePanelController.s_instance.SetText("按 " + player.GetInteractKey().ToString() + " 开船");

            m_ownerShip.OnPlayerEnterShip(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_ownerShip.OnPlayerLeaveShip();
        }
    }
}

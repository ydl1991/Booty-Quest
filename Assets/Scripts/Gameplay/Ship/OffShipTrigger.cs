using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;

//===================================================================================================
// Attach this script to a dock to allow the player get off
//===================================================================================================
public class OffShipTrigger : MonoBehaviour
{
    [SerializeField] private GameObject m_offShipObject = null;
    [SerializeField] private GameObject m_parkingObject = null;   // Consider making this a list to have multiple parking spaces
    private Player m_player = null;

    private void Awake()
    {
        Debug.Assert(m_offShipObject != null, "You haven't assign off ship object to off ship trigger");
        Debug.Assert(m_parkingObject != null, "You haven't assign m_parkingObject to off ship trigger");
    }

    private void Update()
    {
        if (m_player != null && Input.GetKeyDown(m_player.GetInteractKey()) && m_player.GetDrivingShip() != null)
        {
            // Adjust player's rotation and owner ship's position
            PlayerShip playerDrivingShip = m_player.GetDrivingShip();
            playerDrivingShip.GetComponent<Transform>().position = m_parkingObject.GetComponent<Transform>().position;
            playerDrivingShip.GetComponent<Transform>().rotation = m_parkingObject.GetComponent<Transform>().rotation;
            playerDrivingShip.GetComponent<Transform>().SetParent(m_parkingObject.GetComponent<Transform>());

            // Ship stop driving logic
            playerDrivingShip.OnPlayerLeaveShip();
            playerDrivingShip.OnPlayerGetOffShip();

            // Player stop driving logic
            m_player.GetOffShip();
            m_player.GetComponent<Transform>().position = m_offShipObject.GetComponent<Transform>().position;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null && (player.GetDrivingShip() != null))
        {
            if (GameManager.s_instance.gameLanguage == Language.kEnglish)
                MessagePanelController.s_instance.SetText("Press " + player.GetInteractKey().ToString() + " to get off the ship");

            else if (GameManager.s_instance.gameLanguage == Language.kChinese)
                MessagePanelController.s_instance.SetText("按 " + player.GetInteractKey().ToString() + " 下船");

            m_player = player;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            m_player = null;
    }
}

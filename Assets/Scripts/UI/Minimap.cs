using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform m_player;

    void LateUpdate()
    {
        Vector3 newPos = m_player.position;
        newPos.y = transform.position.y;
        transform.position = newPos;
    }
}

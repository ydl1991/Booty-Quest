using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================================================================================
// Spin object
//==========================================================================================================
public class Spinner : MonoBehaviour
{
    [SerializeField] private float m_speed = 1.0f;      
    [SerializeField] private SpinDirection m_direction;

    // Update is called once per frame
    void Update()
    {
        if (m_direction == SpinDirection.kHorizontal)
            GetComponent<Transform>().Rotate(0, m_speed, 0);
    }
}

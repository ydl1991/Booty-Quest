using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Axis
{
    kX = 0,
    kY,
    kZ
}

public class NpcMark : MonoBehaviour
{
    // Rotation Input
    public Axis m_rotationAxis;
    public float m_rotationSpeed;
    private Vector3 m_rotationDir;
    
    // Floating Input
    public float m_amplitude;
    public float m_frequency;
 
    // Position Storage Variables
    Vector3 m_posOffset;
    Vector3 m_tempPos;

    // Start is called before the first frame update
    void Start()
    {
        // Store the starting position & rotation of the object
        m_posOffset = transform.position;

        if (m_rotationAxis == Axis.kX)
            m_rotationDir = new Vector3(1f, 0, 0);
        else if (m_rotationAxis == Axis.kY)
            m_rotationDir = new Vector3(0, 1f, 0);
        else
            m_rotationDir = new Vector3(0, 0, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(m_rotationDir * m_rotationSpeed * Time.deltaTime);

        // Float up/down with a Sin()
        m_tempPos = m_posOffset;
        m_tempPos.y += Mathf.Sin (Time.fixedTime * Mathf.PI * m_frequency) * m_amplitude;
        transform.position = m_tempPos;
    }
}

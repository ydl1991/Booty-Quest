using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthWheel : MonoBehaviour
{
    [SerializeField] HealthComponent m_healthComp;
    [SerializeField] Slider m_healthWheel;
    [SerializeField] Image m_healthFillImage;

    // Start is called before the first frame update
    void Start()
    {
        m_healthFillImage.color = Color.green;
    }

    // Update is called once per frame
    void Update()
    {
        AdjustHealthWheelValueAndColor();
    }

    private void AdjustHealthWheelValueAndColor()
    {
        // get health component from target object, note: HealthComponent might be attouched to 
        // child object, so using GetComponentInChildren to search the object as well as its children
        // to find the first HealthComponent. Also we might note that to only assign one health component
        // to an object.        
        if (m_healthComp == null)
        {
            Debug.Log("There is no health component attached to object");
            return;
        }

        float healthPercent = m_healthComp.healthPercentage;
        
        if (m_healthWheel.value != healthPercent * 100f)
        {
            m_healthWheel.value = healthPercent * 100f;
            m_healthFillImage.color = Color.Lerp(Color.red, Color.green, healthPercent);
        }
    }
}

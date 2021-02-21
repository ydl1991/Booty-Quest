using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagePanelController : MonoBehaviour
{
    [SerializeField] GameObject m_messagePanel = null;
    [SerializeField] Text m_message = null;
    [SerializeField] float m_displayLength;

    public static MessagePanelController s_instance;

    private float m_displayTimer;

    void Awake()
    {
        s_instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_displayTimer > 0f)
        {
            m_displayTimer -= Time.deltaTime;
            if (m_displayTimer <= 0f)
                m_messagePanel.SetActive(false);
        }
    }

    public void SetText(string text)
    {
        m_message.text = text;
        m_displayTimer = m_displayLength;
        m_messagePanel.SetActive(true);
    }
    public void SetText(string text, float displayLength)
    {
        m_message.text = text;
        m_displayTimer = displayLength;
        m_messagePanel.SetActive(true);
    }

    public void StopDisplay()
    {
        m_message.text = "";
        m_displayTimer = 0f;
        m_messagePanel.SetActive(false);
    }
}

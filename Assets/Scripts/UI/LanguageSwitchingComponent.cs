using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSwitchingComponent : MonoBehaviour
{
    public bool m_tick;

    void Start()
    {
        GameManager.s_instance?.RegisterLanguageComponent(this);
    }

    void OnEnable()
    {
        GameManager.s_instance?.RegisterLanguageComponent(this);
    }

    void OnDisable()
    {
        GameManager.s_instance?.RemoveLanguageComponent(this);
    }

    void Update()
    {
        if (m_tick && GameManager.s_instance)
            ApplyLanguageChange(GameManager.s_instance.gameLanguage);
    }

    public void ApplyLanguageChange(Language newLanguage)
    {
        SendMessage("OnLanguageChange", newLanguage);
    }
}

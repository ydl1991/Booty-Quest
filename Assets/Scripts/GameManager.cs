using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Language
{
    kEnglish = 0,
    kChinese,
    kTotalNumLanguage
}

public class GameManager : MonoBehaviour
{
    public static GameManager s_instance;  // Singleton

    public Language gameLanguage { get; private set; }
    public bool mouseOnUI { get; set; }

    private List<LanguageSwitchingComponent> m_languageFlexibleObjects;

    void Awake()
    {
        s_instance = this;
        gameLanguage = Language.kEnglish;
        m_languageFlexibleObjects = new List<LanguageSwitchingComponent>();
    }

    public void ChangeLanguage(int newLanguage)
    {
        if (newLanguage == (int)gameLanguage)
            return;
            
        gameLanguage = (Language)newLanguage;
        ReconfigureGameLanguage();
    }

    // Apply language switch to each of the objects
    public void ReconfigureGameLanguage()
    {
        m_languageFlexibleObjects.ForEach(flexObject => flexObject.ApplyLanguageChange(gameLanguage));
    }

    // Register language flexible object
    public void RegisterLanguageComponent(LanguageSwitchingComponent languageFlexObject)
    {  
        int found = m_languageFlexibleObjects.FindIndex(x => x == languageFlexObject);
        if (found != -1)
            return;
        
        languageFlexObject.ApplyLanguageChange(gameLanguage);
        m_languageFlexibleObjects.Add(languageFlexObject);
    }

    // Remove language flexible object
    public void RemoveLanguageComponent(LanguageSwitchingComponent languageFlexObject)
    {
        int found = m_languageFlexibleObjects.FindIndex(x => x == languageFlexObject);
        if (found == -1)
            return;

        m_languageFlexibleObjects.RemoveAt(found);
    }
}

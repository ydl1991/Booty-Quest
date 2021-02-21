using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelController : MonoBehaviour
{
    private static ReadOnlyCollection<string> s_kSettingMenuLanguageLocalization = new ReadOnlyCollection<string> (new string[] {
        
        // Setting menu text
        "Game Setting", "游戏设置",

        // Language setting text
        "Language", "语言设置",

        // Done button text
        "Done", "完成",

        // Setting buttons
        "Setting", "设置",
        "Info", "指令"
    });

    [SerializeField] GameObject m_instructionPanel = null;
    [SerializeField] Text m_settingButtonText = null;
    [SerializeField] Text m_infoButtonText = null;

    [SerializeField] Text m_settingMenuText = null;
    [SerializeField] Text m_languageSettingText = null;
    [SerializeField] Text m_doneButtonText = null;
    [SerializeField] Dropdown m_dropDown = null;

    public void SwitchInstructionPanel()
    {
        m_instructionPanel.SetActive(!m_instructionPanel.activeSelf);
    }

    private void OnLanguageChange(Language lan)
    {
        int index = (int)lan;
        int numLanguage = (int)Language.kTotalNumLanguage;

        m_settingMenuText.text = s_kSettingMenuLanguageLocalization[0 * numLanguage + index];
        m_languageSettingText.text = s_kSettingMenuLanguageLocalization[1 * numLanguage + index];
        m_doneButtonText.text = s_kSettingMenuLanguageLocalization[2 * numLanguage + index];
        m_dropDown.transform.Find("Label").GetComponent<Text>().text = m_dropDown.options[index].text;
        m_settingButtonText.text = s_kSettingMenuLanguageLocalization[3 * numLanguage + index];
        m_infoButtonText.text = s_kSettingMenuLanguageLocalization[4 * numLanguage + index];
    }
}

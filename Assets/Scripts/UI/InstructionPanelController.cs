using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class InstructionPanelController : MonoBehaviour
{
    // Object Names
    private static ReadOnlyCollection<string> s_kInstructionTextLocalization = new ReadOnlyCollection<string> (new string[] {
        
        // Move instruction text
        "Forward", "前进",
        "Back", "后退",
        "Left", "往左",
        "Right", "往右",

        // Zoom instruction text
        "Rotate", "旋转",
        "Interact", "交流",
        "Zoom", "缩放",

        // Fire instruction text
        "Fire Left", "左炮开火",
        "Fire Right", "右炮开火",
        "Climb / Drive", "攀爬 / 上船",
    });

    [SerializeField] Text m_forwardText = null;
    [SerializeField] Text m_backwardText = null;
    [SerializeField] Text m_leftText = null;
    [SerializeField] Text m_rightText = null;
    [SerializeField] Text m_rotateText = null;
    [SerializeField] Text m_interactText = null;
    [SerializeField] Text m_zoomText = null;
    [SerializeField] Text m_fireLeftText = null;
    [SerializeField] Text m_fireRightText = null;
    [SerializeField] Text m_driveText = null;

    private void OnLanguageChange(Language lan)
    {
        int index = (int)lan;
        int numLanguage = (int)Language.kTotalNumLanguage;

        m_forwardText.text = s_kInstructionTextLocalization[0 * numLanguage + index];
        m_backwardText.text = s_kInstructionTextLocalization[1 * numLanguage + index];
        m_leftText.text = s_kInstructionTextLocalization[2 * numLanguage + index];
        m_rightText.text = s_kInstructionTextLocalization[3 * numLanguage + index];
        m_rotateText.text = s_kInstructionTextLocalization[4 * numLanguage + index];
        m_interactText.text = s_kInstructionTextLocalization[5 * numLanguage + index];
        m_zoomText.text = s_kInstructionTextLocalization[6 * numLanguage + index];
        m_fireLeftText.text = s_kInstructionTextLocalization[7 * numLanguage + index];
        m_fireRightText.text = s_kInstructionTextLocalization[8 * numLanguage + index];
        m_driveText.text = s_kInstructionTextLocalization[9 * numLanguage + index];
    }    
}

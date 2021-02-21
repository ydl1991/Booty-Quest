
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    // Object Names
    private static ReadOnlyCollection<string> s_kHudTexts = new ReadOnlyCollection<string> (new string[] {
        "Jack Sparrow", "杰克船长",
        "Ship", "船舱",
        "Bank", "银行"
    });

    public Player m_player = null;
    public HealthComponent m_playerHealth = null;
    [SerializeField] RawImage m_objectHealthBar = null;
    [SerializeField] Text m_healthText = null;
    [SerializeField] Text m_lootOnShipText = null;
    [SerializeField] Text m_lootInBankText = null;
    [SerializeField] Text m_cannonballText = null;
    [SerializeField] Text m_playerNameText = null;
    [SerializeField] Text m_shipText = null;
    [SerializeField] Text m_bankText = null;
    [SerializeField] BankPanelController m_bank = null;

    private float m_lastUpdatedHealth = 0f;

    // Update is called once per frame
    void Update()
    {
        if (m_playerHealth && m_playerHealth.health != m_lastUpdatedHealth)
        {
            if (m_objectHealthBar)
                m_objectHealthBar.transform.localScale = new Vector3(m_playerHealth.healthPercentage, 1, 1);
            
            if (m_healthText)
                m_healthText.text = (m_playerHealth.healthPercentage * 100f).ToString("0.0") + " %";
                
            m_lastUpdatedHealth = m_playerHealth.health;
        }

        if (m_player)
        {
            m_lootOnShipText.text = m_player.GetCurrentCarryingLoots().ToString();
            m_lootInBankText.text = m_bank.lootInBank.ToString();
            m_cannonballText.text = m_player.GetCurrentCannonShots().ToString();
        }
    }

    // Apply language changing effects
    private void OnLanguageChange(Language lan)
    {
        int index = (int)lan;
        int numLanguage = (int)Language.kTotalNumLanguage;

        m_playerNameText.text = s_kHudTexts[0 * numLanguage + index];
        m_shipText.text = s_kHudTexts[1 * numLanguage + index];
        m_bankText.text = s_kHudTexts[2 * numLanguage + index];
    }
}

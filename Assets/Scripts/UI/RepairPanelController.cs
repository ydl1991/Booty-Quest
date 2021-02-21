
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class RepairPanelController : MonoBehaviour
{
    public int repairCost { get; private set; }
    private static int s_kBaseRepairCost = 5;

    private static ReadOnlyCollection<string> s_kRepairPanelLanguageLocalization = new ReadOnlyCollection<string> (new string[] {
        
        "Ship Healthy State:", "船只健康状况:",
        "Repair Cost:", "修理金额:",
        "Repair", "修理",
    });

    private static ReadOnlyCollection<string> s_kNoMoneyErrorMessage = new ReadOnlyCollection<string> (new string[] {
        
        "Unfortunitely, you don't seem to have that many loots.", 
        "很遗憾，看来你这次并没有带那么多钱。"
    });

    private static ReadOnlyCollection<string> s_kNoDamageErrorMessage = new ReadOnlyCollection<string> (new string[] {
        
        "Your ship doesn't need a repair.", 
        "你的船好像并不需要修缮。"
    });

    [SerializeField] Text m_shipHealth;
    [SerializeField] Text m_repairCost;

    [SerializeField] Text m_shipHealthText;
    [SerializeField] Text m_repairCostText;
    [SerializeField] Text m_repairButtonText;
    [SerializeField] GameObject m_player;

    private LootComponent m_playerLoot;
    private HealthComponent m_playerShipHealth;

    void Awake()
    {
        m_playerLoot = m_player.GetComponent<LootComponent>();
        m_playerShipHealth = m_player.GetComponent<HealthComponent>();
    }

    void Update()
    {
        int healthPercent = Mathf.FloorToInt(m_playerShipHealth.healthPercentage * 100f);
        m_shipHealth.text = healthPercent.ToString() + "%";

        if (healthPercent == 100)
            repairCost = 0;
        else
            repairCost = (((100 - healthPercent) / 10) * s_kBaseRepairCost + s_kBaseRepairCost);

        m_repairCost.text = repairCost.ToString();
    }
    
    public void Repair()
    {
        if (repairCost > m_playerLoot.GetSpendableLootCount())
        {
            MessagePanelController.s_instance.SetText(s_kNoMoneyErrorMessage[(int)GameManager.s_instance.gameLanguage]);
            return;
        }

        if (m_playerShipHealth.healthPercentage >= 1f)
        {
            MessagePanelController.s_instance.SetText(s_kNoDamageErrorMessage[(int)GameManager.s_instance.gameLanguage]);
            return;
        }

        m_playerShipHealth.RecoverToFullHealth();
        m_playerLoot.SpendLoot(repairCost);
    }

    private void OnLanguageChange(Language lan)
    {
        int index = (int)lan;
        int numLanguage = (int)Language.kTotalNumLanguage;

        m_shipHealthText.text = s_kRepairPanelLanguageLocalization[0 * numLanguage + index];
        m_repairCostText.text = s_kRepairPanelLanguageLocalization[1 * numLanguage + index];
        m_repairButtonText.text = s_kRepairPanelLanguageLocalization[2 * numLanguage + index];
    }
}

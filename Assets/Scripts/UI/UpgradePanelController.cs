
using System.Collections.ObjectModel;
using UnityEngine.UI;
using UnityEngine;

public class UpgradePanelController : MonoBehaviour
{
    public int upgradeCost { get; private set; }
    private static int s_kBaseUpgradeCost = 100;

    private static ReadOnlyCollection<string> s_kUpgradePanelLanguageLocalization = new ReadOnlyCollection<string> (new string[] {
        
        "Ship Level:", "船只等级:",
        "Ship Speed:", "船只速度:",
        "Maneuver Ability:", "船体操控性:",
        "Cannon Damage:", "炮弹杀伤力:",
        "Cannon Carrying:", "炮弹携带量:",
        "Loot Carrying:", "战利品携带量:",
        "Ship Armor:", "船体防御力:",
        "Upgrade Cost:", "升级金额:",   
        "Upgrade", "升级"
    });

    private static ReadOnlyCollection<string> s_kNoMoneyErrorMessage = new ReadOnlyCollection<string> (new string[] {
        
        "Unfortunitely, you don't seem to have that many loots.", 
        "很遗憾，看来你这次并没有带那么多钱。"
    });

    // Fixed Texts
    [SerializeField] Text m_shipLevelText;
    [SerializeField] Text m_shipSpeedText;
    [SerializeField] Text m_shipManeuverabilityText;
    [SerializeField] Text m_shipCannonDamageText;
    [SerializeField] Text m_shipCannonCarryText;
    [SerializeField] Text m_shipLootCarryText;
    [SerializeField] Text m_shipArmorText;
    [SerializeField] Text m_upgradeCostText;
    [SerializeField] Text m_upgradeButtonText;

    // Player Figures
    [SerializeField] Text m_currentShipLevel;
    [SerializeField] Text m_shipLevelAfterUpgrade;
    [SerializeField] Text m_currentShipSpeed;
    [SerializeField] Text m_shipSpeedAfterUpgrade;
    [SerializeField] Text m_currentShipManeuverability;
    [SerializeField] Text m_shipManeuverabilityAfterUpgrade;
    [SerializeField] Text m_currentShipCannonDamage;
    [SerializeField] Text m_shipCannonDamageAfterUpgrade;
    [SerializeField] Text m_currentShipCannonCarry;
    [SerializeField] Text m_shipCannonCarryAfterUpgrade;
    [SerializeField] Text m_currentShipLootCarry;
    [SerializeField] Text m_shipLootCarryAfterUpgrade;
    [SerializeField] Text m_shipArmorAfterUpgrade;
    [SerializeField] Text m_upgradeCost;
    [SerializeField] GameObject m_player;

    private LootComponent m_playerLoot;
    private Player m_playerUpgradeScript;

    void Awake()
    {
        m_playerLoot = m_player.GetComponent<LootComponent>();
        m_playerUpgradeScript = m_player.GetComponent<Player>();
    }

    void Update()
    {
        int level = m_playerUpgradeScript.shipLevel;
        m_currentShipLevel.text = level.ToString();
        m_shipLevelAfterUpgrade.text = (level + 1).ToString();

        float speed = m_playerUpgradeScript.m_bonusShipForwardSpeed;
        m_currentShipSpeed.text = speed.ToString();
        m_shipSpeedAfterUpgrade.text = (speed + m_playerUpgradeScript.m_speedUpgradeIndicator).ToString();

        float maneuverability = m_playerUpgradeScript.m_bonusShipManeuverability;
        m_currentShipManeuverability.text = maneuverability.ToString();
        m_shipManeuverabilityAfterUpgrade.text = (maneuverability + m_playerUpgradeScript.m_maneuverabilityUpgradeIndicator).ToString();

        int damage = m_playerUpgradeScript.GetCurrentDamage();
        m_currentShipCannonDamage.text = damage.ToString();
        m_shipCannonDamageAfterUpgrade.text = (damage + m_playerUpgradeScript.m_damageUpgradeIndicator).ToString();

        int cannonCarrying = m_playerUpgradeScript.GetMaxCannonShots();
        m_currentShipCannonCarry.text = cannonCarrying.ToString();
        m_shipCannonCarryAfterUpgrade.text = (cannonCarrying + m_playerUpgradeScript.m_cannonCarryUpgradeIndicator).ToString();

        int lootCarrying = m_playerUpgradeScript.GetMaxCarryingLoots();
        m_currentShipLootCarry.text = lootCarrying.ToString();
        m_shipLootCarryAfterUpgrade.text = (lootCarrying + m_playerUpgradeScript.m_lootCarryUpgradeIndicator).ToString();

        m_shipArmorAfterUpgrade.text = "+" + m_playerUpgradeScript.m_armorUpgradeIndicator.ToString();

        upgradeCost = level * s_kBaseUpgradeCost;
        m_upgradeCost.text = upgradeCost.ToString();
    }
    
    public void Upgrade()
    {
        if (upgradeCost > m_playerLoot.GetSpendableLootCount())
        {
            MessagePanelController.s_instance.SetText(s_kNoMoneyErrorMessage[(int)GameManager.s_instance.gameLanguage]);
            return;
        }

        m_playerUpgradeScript.UpgradeShip();
        m_playerLoot.SpendLoot(upgradeCost);
    }

    private void OnLanguageChange(Language lan)
    {
        int index = (int)lan;
        int numLanguage = (int)Language.kTotalNumLanguage;

        m_shipLevelText.text = s_kUpgradePanelLanguageLocalization[0 * numLanguage + index];
        m_shipSpeedText.text = s_kUpgradePanelLanguageLocalization[1 * numLanguage + index];
        m_shipManeuverabilityText.text = s_kUpgradePanelLanguageLocalization[2 * numLanguage + index];
        m_shipCannonDamageText.text = s_kUpgradePanelLanguageLocalization[3 * numLanguage + index];
        m_shipCannonCarryText.text = s_kUpgradePanelLanguageLocalization[4 * numLanguage + index];
        m_shipLootCarryText.text = s_kUpgradePanelLanguageLocalization[5 * numLanguage + index];
        m_shipArmorText.text = s_kUpgradePanelLanguageLocalization[6 * numLanguage + index];
        m_upgradeCostText.text = s_kUpgradePanelLanguageLocalization[7 * numLanguage + index];
        m_upgradeButtonText.text = s_kUpgradePanelLanguageLocalization[8 * numLanguage + index];
    }
}

using System.Collections;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class TradePanelController : MonoBehaviour
{
    public int tradeCost { get; private set; }
    public int tradeRevenue { get; private set; }

    private static ReadOnlyCollection<string> s_kTradePanelLanguageLocalization = new ReadOnlyCollection<string> (new string[] {
        
        "Cannonballs On Hand:", "现有炮弹:",
        "Spendable Loots:", "现有金额:",
        "Enter number..", "输入数量..",
        "Cost / Revenue:", "费用 / 收入:",
        "Buy", "购买",
        "Sell", "出售"
    });

    private static ReadOnlyCollection<string> s_kBuyErrorMessage = new ReadOnlyCollection<string> (new string[] {
        
        "Unfortunitely, you don't seem to have that many loots to pay for the deal.", 
        "很遗憾，看来你这次并没有带那么多钱跟我们交易。",
        "Looks like your ship can't carry that many cannonballs, my friend.", 
        "看来你的船无法携带那么多炮弹, 我的朋友。"
    });

    private static ReadOnlyCollection<string> s_kSellErrorMessage = new ReadOnlyCollection<string> (new string[] {
        
        "You don't have enough cannonballs here.", 
        "你并没有带那么多炮弹。"
    });

    [SerializeField] Text m_cannonballOnHandText;
    [SerializeField] Text m_lootOnHandText;
    [SerializeField] Text m_enterInputText;
    [SerializeField] Text m_costRevenueText;
    [SerializeField] Text m_buyButtonText;
    [SerializeField] Text m_sellButtonText;

    [SerializeField] Text m_cannonballOnHandNumber;
    [SerializeField] Text m_lootOnHandNumber;
    [SerializeField] Text m_inputField;
    [SerializeField] Text m_costText;
    [SerializeField] Text m_revenueText;
    [SerializeField] LootComponent m_playerLoot;
    [SerializeField] CannonComponent m_playerCannon;
    public int m_lootToBuyOneCannonball;
    public int m_lootFromSellOneCannonball;

    void Update()
    {
        if (m_inputField.text == string.Empty)
        {
            tradeCost = 0;
            tradeRevenue = 0;
        }
        else
        {
            int enterAmount = int.Parse(m_inputField.text);
            tradeCost = enterAmount * m_lootToBuyOneCannonball;
            tradeRevenue = enterAmount * m_lootFromSellOneCannonball;
        }

        m_costText.text = tradeCost.ToString();
        m_revenueText.text = tradeRevenue.ToString();

        m_cannonballOnHandNumber.text = m_playerCannon.cannonballCount.ToString();
        m_lootOnHandNumber.text = m_playerLoot.GetSpendableLootCount().ToString();
    }

    public void Buy()
    {
        int index = (int)GameManager.s_instance.gameLanguage;
        int numLanguage = (int)Language.kTotalNumLanguage;

        int enterAmount = int.Parse(m_inputField.text);
        if (tradeCost > m_playerLoot.GetSpendableLootCount())
        {
            MessagePanelController.s_instance.SetText(s_kBuyErrorMessage[0 * numLanguage + index]);
            return;
        }

        if (enterAmount + m_playerCannon.cannonballCount > m_playerCannon.GetMaxCannonCount())
        {
            MessagePanelController.s_instance.SetText(s_kBuyErrorMessage[1 * numLanguage + index]);
            return;
        }
            
        m_playerLoot.SpendLoot(tradeCost);
        m_playerCannon.AddCannon(enterAmount);
    }

    public void Sell()
    {
        int index = (int)GameManager.s_instance.gameLanguage;
        int numLanguage = (int)Language.kTotalNumLanguage;

        int enterAmount = int.Parse(m_inputField.text);
        if (enterAmount > m_playerCannon.cannonballCount)
        {
            MessagePanelController.s_instance.SetText(s_kSellErrorMessage[0 * numLanguage + index]);
            return;
        }
            
        m_playerLoot.EarnSpendableLoot(tradeRevenue);
        m_playerCannon.AddCannon(-enterAmount);
    }

    private void OnLanguageChange(Language lan)
    {
        int index = (int)lan;
        int numLanguage = (int)Language.kTotalNumLanguage;

        m_cannonballOnHandText.text = s_kTradePanelLanguageLocalization[0 * numLanguage + index];
        m_lootOnHandText.text = s_kTradePanelLanguageLocalization[1 * numLanguage + index];
        m_enterInputText.text = s_kTradePanelLanguageLocalization[2 * numLanguage + index];
        m_costRevenueText.text = s_kTradePanelLanguageLocalization[3 * numLanguage + index];
        m_buyButtonText.text = s_kTradePanelLanguageLocalization[4 * numLanguage + index];
        m_sellButtonText.text = s_kTradePanelLanguageLocalization[5 * numLanguage + index];
    }
}

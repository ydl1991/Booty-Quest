using System.Collections;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class BankPanelController : MonoBehaviour
{
    public int lootInBank { get; private set; }

    private static ReadOnlyCollection<string> s_kBankPanelLanguageLocalization = new ReadOnlyCollection<string> (new string[] {
        
        "Loots In Bank:", "银行存款:",
        "Loots On Hand:", "现有金额:",
        "Enter number..", "输入金额..",
        "Withdraw", "取款",
        "Save", "存款"
    });

    private static ReadOnlyCollection<string> s_kSaveErrorMessage = new ReadOnlyCollection<string> (new string[] {
        
        "Unfortunitely, you don't seem to have that many loots.", 
        "很遗憾，看来你这次并没有带那么多钱。"
    });

    private static ReadOnlyCollection<string> s_kWithdrawErrorMessage = new ReadOnlyCollection<string> (new string[] {
        
        "Are you trying to trick me? You don't have that much wealth here.", 
        "别想耍花招！你可没有这么多钱存在这。"
    });

    [SerializeField] Text m_lootInBankText;
    [SerializeField] Text m_lootOnHandText;
    [SerializeField] Text m_enterInputText;
    [SerializeField] Text m_withdrawButtonText;
    [SerializeField] Text m_saveButtonText;

    [SerializeField] Text m_lootInBankNumber;
    [SerializeField] Text m_lootOnHandNumber;

    [SerializeField] Text m_inputField;
    [SerializeField] LootComponent m_playerLoots;

    void Update()
    {
        m_lootInBankNumber.text = lootInBank.ToString();
        m_lootOnHandNumber.text = m_playerLoots.GetLootCount().ToString();
    }

    public void ChangeLootInBank(int amount)
    {
        lootInBank += amount;
    }

    public void Save()
    {
        int saveAmount = int.Parse(m_inputField.text);
        if (saveAmount > m_playerLoots.GetLootCount())
        {
            MessagePanelController.s_instance.SetText(s_kSaveErrorMessage[(int)GameManager.s_instance.gameLanguage]);
            return;
        }

        ChangeLootInBank(saveAmount);
        m_playerLoots.Loot(-saveAmount);
    }

    public void Withdraw()
    {
        int withdrawAmount = int.Parse(m_inputField.text);
        if (withdrawAmount > lootInBank)
        {
            MessagePanelController.s_instance.SetText(s_kWithdrawErrorMessage[(int)GameManager.s_instance.gameLanguage]);
            return;
        }
        
        ChangeLootInBank(-withdrawAmount);
        m_playerLoots.Loot(withdrawAmount);
    }

    private void OnLanguageChange(Language lan)
    {
        int index = (int)lan;
        int numLanguage = (int)Language.kTotalNumLanguage;

        m_lootInBankText.text = s_kBankPanelLanguageLocalization[0 * numLanguage + index];
        m_lootOnHandText.text = s_kBankPanelLanguageLocalization[1 * numLanguage + index];
        m_enterInputText.text = s_kBankPanelLanguageLocalization[2 * numLanguage + index];
        m_withdrawButtonText.text = s_kBankPanelLanguageLocalization[3 * numLanguage + index];
        m_saveButtonText.text = s_kBankPanelLanguageLocalization[4 * numLanguage + index];
    }
}

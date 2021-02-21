using System.Collections;
using System.Collections.ObjectModel;
using UnityEngine;



public class ServiceNPC : MonoBehaviour
{
    public enum ServiceType
    {
        kBanker,
        kRepairer,
        kTrader,
        kShipUpgrader
    }

    private static ReadOnlyCollection<string> s_kWelcomeMessage = new ReadOnlyCollection<string>(new string[] 
    {
        "Hah, you are still alive, with all that big gains! What a lucky guy.",
        "哟, 活着呐，还赚的盆满钵满！真是命硬啊。",
        "Welcome back my friend, let me check your ship!",
        "欢迎回来我的朋友，让我来检查检查你的船！",
        "Come take a good! We got some really good stuff here!",
        "走过路过不要错过！我们这卖的东西可是童叟无欺！",
        "Your ship looks great, but you should get some upgrade!",
        "你的船看起来很不错，但是你该给它升升级了！"
    });

    private static ReadOnlyCollection<string> s_kExitMessage = new ReadOnlyCollection<string>(new string[] 
    {
        "Hope never see you again. If you die, all your treasures will be mine! (Laugh sound..)",
        "真希望你死在外面啊。这么多财宝就归我啦！（哇哈哈哈哈..）",
        "See you next time, my friend! My skill is the best on the island.",
        "下次再见，我的朋友！我的手艺可是这岛上最好的.",
        "Good times are always short! Thank you for your business, we hope to see you again!",
        "愉快的时光总是很短暂！谢谢惠顾，期待下次光临！",
        "Don't forget to check back, we might get you some discount.",
        "别忘了经常过来看看，我们也许还能给你打折。"
    });

    private static ReadOnlyCollection<string> s_kRemindClickMessage = new ReadOnlyCollection<string>(new string[] 
    {
        "Click NPC for service.",
        "点击NPC激活服务。",
    });

    public ServiceType m_type;
    public GameObject m_servicePanel;
    public GameObject m_player;

    private Ray m_ray;
    private RaycastHit m_hit;
    private int m_layer;

    private bool m_onService;
    private bool m_remindMessage;

    void Awake()
    {
        m_onService = false;
        m_remindMessage = false;
        m_layer = 1 << 9;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, m_player.transform.position) <= 5f)
        {
            if (!m_remindMessage)
            {
                m_remindMessage = true;
                MessagePanelController.s_instance.SetText(s_kRemindClickMessage[(int)GameManager.s_instance.gameLanguage]);
            }

            if (Input.GetMouseButtonDown(0))
            {
                m_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(m_ray, out m_hit, 1000f, m_layer))
                {
                    // NPC look at player
                    Quaternion lookRotation = Quaternion.LookRotation((m_player.transform.position - transform.position).normalized);
                    lookRotation.x = 0;
                    lookRotation.z = 0;
                    transform.rotation = lookRotation;

                    OpenServicePanel();            
                }
            }
        }
        else
        {
            m_remindMessage = false;
        }
    }

    public void CloseServicePanel()
    {
        if (!m_onService)
            return;
        
        StartCoroutine(ServiceEnd());
    }

    IEnumerator ServiceEnd()
    {
        yield return null;
        m_servicePanel.SetActive(false);
        Language lan = GameManager.s_instance.gameLanguage;
        MessagePanelController.s_instance.SetText(s_kExitMessage[(int)m_type * (int)Language.kTotalNumLanguage + (int)lan]);
        m_onService = false;
    }

    private void OpenServicePanel()
    {
        if (m_onService)
            return;

        m_onService = true;
        StartCoroutine(ServiceStart());
    }

    IEnumerator ServiceStart()
    {
        Language lan = GameManager.s_instance.gameLanguage;
        MessagePanelController.s_instance.SetText(s_kWelcomeMessage[(int)m_type * (int)Language.kTotalNumLanguage + (int)lan]);
        yield return new WaitForSeconds(0.5f);
        m_servicePanel.SetActive(true);
    }
}

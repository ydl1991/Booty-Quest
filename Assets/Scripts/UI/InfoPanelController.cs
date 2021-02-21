using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelController : MonoBehaviour
{
    // Object Tag Names
    private static ReadOnlyCollection<string> s_kTagNames = new ReadOnlyCollection<string> (new string[] {
        "NavyShip", "MerchantShip"
    });
    private static ReadOnlyCollection<string> s_kTagNamesInChinese = new ReadOnlyCollection<string> (new string[] {
        "海军战舰", "商船"
    });

    // Object Icons
    [SerializeField] Sprite[] m_objectIcons;

    // UI Object References
    [SerializeField] GameObject m_objectInfoPanel = null;
    [SerializeField] Text m_objectName = null;
    [SerializeField] Image m_objectImage = null;
    [SerializeField] Text m_lootText = null;
    [SerializeField] Text m_cannonballText = null;
    
    private Ray m_ray;
    private RaycastHit m_hit;

    private int m_ignoreLayer;

    // Start is called before the first frame update
    void Start()
    {
        m_objectInfoPanel.SetActive(false);
        m_ignoreLayer = ~(1 << 4 | 1 << 2 | 1 << 8);
    }

    // Update is called once per frame
    void Update()
    {
        m_ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // if mouse touch a object, get object info and show
        if (Physics.Raycast(m_ray, out m_hit, 1000f, m_ignoreLayer))
        {
            // check if object tag is one of the tags we care
            int index = s_kTagNames.IndexOf(m_hit.collider.tag);
            if (index != -1)
            {
                if (!m_objectInfoPanel.activeSelf)
                    m_objectInfoPanel.SetActive(true);

                Debug.Log("Index: " + index.ToString());
                AdjustObjectInfoAndImage(index);
            }
            else if (m_objectInfoPanel.activeSelf)
            {
                m_objectInfoPanel.SetActive(false);
            }
        }
        // else if no object is touched and object info panel is on, hide the panel
        else if (m_objectInfoPanel.activeSelf)
        {
            m_objectInfoPanel.SetActive(false);
        }
    }

    private void AdjustObjectInfoAndImage(int index)
    {
        if (m_objectInfoPanel.activeSelf)
        {
            Language lan = GameManager.s_instance.gameLanguage;
            m_objectName.text = lan == Language.kEnglish ? s_kTagNames[index] + " Info" : s_kTagNamesInChinese[index] + " 信息";
            m_objectImage.sprite = m_objectIcons[index];

            Ship shipComp = m_hit.collider.GetComponent<Ship>(); 
            m_lootText.text = shipComp ? shipComp.minDropValue.ToString() + " ~ " + shipComp.maxDropValue.ToString() : "?";
     
            CannonComponent cannon = m_hit.collider.GetComponent<CannonComponent>();
            m_cannonballText.text = cannon ? cannon.cannonballCount.ToString() : "?";
        }
    }
}

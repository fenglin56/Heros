using UnityEngine;
using System.Collections;


public class StroyLinePanel : View {


    public UILabel DialogText;
    public UILabel NpcNameText;

    
    private int m_dialogLength = 0;
    private string m_dialogText;
    private int m_charsPerSecond = 20;
    private float m_NextChar = 0f;
    private bool m_isFlag = false;
    private StroyDialogConfigData m_dialogData;

	// Use this for initialization
	void Start () {
        m_dialogLength = 0;
	}

    // 初始化对话数据
    public void InitDialogPanel(StroyDialogConfigData item)
    {
        m_dialogData = item;

        string name = LanguageTextManager.GetString(item._NpcName);
        string playerName = System.Text.Encoding.UTF8.GetString(PlayerManager.Instance.FindHeroDataModel().m_name);
        NpcNameText.text = name.Contains("{0}") ? playerName : name;
        
        //ClickSignText.text = LanguageTextManager.GetString("IDS_H1_218");
        
        m_dialogLength = 0;
        m_dialogText = LanguageTextManager.GetString(item._Content);

        //NpcIcon.spriteName = item[m_curDialogIndex]._NpcIconName;

        m_isFlag = true;
    }

    /// <summary>
    /// 打字机效果
    /// </summary>
    void FixedUpdate()
    {
        if (m_isFlag)
        {
            if (m_dialogLength < m_dialogText.Length)
            {
                if (m_NextChar <= Time.time)
                {
                    m_charsPerSecond = Mathf.Max(1, m_charsPerSecond);

                    float delay = 1f / m_charsPerSecond;
                    char c = m_dialogText[m_dialogLength];
                    if (c == '。' || c == '\n' || c == '！' || c == '？') delay *= 4f;

                    m_NextChar = Time.time + delay;
                    DialogText.text = m_dialogText.Substring(0, ++m_dialogLength);
                }
            }
            else
            {
                m_isFlag = false;
                //Invoke("Destroy", 0.5f);
            }
        }
       
    }

    void Destroy()
    {
        Destroy(this.gameObject);
    }

    public bool IsShowComplate()
    {
        if (m_dialogLength != m_dialogText.Length)
        {
            m_dialogLength = m_dialogText.Length - 1;
            return true;
        }
        else
            return false;
    }



    protected override void RegisterEventHandler()
    {
        return;
    }
}

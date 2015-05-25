using UnityEngine;
using System.Collections;

namespace Guide
{
    public class NewbieDialogPanel : View {

        //public UILabel NameText;
        //public UILabel DialogText;
        //public UILabel ClickSign;
        //public SpriteSwith NpcIcon;
        //public LocalButtonCallBack DialogBtn;
        //public UILabel TaskName;

        //private int m_curDialogIndex = 0;
        //private GuideConfigData m_curGuideData;

        //// Use this for initialization
        //void Start () {
        //    DialogBtn.SetCallBackFuntion(DialogButtonHandle);
        //}
	
        //// 初始化对话数据
        //public void InitDialogPanel(GuideConfigData item)
        //{
        //    m_curGuideData = item;
        //    NameText.text = LanguageTextManager.GetString(item._NpcName);//System.Text.Encoding.UTF8.GetString( PlayerManager.Instance.FindHeroDataModel().m_name);
        //    DialogText.text = LanguageTextManager.GetString(item._PreDialogList[0]);
        //    ClickSign.text = LanguageTextManager.GetString("IDS_H1_218");
        //    TaskName.text = LanguageTextManager.GetString(item._DialogTitle);
        //    NpcIcon.ChangeSprite(item._NpcIcon);
        //    m_curDialogIndex += 1;
        //}

        ///// <summary>
        ///// Button事件函数
        ///// </summary>
        //void DialogButtonHandle(object obj)
        //{
        //    if (m_curDialogIndex < m_curGuideData._PreDialogList.Length)
        //    {
        //        DialogText.text = LanguageTextManager.GetString(m_curGuideData._PreDialogList[m_curDialogIndex]);
        //        m_curDialogIndex += 1;
        //    }
        //    else
        //    {
        //        OnDestroy();
        //        TownGuideUIManger_V2.Instance.DialogEndHandle();
        //    }
        //}

        //void OnDestroy()
        //{
        //    m_curDialogIndex = 0;
        //    Destroy(this.gameObject);
        //}

        protected override void RegisterEventHandler()
        {
            return;
        }
    }
}

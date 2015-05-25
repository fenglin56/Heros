using UnityEngine;
using System.Collections;
namespace UI.PlayerRoom
{
    public class PlayerRoomBreakPanel : MonoBehaviour
    {
        public UILabel Label_AdditionBySiren;
        public UILabel Label_ForceTime;
        public UILabel Label_BreakthroughGet;
        public UILabel Label_BreakthroughTimes;
        public UILabel Label_Cost;
        public SingleButtonCallBack Button_Practice;
        public SingleButtonCallBack Button_Cancel;

        private bool m_IsCantBreak = false;
        private bool m_IsNotEnoughBind = false;

        private int[] m_guideBtnID = new int[2];

        void Awake()
        {
            Button_Practice.SetCallBackFuntion(BreakHandle, null);
            Button_Cancel.SetCallBackFuntion(ClosePanelHandle, null);
        }

        void Start()
        {
            //TODO GuideBtnManager.Instance.RegGuideButton(Button_Practice.gameObject, UI.MainUI.UIType.MartialArtsRoom, SubType.PlayerRoomBreakPanel, out m_guideBtnID[0]);
            //TODO GuideBtnManager.Instance.RegGuideButton(Button_Cancel.gameObject, UI.MainUI.UIType.MartialArtsRoom, SubType.PlayerRoomBreakPanel, out m_guideBtnID[1]);
        }

        void OnDestroy()
        {
            for (int i = 0; i < m_guideBtnID.Length; i++)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }
        }

        public void ShowPanel()
        {
            transform.localPosition = Vector3.zero;
        }

        public void HidePanel()
        {
            transform.localPosition = new Vector3(0, 0, -1000);
        }

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="sirenAddition">自身妖女加成</param>
        /// <param name="practiceResult">4小时修为</param>
        public void Show(int sirenAddition, int practiceResult)
        {
            Label_AdditionBySiren.text = sirenAddition.ToString() + "%";
            Label_ForceTime.text = "4" + LanguageTextManager.GetString("IDS_H1_514");
            Label_BreakthroughGet.text = practiceResult.ToString();
            var playerData = PlayerManager.Instance.FindHeroDataModel();
            int PLAYER_FIELD_BREAKTHROUGHNUM = playerData.PlayerValues.PLAYER_FIELD_BREAKTHROUGHNUM;
            Label_BreakthroughTimes.text = PLAYER_FIELD_BREAKTHROUGHNUM.ToString() + "/" + CommonDefineManager.Instance.CommonDefine.PlayerRoom_PayTime.ToString();
            if (PLAYER_FIELD_BREAKTHROUGHNUM <= 0)
            {
                m_IsCantBreak = true;
            }
            else
            {
                m_IsCantBreak = false;
            }
            int bindPay = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY;
            int break_cost = CommonDefineManager.Instance.CommonDefine.PlayerRoom_Pay;
            if (bindPay < break_cost)
            {
                Label_Cost.color = Color.red;
                m_IsNotEnoughBind = true;
            }
            else
            {
                Label_Cost.color = Color.white;
                m_IsNotEnoughBind = false;
            }
            Label_Cost.text = break_cost.ToString();
            this.ShowPanel();
        }

        void BreakHandle(object obj)
        {
            if (m_IsCantBreak)
            {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
                MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_H1_481"), 1);
            }
            else
            {
                if (m_IsNotEnoughBind)
                {
                    SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
                    MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_H2_44"), 1);
                }
                else
                {
                    SoundManager.Instance.PlaySoundEffect("Sound_Button_Pay");
                    NetServiceManager.Instance.EntityService.SendBreakInfo();
                }
            }            
            HidePanel();
        }

        void ClosePanelHandle(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            HidePanel();
        }
    }
}

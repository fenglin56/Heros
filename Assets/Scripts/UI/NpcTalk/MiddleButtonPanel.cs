using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI
{

    public class MiddleButtonPanel : MonoBehaviour
    {

        public UISprite Background;
        public GameObject NpcPanelBtn;

        private List<NpcPanelButton> m_buttonList = new List<NpcPanelButton>();

        private SMsgInteractCOMMONPackage sMsgInteractCOMMONPackage;

        void Awake()
        {
            Background.enabled = false;
        }

        public void CreateMiddleButton(SMsgInteractCOMMONPackage sMsgInteractCOMMONPackage)
        {
            this.sMsgInteractCOMMONPackage = sMsgInteractCOMMONPackage;
            var MyButtonList = sMsgInteractCOMMONPackage.sMsgInteractCOMMONBtn_SC;
            Background.enabled = true;
            Background.transform.localScale = new Vector3(285,75*MyButtonList.Length,1);
            if (m_buttonList.Count > 0)
            {
                foreach (var child in m_buttonList)
                {
                    if (child != null && child.gameObject != null)
                    {
                        child.GetComponent<NpcPanelButton>().OnDestroy();
                        Destroy(child.gameObject);
                    }
                }
                m_buttonList.Clear();
            }
            for (int i = 0; i < MyButtonList.Length; i++)
            {
                var button = CreatObjectToNGUI.InstantiateObj(NpcPanelBtn, transform).GetComponent<NpcPanelButton>();
                button.transform.localPosition = new Vector3(100, 60 -(i * 70), -1);
                //button.InitButton();
                m_buttonList.Add(button);

            }
            SetLocalButtonAttribute(sMsgInteractCOMMONPackage);
        }
        void SetLocalButtonAttribute(SMsgInteractCOMMONPackage sMsgInteractCOMMONPackage)//设置每个按钮的属性
        {
            SMsgInteractCOMMONBtn_SC[] BtnListAttribute = sMsgInteractCOMMONPackage.sMsgInteractCOMMONBtn_SC;
            for (int i = 0; i < BtnListAttribute.Length; i++)
            {
                if (m_buttonList[i] != null)
                {

                    switch (BtnListAttribute[i].byBtnType)
                    {
                        case 1:
                            NPCSpecialConfigData nSCData = NPCConfigManager.Instance.NPCSpecialConfigList.FirstOrDefault(P => P._NPCID == sMsgInteractCOMMONPackage.sMsgInteractCOMMON_SC.dwNPCID
                             && P.Parameters == BtnListAttribute[i].dwParam2);
                           
                            TraceUtil.Log("查找ButtonID:" + BtnListAttribute[i].dwParam2);
                            //m_buttonList[i].SetButtonText(LanguageTextManager.GetString(nSCData._FunctionDesc));
                            if(nSCData!=null)
                            {
                            m_buttonList[i].InitButton(nSCData.ShopIcon,nSCData.ShopTitle);
                            m_buttonList[i].SetCallBackFuntion(OnSpecialButtonTapped, BtnListAttribute[i]);
                            }
                            break;
                        case 2:
                            m_buttonList[i].SetButtonText("2，任务");
                            break;
                        case 3:
                            m_buttonList[i].SetButtonText("3，GM");
                            m_buttonList[i].InitButton("","");
                            m_buttonList[i].SetCallBackFuntion(OnGMButtonTapped, null);
                            break;
                        default:
                            break;

                    }
                }
            }
        }

        void OnSpecialButtonTapped(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_TownMain_Change");
            SMsgInteractCOMMONBtn_SC att = (SMsgInteractCOMMONBtn_SC)obj;

            SMsgInteractCOMMON_CS msgInteract;
            msgInteract.dwNPCID = this.sMsgInteractCOMMONPackage.sMsgInteractCOMMON_SC.dwNPCID;
            msgInteract.byOperateType = att.byBtnType;
            msgInteract.dwParam1 = att.dwParam1;
            msgInteract.dwParam2 = att.dwParam2;
            msgInteract.byIsContext = 0;

            SMsgInteractCOMMONContext_CS msgContext;
            msgContext.szContext = new byte[32];
            NetServiceManager.Instance.InteractService.SendInteractCOMMON(msgInteract, msgContext);

            UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowTopCommonUI, null);
        }

        void OnGMButtonTapped(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_TownMain_Change");
            CheatManager.Instance.ShowGM();
        }


    }
}
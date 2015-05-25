using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace UI.MainUI
{

    public class TrialsEctypePanelManager : BaseUIPanel
    {

        public GameObject TrialsEctypePanelPrefab;
        public GameObject UIBottomBtnPrefab;
        public Transform CreatBottomBtnPoint;
        public Transform Grid;
        public UILabel PanelTitle;
        private CommonUIBottomButtonTool commonUIBottomButtonTool;

        List<TrialsEctypePanelList> TrialsEctypePanelListArray = new List<TrialsEctypePanelList>();

        public SMSGEctypeTrialsInfo_SC sMSGEctypeTrialsInfo_SC { get; private set; }
        private int m_guideBtnID = 0;

        public GameObject CommonToolPrefab;
        void Awake()
        {
            if (UI.MainUI.MainUIController.Instance.IsShowCommonTool) { CreatObjectToNGUI.InstantiateObj(CommonToolPrefab, transform); }
            //MainUIController.Instance.SetPanelActivEvent += new MainUIController.SetPanelDelegate(SetPanelActive);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.TrialsEctypeList,UnlockPanel);
        }

        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.TrialsEctypeList, UnlockPanel); 
        }

        //public void SetPanelActive(int[] UIStatus)
        //{
        //    switch ((UIType)UIStatus[0])
        //    {
        //        case UIType.TrialsEctypePanel:
        //            ShowPanel();
        //            break;
        //        default:
        //            ClosePanel(null);
        //            break;
        //    }
        //}

        public override void Show(params object[] value)
        {
            SoundManager.Instance.StopBGM(0.0f);
            SoundManager.Instance.PlayBGM("Music_UIBG_TrialsEctype", 0.0f);
            if (commonUIBottomButtonTool == null)
            {
                commonUIBottomButtonTool = CreatObjectToNGUI.InstantiateObj(UIBottomBtnPrefab, CreatBottomBtnPoint).GetComponent<CommonUIBottomButtonTool>();
                ShowBottomBtn();
            }
            else
            {
                commonUIBottomButtonTool.ShowAnim();
            }
            InitPanel();
            NetServiceManager.Instance.EctypeService.SendGetTrialsEctypePanelInfo();
            TraceUtil.Log("发送打开试炼 副本列表请求");
            base.Show(value);
        }

        public override void Close()
        {
            if(!IsShow)
                return;
            GameManager.Instance.PlaySceneMusic();
            UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
            base.Close();
        }


        void InitPanel()
        {
            LoadingUI.Instance.Show();
            Grid.ClearChild();
            TrialsEctypePanelListArray.Clear();
            EctypeConfigManager.Instance.EctypeSelectConfigFile._dataTable.ApplyAllItem(P=>P.InitectContainer());
            int MaxPanel = 0;
            var ectypeList = EctypeConfigManager.Instance.EctypeContainerConfigFile.ectypeContainerDataList.Where(P=>P.lEctypeType == 5);
            ectypeList.ApplyAllItem(P=>MaxPanel = int.Parse(P.lEctypePos[0])>MaxPanel?int.Parse(P.lEctypePos[0]):MaxPanel);
            for (int i = 0; i < MaxPanel; i++)
            {
                TrialsEctypePanelList trialsEctypePanelList = CreatObjectToNGUI.InstantiateObj(TrialsEctypePanelPrefab, Grid).GetComponent<TrialsEctypePanelList>();
                trialsEctypePanelList.InitPanel(i+1,this);
                TrialsEctypePanelListArray.Add(trialsEctypePanelList);
            }
        }

        void Destroy()
        {
            //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
        }

        void UnlockPanel(object obj)
        {
            TraceUtil.Log("角色试炼次数：" + PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_SHILIAN_TIMES);
            LoadingUI.Instance.Close();
            sMSGEctypeTrialsInfo_SC = (SMSGEctypeTrialsInfo_SC)obj;
            sMSGEctypeTrialsInfo_SC.sInfos.ApplyAllItem(P => TraceUtil.Log("收到解锁副本："+P.dwEctypeID+","+P.byDiff));
            //TraceUtil.Log("收到解锁副本：" + sMSGEctypeTrialsInfo_SC.sInfos[0].dwEctypeID +","+ sMSGEctypeTrialsInfo_SC.sInfos[0].byDiff);
            //PanelTitle.SetText(sMSGEctypeTrialsInfo_SC.dwTotalTimes.ToString());
            int ShilianLeftTime =CommonDefineManager.Instance.CommonDefine.TrialsEctype_FreeTime - PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_SHILIAN_TIMES;
            PanelTitle.SetText(string.Format(LanguageTextManager.GetString("IDS_H1_486"), ShilianLeftTime < 0 ? 0 : ShilianLeftTime));
            Dictionary<int, SEctypeTrialsInfo> EctypeDataList = new Dictionary<int, SEctypeTrialsInfo>();
            sMSGEctypeTrialsInfo_SC.sInfos.ApplyAllItem(P=>EctypeDataList.Add(EctypeConfigManager.Instance.EctypeSelectConfigList[(int)P.dwEctypeID].VectContainerList[P.byDiff],P));
            TrialsEctypePanelListArray.ApplyAllItem(P => P.UnLockPanel(EctypeDataList));
        }

        void ShowBottomBtn()
        {
            CommonBtnInfo btnInfo = new CommonBtnInfo(0, "JH_UI_Button_1116_06", "JH_UI_Button_1116_00", OnBackButtonTapped);
            commonUIBottomButtonTool.Show(new List<CommonBtnInfo>() { btnInfo});

            var btnInfoComponent = commonUIBottomButtonTool.GetButtonComponent(btnInfo);
            //if (btnInfoComponent != null)
                //TODO GuideBtnManager.Instance.RegGuideButton(btnInfoComponent.gameObject, UIType.TrialsEctypePanel, SubType.ButtomCommon, out m_guideBtnID);
        }

        void OnBackButtonTapped(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            Close();
            CleanUpUIStatus();
        }

        public void SendJoinEctypeToSever(int ectypeID)
        {
            EctypeSelectConfigData ectypeSelectConfigData = null;
            int diff = 0;
            foreach (var child in EctypeConfigManager.Instance.EctypeSelectConfigFile._dataTable)
            {
                child.InitectContainer();
                foreach (var m_id in child.VectContainerList)
                {
                    if (m_id.Value == ectypeID)
                    {
                        ectypeSelectConfigData = child;
                        diff = m_id.Key;
                        break;
                    }
                }
                if (ectypeSelectConfigData != null)
                    break;
            }
            SMSGEctypeRequestCreate_CS sMSGEctypeRequestCreate_CS = new SMSGEctypeRequestCreate_CS()
			{
				//TODO:进入技能协议有更改，去掉副本iD和难度,需要从新修改
//                uidEntity = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity,
//                dwEctypeId = (int)ectypeSelectConfigData._lEctypeID,
//                byDifficulty = (byte)diff,
            };
            NetServiceManager.Instance.EctypeService.SendEctypeRequest(sMSGEctypeRequestCreate_CS);
//            TraceUtil.Log("发送进入副本请求：" + sMSGEctypeRequestCreate_CS.dwEctypeId + "," + sMSGEctypeRequestCreate_CS.byDifficulty);
        }
    }
}
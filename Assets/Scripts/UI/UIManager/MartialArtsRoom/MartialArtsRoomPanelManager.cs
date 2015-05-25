using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.MainUI
{

    public class MartialArtsRoomPanelManager : BaseUIPanel
    {
        public MartialArtsMainPanel MartialArtsMainPanel;
        public MartialArtsRoomListPanel MartialArtsRoomListPanel;


        public GameObject CommonToolPrefab;
        void Awake()
        {
            if (UI.MainUI.MainUIController.Instance.IsShowCommonTool) { CreatObjectToNGUI.InstantiateObj(CommonToolPrefab, transform); }
            //MainUIController.Instance.SetPanelActivEvent += new MainUIController.SetPanelDelegate(SetPanelActive);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.EctypeRoleFull, ShowRoleFullMsg);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.EctypeCantFindRoom, ShowCandFindRoomMsg);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.EctypeLevelError, ShowLevelErrorMsg);
            MartialArtsMainPanel.Init(this);
            MartialArtsRoomListPanel.Init(this);
        }

        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EctypeRoleFull, ShowRoleFullMsg);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EctypeCantFindRoom, ShowCandFindRoomMsg);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EctypeLevelError, ShowLevelErrorMsg);
        }

        //public void SetPanelActive(int[] UIStatus)
        //{
        //    switch ((UIType)UIStatus[0])
        //    {
        //        case UIType.MartialArtsRoom:
        //            Show();
        //            break;
        //        default:
        //            //Close(null);
        //            MartialArtsMainPanel.Close(null);
        //            break;
        //    }
        //}

        public override void Show(params object[] value)
        {
            MartialArtsMainPanel.Show();
            MartialArtsRoomListPanel.Close(null);
            base.Show(value);
        }

        public override void Close()
        {
            if (!IsShow)
                return;
            MartialArtsMainPanel.Close(null);
            base.Close();
        }

        public void ShowRoomListPanel()
        {
            MartialArtsRoomListPanel.Show();
        }
        /// <summary>
        /// 获取房间列表
        /// </summary>
        public void SendGetRoomListMsgToSever()
        {
            NetServiceManager.Instance.EctypeService.SendGetMartialArtsRoomListMsg();
        }

        public void CreatNewRoom()
        {
            TraceUtil.Log("新建房间");
            EctypeSelectConfigData ectypeSelectConfigData = EctypeConfigManager.Instance.EctypeSelectConfigList[10001];
            SMSGEctypeRequestCreate_CS sMSGEctypeRequestCreate_CS = new SMSGEctypeRequestCreate_CS()
			{
				//TODO:进入技能协议有更改，去掉副本iD和难度,需要从新修改
//                uidEntity = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity,
//                dwEctypeId = ectypeSelectConfigData._lEctypeID,
//                byDifficulty = (byte)ectypeSelectConfigData._vectDifficulty[0],
            };
            NetServiceManager.Instance.EctypeService.SendEctypeRequest(sMSGEctypeRequestCreate_CS);
        }

        public void QuickJoin()
        {
            TraceUtil.Log("快速加入");
            NetServiceManager.Instance.EctypeService.SendQuickJoinMartialArtsRoom();
        }

        public void SendJoinRoomMsgToSever(uint roomID)
        {
            TraceUtil.Log("加入房间");
            NetServiceManager.Instance.EctypeService.SendJoinMartialArtsRoom(roomID);
            LoadingUI.Instance.Show();
        }

        void ShowRoleFullMsg(object obj)
        {
            if (!IsShow)
                return;
            LoadingUI.Instance.Close();
            MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_477"),1);
        }

        void ShowLevelErrorMsg(object obj)
        {
            if (!IsShow)
                return;
            LoadingUI.Instance.Close();
            EctypeLevelError ectypeLevelError = (EctypeLevelError)obj;
            MessageBox.Instance.ShowTips(3, string.Format(LanguageTextManager.GetString("IDS_H1_478"), ectypeLevelError.LevelList[0], ectypeLevelError.LevelList[1]), 1);
        }

        void ShowCandFindRoomMsg(object obj)
        {
            if (!IsShow)
                return;
            LoadingUI.Instance.Close();
            MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_479"), 1);
        }

        public void OnCloseBtnClick()
        {
            Close();
            CleanUpUIStatus();
        }
    }
}
using UnityEngine;
using System.Collections;

namespace UI.MainUI
{
    public enum EctypeGradeType {Error = 0,C,B,A,S,SS,SSS }
    public class EctypePanleManager : BaseUIPanel
    {

        public GameObject EctypePrefab;
		private EctypePanel_V5 ectypePanelScript;

        void Start()
        {
            //UIEventManager.Instance.RegisterUIEvent(UIEventType.EctypeUIInfo, ShowEctypePanel);
            //UIEventManager.Instance.RegisterUIEvent(UIEventType.UpdateYaoqiValue, UpdateYaoqiValueHandle);
        }

		public override void Show(params object[] value)
        {
            //SendGetEctypePanelInfoMsg();
//            SoundManager.Instance.StopBGM(0.0f);
//            SoundManager.Instance.PlayBGM("Music_UIBG_EctypeDifficulty", 0.0f);

			//add by lee
			if(JudgeAndExitTeam())
			{
				return;
			}

            base.Show(value);
			//jamfing
			ShowEctypePanel (value);
        }

		private bool JudgeAndExitTeam()
		{
			bool isReturn = false;
			if(TeamManager.Instance.IsTeamExist())
			{
				if(TeamManager.Instance.GetCurrentEctypeType() == 0)
				{
					MainUIController.Instance.OpenMainUI(UIType.TeamInfo,1);
					isReturn = true;
				}
			}
			return isReturn;
		}

        public void OnClosePanelBtnClick()
        {
            CleanUpUIStatus();
            Close();
        }

        public override void Close()
        {
            if (!IsShow)
                return;
            base.Close();
        }
		//屏蔽掉了
        void SendGetEctypePanelInfoMsg()
        {            
            TraceUtil.Log("发送打开副本界面请求");        
			long UID = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity;
			NetServiceManager.Instance.EctypeService.SendEctypeGoBattleRequest(UID);
			LoadingUI.Instance.Show();               
        }

		void ShowEctypePanel(params object[] value)//object obj)
        {
            //TraceUtil.Log("收到打开副本界面请求");
            //LoadingUI.Instance.Close();

            //SMSGEctypeSelect_SC sMSGEctypeSelect_SC = (SMSGEctypeSelect_SC)obj;
//            if (PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity != sMSGEctypeSelect_SC.uidEntity)
//                return;
			int ectypeID = -1;
			if (value != null && value.Length > 0) {
				ectypeID = (int)value[0];	
			}
			if (ectypePanelScript == null)
			{
				ectypePanelScript = CreatObjectToNGUI.InstantiateObj(EctypePrefab, transform).GetComponent<EctypePanel_V5>();
				//ectypePanelScript.Init(this);
			}
			ectypePanelScript.Show(EctypeModel.Instance.sMSGEctypeSelect_SC,this,ectypeID);
        }

//        void UpdateYaoqiValueHandle(object obj)
//        {
//            SMSGEctypeYaoqiProp_SC yaoqiProp = (SMSGEctypeYaoqiProp_SC)obj;
//            ectypePanelScript.UpdateYaoqiProp(yaoqiProp);
//        }

        void OnDestroy()
        {
            //UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EctypeUIInfo, ShowEctypePanel);
            //UIEventManager.Instance.RemoveUIEventHandel(UIEventType.UpdateYaoqiValue, UpdateYaoqiValueHandle);
        }


    }
}
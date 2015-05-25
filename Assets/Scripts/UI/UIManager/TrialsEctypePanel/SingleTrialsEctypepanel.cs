using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI
{

    public class SingleTrialsEctypepanel : MonoBehaviour
    {
        public GameObject CostLabelPreafab;
        public GameObject LockIcon;
        public GameObject UnLockIcon;
        public SingleButtonCallBack ShowEctypeGetAtrributeBtn;
        public SingleButtonCallBack GoButton;
        public SingleButtonCallBack BtnCostTypeIcon;
        public UILabel LockLevelLabel;
       // public UISprite Background;
        public Transform CreatBackgroundPoint;

        public SingleTrialsAtbPanel singleTrialsAtbPanel;

        public EctypeContainerData LocalEctypeData { get; private set; }

        public TrialsEctypePanelList myParent { get; private set; }

        public SEctypeTrialsInfo UnlockData;

        public EctypeContainerIconPrefabDataBase EctypeIconDataBase;

        private int[] m_guideBtnID = new int[2];

        private bool IsLock = true;
        private bool IsUseUp = false;
        
        public void InitPanel(int PositionID, EctypeContainerData LocalEctypeData, TrialsEctypePanelList myParent)
        {
            ShowEctypeGetAtrributeBtn.SetCallBackFuntion(OnShowGetAtbBtnClick);
            GoButton.SetPressCallBack(OnPress);
            transform.localPosition = new Vector3((PositionID-2)*250,0,0);
            this.myParent = myParent;
            this.LocalEctypeData = LocalEctypeData;
            CreatBackgroundPoint.ClearChild();
            CreatObjectToNGUI.InstantiateObj(EctypeIconDataBase.GetIconData(LocalEctypeData.lEctypeContainerID,LocalEctypeData.lDifficulty).EctypeIconPrefab,CreatBackgroundPoint);
            //Background.spriteName = LocalEctypeData.lEctypeIcon;
            LockLevelLabel.SetText(string.Format("{0}\n{1}", LanguageTextManager.GetString("IDS_H1_483"), string.Format(LanguageTextManager.GetString("IDS_H1_484"), LocalEctypeData.lMinActorLevel)));
            SetPanelLockActive(true);

            //TODO GuideBtnManager.Instance.RegGuideButton(ShowEctypeGetAtrributeBtn.gameObject, UIType.TrialsEctypePanel, SubType.ShowAtrribute, out m_guideBtnID[0]);
            //TODO GuideBtnManager.Instance.RegGuideButton(GoButton.gameObject, UIType.TrialsEctypePanel, SubType.GoButton, out m_guideBtnID[1]);
        }

        public void UnlockPanel(Dictionary<int, SEctypeTrialsInfo> EctypeDataList)
        {
            if (EctypeDataList.TryGetValue(LocalEctypeData.lEctypeContainerID, out UnlockData))
            {
                SetPanelLockActive(false);
                SetGoBtnStatus();
            }
        }

        void OnDestroy()
        {
            for (int i = 0; i < m_guideBtnID.Length; i++)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }
        }

        void OnPress(bool isPressed)
        {
            if (IsLock||IsUseUp)
                return;
            if (isPressed)
            {
                int joinTime = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_SHILIAN_TIMES;
                if (joinTime < 3)
                {
                    SendGoBattleToServer(null);
                }
                else
                {
                    CheckCanJoin();
                }
            }
            GoButton.BackgroundSprite.enabled = isPressed;
        }

        bool CheckCanJoin()
        {
            bool flag = false;
            switch (LocalEctypeData.lCostType)
            {
                case 1:
                    int CostActiveLife = int.Parse(LocalEctypeData.lCostEnergy);
                    int Cost = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE;
                    flag = CostActiveLife <= Cost;
                    if (!flag)
                    {
                        MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_455"), 1);
                    }
                    break;
                case 2:
                    int CostMoney = int.Parse(LocalEctypeData.lCostEnergy);
                    int PayMoney = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY;
                    flag = CostMoney <= PayMoney;
                    if (!flag)
                    {
                        MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_44"), 1);
                    }
                    break;
                case 3:
                    int CostGold = int.Parse(LocalEctypeData.lCostEnergy);
                    int BINDPAY = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
                    flag = BINDPAY >= CostGold;
                    if (!flag)
                    {
                        MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_231"), 1);
                    }
                    break;
                default:
                    break;
            }
            if (flag)
            {
                SoundManager.Instance.PlaySoundEffect("sound0057");
                ShowCostTipsAndJoin(); 
            }
            return flag;
        }

        void ShowCostTipsAndJoin()
        {
            int costNumber = int.Parse(LocalEctypeData.lCostEnergy);
            SingleButtonCallBack Tips = CreatObjectToNGUI.InstantiateObj(CostLabelPreafab, GoButton.transform).GetComponent<SingleButtonCallBack>();
            Vector3 fromPoint = new Vector3(0, 50, -30);
            Vector3 toPoint = new Vector3(0, 0, -30);
            TweenPosition.Begin(Tips.gameObject, 0.5f, fromPoint, toPoint, SendGoBattleToServer);
            TweenAlpha.Begin(Tips.gameObject, 0.5f, 1, 0, null);
            Tips.SetButtonBackground(LocalEctypeData.lCostType);
            Tips.SetButtonText(string.Format("-{0}",costNumber));
        }

        void SendGoBattleToServer(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Fight");
			JudgeAndExitTeam();//add by lee
            myParent.MyParent.SendJoinEctypeToSever(LocalEctypeData.lEctypeContainerID);
        }

		private void JudgeAndExitTeam()
		{
			if(TeamManager.Instance.IsTeamExist())
			{
				if(TeamManager.Instance.GetCurrentEctypeType() == 0 || TeamManager.Instance.GetCurrentEctypeType() == 9)
				{
					var playerData = PlayerManager.Instance.FindHeroDataModel();
					var teamSmg = TeamManager.Instance.MyTeamProp;
					if(playerData.ActorID == teamSmg.TeamContext.dwCaptainId)
					{
						NetServiceManager.Instance.TeamService.SendTeamDisbandMsg(new SMsgTeamDisband_CS{
							dwActorID = (uint)playerData.ActorID,
							dwTeamID = teamSmg.TeamContext.dwId
						});
					}
					else
					{
						NetServiceManager.Instance.TeamService.SendTeamMemberLeaveMsg(new SMsgTeamMemberLeave_SC(){
							dwActorID = (uint)playerData.ActorID,
							dwTeamID = teamSmg.TeamContext.dwId
						});
					}
				}
			}
		}

        void SetGoBtnStatus()
        {
            BtnCostTypeIcon.gameObject.SetActive(false);
            GoButton.SetImageButtonComponentActive(true);
            int joinTime = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_SHILIAN_TIMES;
            if (joinTime < CommonDefineManager.Instance.CommonDefine.TrialsEctype_FreeTime)
            {
                GoButton.SetButtonBackground(1);
            }
            else if (joinTime < (CommonDefineManager.Instance.CommonDefine.TrialsEctype_FreeTime + CommonDefineManager.Instance.CommonDefine.TrialsEctype_PayTime))
            {
                GoButton.SetButtonBackground(2);
                BtnCostTypeIcon.gameObject.SetActive(true);
                BtnCostTypeIcon.SetButtonBackground(LocalEctypeData.lCostType);
                BtnCostTypeIcon.SetButtonText(LocalEctypeData.lCostEnergy);
            }
            else
            {
                IsUseUp = true;
                GoButton.SetImageButtonComponentActive(false);
                GoButton.SetButtonBackground(3);
            }
        }

        void SetPanelLockActive(bool flag)
        {
            IsLock = flag;
            LockIcon.SetActive(flag);
            UnLockIcon.SetActive(!flag);
            GoButton.gameObject.SetActive(flag?false:true);
        }

        void OnShowGetAtbBtnClick(object obj)
        {
            TraceUtil.Log("OnShowGetAtbBtnClick:"+IsLock.ToString());
            if (IsLock)
                return;
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            bool IsShow = !singleTrialsAtbPanel.IsShow;
            ShowEctypeGetAtrributeBtn.SetButtonBackground(IsShow?2:1);
            if (IsShow)
            {
                singleTrialsAtbPanel.Show(LocalEctypeData, this.UnlockData.byClearance == 1);
            }
            else
            {
                singleTrialsAtbPanel.Close();
            }
        }

    }
}
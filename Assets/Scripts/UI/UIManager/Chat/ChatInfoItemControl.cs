using UnityEngine;
using System.Collections;

namespace Chat
{

	public delegate void SelectedChatTargetDelegate(object talkerInfo, Transform boxPoint);

    public class ChatInfoItemControl : MonoBehaviour
    {
        public LocalButtonCallBack Button_Select;
        public LocalButtonCallBack Button_Join;
        public UILabel Label_chat;
        public Transform TalkerBoxPoint;

		public SpriteSwith Swith_CannelIcon;
		public SpriteSwith Swith_VipIcon;

		private SelectedChatTargetDelegate m_SelectedDelegate;

        private int uiChatActorID = 0;
        private int friendTeamID = 0;
        private string sChatName = "";

        //void Awake()
        //{
        //    Button_Select.SetCallBackFuntion(BeSelect, null);
        //}

        public void Init(int channel, int actorID, string name, int vipLevel, string chat, int chatType, int fTeamID, SelectedChatTargetDelegate selectedChatTargetDelegate)
        {
            this.uiChatActorID = actorID;
            this.friendTeamID = fTeamID;
            
			Swith_CannelIcon.ChangeSprite(channel);

			if(vipLevel>0)
			{
				Swith_VipIcon.ChangeSprite(vipLevel);
				Swith_VipIcon.gameObject.SetActive(true);
			}
			else
			{
				Swith_VipIcon.gameObject.SetActive(false);
			}

			Label_chat.text = chat;
            sChatName = name;
            this.m_SelectedDelegate = selectedChatTargetDelegate;



            var playerData = PlayerManager.Instance.FindHeroDataModel();
            if (actorID != playerData.ActorID)
            {
				if(m_SelectedDelegate != null)
				{
					Button_Select.SetCallBackFuntion(BeSelect, null);
				}                
                //Label_chat.color = ChatPanelUIManager.ColorOther;
            }
            else
            {
                //Label_chat.color = ChatPanelUIManager.ColorMy;
            }

			if(Button_Join!=null)
			{
				if (chatType == 1)
				{
					Button_Join.gameObject.SetActive(true);
					Button_Join.SetCallBackFuntion(OnJoinHandle, null);
				}
				else
				{
					Button_Join.gameObject.SetActive(false);
				}
			}            
        }

        void BeSelect(object obj)
        {
			SoundManager.Instance.PlaySoundEffect("Sound_Button_ChatChoice");
            TalkerInfo talkerInfo = new TalkerInfo()
            {
                ActorID = this.uiChatActorID,
                Name = this.sChatName
            };
            m_SelectedDelegate(talkerInfo, TalkerBoxPoint);
        }

        void OnJoinHandle(object obj)
        {
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamJoin");
            var playerData = PlayerManager.Instance.FindHeroDataModel();  

			if(TeamManager.Instance.IsTeamExist())
			{
				if(TeamManager.Instance.MyTeamProp.TeamContext.dwId == friendTeamID)
				{
					UI.MainUI.MainUIController.Instance.OpenMainUI(UI.MainUI.UIType.TeamInfo,1);
				}
				else
				{
					TeamManager.Instance.ShowLeaveTeamTip(()=>{
						//发送解散消息
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
						//设置回调 收到解散消息就发送加入队伍申请
						TeamManager.Instance.SetWaitExitTeamAction(()=>{
							NetServiceManager.Instance.TeamService.SendTeamMemberJoinMsg(new SMsgTeamMemberJoin_CS()
							                                                             {
								byJoinAnswer = 1,
								byJoinType = 2,
								dwActorID = (uint)PlayerManager.Instance.FindHeroDataModel().ActorID,
								dwTeamID = (uint)friendTeamID
							});
						});
					});
				}

			}
			else
			{
				NetServiceManager.Instance.TeamService.SendTeamMemberJoinMsg(new SMsgTeamMemberJoin_CS()
				                                                             {
					byJoinAnswer = 1,
					byJoinType = 2,
					dwActorID = (uint)PlayerManager.Instance.FindHeroDataModel().ActorID,
					dwTeamID = (uint)friendTeamID
				});
			}
						 
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }

    public class TalkerInfo
    {
        public int ActorID;
        public string Name;
    }
}

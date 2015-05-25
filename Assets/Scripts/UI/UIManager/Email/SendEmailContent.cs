using UnityEngine;
using System.Collections;
using UI.Friend;
using System.Text;

namespace UI.MainUI
{
public class SendEmailContent : BaseSubUiPanel {
		public SingleButtonCallBack ChoseFriendBtn;
		public UIInput FriendInput;
		public UIInput TitleInput;
		public UIInput ContentInput;
		private SEmailWrite_CS EmailWrite=new SEmailWrite_CS();
		private uint FriendId;
        public UILabel Cost_des;
        private int Cost;
       // public UILabel Cost_num;
		void Awake ()
		{
			ChoseFriendBtn.SetCallBackFuntion(ChoseFriend);
            Cost = CommonDefineManager.Instance.CommonDefine.SendMailConsumption;
            RegisterEventHandler();
		}

        public override void ShowPanel()
        {
            base.ShowPanel();
//            if(!PlayerManager.Instance.IsMoneyEnough(Cost))
//            {
//                Cost_des.SetText(string.Format( LanguageTextManager.GetString("IDS_I22_31"),UI.NGUIColor.SetTxtColor(Cost,TextColor.red)));
//            }
//            else
//            {
//                Cost_des.SetText(string.Format( LanguageTextManager.GetString("IDS_I22_31"),UI.NGUIColor.SetTxtColor(Cost,TextColor.ChatYellow)));
//            }
            UpdateCost(null);
        }
        public override void HidePanel()
        {
            base.HidePanel();
            ClearALL();
        }

		void ChoseFriend(object obj)
		{
            SoundManager.Instance.PlaySoundEffect("Sound_Button_MailSelectRecipient");
			EmailInfoPanelManager.GetInstance().ChangeEmailPageStatus(EmailPageStatus.ShowEmail,EmaiSubPageStatus.ChoseFriend);
		}

		public void  SetFriendInfo(PanelElementDataModel Friend)
		{
			//EmailDataManager.Instance.EmailWrite.dwRecvActorID=1;//Friend.sMsgRecvAnswerFriends_SC.dwFriendID;
            EmailDataManager.Instance.CurrentFriendId=Friend.sMsgRecvAnswerFriends_SC.dwFriendID;
			EmailWrite.dwRecvActorID=Friend.sMsgRecvAnswerFriends_SC.dwFriendID;
			FriendInput.text=Friend.sMsgRecvAnswerFriends_SC.Name;
            EmailInfoPanelManager.GetInstance().UpdateEmaiBottom();
		}
        public void SetFriendInfo(uint id,string Name)
        {
            EmailDataManager.Instance.CurrentFriendId=id;
            EmailInfoPanelManager.GetInstance().UpdateEmaiBottom();
            EmailWrite.dwRecvActorID=id;
            FriendInput.text=Name;
        }

	   public void SendEmail()
		{
            string Title = TitleInput.text;
            string Content = ContentInput.text;
           
			if(EmailWrite.dwRecvActorID==0)
			{
				MessageBox.Instance.ShowTips(5,LanguageTextManager.GetString("IDS_I22_32"),1.5f);
			}
            else if(!PlayerManager.Instance.IsMoneyEnough(Cost))
			{
               // PlayerManager.Instance.IsMoneyEnough(100)
                MessageBox.Instance.ShowNotEnoughMoneyMsg(()=>{
                    MessageBox.Instance.CloseMsgBox();
                });
			}
            else if(Title!=null&& Encoding.UTF8.GetBytes(Title).Length>19)
            {
                MessageBox.Instance.ShowTips(1,LanguageTextManager.GetString("IDS_I23_24"),1.5f);
            }
            else if(Content!=null&& Encoding.UTF8.GetBytes(Content).Length>160)
            {
                MessageBox.Instance.ShowTips(1,LanguageTextManager.GetString("IDS_I23_25"),1.5f);
            }
			else
			{

			EmailWrite.dwActorID=(uint)PlayerManager.Instance.FindHeroDataModel().ActorID;
			//EmailWrite.dwRecvActorID=FriendId;
				byte[] title=new byte[19];
                if(Title!=null)
                {
                byte[] title0=Encoding.UTF8.GetBytes(Title);
				title0.CopyTo(title,0);
                }
				EmailWrite.szTitle= title;
                byte[] Context=new byte[160];
                if(Content!=null)
                {
                byte[] Context0=Encoding.UTF8.GetBytes(Content);
				Context0.CopyTo(Context,0);
                }
			    EmailWrite.szContext=Context;
			    NetServiceManager.Instance.EmailService.SendSEmailWrite_CS(EmailWrite);
				//LoadingUI.Instance.Show();
                ClearALL();
                MessageBox.Instance.ShowTips(2,LanguageTextManager.GetString("IDS_I22_35"),1.5f);
			}
		}
		public void ClearALL()
		{
			FriendInput.text=null;
			TitleInput.text=null;
			ContentInput.text=null;
            EmailWrite=new SEmailWrite_CS();
          
          
		}

        void UpdateCost(object obj)
        {
            if(!PlayerManager.Instance.IsMoneyEnough(Cost))
            {
                Cost_des.SetText(string.Format( LanguageTextManager.GetString("IDS_I22_31"),UI.NGUIColor.SetTxtColor(Cost,TextColor.red)));
            }
            else
            {
                Cost_des.SetText(string.Format( LanguageTextManager.GetString("IDS_I22_31"),UI.NGUIColor.SetTxtColor(Cost,TextColor.ChatYellow)));
            }
        }
        protected override void RegisterEventHandler()
        {
            AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateCost);
        }
        void OnDestroy()
        {
            RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateCost);
        }
//		void RegUIEvent()
//		{
//			UIEventManager.Instance.RegisterUIEvent(UIEventType.SendEamil,ClearALL);
//			//LoadingUI.Instance.Close();
//		}
}
}

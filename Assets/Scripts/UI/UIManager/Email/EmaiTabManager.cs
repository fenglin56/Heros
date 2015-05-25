using UnityEngine;
using System.Collections;
namespace UI.MainUI
{
public class EmaiTabManager : BaseSubUiPanel {

		public SingleButtonCallBack AllEmailBtn;
		public SingleButtonCallBack SystemEmailBtn;
		public SingleButtonCallBack WriteEmailBtn;
		public EmailContainerList Sc_EmailContainerList;
		public SendEmailContent Sc_SendEmailContent;
		public EmailContent Sc_EmailContent;
		public GameObject FriendListPanel_prefab;
		public EmaiFriendList Sc_EmaiFriendList;
        public GameObject HasUnreaderMail_all;
        public GameObject HasUnreaderMail_sys;
        void Awake()
        {
            AllEmailBtn.SetCallBackFuntion(OnAllEmailBtnClick);
			SystemEmailBtn.SetCallBackFuntion(OnSystemEmailBtn);
			WriteEmailBtn.SetCallBackFuntion(OnWriteEmailBtn);
			Sc_EmaiFriendList=CreatObjectToNGUI.InstantiateObj(FriendListPanel_prefab,transform).GetComponent<EmaiFriendList>();
			//Sc_EmaiFriendList.HidePanel();

            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            AllEmailBtn.gameObject.RegisterBtnMappingId(UIType.Mail, BtnMapId_Sub.Mail_ReceiveMail);
            SystemEmailBtn.gameObject.RegisterBtnMappingId(UIType.Mail, BtnMapId_Sub.Mail_Notice);
            WriteEmailBtn.gameObject.RegisterBtnMappingId(UIType.Mail, BtnMapId_Sub.Mail_WriteMail);
        }
//        void RegEvent()
//        {
            //UIEventManager.Instance.RegisterUIEvent(UIEventType.GetAllAttachment,GetAllEamilAtt);
            //UIEventManager.Instance.RegisterUIEvent(UIEventType.GetAttachment,GetEamilAtt);
           // UIEventManager.Instance.RegisterUIEvent(UIEventType.DeleteEmail,DeleteEamil);
//        }
 
//        void GetEamilAtt(object obj)
//        {
//            SEmailUpdate_SC sEmailUpdate_SC=(SEmailUpdate_SC)obj;
//            var  Email=EmailDataManager.Instance.EmailOpenUI_SC.mailList.Find(c=>c.llMailID==sEmailUpdate_SC.dwEmailID);
//            GoodsMessageManager.Instance.Show((int)Email.dwGoodsID,(int)Email.dwGoodsNum);
//        }
//        void GetAllEamilAtt(object obj)
//        {
//            SEmailGetAllGoods_SC sEmailGetAllGoods_SC=(SEmailGetAllGoods_SC)obj;
//            sEmailGetAllGoods_SC.mailIdList.ApplyAllItem(p=>{
//
//              var  Email=EmailDataManager.Instance.EmailOpenUI_SC.mailList.Find(c=>c.llMailID==p);
//                GoodsMessageManager.Instance.Show((int)Email.dwGoodsID,(int)Email.dwGoodsNum);
//            });
//
//
//        }
		public void Init(EmaiSubPageStatus stat)
		{
            if(stat==EmaiSubPageStatus.EmailList)
            {
            OnSystemEmailBtn(null);
            }
            else
            {
                OnWriteEmailBtn(null);
            }
		}
		void OnAllEmailBtnClick(object obj)
		{
            SoundManager.Instance.PlaySoundEffect("Sound_Button_MailChange");
			AllEmailBtn.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(2));
			SystemEmailBtn.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(1));
			WriteEmailBtn.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(1));
			Sc_EmailContainerList.Init(EmailType.UserEmail);
			EmailInfoPanelManager.GetInstance().ChangeEmailPageStatus(EmailPageStatus.ShowEmail,EmaiSubPageStatus.EmailList);

        }
		void OnSystemEmailBtn(object obj)
		{
            SoundManager.Instance.PlaySoundEffect("Sound_Button_MailChange");
			AllEmailBtn.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(1));
			SystemEmailBtn.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(2));
			WriteEmailBtn.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(1));
			Sc_EmailContainerList.Init(EmailType.systemEmail);
			EmailInfoPanelManager.GetInstance().ChangeEmailPageStatus(EmailPageStatus.ShowEmail,EmaiSubPageStatus.EmailList);

        }
		void OnWriteEmailBtn(object obj)
		{
            SoundManager.Instance.PlaySoundEffect("Sound_Button_MailChange");
			AllEmailBtn.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(1));
			SystemEmailBtn.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(1));
			WriteEmailBtn.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(2));
			EmailInfoPanelManager.GetInstance().ChangeEmailPageStatus(EmailPageStatus.ShowEmail,EmaiSubPageStatus.EmailWrite);
			//StartCoroutine(ChangeSubPanel(true));
        }

        public void UpdateUnreaderIcon()
        {
            if(EmailDataManager.Instance.GetUnReadCount(EmailType.UserEmail)>0)
            {
                HasUnreaderMail_all.SetActive(true);
            }
            else
            {
                HasUnreaderMail_all.SetActive(false);
            }

            if(EmailDataManager.Instance.GetUnReadCount(EmailType.systemEmail)>0)
            {
                HasUnreaderMail_sys.SetActive(true);
            }
            else
            {
                HasUnreaderMail_sys.SetActive(false);
            }

        }
		public void ChangeStatus(EmaiSubPageStatus status)
		{
			switch(status)
			{
		
			case EmaiSubPageStatus.EmailList:
			
					Sc_EmailContainerList.ShowPanel();
					Sc_SendEmailContent.HidePanel();
					Sc_EmailContent.HidePanel();
				    Sc_EmaiFriendList.HidePanel();

				break;
			case EmaiSubPageStatus.EmailRead:
			
					Sc_EmailContainerList.ShowPanel();
					Sc_SendEmailContent.HidePanel();
					Sc_EmailContent.ShowPanel();
				    Sc_EmaiFriendList.HidePanel();
				break;
            case EmaiSubPageStatus.EmailWrite:

                    Sc_EmailContainerList.HidePanel();
                    Sc_SendEmailContent.ShowPanel();
                    Sc_EmailContent.HidePanel();
				    Sc_EmaiFriendList.HidePanel();
                
                break;
			case EmaiSubPageStatus.ChoseFriend:
			
					//Sc_EmailContainerList.HidePanel();
					//Sc_SendEmailContent.ShowPanel();
					//Sc_EmailContent.HidePanel();
					Sc_EmaiFriendList.ShowPanel();


                break;
            }
        }
        
        
    }
}
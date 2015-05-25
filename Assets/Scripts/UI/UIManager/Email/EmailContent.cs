using UnityEngine;
using System.Collections;
using System.Linq;
using System.Text;
using System;


namespace UI.MainUI
{
public class EmailContent : BaseSubUiPanel {
		public UILabel From_Des;
		public UILabel Title_Des;
		public UILabel FromUserName;
		public GameObject FromSystem;
		public UILabel Title;
		public UILabel Context;
		public UILabel Noneattachments;
		public Emailattachments attachments;
		private SEmailSendUint Email;
        private long _EmailId;
       
		void Awake()
		{
			From_Des.SetText(LanguageTextManager.GetString("IDS_I22_13"));
			Title_Des.SetText(LanguageTextManager.GetString("IDS_I22_14"));
            Noneattachments.SetText(LanguageTextManager.GetString("IDS_I22_18"));
            RegEvent();
		}
        public override void ShowPanel()
        {
            base.ShowPanel();
          
        }
        public void Init(long EmailID)
        {
            _EmailId=EmailID;
        }
	   public void Refresh()
		{
            EmailInfoPanelManager.GetInstance().UpdateEmaiBottom();
			//Email=EmailDataManager.Instance.EmailOpenUI_SC.mailList.Find(c=>c.llMailID==EmailDataManager.Instance.EmailRead.llEmailID);
			if(Email.llMailID==0)
			{
               
			}
			else
			{
			
			if(Email.byIsSystem!=0)
				{
					FromSystem.SetActive(true);
					FromUserName.gameObject.SetActive(false);
                    var maildata=EmailInfoPanelManager.GetInstance().systemMailConfigDataBase.SystemMailConfigDataList.First(p=>p.MailType==(int)Email.wEmailType);
                    Title.SetText(LanguageTextManager.GetString(maildata.MailTitle));
                    Context.SetText(LanguageTextManager.GetString(maildata.MailText));
				}
				else
				{
					FromSystem.SetActive(false);
					FromUserName.gameObject.SetActive(true);
					FromUserName.SetText(Encoding.UTF8.GetString(Email.szSendActorName));
                    Title.SetText(Encoding.UTF8.GetString(Email.szTitle));
                    Context.SetText(Encoding.UTF8.GetString(EmailDataManager.Instance.EmailRead.szEmailContext));
				}

			
			}
			if(Convert.ToInt32(Email.byGoodsType)==(int)emEMAIL_EXTRA_TYPE.EMAIL_NONE_EXTRA_TYPE)
			{
				Noneattachments.gameObject.SetActive(true);
				attachments.gameObject.SetActive(false);
			}
			else
			{
				Noneattachments.gameObject.SetActive(false);
				attachments.gameObject.SetActive(true);
				attachments.Init(Email);
			}

		}
        void ReadEmailHandel(object obj)
        {
            Email=  EmailDataManager.Instance.GetEamilFromLocal(EmailDataManager.Instance.EmailRead.llEmailID);
            Refresh();
        }
        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ReadEmail,ReadEmailHandel);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.GetAttachment,ReadEmailHandel);
        }
        void RegEvent()
        {
            UIEventManager.Instance.RegisterUIEvent(UIEventType.ReadEmail,ReadEmailHandel);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.GetAttachment,ReadEmailHandel);
        }
}
}
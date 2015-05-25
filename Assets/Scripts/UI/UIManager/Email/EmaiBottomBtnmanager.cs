using UnityEngine;
using System.Collections;
namespace UI.MainUI
{
public class EmaiBottomBtnmanager : BaseSubUiPanel {
		public SingleButtonCallBack OneKeyDeleteBtn;
		public SingleButtonCallBack OneKeyExtractBtn;
		public SingleButtonCallBack extractBtn;
		public SingleButtonCallBack DeleteBtn;
		public SingleButtonCallBack SendEmailBtn;
        public GameObject extractBtnEff;
        public GameObject sendEamilEff;
        private EmaiSubPageStatus currentStatus;
	    void Awake ()
		{
			SendEmailBtn.SetCallBackFuntion(SendEmail);
			OneKeyDeleteBtn.SetCallBackFuntion(OneKeyDelete);
			DeleteBtn.SetCallBackFuntion(DeleteCurrentEmail);
			OneKeyExtractBtn.SetCallBackFuntion(OneKeyExtract);
			extractBtn.SetCallBackFuntion(extractCurrent);
            Regevent();
            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            OneKeyDeleteBtn.gameObject.RegisterBtnMappingId(UIType.Mail, BtnMapId_Sub.Mail_DelByOneKey);
            OneKeyExtractBtn.gameObject.RegisterBtnMappingId(UIType.Mail, BtnMapId_Sub.Mail_CatchByOneKey);
            extractBtn.gameObject.RegisterBtnMappingId(UIType.Mail, BtnMapId_Sub.Mail_CatchMail);
            DeleteBtn.gameObject.RegisterBtnMappingId(UIType.Mail, BtnMapId_Sub.Mail_DeleteMail);
            SendEmailBtn.gameObject.RegisterBtnMappingId(UIType.Mail, BtnMapId_Sub.Mail_SendMail);
        }
    

		public void extractCurrent(object obj)
		{

            SoundManager.Instance.PlaySoundEffect("Sound_Button_MailExtraction");
            if( ContainerInfomanager.Instance.PackIsFull())
            {
                MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_I22_17"),1);
            }
            else
            {
			  EmailDataManager.Instance.GetEmailAttachment();
            }

		}


		public void OneKeyExtract(object obj)
		{
          
            SoundManager.Instance.PlaySoundEffect("Sound_Button_MailExtraction");
            MessageBox.Instance.Show(5,"",LanguageTextManager.GetString("IDS_I22_22"),LanguageTextManager.GetString("IDS_I22_24"),LanguageTextManager.GetString("IDS_I22_23"),EnsureBtnClick_OneKeyExtract,CancelButtonClick);
		}

        void CancelButtonClick()
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_MailBatchDeleteCancel");
            MessageBox.Instance.CloseMsgBox();
        }
        void EnsureBtnClick_OneKeyExtract()
        {
         
            SoundManager.Instance.PlaySoundEffect("Sound_Button_MailBatchExtractionConfirmation");
            MessageBox.Instance.CloseMsgBox();
            if( ContainerInfomanager.Instance.PackIsFull())
            {
                MessageBox.Instance.ShowTips(5,LanguageTextManager.GetString("IDS_I22_17"),1);
            }
            else
            {
                LoadingUI.Instance.Show();
                EmailDataManager.Instance.GetAllttachment();
            }
        }


        void EnsureBtnClick_OneKeyDelete()
        {
            LoadingUI.Instance.Show();
            SoundManager.Instance.PlaySoundEffect("Sound_Button_MailBatchDeleteConfirmation");
            MessageBox.Instance.CloseMsgBox();
            EmailInfoPanelManager.GetInstance().SC_EmailTabManager.Sc_EmailContainerList.DeleteAllEmail();
        }


		public void DeleteCurrentEmail(object obj)
		{
            SoundManager.Instance.PlaySoundEffect("Sound_Button_MailDelete");
           if(EmailDataManager.Instance.GetIfCurretEamilHasAttachment())
            {
                MessageBox.Instance.ShowTips(5,LanguageTextManager.GetString("IDS_I22_19"),1);
            }
            else
            {
			   EmailDataManager.Instance.DeleteCurrentEmail();
            }
		}

		public void SendEmail(object obj)
		{
      
            SoundManager.Instance.PlaySoundEffect("Sound_Button_MailDelete");

			EmailInfoPanelManager.GetInstance().SC_EmailTabManager.Sc_SendEmailContent.SendEmail();
		}


		public void OneKeyDelete(object obj)
		{
            SoundManager.Instance.PlaySoundEffect("Sound_Button_MailDelete");
            MessageBox.Instance.Show(5,"",LanguageTextManager.GetString("IDS_I22_21"),LanguageTextManager.GetString("IDS_I22_24"),LanguageTextManager.GetString("IDS_I22_23"),EnsureBtnClick_OneKeyDelete,CancelButtonClick);
		
		}


		public void ChangeEmailStatus(EmaiSubPageStatus status)
		{
			switch(status)
			{
			case EmaiSubPageStatus.EmailList:
                    if(EmailDataManager.Instance.CurrentSubStatue==EmaiSubPageStatus.EmailWrite)
                        return;
                   // currentStatus=EmaiSubPageStatus.EmailList;
                    if(EmailDataManager.Instance.GetEmailList().Count==0)
                    {
                        OneKeyDeleteBtn.gameObject.SetActive(false);
                        OneKeyExtractBtn.gameObject.SetActive(false);
                    }
                    else
                    {
                        OneKeyDeleteBtn.gameObject.SetActive(true);
                        if( EmailDataManager.Instance.IfHasAttachmentInEmailList())
                        {
                            OneKeyExtractBtn.gameObject.SetActive(true);
                        }
                       else
                        {
                            OneKeyExtractBtn.gameObject.SetActive(false);
                        }
                    }
                 
				extractBtn.gameObject.SetActive(false);
				DeleteBtn.gameObject.SetActive(false);
				SendEmailBtn.gameObject.SetActive(false);
				break;
			case EmaiSubPageStatus.EmailRead:
                   // currentStatus=EmaiSubPageStatus.EmailRead;
				OneKeyDeleteBtn.gameObject.SetActive(false);
				OneKeyExtractBtn.gameObject.SetActive(false);
                  if(  EmailDataManager.Instance.GetIfCurretEamilHasAttachment())
                    {
                        extractBtn.gameObject.SetActive(true);
                    }
			     else
                    {
                        extractBtn.gameObject.SetActive(false);
                    }
				DeleteBtn.gameObject.SetActive(true);
				SendEmailBtn.gameObject.SetActive(false);
				break;
			case EmaiSubPageStatus.EmailWrite:

                    //currentStatus=EmaiSubPageStatus.EmailWrite;
                    if(EmailDataManager.Instance.CurrentFriendId!=0)
                    {
                        sendEamilEff.SetActive(true);
                    }
                    else
                    {
                        sendEamilEff.SetActive(false);
                    }
				OneKeyDeleteBtn.gameObject.SetActive(false);
				OneKeyExtractBtn.gameObject.SetActive(false);
				extractBtn.gameObject.SetActive(false);
				DeleteBtn.gameObject.SetActive(false);
				SendEmailBtn.gameObject.SetActive(true);
				break;
			}
		}

        void UpdatedEamilList(object obj)
        {
            ChangeEmailStatus(EmaiSubPageStatus.EmailList);
        }
        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.UpdatedEmailList,UpdatedEamilList);
        }
        void Regevent()
        {
            UIEventManager.Instance.RegisterUIEvent(UIEventType.UpdatedEmailList,UpdatedEamilList);
        }
}
}
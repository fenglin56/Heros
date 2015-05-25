using UnityEngine;
using System.Collections;
using System;

namespace UI.MainUI
{
public class EmailInfoPanelManager : BaseUIPanel {
		public GameObject EmailTabManager_prefab,EmaiBottomBtnmanager_prefab;
		public  SingleButtonCallBack BackBtn;
		[HideInInspector]
		public EmaiTabManager SC_EmailTabManager;
		private EmaiBottomBtnmanager SC_EmaiBottomBtnmanager;

		private static EmailInfoPanelManager instance;
		public EmailPageStatus CurrentStatus;
        private uint m_uid;
        private string m_name;
        public SystemMailConfigDataBase systemMailConfigDataBase;
		public static EmailInfoPanelManager GetInstance ()
		{
			if (!instance) {
				instance = (EmailInfoPanelManager)GameObject.FindObjectOfType (typeof(EmailInfoPanelManager));
				if (!instance)
					Debug.LogError ("没有附加JewelBesetManager脚本的gameobject在场景中");
            }
            return instance;
        }
        
        void Awake()
        {

            BackBtn.SetCallBackFuntion(OnBackButtonClick);
			BackBtn.SetPressCallBack(OnBackButtonPress);
			SC_EmailTabManager=CreatObjectToNGUI.InstantiateObj(EmailTabManager_prefab,transform).GetComponent<EmaiTabManager>();
			SC_EmaiBottomBtnmanager=CreatObjectToNGUI.InstantiateObj(EmaiBottomBtnmanager_prefab,transform).GetComponent<EmaiBottomBtnmanager>();
			//TraceUtil.Log(SystemModel.wanglei,"ssd");
            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            BackBtn.gameObject.RegisterBtnMappingId(UIType.Mail, BtnMapId_Sub.Mail_Back);
        }

        IEnumerator GoTOWriter(uint FUid,string FName)
        {
            ChangeEmailPageStatus(EmailPageStatus.InitEmail, EmaiSubPageStatus.EmailWrite);
            yield return null;
            SC_EmailTabManager.Sc_SendEmailContent.SetFriendInfo(FUid, FName);
        }

		public override void Show(params object[] value)
		{
           
			base.Show();
            EmailDataManager.Instance.GetAllEmailsOnService();
            if(value.Length==0)
            {
            //SoundManager.Instance.PlaySoundEffect("Sound_UIEff_MailUIAppear");
            //SoundManager.Instance.PlaySoundEffect("Sound_UIEff_MailUIAppear");
			
			ChangeEmailPageStatus(EmailPageStatus.InitEmail,EmaiSubPageStatus.EmailList);
            }
            else
            {
                EmaiSubPageStatus status=(EmaiSubPageStatus)value[0];
                if(status==EmaiSubPageStatus.EmailWrite)
                {
                    StartCoroutine(GoTOWriter( Convert.ToUInt32(value[1]),Convert.ToString(value[2])));
                  
                }

            }
		}

	
		public void ChangeEmailPageStatus(EmailPageStatus status,EmaiSubPageStatus subStatus) 
		{
            EmailDataManager.Instance.CurrentMainStatus=status;
            EmailDataManager.Instance.CurrentSubStatue=subStatus;
			switch(status)
			{
			case EmailPageStatus.InitEmail:
				SC_EmailTabManager.ShowAnim();
				SC_EmaiBottomBtnmanager.ShowAnim();
				SC_EmaiBottomBtnmanager.ChangeEmailStatus(subStatus);
                //SC_EmailTabManager.ChangeStatus(subStatus);
                SC_EmailTabManager.Init(subStatus);
                
				break;
			case EmailPageStatus.ShowEmail:
				SC_EmailTabManager.ChangeStatus(subStatus);
				SC_EmaiBottomBtnmanager.ChangeEmailStatus(subStatus);
				break;
			case EmailPageStatus.CloseEmail:
                 StartCoroutine (AnimToClose ());
				SC_EmaiBottomBtnmanager.ChangeEmailStatus(subStatus);
				break;

			}
		}

        public void UpdateEmaiBottom() 
        {
            SC_EmaiBottomBtnmanager.ChangeEmailStatus(EmailDataManager.Instance.CurrentSubStatue);
        }
		void OnBackButtonClick(object obj)
		{
		    SoundManager.Instance.PlaySoundEffect ("Sound_Button_Equipment_Cancel");
			this.Close ();
		}
		void OnBackButtonPress(bool isPressed)
		{
			BackBtn.spriteSwithList.ApplyAllItem (P => P.ChangeSprite (isPressed ? 2 : 1));

		}
		public override void Close()
		{
			if (!IsShow)
				return;
			
            ChangeEmailPageStatus(EmailPageStatus.CloseEmail,EmailDataManager.Instance.CurrentSubStatue);
		}
		IEnumerator AnimToClose ()
		{
			
			SC_EmailTabManager.CloseAnim();
			SC_EmaiBottomBtnmanager.CloseAnim();
			yield return new WaitForSeconds (SC_EmailTabManager.TweenDuration);
			base.Close ();
		}
		public void SetFriend(UI.Friend.PanelElementDataModel friend)
		{
			SC_EmailTabManager.Sc_SendEmailContent.SetFriendInfo(friend);
		}

       
	
}
	public enum EmailPageStatus
	{
		InitEmail,
		ShowEmail,
		CloseEmail,


	}

	public enum EmaiSubPageStatus
	{
		EmailList,
		EmailRead,
		EmailWrite,
		ChoseFriend,
	}
}

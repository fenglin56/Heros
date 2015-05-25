using UnityEngine;
using System.Collections;

namespace UI.Login
{

    public class CreatingRolesUIPanel_V3 : IUIPanel
    {

        private static CreatingRolesUIPanel_V3 m_instance;
        public static CreatingRolesUIPanel_V3 Instance { get { return m_instance; } }

        public SingleButtonCallBack JoinGameButton;
		public SingleButtonCallBack BackButton;
		public UILabel BackButtonText;
		
        public HeroModelManager HeroModelCamera;
        public HeroNameEditor HeroNameEditor;
        public HeroSlectBtnList_V3 HeroButtonSlectList;
        public UILabel DescribeLable;
	

        public void Awake()
        {
            UIEventManager.Instance.RegisterUIEvent(UIEventType.CreateFailedMaxnum, ReceiveDbError_CreateFailedMaxnum);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.CreateFailedDuplicate, ReceiveDbError_CreateFailedDuplicate);
            LoginManager.Instance.CreateActorButtonEnable = true;
            m_instance = this;
			
        }
        void OnDestroy()
        {
            m_instance = null;
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CreateFailedMaxnum, ReceiveDbError_CreateFailedMaxnum);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CreateFailedDuplicate, ReceiveDbError_CreateFailedDuplicate);
        }


        void Start()
        {
			BackButton.SetCallBackFuntion(OnBackButtonClick);
			BackButtonText.SetText(LanguageTextManager.GetString("IDS_H2_8"));
            JoinGameButton.SetCallBackFuntion(OnJoinGameButtonClick);
            Show();
        }

        public override void Show()
        {
            this.HeroNameEditor.ClearName();
            HeroButtonSlectList.InitSelectBtn(1);
            transform.localPosition = Vector3.zero;
        }

        public override void Close()
        {
            transform.localPosition = new Vector3(0,0,-1000);
            HeroModelCamera.ClearModel();
        }

        public void OnSelectRole(int rolePosition)
        {
            var roleId = ConvertPosToRoleId(rolePosition);
            NewCharacterConfigData configData = LoginDataManager.Instance.GetNewCharacterConfigData(roleId);
            SoundManager.Instance.PlaySoundEffect(configData.ChooseVoice);
            this.HeroModelCamera.ShowRoleModel(configData);            
            this.HeroNameEditor.SetHeroSex(roleId);
            //this.HeroModelCamera.PlayerCreatRoleAnimation();
            StartCoroutine(LatePlayerCreatRoleAnimation());
            //DescribeLable.spriteName = configData.Introductions;
        }
        IEnumerator LatePlayerCreatRoleAnimation()
        {
            yield return new WaitForSeconds(0.1f);
            this.HeroModelCamera.PlayerCreatRoleAnimation();
        }
        private byte ConvertPosToRoleId(int rolePos)
        {
            byte roleId = 1;
            switch (rolePos)
            {
                case 1:
                    roleId = 1;   //第一个栏位是剑客
                    break;
                case 2:
                    roleId = 4;  //第二个栏位是刺客
                    break;
            }
            return roleId;
        }
		
		void OnBackButtonClick(object obj)
        {
			if (LoginManager.Instance.NewSSUserLoginRes.lActorNum == 0)
            {
				return;
			}
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
			GameDataManager.Instance.ResetData(DataType.ActorSelector, LoginManager.Instance.NewSSUserLoginRes.SSActorInfos);
			UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.SelectRole);
		}
		
		
        void OnJoinGameButtonClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            if (!LoginManager.Instance.CreateActorButtonEnable) return;           
            string NameStr = HeroNameEditor.InPutLable.text;
            if (string.IsNullOrEmpty(NameStr))//空名字
            {
                //HeroModelCamera.SetModelEnable(false);
                //MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_229"), LanguageTextManager.GetString("IDS_H2_55"), OnCloseMessageBox);
                MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_H1_229"),1);
                return;
            }
            if (NameStr.IndexOf(" ") >= 0)//含有空格
            {
                //HeroModelCamera.SetModelEnable(false);
                //MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_42"), LanguageTextManager.GetString("IDS_H2_55"), OnCloseMessageBox);
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_42"), 1);
                return;
            }
            LoginManager.Instance.CreateActorButtonEnable = false;
            NetServiceManager.Instance.LoginService.SubmitCharacterInfo(HeroNameEditor.InPutLable.text.Trim(), HeroNameEditor.Vocation);
        }

        void ReceiveDbError_CreateFailedDuplicate(object obj)
        {
            //HeroModelCamera.SetModelEnable(false);
            //UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_43"), LanguageTextManager.GetString("IDS_H2_13"), OnCloseMessageBox);
            MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_43"), 1);
        }
        void ReceiveDbError_CreateFailedMaxnum(object obj)
        {
            //HeroModelCamera.SetModelEnable(false);
            //UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_235"), LanguageTextManager.GetString("IDS_H2_13"), OnCloseMessageBox);
            MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_235"), 1);
        }

        void OnCloseMessageBox()
        {
            //HeroModelCamera.SetModelEnable(true);
            this.HeroModelCamera.PlayerCreatRoleAnimation();
        }

        public override void DestroyPanel()
        {
            throw new System.NotImplementedException();
        }
    }
}
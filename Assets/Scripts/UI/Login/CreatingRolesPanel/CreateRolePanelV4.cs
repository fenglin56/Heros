using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.Login
{
    public class CreateRolePanelV4 : MonoBehaviour
    {
        public HeroNameEditor HeroNameEditor;
        public SingleButtonCallBack JoinGameButton;
        public SingleButtonCallBack BackButton;
        //public UILabel RoleDescribe;
        public UISprite RoleIcon;

        public RoleStarList[] MyRoleStarList;

        public CreateRoleUIData MyRoleData { get; private set; }

        void Awake()
        {
            UIEventManager.Instance.RegisterUIEvent(UIEventType.CreateFailedMaxnum, ReceiveDbError_CreateFailedMaxnum);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.CreateFailedDuplicate, ReceiveDbError_CreateFailedDuplicate);
            LoginManager.Instance.CreateActorButtonEnable = true;
        }

        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CreateFailedMaxnum, ReceiveDbError_CreateFailedMaxnum);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CreateFailedDuplicate, ReceiveDbError_CreateFailedDuplicate);
        }

        // Use this for initialization
        void Start()
        {
            BackButton.SetCallBackFuntion(OnBackButtonClick);
            JoinGameButton.SetCallBackFuntion(OnJoinGameButtonClick);
        }

        public void InitData(CreateRoleUIData data)
        {
            //RoleDescribe.text = "           " + LanguageTextManager.GetString(data._IntroText);
            RoleIcon.spriteName = data._HeadIcon;
            this.HeroNameEditor.SetHeroSex((byte)data._VocationID);
            MyRoleData = data;
            InitRoleStarList();
        }

        void InitRoleStarList()
        {
            for (int i = 0; i < MyRoleStarList.Length; i++)
            {
                MyRoleStarList[i].Init(MyRoleData.PlayerAbility[i]);
            }
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
			SoundManager.Instance.PlaySoundEffect("Sound_Button_ChangePage");
            if (!LoginManager.Instance.CreateActorButtonEnable) return;
            string NameStr = HeroNameEditor.InPutLable.text;

            if (string.IsNullOrEmpty(NameStr))//空名字
            {
                //HeroModelCamera.SetModelEnable(false);
                //MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_229"), LanguageTextManager.GetString("IDS_H2_55"), OnCloseMessageBox);
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_229"), 1);
                return;
            }
            if (NameStr.IndexOf(" ") >= 0)//含有空格
            {
                //HeroModelCamera.SetModelEnable(false);
                //MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_42"), LanguageTextManager.GetString("IDS_H2_55"), OnCloseMessageBox);
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_42"), 1);
                return;
            }
            if (System.Text.Encoding.UTF8.GetBytes(NameStr).Length>19)//长度超出限制
            {
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_560"), 1);
                return;
            }
            //非法字符验证
            if (!CommonDefineManager.Instance.IllegalCharacterConfig.ValidCharacter(NameStr))//没通过敏感词验证
            {
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_561"), 1);
                return;
            }
            //重名验证验证
            if (!CommonDefineManager.Instance.IllegalNameConfig.ValidCharacter(NameStr))//没通过敏感词验证
            {
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_43"), 1);
                return;
            }
            LoginManager.Instance.CreateActorButtonEnable = false;
            NetServiceManager.Instance.LoginService.SubmitCharacterInfo(NameStr.Trim(), HeroNameEditor.Vocation);
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

    }
}
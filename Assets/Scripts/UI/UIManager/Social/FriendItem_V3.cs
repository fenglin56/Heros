using UnityEngine;
using System.Collections;
using System.Linq;
using System;

namespace UI.Friend
{

	public enum FriendBtnType
	{
		Town = 0,
		MyFriend,
		Request,
	}


	public class FriendItem_V3 : MonoBehaviour ,IPagerItem
	{
		public UISlicedSprite UI_Head;
		public UISlicedSprite UI_Profession;
		public UILabel Label_Name;
		public UILabel Label_Level;
		public UILabel Label_Combat;
		public GameObject[] ButtonInterface= new GameObject[3];

		public GameObject Tip_Unline;

		public LocalButtonCallBack Button_Add;
		public LocalButtonCallBack Button_Chat;
		public LocalButtonCallBack Button_Email;
		public LocalButtonCallBack Button_Delete;
		public LocalButtonCallBack Button_Refusal;
		public LocalButtonCallBack Button_Consent;

		private SMsgRecvAnswerFriends_SC m_FriendInfo;
		public uint FriendActorID
		{
			get{
				return m_FriendInfo.dwFriendID;
			}
		}

		private Action<int> m_callBackAction;

		void Awake()
		{
			Button_Add.SetCallBackFuntion(OnAddClick,null);
			Button_Chat.SetCallBackFuntion(OnChatClick,null);
			Button_Email.SetCallBackFuntion(OnEmailClick,null);
			Button_Delete.SetCallBackFuntion(OnDeleteClick,null);
			Button_Refusal.SetCallBackFuntion(OnRefusalClick,null);
			Button_Consent.SetCallBackFuntion(OnConsentClick,null);
		}

		public void Init(SMsgRecvAnswerFriends_SC smsg, FriendBtnType type, Action<int> aciton)
		{
			m_FriendInfo = smsg;
			m_callBackAction = aciton;
			for(int i=0;i<ButtonInterface.Length;i++)
			{
				if(i == (int)type)
				{
					ButtonInterface[i].SetActive(true);
				}
				else
				{
					ButtonInterface[i].SetActive(false);
				}
			}
			var headRes = CommonDefineManager.Instance.CommonDefine.HeroIcon_MailFriend.FirstOrDefault(P=>P.VocationID == smsg.dProfession&&P.FashionID == smsg.dbSysActorImageHeadID);;
			if(headRes!=null)
			{
				UI_Head.spriteName = headRes.ResName;
			}
			UI_Profession.spriteName = Team.SpriteName.PROFESSION_CHAR + smsg.dProfession.ToString();
			Label_Name.text = smsg.Name;
			Label_Level.text = LanguageTextManager.GetString("IDS_I22_38")+ smsg.sActorLevel.ToString();
			Label_Combat.text = smsg.dwFight.ToString();

			Tip_Unline.SetActive(smsg.bOnLine == 0);
		}

		public void PlayAppearAnimation(float delayTime)
		{
			transform.localScale = new Vector3(1f,0.01f,1f);
			StartCoroutine(LateAppear(delayTime));
		}
		IEnumerator LateAppear(float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			TweenScale.Begin(gameObject,0.1f,Vector3.one);
		}

		void OnAddClick(object obj)
		{
			//\判断好友人数是否已到上限
			//......
			//......
			//return;

			SoundManager.Instance.PlaySoundEffect("Sound_Button_FriendAdd");

			if(FriendDataManager.Instance.GetOnlineFriendList.Any(p=>p.sMsgRecvAnswerFriends_SC.dwFriendID == m_FriendInfo.dwFriendID))
			{
				MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I23_19"),1f);
				return;
			}

			var playerData = PlayerManager.Instance.FindHeroDataModel();
			NetServiceManager.Instance.FriendService.SendAddFriendRequst(new SMsgAddFriends_CS()
			                                                             {
				dwActorID = (uint)playerData.ActorID,
				dwFriendID = (uint)m_FriendInfo.dwFriendID
			});
			Button_Add.SetBoxCollider(false);
			Button_Add.SetButtonTextureColor(Color.gray);
		}
		void OnChatClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_FriendChat");
			UI.MainUI.MainUIController.Instance.OpenMainUI(UI.MainUI.UIType.Chat, Chat.WindowType.Private, (int)m_FriendInfo.dwFriendID,m_FriendInfo.Name);
		}
		void OnEmailClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_FriendMail");
			UI.MainUI.MainUIController.Instance.OpenMainUI(UI.MainUI.UIType.Mail,UI.MainUI.EmaiSubPageStatus.EmailWrite,m_FriendInfo.dwFriendID,m_FriendInfo.Name);
		}
		void OnDeleteClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_FriendDelete");
			string str = string.Format(LanguageTextManager.GetString("IDS_I23_9"),m_FriendInfo.Name);
			MessageBox.Instance.Show(4,"",str,LanguageTextManager.GetString("IDS_I23_10"),LanguageTextManager.GetString("IDS_I23_11"),CancelDeleteHandle,SureDeleteHandle);
		}
		void SureDeleteHandle()
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_FriendDeleteConfirmation");
			FriendDataManager.Instance.IsDelFriendIsMe = true;
			var playerData = PlayerManager.Instance.FindHeroDataModel();
			SMsgDelFriends_CS sMsgDelFriends_CS = new SMsgDelFriends_CS();
			sMsgDelFriends_CS.dwActorID = (uint)PlayerManager.Instance.FindHeroDataModel().ActorID;
			sMsgDelFriends_CS.dwbDelActorID = m_FriendInfo.dwFriendID;
			NetServiceManager.Instance.FriendService.SendDelFriendRequst(sMsgDelFriends_CS);

			m_callBackAction((int)m_FriendInfo.dwFriendID);
		}
		void CancelDeleteHandle()
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_FriendDeleteCancel");
		}
		void OnRefusalClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_FriendRefuse");

			var playerData = PlayerManager.Instance.FindHeroDataModel();
			SMsgAnswerFriends_CS sMsgAnswerFriends_CS = new SMsgAnswerFriends_CS();
			sMsgAnswerFriends_CS.bAnswer = 0;  //0代表拒绝， 1代表同意
			sMsgAnswerFriends_CS.dwActorID = (uint)PlayerManager.Instance.FindHeroDataModel().ActorID;
			sMsgAnswerFriends_CS.dwbAnswerActorID = m_FriendInfo.dwFriendID;
			
			NetServiceManager.Instance.FriendService.SendAnswerFriendRequst(sMsgAnswerFriends_CS);
			m_callBackAction((int)m_FriendInfo.dwFriendID);
			FriendDataManager.Instance.DeleteRequest(m_FriendInfo.dwFriendID);//拒绝请求，主动删除请求数据
		}
		void OnConsentClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_FriendAccept");

			if(FriendDataManager.Instance.GetOnlineFriendList.Any(p=>p.sMsgRecvAnswerFriends_SC.dwFriendID == m_FriendInfo.dwFriendID))
			{
				MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I23_19"),1f);
				return;
			}

			var playerData = PlayerManager.Instance.FindHeroDataModel();
			SMsgAnswerFriends_CS sMsgAnswerFriends_CS = new SMsgAnswerFriends_CS();
			sMsgAnswerFriends_CS.bAnswer = 1;  //0代表拒绝， 1代表同意
			sMsgAnswerFriends_CS.dwActorID = (uint)PlayerManager.Instance.FindHeroDataModel().ActorID;
			sMsgAnswerFriends_CS.dwbAnswerActorID = m_FriendInfo.dwFriendID;
			
			NetServiceManager.Instance.FriendService.SendAnswerFriendRequst(sMsgAnswerFriends_CS);
			if(FriendDataManager.Instance.GetFriendListData.Count >= FriendDataManager.Instance.FriendMaxNum)
			{
				m_callBackAction((int)m_FriendInfo.dwFriendID);
			}
		}

		public void OnGetFocus ()
		{
		}
		public void OnLoseFocus ()
		{
		}
		public void OnBeSelected ()
		{
		}
		public Transform GetTransform ()
		{
			return transform;
		}
	}

}

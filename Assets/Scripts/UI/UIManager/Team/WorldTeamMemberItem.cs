using System.Linq;
using UnityEngine;
using System.Collections;

namespace UI.Team
{
	public class WorldTeamMemberItem : MonoBehaviour 
	{
		public UISprite UI_Icon;
		public UISprite UI_Head;
		public UISprite UI_Process;
		public UILabel Label_Level;
		public UILabel Label_Name;

		private SMsgTeamPropMember_SC MsgTeamPropMember;

		public void Init()
		{

		}
		public void InitInfo(Transform posTrans)
		{
			transform.parent = posTrans;
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
		}
		
		//public void InitInfo(Transform posTrans, SMsgTeamPropMember_SC sMsgTeamPropMember)
		//{
		//    transform.parent = posTrans;
		//    transform.localPosition = Vector3.zero;
		//    transform.localScale = Vector3.one;
		
		
		//}
		
		
		public void UpdateInfo(SMsgTeamPropMember_SC sMsgTeamPropMember)
		{
			this.MsgTeamPropMember = sMsgTeamPropMember;
			
			if (!gameObject.activeInHierarchy)
			{
				gameObject.SetActive(true);
			}
			
			//AvatarSwitch.ChangeSprite(sMsgTeamPropMember.TeamMemberContext.byKind);

			UI_Icon.spriteName = SpriteName.PROFESSION_ICON + sMsgTeamPropMember.TeamMemberContext.byKind.ToString();
			UI_Process.spriteName = SpriteName.PROFESSION_CHAR + sMsgTeamPropMember.TeamMemberContext.byKind.ToString();

			//var professionData = PlayerDataManager.Instance.GetProfessionConfigData(sMsgTeamPropMember.TeamMemberContext.byKind);
			//Label_Profession.text = LanguageTextManager.GetString(professionData._professionName);
			var resData = CommonDefineManager.Instance.CommonDefine.HeroIcon_MailFriend.FirstOrDefault(P => 
			 P.VocationID == sMsgTeamPropMember.TeamMemberContext.byKind && P.FashionID == sMsgTeamPropMember.TeamMemberContext.nHead);
			UI_Head.spriteName = resData.ResName;

			Label_Level.text = string.Format(LanguageTextManager.GetString("IDS_I9_29"), MsgTeamPropMember.TeamMemberContext.nLev.ToString());
			Label_Name.text = MsgTeamPropMember.TeamMemberContext.szName;
			
		}
		
		public void Close()
		{
			gameObject.SetActive(false);
		}
	}

}
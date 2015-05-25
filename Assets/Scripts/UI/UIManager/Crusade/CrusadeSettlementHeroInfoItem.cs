using System.Linq;
using UnityEngine;
using System.Collections;

namespace UI.Crusade
{
	public class SpriteName 
	{
		public const string PROFESSION_CHAR = "JH_UI_Typeface_7115_0";
	}

	public class CrusadeSettlementHeroInfoItem : MonoBehaviour 
	{
		public UIPanel Panel_Head;
		public UIPanel Panel_Info;

		public UISprite UI_Head;
		public UISprite UI_Profession;
		public UILabel Label_Combat;
		public UILabel Label_Name;
		public UISlider Slider_Hit;
		public UILabel Label_Hit;
		public GameObject Tip_Caption;
		public GameObject Tip_Member;

		private int m_hitNum = 0;
		private int m_allHit = 1;

		public void Show(SHitNumInfo hitNumInfo, IEntityDataStruct dataStruct, int allHit, bool isCaption, float delayTimeShow)
		{
			this.m_hitNum = hitNumInfo.dwHitNum;
			this.m_allHit = allHit;

			SoundManager.Instance.PlaySoundEffect("Sound_UIEff_CoopJudgeFlyin");

			Tip_Caption.SetActive(isCaption);
			Tip_Member.SetActive(!isCaption);

			if(dataStruct is SMsgPropCreateEntity_SC_MainPlayer)
			{
				var heroProp = (SMsgPropCreateEntity_SC_MainPlayer)dataStruct;
				Label_Combat.text = heroProp.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHTING.ToString();
				Label_Name.text = heroProp.Name;
				var resData = CommonDefineManager.Instance.CommonDefine.HeroIcon_MailFriend.FirstOrDefault(
					P => P.VocationID == heroProp.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION 
					&& P.FashionID == heroProp.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION);
				UI_Head.spriteName = resData.ResName;
				UI_Profession.spriteName = SpriteName.PROFESSION_CHAR+heroProp.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION.ToString();
			}
			else if(dataStruct is SMsgPropCreateEntity_SC_OtherPlayer)
			{
				var heroProp = (SMsgPropCreateEntity_SC_OtherPlayer)dataStruct;
				Label_Combat.text = heroProp.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHTING.ToString();
				Label_Name.text = heroProp.Name;
				var resData = CommonDefineManager.Instance.CommonDefine.HeroIcon_MailFriend.FirstOrDefault(
					P => P.VocationID == heroProp.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION 
					&& P.FashionID == heroProp.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION);
				UI_Head.spriteName = resData.ResName;
				UI_Profession.spriteName = SpriteName.PROFESSION_CHAR+heroProp.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION.ToString();
			}

			Slider_Hit.sliderValue = 0;
			Label_Hit.text = "";


			StartCoroutine(LateShow(delayTimeShow));
		}

		IEnumerator LateShow(float time)
		{
			yield return new WaitForSeconds(time);
			Vector3 endPos = Panel_Head.transform.localPosition;
			TweenPosition.Begin(Panel_Head.gameObject,0.1f,endPos+Vector3.left*50,endPos);
			TweenAlpha.Begin(Panel_Head.gameObject,0.1f,0,1f,MoveOverHandle);
		}

		void MoveOverHandle(object obj)
		{
			TweenAlpha.Begin(Panel_Info.gameObject,0.1f,0,1,AppearOverHandle);
		}

		void AppearOverHandle(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_UIEff_CoopJudgeDamageCal");
			TweenFloat.Begin(2f,0,m_hitNum,ChangeValueCallBack);
		}
		void ChangeValueCallBack(float value)
		{
			Label_Hit.text = ((int)value).ToString();
			Slider_Hit.sliderValue = value/m_allHit;
		}
	}
}
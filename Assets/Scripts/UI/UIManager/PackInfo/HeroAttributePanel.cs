using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI{

	public class HeroAttributePanel : View {

		public UIPanel MyUIPanel;
		public SpriteSwith VocationSprite;
		public UILabel AtkLabel;
		public UILabel ExpLabel;
		public UISlider ExpSliderBar;
		public RoleAttributePanel RoleAttributePanel;
		public AtrributePanelSingleSirenIcon[] SirenIconList;
		public Vector3 HidePos;
		public Vector3 ShowPos;

		public bool IsShow{get;private set;}
		private GameObject TweenObj;

		void Awake()
		{
			IsShow = false;
			AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
			transform.localPosition = HidePos;
			MyUIPanel.alpha = 0;
		}

		protected override void RegisterEventHandler ()
		{
			throw new System.NotImplementedException ();
		}

		void OnDestroy()
		{
			RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
		}
		
		public void UpdateViaNotify(INotifyArgs inotifyArgs)//设置各种属性
		{
			EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)inotifyArgs;
			if (IsShow&&entityDataUpdateNotify.IsHero && entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_PLAYER)
			{
				UpdateAttribute();
			}
		}


		public void TweenShow()
		{
			float animTime = 0.3f;
			IsShow = true;
			if(TweenObj!=null){DestroyImmediate(TweenObj);}
			TweenObj = TweenFloat.Begin(animTime,MyUIPanel.alpha,1,SetPanelAlpha);
			TweenPosition.Begin(gameObject,animTime,transform.localPosition,ShowPos);
			UpdateAttribute();
		}

		public void TweenClose()
		{
			float animTime = 0.3f;
			IsShow = false;
			if(TweenObj!=null){DestroyImmediate(TweenObj);}
			TweenObj = TweenFloat.Begin(animTime,MyUIPanel.alpha,0,SetPanelAlpha);
			TweenPosition.Begin(gameObject,animTime,transform.localPosition,HidePos);
		}

		public void Close()
		{
			IsShow = false;
			if(TweenObj!=null){DestroyImmediate(TweenObj);}
			MyUIPanel.alpha = 0;
			transform.localPosition = HidePos;
		}

		void UpdateAttribute()
		{
			RoleAttributePanel.ShowAttributePanelInfo();
			var m_HeroDataModel = PlayerManager.Instance.FindHeroDataModel();
			ExpLabel.SetText(string.Format("{0}/{1}",m_HeroDataModel.PlayerValues.PLAYER_FIELD_EXP ,m_HeroDataModel.PlayerValues.PLAYER_FIELD_NEXT_LEVEL_EXP));
			ExpSliderBar.sliderValue = (float)m_HeroDataModel.PlayerValues.PLAYER_FIELD_EXP/(float)m_HeroDataModel.PlayerValues.PLAYER_FIELD_NEXT_LEVEL_EXP;
			VocationSprite.ChangeSprite(m_HeroDataModel.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION);
			int newAtk = HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_Combat, PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHTING);
			AtkLabel.SetText(newAtk);
			ShowSirenItem();
		}

		void ShowSirenItem()
		{
			List<PlayerSirenConfigData> getDataList = new List<PlayerSirenConfigData>();
			foreach(var child in SirenDataManager.Instance.GetPlayerSirenList())
			{
				if(getDataList.FirstOrDefault(P=>P._sirenID == child._sirenID)==null)
				{
					getDataList.Add(child);
				}
			}
			var unLockDataList = SirenManager.Instance.GetYaoNvList();
			for(int i = 0;i<SirenIconList.Length;i++)
			{
				if(getDataList.Count>i)
				{
					int level = unLockDataList.FirstOrDefault(P=>(int)P.byYaoNvID == getDataList[i]._sirenID).byLevel;
					SirenIconList[i].Show(getDataList[i]._nameRes,level);
				}
			}
		}

		void SetPanelAlpha(float value)
		{
			MyUIPanel.alpha = value;
		}
	}
}
using UnityEngine;
using System.Collections;
using System;


namespace UI.Team
{
	public class EctypeCardItem_V2 : MonoBehaviour 
	{
		public enum CardType
		{
			Area = 0,
			Ectype,
		}

		public GameObject AreaInterface;
		public GameObject EctypeInterface;

		public GameObject LeftTip;
		public GameObject RightTip;

		public Transform EctypeIconPoint;
		public UISprite UI_EctpyeIcon;
		public UILabel Label_EctypeName;
		public UILabel Label_Level;
		public UILabel Label_AreaName;

		private Action<int> m_OnClickCallBack;
		public GameObject m_EctypeIcon;

		private int m_EctypeID;
		public int EctypeID{get{return m_EctypeID;}}

		public void Init(CardType type, object ectypeConfig, Action<int> act)
		{
			if(m_EctypeIcon != null)
			{
				Destroy(m_EctypeIcon);
			}

			if(type == CardType.Area)
			{
				var ectypeData = (EctypeSelectConfigData)ectypeConfig;
				m_EctypeID = ectypeData._lEctypeID;
				m_EctypeIcon = UI.CreatObjectToNGUI.InstantiateObj(ectypeData._EctypeIconPrefab, EctypeIconPoint);
				Label_AreaName.text = LanguageTextManager.GetString( ectypeData._szName);
				AreaInterface.SetActive(true);
				EctypeInterface.SetActive(false);

			}
			else
			{
				var containerData = (EctypeContainerData)ectypeConfig;
				m_EctypeID = containerData.lEctypeContainerID;
				var containerIconData = TownEctypeResDataManager.Instance.GetEctypeContainerResData(m_EctypeID);
				m_EctypeIcon = UI.CreatObjectToNGUI.InstantiateObj(containerIconData.EctypeIconPrefab, EctypeIconPoint);
				Label_EctypeName.text = LanguageTextManager.GetString( containerData.lEctypeName);
				Label_Level.text = containerData.lMinActorLevel.ToString() + LanguageTextManager.GetString( "IDS_H1_156");
				AreaInterface.SetActive(false);
				EctypeInterface.SetActive(true);
			}
			m_OnClickCallBack = act;
		}

		public void ShowLeftTip(bool isFlag)
		{
			LeftTip.SetActive(isFlag);
		}
		public void ShowRightTip(bool isFlag)
		{
			RightTip.SetActive(isFlag);
		}

		void OnClick()
		{
			m_OnClickCallBack(m_EctypeID);
		}
	}
}
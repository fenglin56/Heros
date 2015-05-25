using UnityEngine;
using System.Collections;
using System;

namespace UI.Crusade
{
	public class CrusadeLeaderItem : MonoBehaviour
	{
		public Transform EctypeIcon_Point;
		//public GameObject UI_EctypeIcon;
		private GameObject UI_EctypeIcon;

		public GameObject Mark;

		public UILabel Label_NeedTip;
		public UILabel Label_Level;
		public UILabel Label_EctypeName;

		public GameObject Interface_TeamNum;
		public UILabel Label_TeamNum;

		public GameObject Article_Left;
		public GameObject Article_Right;

		private Color m_ectypeNameColor;

		//private GameObject m_EctypeIcon;
		private EctypeContainerData m_ectypeData;
		public EctypeContainerData EctypeData
		{
			get{return m_ectypeData;}
		}

		private Action<int> m_action;
		private bool m_isUnlock;

		public void Init(GameObject ectypeIconPrefab, EctypeContainerData data, int heroLevel, Action<int> action,int index)
		{
			this.m_ectypeData = data;
			this.m_action = action;
			Label_Level.text = data.lMinActorLevel.ToString();
			Label_EctypeName.text = LanguageTextManager.GetString(data.lEctypeName);

			if(UI_EctypeIcon != null)
			{
				Destroy(UI_EctypeIcon);
			}


			if(index%2 == 1)
			{
				Article_Left.SetActive(true);
				Article_Right.SetActive(false);
			}
			else
			{
				Article_Left.SetActive(false);
				Article_Right.SetActive(true);
			}

			m_ectypeNameColor = Label_EctypeName.color;

			//var containerIconData = TownEctypeResDataManager.Instance.GetEctypeContainerResData(data.lEctypeContainerID);
			UI_EctypeIcon = UI.CreatObjectToNGUI.InstantiateObj(ectypeIconPrefab, EctypeIcon_Point);

			this.UpdateInfo(heroLevel);

//			if(heroLevel < data.lMinActorLevel)
//			{
//				Label_Level.color = Color.red;
//				Label_EctypeName.color = Color.red;
//			}
//			else
//			{
//				Label_Level.color = new Color(1f,0.95f,0);
//				Label_EctypeName.color = Color.white;
//			}
		}

		void OnClick()
		{
			if(m_isUnlock)
			{
				SoundManager.Instance.PlaySoundEffect("Sound_Button_CoopSelect");
				m_action(m_ectypeData.lEctypeContainerID);
			}
		}

		public void UpdateInfo(int heroLevel)
		{
			if(heroLevel < m_ectypeData.lMinActorLevel)
			{
				Label_Level.color = Color.red;
				Label_NeedTip.color = Color.red;
				Label_EctypeName.color = Color.gray;
				Mark.SetActive(true);
				Interface_TeamNum.SetActive(false);
				m_isUnlock = false;
			}
			else
			{
				Label_Level.color = new Color(1f,0.95f,0);
				Label_NeedTip.color = new Color(0.91f,0.875f,0.617f);
				Label_EctypeName.color = m_ectypeNameColor;
				Mark.SetActive(false);
				Interface_TeamNum.SetActive(true);
				m_isUnlock = true;
			}
		}

		public void UpdateTeamNum(int num)
		{
			Label_TeamNum.text = num.ToString();
		}
	}
}
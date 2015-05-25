using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI{

	public class EctypeContainerListPanel : MonoBehaviour {

		public EctypeContainerAtttributePanel m_EctypeContainerAtttributePanel;//副本属性介绍面板
		public List<SingleEctypeContainerItem> MyContainerItemSlotList;//副本容器列表面板
		public SingleTreasureChestsItem m_SingleTreasureChestsItem ;//宝箱
		public TreasureChestsPanel m_TreasureChestsPanel;//宝箱奖励面板
		public EvaluateConfigDataBase EvaluateData;//结算星级资源
		public EctypeContainerIconPrefabDataBase EctypeIconData;//副本图标资源
		public EctypePanel_V5 MyParent{get;private set;}
		public EctypeSelectConfigData EctypeSelectData{get;private set;}
		public List<int> myEctypeContainerIDList = new List<int> ();
		int MaxEctypeContaienrID;//最大解锁副本ID

		public UIPanel[] MyPanels;
		public Vector3 HidePos;
		public Vector3 ShowPos;
		[HideInInspector]
		public int curSelectEasyEctypeID;
		[HideInInspector]
		public int curSelectDiffEctypeID;
		private Vector3 originalPos;
		GameObject TweenFloatObj;
		float AnimTime = 0.3f;
		public EctypeSweepPop sweepPopPanel;
		bool isRead = false;
		void Init()
		{
			if (isRead)
				return;
			isRead = true;
			originalPos = m_SingleTreasureChestsItem.transform.localPosition;
			sweepPopPanel.gameObject.SetActive (false);
		}
		public void Show(EctypeSelectConfigData selectData,EctypePanel_V5 myParent)
		{
			Init ();
			m_TreasureChestsPanel.Init(this);
			EctypeSelectData = selectData;
			MyParent = myParent;
			//查找当前页面最大解锁副本id
			MaxEctypeContaienrID = 0;
			myEctypeContainerIDList.Clear ();
			selectData._vectContainer.ApplyAllItem(c=>myEctypeContainerIDList.Add(c));
			selectData.Difficult2Container.ApplyAllItem(c=>myEctypeContainerIDList.Add(c));
			foreach(var child in EctypeModel.Instance.sMSGEctypeSelect_SC.sMSGEctypeData_SCs)//myParent.UnlockEctypeData.sMSGEctypeData_SCs)
			{
				if(myEctypeContainerIDList.Contains((int)child.dwEctypeContaienrID)&&(int)child.dwEctypeContaienrID>MaxEctypeContaienrID)
				{
					MaxEctypeContaienrID = (int)child.dwEctypeContaienrID;
				}
			}
			bool islock = false;
			MyContainerItemSlotList.ApplyAllItem(c=>c.SetMyselfActive(false));
			int i = 0;
			for(;i<MyContainerItemSlotList.Count;i++)
			{
				if(selectData._vectContainer.Length>i)
				{
					MyContainerItemSlotList[i].SetMyselfActive(true);
					islock = EctypeIDIsLock(GetContainerID(i,true));
					MyContainerItemSlotList[i].Init(GetContainerID(i,true),GetContainerID(i,false),this,MyParent.jumpPointEctypeID);
					if(islock)
					{
						if(i < MyContainerItemSlotList.Count - 1)
						{
							m_SingleTreasureChestsItem.transform.localPosition = MyContainerItemSlotList[i+1].transform.localPosition;
						}
						else
						{
							m_SingleTreasureChestsItem.transform.localPosition = originalPos;
						}
						break;
					}
				}
				else
				{
					break;
				}
			}
			if(!islock)
			{
				if(i >= MyContainerItemSlotList.Count)
				{
					i = MyContainerItemSlotList.Count;
					m_SingleTreasureChestsItem.transform.localPosition = originalPos;
				}
				else
				{
					m_SingleTreasureChestsItem.transform.localPosition = MyContainerItemSlotList[i].transform.localPosition;
				}
			}
			m_SingleTreasureChestsItem.UpdateStatus(this);
		}

		public int GetContainerID(int index,bool isEsy)
		{
			int getID = 0;
			if(isEsy&&EctypeSelectData._vectContainer.Length>index)
			{
				getID = EctypeSelectData._vectContainer[index];
			}
			else if(!isEsy&&EctypeSelectData.Difficult2Container.Count>index)
			{
				getID = EctypeSelectData.Difficult2Container[index];
			}
			return getID;
		}

		public void TweenShow ()
		{
			if(TweenFloatObj!=null){DestroyImmediate(TweenFloatObj);}
			TweenFloatObj = TweenFloat.Begin(AnimTime,0,1,SetMyPanelsAlpha);
			TweenPosition.Begin(gameObject,AnimTime,HidePos,ShowPos);
		}

		public void TweenClose()
		{
			if(TweenFloatObj!=null){DestroyImmediate(TweenFloatObj);}
			TweenFloatObj = TweenFloat.Begin(AnimTime,1,0,SetMyPanelsAlpha);
			TweenPosition.Begin(gameObject,AnimTime,ShowPos,HidePos);
		}

		void SetMyPanelsAlpha(float value)
		{
			MyPanels.ApplyAllItem(c=>c.alpha = value);
		}

		public void OnEctypeContainerItemClick(SingleEctypeContainerItem selectItem)
		{
			curSelectEasyEctypeID = selectItem.EsyContainerID;
			curSelectDiffEctypeID = selectItem.DiffContainerID;
			m_EctypeContainerAtttributePanel.Init(selectItem.EsyContainerID,selectItem.DiffContainerID,this);
			MyContainerItemSlotList.ApplyAllItem(P=>P.SetMySelfSelectStatus(selectItem));
		}

		/// <summary>
		/// 副本ID是否被锁
		/// </summary>
		public bool EctypeIDIsLock(int ectypeContainerID)
		{
			bool flag = false;
			flag = ectypeContainerID==0||EctypeModel.Instance.sMSGEctypeSelect_SC.sMSGEctypeData_SCs.FirstOrDefault(C=>C.dwEctypeContaienrID == ectypeContainerID).dwEctypeContaienrID==0;
			return flag;
		}
		/// <summary>
		/// 是否为最后一个解锁的副本ID
		/// </summary>
		public bool isMaxEctypeContaienrID(int containerID)
		{
			return containerID!=0&&containerID == MaxEctypeContaienrID;
		}
	}
}
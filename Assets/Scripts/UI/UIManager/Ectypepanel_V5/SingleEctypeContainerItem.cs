using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI{

	public class SingleEctypeContainerItem : MonoBehaviour {

		public SingleButtonCallBack EctypeNameLabel;
		public SingleButtonCallBack LevelLabel;
		public Transform EctypeIconPos;
		public Transform StarPos_Esy;
		public Transform StarPos_Diff;
		public Transform LockPos;
		public GameObject SelectStatus;
		public GameObject mobaMark;
		public EctypeContainerListPanel MyParent{get;private set;}

		public int EsyContainerID{get;private set;}
		public int DiffContainerID{get;private set;}
		public bool IsLock{get;private set;}

		public void Init(int esyContaienrID,int diffContainerID,EctypeContainerListPanel myParent,int pointEctypeID)
		{
			MyParent = myParent;
			IsLock = myParent.EctypeIDIsLock(esyContaienrID);
			EsyContainerID = esyContaienrID;
			DiffContainerID = diffContainerID;
			LockPos.gameObject.SetActive(IsLock);
			LevelLabel.gameObject.SetActive(!IsLock);
			EctypeNameLabel.gameObject.SetActive(!IsLock);
			//EctypeIconPos.gameObject.SetActive(!IsLock);
			StarPos_Esy.gameObject.SetActive(!IsLock);
			StarPos_Diff.gameObject.SetActive(!IsLock);
			EctypeIconPos.ClearChild();
			EctypeContainerIconData ectypeIconData = myParent.EctypeIconData.iconDataList.First(P=>P.lEctypeContainerID == EsyContainerID);
			CreatObjectToNGUI.InstantiateObj(ectypeIconData.EctypeIconPrefab,EctypeIconPos);
			if(IsLock){
				return;
			}

			StarPos_Esy.ClearChild();
			StarPos_Diff.ClearChild();
			CreatObjectToNGUI.InstantiateObj(GetStarPrefab(esyContaienrID),StarPos_Esy);
			CreatObjectToNGUI.InstantiateObj(GetStarPrefab(diffContainerID),StarPos_Diff);
			var ectypeContainerData = EctypeConfigManager.Instance.EctypeContainerConfigList[esyContaienrID];
			EctypeNameLabel.SetButtonText(LanguageTextManager.GetString(ectypeContainerData.lEctypeName));
			LevelLabel.SetButtonText(ectypeContainerData.lMinActorLevel.ToString());
			//if(isPointEctype || (!isPointEctype && (myParent.isMaxEctypeContaienrID(esyContaienrID)||myParent.isMaxEctypeContaienrID(diffContainerID))) )
			if (pointEctypeID != -1 && (pointEctypeID == esyContaienrID || pointEctypeID == diffContainerID)) {
				OnClickEvent(false);
			}
			else if(pointEctypeID == -1 && (myParent.isMaxEctypeContaienrID(esyContaienrID)||myParent.isMaxEctypeContaienrID(diffContainerID)) )
			{
				OnClickEvent(false);
			}
			mobaMark.SetActive (ectypeContainerData.IsMOBA==0?false:true);
            #region`    引导注入代码
            gameObject.RegisterBtnMappingId(esyContaienrID, UIType.Battle, BtnMapId_Sub.Battle_EctypeChoice);
            #endregion

		}

		public void SetMyselfActive(bool flag)
		{
			gameObject.SetActive(flag);
		}
		void OnClickEvent(bool withSFX)
		{
			if(IsLock)
				return;
			if(withSFX)
				SoundManager.Instance.PlaySoundEffect("Sound_Button_EctypeChoice");
			MyParent.OnEctypeContainerItemClick(this);
		}
		void OnClick()
		{
			OnClickEvent (true);

		}

		public void SetMySelfSelectStatus(SingleEctypeContainerItem selectItem)
		{
			bool isSelectMyself = selectItem!=null&&selectItem == this;
			SelectStatus.SetActive(isSelectMyself);
		}

		/// <summary>
		/// 获取评价星星资源
		/// </summary>
		/// <returns>The star prefab.</returns>
		GameObject GetStarPrefab(int ectypeContainerID)
		{
			GameObject starPrefab = MyParent.EvaluateData.EvaluateDataList[0].StarIconPrefab;
			var unlockData = EctypeModel.Instance.sMSGEctypeSelect_SC.sMSGEctypeData_SCs.FirstOrDefault(c=>c.dwEctypeContaienrID == ectypeContainerID);//  MyParent.MyParent.UnlockEctypeData.sMSGEctypeData_SCs.FirstOrDefault(c=>c.dwEctypeContaienrID == ectypeContainerID);
			//Debug.Log ("byGrade=="+unlockData.byGrade+"dwEctypeContaienrID==="+unlockData.dwEctypeContaienrID);
			if(unlockData.dwEctypeContaienrID!=0)
			{
				//TraceUtil.Log(SystemModel.Jiang,"GradByte:"+unlockData.byGrade);
				starPrefab = MyParent.EvaluateData.EvaluateDataList.First(c=>c.Evaluate ==GetGrade(unlockData.byGrade)).StarIconPrefab;
			}
			if(ectypeContainerID == 0)
			{
				starPrefab = null;
			}
			return starPrefab;
		}

		string GetGrade(byte _byGrade)
		{
			switch (_byGrade)
			{
			case 0:
			case 1:
				return "C";
			case 2:
				return "B";
			case 3:
				return "A";
			case 4:
				return "S";
			case 5:
				return "SS";
			case 6:
				return "SSS";
			default:
				return "INVAILD";
			}
		}
	}
}
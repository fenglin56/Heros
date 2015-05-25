using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace UI.MainUI
{
	public class SkillUpgradePanel : BaseUIPanel {
		//各按钮
		public SingleButtonCallBack btnBack;
		public BaseCommonPanelTitle comPanelTitle;
		//
		public GameObject skillLinePrefab;
		public List<GameObject> skillParentPoint ;
		private List<SkillUpgradeLineItem> skillLineList = new List<SkillUpgradeLineItem>();
		public SkillUpgradeInfo rightPanelInfo;
		public List<GameObject> effList = new List<GameObject> ();
		public GameObject skillUpgradeEff;
		public GameObject skillAdvanceEff;
		private bool isRead = false;
		void Init()
		{
			if (isRead)
				return;
			isRead = true;
			btnBack.SetCallBackFuntion (OnClickBackEvent);

			skillLinePrefab.SetActive (true);
			int Count = 4;
			for (int i = 0; i < Count; i++) {
				GameObject item = NGUITools.AddChild (skillParentPoint[i],skillLinePrefab);// CreatObjectToNGUI.InstantiateObj(skillLinePrefab,skillParentPoint[i].transform);
				SkillUpgradeLineItem scrItem = item.GetComponent<SkillUpgradeLineItem>();
				skillLineList.Add(scrItem);
			}
			skillLinePrefab.SetActive (false);
			/*for (int i= 0; i < 8; i++) {
				var sskillInfo = new SSkillInfo () { wSkillID = (ushort)(60440+i), wSkillLV = (byte)2 };
								PlayerManager.Instance.HeroSMsgSkillInit_SC.UpgradeSkill (sskillInfo);
						}*/
			skillUpgradeEff.SetActive (false);
			skillAdvanceEff.SetActive (false);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.SkillStrengthenEvent,OnStrengthenSuccEvent);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.SkillAdvanceEvent,OnAdvanceSuccEvent);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.UpgrateSkillInfo,OnUpgradeSuccEvent);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.ShopsBuySuccess, OnBuySuccess);
			comPanelTitle.Init(CommonTitleType.Money,CommonTitleType.GoldIngot);
			TaskGuideBtnRegister ();
		}

		public override void Show(params object[] value)
		{
			base.Show ();
			Init ();
			comPanelTitle.TweenShow ();
			ShowPanel ();
			//默认选中//
			int ID = SkillModel.Instance.GetFirstEquipSkillID ();
			SelectSkill(ID,SkillModel.Instance.GetSkillTreeID(ID),SkillModel.Instance.GetSkillPosIndex(ID));
		}
		void ShowPanel()
		{
			int i = 0;
			foreach (var skill in SkillModel.Instance.skillMap) {
				skillLineList [i++].Show (skill.Key,this);
			}
		}
		void OnClickBackEvent(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_SkillExit");
			Close ();
		}
		public override void Close()
		{
			if (!IsShow)
				return;
			base.Close();
			comPanelTitle.tweenClose ();
		}
		public void SelectSkill(int skillID,int treeId,int posIndex)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_SkillSelect");
			if (SkillModel.Instance.curSelectSkillID != 0) {
				int preTreeID = SkillModel.Instance.GetSkillTreeID (SkillModel.Instance.curSelectSkillID);
				int preIndex = SkillModel.Instance.GetSkillPosIndex (SkillModel.Instance.curSelectSkillID);
				skillLineList [preTreeID - 1].SelectSkill (false, preIndex);
			}
			SkillModel.Instance.curSelectSkillID = skillID;
			skillLineList [treeId - 1].SelectSkill (true,posIndex);
			rightPanelInfo.Show (skillID,treeId,posIndex,this);
		}
		void OnUpgradeSuccEvent(object obj)
		{
			SSkillInfo skillInfo = (SSkillInfo)obj;
			ShowEff (skillInfo.wSkillID,effList[0]);
			skillUpgradeEff.SetActive (false);
			skillUpgradeEff.SetActive (true);
		}
		void OnAdvanceSuccEvent(object obj)
		{
			SMsgSkillUnLock_SC skillInfo = (SMsgSkillUnLock_SC)obj;
			ShowEff ((int)skillInfo.wSkillId,effList[1]);
			skillAdvanceEff.SetActive (false);
			skillAdvanceEff.SetActive (true);
		}
		void OnStrengthenSuccEvent(object obj)
		{
			SMsgSkillStrengthen_SC skillInfo = (SMsgSkillStrengthen_SC)obj;
			ShowEff (skillInfo.wSkillId,effList[2]);
		}
		void ShowEff(int skillID,GameObject effPrefab)
		{
			int treeID = SkillModel.Instance.GetSkillTreeID (skillID);
			int index = SkillModel.Instance.GetSkillPosIndex (skillID);
			GameObject eff = NGUITools.AddChild (skillLineList [treeID - 1].skillList [index - 1].gameObject,effPrefab);
			StartCoroutine (DealEffSuccess(eff));
			RefashView (skillID);
		}
		IEnumerator DealEffSuccess(GameObject eff)
		{
			yield return new WaitForSeconds (1.2f);
			DestroyImmediate (eff);
		}
		void RefashView(int skillID)
		{
			ShowPanel ();
			SelectSkill(skillID,SkillModel.Instance.GetSkillTreeID(skillID),SkillModel.Instance.GetSkillPosIndex(skillID));
		}
		void OnBuySuccess(object obj)
		{
			SelectSkill(SkillModel.Instance.curSelectSkillID,SkillModel.Instance.GetSkillTreeID(SkillModel.Instance.curSelectSkillID),
			            SkillModel.Instance.GetSkillPosIndex(SkillModel.Instance.curSelectSkillID));
		}
		void OnDestroy()
		{
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.SkillStrengthenEvent,OnStrengthenSuccEvent);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.SkillAdvanceEvent,OnAdvanceSuccEvent);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.UpgrateSkillInfo,OnUpgradeSuccEvent);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ShopsBuySuccess, OnBuySuccess);
		}

		/// <summary>
		/// 引导按钮注入代码
		/// </summary>
		private void TaskGuideBtnRegister()
		{
			btnBack.gameObject.RegisterBtnMappingId(UIType.Skill, BtnMapId_Sub.Skill_UpgrdeBack);
		}
	}
}
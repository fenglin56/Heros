using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace UI.MainUI
{
	public class SkillUpgradeLineItem : MonoBehaviour {
		public GameObject skillItemPrefab;
		public SpriteSwith swithBg;
		public List<GameObject> parentPointList;
		[HideInInspector]
		public List<SkillUpgradeIconItem> skillList = new List<SkillUpgradeIconItem> ();
		private int curTreeID ;
		public SkillUpgradePanel myParent;
		private bool isRead = false;
		void Init()
		{
			if (isRead)
				return;
			isRead = true;
			skillItemPrefab.SetActive (true);
			int Count = SkillModel.Instance.skillMap [curTreeID].Count;
			for (int i = 0; i < Count; i++) {
				int index = (i==0?0:(Count==2?2:i));
				Transform parentPoint = parentPointList[index].transform;
				GameObject item = NGUITools.AddChild(parentPoint.gameObject,skillItemPrefab);//CreatObjectToNGUI.InstantiateObj(skillItemPrefab,parentPoint);
				SkillUpgradeIconItem scrItem = item.GetComponent<SkillUpgradeIconItem>();
				SingleButtonCallBack btnCB = item.GetComponent<SingleButtonCallBack>();
				btnCB.SetCallBackFuntion(OnClickSkillItemEvent,i);
				btnCB.gameObject.RegisterBtnMappingId(SkillModel.Instance.skillMap [curTreeID][i],UIType.Skill, BtnMapId_Sub.Skill_UpgrdeListItem);
				skillList.Add(scrItem);
			}
			skillItemPrefab.SetActive (false);
			swithBg.ChangeSprite ((curTreeID%2)==0?2:1);
		}
		
		public void Show(int treeID,SkillUpgradePanel myParent)
		{
			curTreeID = treeID;
			this.myParent = myParent;
			Init ();
			ShowPanel ();
		}
		public void SelectSkill(bool isSelect,int posIndex)
		{
			skillList [posIndex - 1].SelectSkill (isSelect);
		}
		void ShowPanel()
		{
			for (int i = 0; i < skillList.Count; i++) {
				skillList [i].Show (SkillModel.Instance.skillMap [curTreeID][i], curTreeID, i + 1, skillList.Count);
			}
		}
		void OnClickSkillItemEvent(object obj)
		{
			int pos = (int)obj;
			myParent.SelectSkill (skillList[pos].skillID,curTreeID,pos+1);
		}
	}
}

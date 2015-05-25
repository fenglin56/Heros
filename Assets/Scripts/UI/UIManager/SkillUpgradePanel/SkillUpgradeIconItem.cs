using UnityEngine;
using System.Collections;
namespace UI.MainUI
{
	public class SkillUpgradeIconItem : MonoBehaviour {
		public SpriteSwith selectSkillBg;
		public GameObject parentIconPoint;
		public GameObject iconNoOpen;
		public UILabel iconRedTip;
		public UILabel iconGreenTip;
		
		public GameObject skillMark;
		public GameObject curLevel;
		//public GameObject curLevelFull;
		public UILabel curLvLabel;
		public GameObject curStreng;
//		public GameObject curStrengFull;
		public UILabel curStrengLabel;
		public GameObject markLevel;
		public GameObject markStreng;
		public GameObject line;
		private SkillConfigData configData;
		private GameObject iconObj;
		private int width = 70;
		[HideInInspector]
		public int skillID = 0;
		private int treeID = 0;
		private int skillPosIndex ;
		private int skillCount;
		private bool isRead = false;
		void Init()
		{
			if (isRead)
				return;
			isRead = true;
			if ( (skillPosIndex == 2 && skillCount == 2) || (skillPosIndex == 3 && skillCount == 3)) {
				line.SetActive (false);			
			} else if(skillPosIndex == 2 && skillCount == 3){
				line.transform.localScale = new Vector3(width,line.transform.localScale.y,1);
			}
			else if(skillPosIndex == 1 && skillCount == 2)
			{
				line.transform.localScale = new Vector3(240,line.transform.localScale.y,1);
			}
			configData = SkillDataManager.Instance.GetSkillConfigData (skillID);
			if (iconObj == null)
				iconObj = CreatObjectToNGUI.InstantiateObj (configData.Icon_CirclePrefab,parentIconPoint.transform);	
		}
		//位置1-3，及总个数1-3//
		public void Show(int ID,int treeId,int posIndex,int count)
		{
			skillID = ID;
			treeID = treeId;
			skillPosIndex = posIndex;
			skillCount = count;
			Init ();
			CountSkill ();
		}
		public void SelectSkill(bool isSelect)
		{
			if (isSelect) {
				selectSkillBg.ChangeSprite (2);		
			} else {
				selectSkillBg.ChangeSprite (1);		
			}
		}
		void CountSkill()
		{
			if (!SkillModel.Instance.IsUnLockSkill (skillID)) {
				//未解锁，显示解锁等级//
				iconNoOpen.SetActive (true);
				iconRedTip.enabled = true;
				iconGreenTip.enabled = false;
				iconRedTip.text = LanguageTextManager.GetString ("IDS_I7_100") + "Lv." + configData.m_unlockLevel;
				skillMark.SetActive(false);
			} 
			else if (SkillModel.Instance.IsCanAdvanceSkill (skillID)) {
				//未进阶 但可进阶
				iconNoOpen.SetActive (true);
				iconRedTip.enabled = false;
				iconGreenTip.enabled = true;
				iconGreenTip.text = LanguageTextManager.GetString ("IDS_I7_115");
				skillMark.SetActive(false);
			}
			else if (!SkillModel.Instance.IsOpenSkill (skillID)) {
				//未进阶
				iconNoOpen.SetActive (true);
				iconRedTip.enabled = false;
				iconGreenTip.enabled = false;
				skillMark.SetActive(false);
			}
			else {
				iconNoOpen.SetActive (false);
				skillMark.SetActive(true);
				CountSkillMark ();
			}
		}
		//
		void CountSkillMark()
		{
			SSkillInfo? curSkill = SkillModel.Instance.GetCurSkill (skillID);
			if (curSkill == null) {
				int a = 0;			
			}
			//升级
			if (curSkill != null && curSkill.Value.wSkillLV >= configData.m_maxLv) {
				//升级满了
				curLevel.SetActive (false);
//				curLevelFull.SetActive (true);
				curLvLabel.enabled = false;
				markLevel.SetActive (false);
			} else {
				curLevel.SetActive (true);
//				curLevelFull.SetActive (false);
				curLvLabel.enabled = true;
				curLvLabel.text = curSkill.Value.wSkillLV.ToString();
				//可升级 箭头标记//
				if(PlayerManager.Instance.FindHeroDataModel ().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL >= 
				   configData.m_unlockLevel + curSkill.Value.wSkillLV*configData.m_UpdateInterval )
				{
					markLevel.SetActive (true);	
				}
				else
				{
					markLevel.SetActive (false);	
				}
			}
			//强化
			if (configData.SkillStrengthen == 0) {
				//不能强化
				curStreng.SetActive (false);
				markStreng.SetActive (false);
			} else if (curSkill.Value.byStrengthenLv >= configData.SkillStrengthen) {
				//满		
				curStreng.SetActive (false);
//				curStrengFull.SetActive (true);
				curStrengLabel.enabled = false;
				markStreng.SetActive (false);
			} else {
				//没满
				curStreng.SetActive (true);
//				curStrengFull.SetActive (false);
				curStrengLabel.enabled = true;
				curStrengLabel.text = curSkill.Value.byStrengthenLv.ToString();
				if(PlayerManager.Instance.IsMoneyEnough(configData.skillStrMoneyList[curSkill.Value.byStrengthenLv-1]))
				{
					markStreng.SetActive (true);
				}
				else
				{
					markStreng.SetActive (false);
				}
			}
		}
		void ShowSkillOpen()
		{
			if (!SkillModel.Instance.IsUnLockSkill (skillID)) {
				//未解锁，显示解锁等级//
				iconNoOpen.SetActive (true);
				iconRedTip.enabled = true;
				iconGreenTip.enabled = false;
				iconRedTip.text = LanguageTextManager.GetString ("IDS_I7_100") + "Lv." + configData.m_unlockLevel;
			} 
			else if (SkillModel.Instance.IsCanAdvanceSkill (skillID)) {
				//未进阶 但可进阶
				iconNoOpen.SetActive (true);
				iconRedTip.enabled = false;
				iconGreenTip.enabled = true;
				iconGreenTip.text = LanguageTextManager.GetString ("IDS_I7_115");		
			}
			else if (SkillModel.Instance.IsOpenSkill (skillID)) {
				//未进阶
				iconNoOpen.SetActive (true);
				iconRedTip.enabled = false;
				iconGreenTip.enabled = false;
			}
			else {
				iconNoOpen.SetActive (false);
			}
		}
	}
}

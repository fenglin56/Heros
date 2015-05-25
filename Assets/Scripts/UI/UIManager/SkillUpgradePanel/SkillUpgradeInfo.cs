using UnityEngine;
using System.Collections;
namespace UI.MainUI
{
	public class SkillUpgradeInfo : MonoBehaviour {
		//top
		public GameObject InfoObj;
		public UILabel skillNameLabel;
		public UILabel line1Title;
		public UILabel line1Lv;
		public UILabel line1LvMark;
		public UILabel line2Title;
		public UILabel line2Precent;
		public UILabel line3Info;
		//middle
		public GameObject unlockObj;
		public UILabel unlockTitle;
		public UILabel unlockCondi;
		public UILabel unlockCondiLv;
		public GameObject UpgradeObj;
		public SingleButtonCallBack upgradeBtn;
		public UILabel upgardeBtnWord;
		public GameObject upgradeBtnEff;
		public UILabel upgradeTitle;
		public UILabel upgradeLvTit;
		public UILabel upgradeLvDat;
		public UILabel upgradeMonTit;
		public UILabel upgradeMonDat;
		public GameObject upgradeFullObj;
		public GameObject AdvanceObj;
		public SingleButtonCallBack advanceBtn;
		public UILabel advanceBtnWord;
		public GameObject advanceBtnEff;
		public UILabel advanceTitle;
		public UILabel advanceLvTit;
		public UILabel advanceLvDat;
		public UILabel advanceMonTit;
		public UILabel advanceMonDat;
		public SingleButtonCallBack advanceIconBtn;
		public VipAwardItemIcon advanceIcon;
		//bottom
		public GameObject notStrengObj;
		public GameObject strengObj;
		public UILabel strengLine1Name;
		public UILabel strengLine1Lv;
		public UILabel strengLine2Name;
		public UILabel strengLine2Precent;
		public GameObject canStrengObj;
		public GameObject isFullObj;
		public UILabel strengCondiName;
		public UILabel strengCond;
		public SingleButtonCallBack strengthenBtn;
		public UILabel strengthenBtnWord;
		public GameObject strengthenBtnEff;
		//数据
		private SkillConfigData configData;
		private SSkillInfo? curSkill;
		private int skillID = 0;
		private int treeID = 0;
		private int skillPosIndex ;
		private SkillUpgradePanel myParent;
		private bool isRead = false;
		void Init()
		{
			if (isRead)
				return;
			isRead = true;
			line1Title.text = LanguageTextManager.GetString ("IDS_I7_101");
			line2Title.text = LanguageTextManager.GetString ("IDS_I7_103");
			upgradeTitle.text = LanguageTextManager.GetString ("IDS_I7_111");
			upgradeLvTit.text = LanguageTextManager.GetString ("IDS_I7_105");
			upgradeMonTit.text = LanguageTextManager.GetString ("IDS_I7_112");
			advanceTitle.text = LanguageTextManager.GetString ("IDS_I7_107");
			advanceLvTit.text = LanguageTextManager.GetString ("IDS_I7_108");
			advanceMonTit.text = LanguageTextManager.GetString ("IDS_I7_109");
			strengLine1Name.text = LanguageTextManager.GetString ("IDS_I7_113");
			strengCondiName.text = LanguageTextManager.GetString ("IDS_I7_114");
			unlockTitle.text = LanguageTextManager.GetString ("IDS_I7_104");
			unlockCondi.text = LanguageTextManager.GetString ("IDS_I7_105");

			upgardeBtnWord.text = LanguageTextManager.GetString ("IDS_I7_117");
			advanceBtnWord.text = LanguageTextManager.GetString ("IDS_I7_116");
			strengthenBtnWord.text = LanguageTextManager.GetString ("IDS_I7_118");

			upgradeBtn.SetCallBackFuntion(OnUpgradeEvent);
			advanceBtn.SetCallBackFuntion(OnAdvanceEvent);
			strengthenBtn.SetCallBackFuntion(OnStrengthenEvent);
			advanceIconBtn.SetCallBackFuntion(OnItemBoxEvent);
			TaskGuideBtnRegister ();
		}

		public void Show(int ID,int treeId,int posIndex,SkillUpgradePanel myparent)
		{
			skillID = ID;
			treeID = treeId;
			skillPosIndex = posIndex;
			myParent = myparent;
			configData = SkillDataManager.Instance.GetSkillConfigData (skillID);
			curSkill = SkillModel.Instance.GetCurSkill (skillID);
			Init ();
			ShowInfo ();
		}
		void ShowInfo()
		{
			ShowTop ();
			ShowMid ();
			ShowBottom ();
			//SkillMiddle (true,advanceLvDat,advanceMonDat);
			//进阶
			//advanceTitle
		}
		void ShowTop()
		{
			skillNameLabel.text = LanguageTextManager.GetString (configData.m_name);
			if (!SkillModel.Instance.IsUnLockSkill (skillID)) {
				line1LvMark.enabled = true;
				line1Lv.enabled = false;
				line1LvMark.text = LanguageTextManager.GetString ("IDS_I7_102");
			} else if (!SkillModel.Instance.IsOpenSkill (skillID)) {
				line1LvMark.enabled = true;
				line1Lv.enabled = false;
				line1LvMark.text = LanguageTextManager.GetString ("IDS_I7_106");
			} else{
				line1LvMark.enabled = false;
				line1Lv.enabled = true;
				line1Lv.text = "Lv." + curSkill.Value.wSkillLV;
			}
			line2Precent.text = string.Format ("{0}$",SkillModel.Instance.SkillHurt(skillID));
			line3Info.text = LanguageTextManager.GetString (configData.m_desc);
		}
		void ShowMid()
		{
			upgradeFullObj.SetActive (false);
			UpgradeObj.SetActive (false);
			unlockObj.SetActive (false);
			AdvanceObj.SetActive (false);
			upgradeBtnEff.SetActive(false);
			advanceBtnEff.SetActive(false);
			if (SkillModel.Instance.IsOpenSkill (skillID)) {
				if(curSkill.Value.wSkillLV >= configData.m_maxLv)
				{
					//最高等级，显示不能升级了//
					upgradeFullObj.SetActive(true);
				}
				else
				{
					//可升级
					UpgradeObj.SetActive(true);
					upgradeBtn.Enable = true;
					if(SkillMiddle(true,upgradeLvDat,upgradeMonDat))
					{
						upgradeBtnEff.SetActive(true);
					}
				}
			} else {
				if(configData.PreSkill == 0)
				{
					//未解锁
					unlockObj.SetActive(true);
					unlockCondiLv.text = string.Format("[FF0000]{0}/{1}",PlayerManager.Instance.FindHeroDataModel ().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL,
					                                   configData.m_unlockLevel);
				}
				else
				{
					//进阶
					AdvanceObj.SetActive(true);
					if(SkillMiddle(false,advanceLvDat,advanceMonDat))
					{
						advanceBtn.Enable = true;
					}
					else
					{
						advanceBtn.Enable = false;
					}
					string countStr = "[FF0000]{0}/{1}[-]";
					int goodsCount = ContainerInfomanager.Instance.GetItemNumber (configData.advItemID);
					if ( goodsCount >= configData.advItemCount)
					{
						if(advanceBtn.Enable)
						{
							advanceBtnEff.SetActive(true);
						}
						countStr = "[54E44F]{0}/{1}[-]";
					}
					advanceIcon.ShowGoods(configData.advItemID,string.Format(countStr,goodsCount,configData.advItemCount));
				}
			}
		}
		void ShowBottom()
		{
			notStrengObj.SetActive (false);
			strengObj.SetActive (false);
			canStrengObj.SetActive (false);
			isFullObj.SetActive (false);
			strengthenBtnEff.SetActive(false);
			string str = "{0}/{1}";
			strengLine2Name.text = LanguageTextManager.GetString (configData.SkillStrengthen_Text);
			string monStr = "";
			if (configData.SkillStrengthen == 0) {
				//不能强化
				notStrengObj.SetActive (true);
			} else if (curSkill != null && curSkill.Value.byStrengthenLv >= configData.SkillStrengthen) {
				//强化满了	
				isFullObj.SetActive (true);
				strengObj.SetActive (true);
				str = "{0}";
				strengLine1Lv.text = string.Format (str, curSkill.Value.byStrengthenLv);
				strengLine2Precent.text = string.Format("+{0}$",configData.skillStrDamegeList [curSkill.Value.byStrengthenLv-1]);// 
			} else {
				//可以强化//
				strengObj.SetActive (true);
				canStrengObj.SetActive (true);
				if(curSkill == null)
				{
					strengthenBtn.gameObject.SetActive(false);
					//System.DateTime
				}
				else
				{
					strengthenBtn.gameObject.SetActive(true);
					strengthenBtn.Enable = true;
				}
				int strengthenLv = curSkill == null?1:(int)(curSkill.Value.byStrengthenLv);
				strengLine1Lv.text = string.Format (str, strengthenLv,configData.SkillStrengthen);
				//{0}{1}${2}[{3}{4}$]
				strengLine2Precent.text = string.Format(LanguageTextManager.GetString("IDS_I7_120"),"+",configData.skillStrDamegeList[strengthenLv-1],"[54E44F]","+",
				                                        configData.skillStrDamegeList[strengthenLv]-configData.skillStrDamegeList[strengthenLv-1]);// 			
				if(PlayerManager.Instance.IsMoneyEnough(configData.skillStrMoneyList[strengthenLv-1]))
				{
					monStr = "[54E44F]"+configData.skillStrMoneyList[strengthenLv-1];
					if(strengthenBtn.Enable)
					{
						strengthenBtnEff.SetActive(true);
					}
				}
				else
				{
					monStr = "[FF0000]"+configData.skillStrMoneyList[strengthenLv-1];
				}
				strengCond.text = "[FFFFFF]"+string.Format(LanguageTextManager.GetString("IDS_I7_121"),monStr);
			}
		}
		bool SkillMiddle(bool isUpgrade,UILabel lvLabel,UILabel monLabel)
		{
			bool isCanClick = false;
			string monStr = "";
			//升级
			string upgradeStr = "[FF0000]{0}/{1}[-]";
			if (isUpgrade) {
				if (PlayerManager.Instance.FindHeroDataModel ().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL >= 
				    configData.m_unlockLevel + curSkill.Value.wSkillLV * configData.m_UpdateInterval) {
					upgradeStr = "[54E44F]{0}/{1}[-]";
					isCanClick = true;
				}
				lvLabel.text = string.Format (upgradeStr, PlayerManager.Instance.FindHeroDataModel ().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL,
				                              configData.m_unlockLevel + curSkill.Value.wSkillLV * configData.m_UpdateInterval);
				if (PlayerManager.Instance.IsMoneyEnough (configData.m_upgradeMoneyParams [curSkill.Value.wSkillLV - 1])) {
					monStr = "[54E44F]" + configData.m_upgradeMoneyParams [curSkill.Value.wSkillLV - 1];
					if(!isCanClick)
						isCanClick = false;
				} else {
					monStr = "[FF0000]" + configData.m_upgradeMoneyParams [curSkill.Value.wSkillLV - 1];		
					if(isCanClick)
						isCanClick = false;
				}
				monLabel.text = "[FFFFFF]"+string.Format(LanguageTextManager.GetString("IDS_I7_121"),monStr);// ;
			} else {
				SkillConfigData configPre = SkillDataManager.Instance.GetSkillConfigData (configData.PreSkill);
				SSkillInfo? preSkill = SkillModel.Instance.GetCurSkill (configData.PreSkill);
				lvLabel.text = LanguageTextManager.GetString (configPre.m_name);
				int preLv = preSkill==null?0:(int)preSkill.Value.wSkillLV;
				if(preSkill != null && preSkill.Value.wSkillLV >= configPre.m_maxLv)
				{
					upgradeStr = "[54E44F]{0}/{1}[-]";
					isCanClick = true;
				}
				monLabel.text = string.Format(upgradeStr,preLv,configPre.m_maxLv);	
			}

			return isCanClick;
		}
		void OnUpgradeEvent(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_SkillLevelUp");
			if (PlayerManager.Instance.FindHeroDataModel ().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL < 
			    configData.m_unlockLevel + curSkill.Value.wSkillLV * configData.m_UpdateInterval) {
				UI.MessageBox.Instance.ShowTips (3, LanguageTextManager.GetString ("IDS_I7_119"), 1);
				return;
			}
			if (!PlayerManager.Instance.IsMoneyEnough (configData.m_upgradeMoneyParams [curSkill.Value.wSkillLV - 1])) {
				UI.MessageBox.Instance.ShowNotEnoughMoneyMsg(null);
			}
			NetServiceManager.Instance.EntityService.SendSkillUpgrade((ushort)skillID);
		}
		void OnAdvanceEvent(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_SkillUnlock");
		 	//configData.advItemCount
			if (ContainerInfomanager.Instance.GetItemNumber (configData.advItemID) >= configData.advItemCount) {
				NetServiceManager.Instance.EntityService.SendSkillAdvance ((ushort)skillID);
				} else {
				//直接弹出tip
				UI.MessageBox.Instance.ShowTips (3, LanguageTextManager.GetString ("IDS_I7_123"), 1);
				//ItemInfoTipsManager.Instance.Show(configData.advItemID);
			}
		}
		void OnStrengthenEvent(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_SkillStrengthen");
			if(!PlayerManager.Instance.IsMoneyEnough(configData.skillStrMoneyList[curSkill.Value.byStrengthenLv-1])){
				UI.MessageBox.Instance.ShowNotEnoughMoneyMsg(null);
				return;
			}
			NetServiceManager.Instance.EntityService.SendSkillStrengthen ((ushort)skillID);
		}
		void OnItemBoxEvent(object obj)
		{
			//直接弹出tip
			ItemInfoTipsManager.Instance.Show(configData.advItemID);
		}
		/// <summary>
		/// 引导按钮注入代码
		/// </summary>
		private void TaskGuideBtnRegister()
		{
			upgradeBtn.gameObject.RegisterBtnMappingId(UIType.Skill, BtnMapId_Sub.Skill_UpgrdeUpgrade);
			advanceBtn.gameObject.RegisterBtnMappingId(UIType.Skill, BtnMapId_Sub.Skill_UpgrdeAdvance);
			strengthenBtn.gameObject.RegisterBtnMappingId(UIType.Skill, BtnMapId_Sub.Skill_UpgrdeStrengthen);
			advanceIconBtn.gameObject.RegisterBtnMappingId(UIType.Skill, BtnMapId_Sub.Skill_UpgrdeGoodsIcon);
		}
	}
}
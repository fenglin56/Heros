using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI
{

    public class GoodsOperateArtificePanel : View
    {
        public GameObject UpgradeEffectPrefab;
        public Transform CreatUpgradeEffectTransform;

        public UILabel CurrentLevelLabel;//当前技能等级
        public UILabel ProgressLabel;//进度显示
        public UISlider CurrentProgress;//当前技能升级进度
        public UISlider AddProgress;//预判的技能升级进度
        public GameObject FullLevelIcon;//当满级时出现的图标

        public SinglePassiveSkillItem[] PassiveSkillItemList;//被动技能显示面板列表
        public SelectArtificeGoodsPanel SelectArtificeGoodsPanel;//选择炼化装备面板
        public OperateSophisticationMsgPanel OperateSophisticationMsgPanel;//装备洗练确认窗口
        public SingleButtonCallBack ResetEquipmentSkillBtn;//洗练按钮
        public SingleButtonCallBack EquipmentOperateArtificeBtn;//炼化按钮
        public EquipmentRefiningDataBase EquipmentRefiningDataBase;//装备炼化配表
        public ItemFielInfo SelectItemData { get;private set;}
        public EquipStrenManager MyParent { get; private set; }

        public bool IsResetSkill { get; private set; }
        bool IsUpdate = false;
        bool IsUpdatePanel = false;
        EquipmentEntity CurrentItemEquiptEntity;

		private int[] m_guideID;

        void Awake()
        {
            PassiveSkillItemList.ApplyAllItem(P => P.ResetWidgetInfo());
            CurrentLevelLabel.text = "";
            ProgressLabel.text = "";
            CurrentProgress.sliderValue = 0;
            AddProgress.sliderValue = 0;
            CurrentItemEquiptEntity = new EquipmentEntity();
            IsResetSkill = false;
            FullLevelIcon.SetActive(false);
        }

        void Start()
        {
            SelectArtificeGoodsPanel.Init(this);
            EquipmentOperateArtificeBtn.SetCallBackFuntion(OnEquipmentOperateArtificeBtnClick);
            ResetEquipmentSkillBtn.SetCallBackFuntion(OnResetSkillBtnClick);
            AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), ResetStatus);
			m_guideID = new int[2];
			//TODO GuideBtnManager.Instance.RegGuideButton (ResetEquipmentSkillBtn.gameObject, UIType.EquipStrengthen, SubType.EquipStrenOperate, out m_guideID [0]);
			//TODO GuideBtnManager.Instance.RegGuideButton (EquipmentOperateArtificeBtn.gameObject, UIType.EquipStrengthen, SubType.EquipStrenOperate, out m_guideID [1]);
        }

        void OnDestroy()
        {
            RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), ResetStatus); 
			for (int i = 0; i < m_guideID.Length; i++) {
				//TODO GuideBtnManager.Instance.DelGuideButton(m_guideID[i]);
			}
        }


        void ResetStatus(INotifyArgs inotifyArgs)
        {
            EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)inotifyArgs;
            //TraceUtil.Log("更新装备");
            if (entityDataUpdateNotify.IsHero&&CheckIsChange())
            {
                //TraceUtil.Log("可以更新装备");
                if (!IsUpdatePanel)
                {
                    IsUpdatePanel = true;
                    DoForTime.DoFunForTime(0.2f, UpdatePanelForTime, null);
                }
                //Show(SelectItemData,MyParent);
            }
        }
        /// <summary>
        /// 当连续几次单体更新过来时，过一段时间再更新 
        /// </summary>
        /// <param name="obj"></param>
        void UpdatePanelForTime(object obj)
        {
            IsUpdatePanel = false;
            UpdatePanel();
        }

        bool CheckIsChange()
        {
            IsUpdate = false;
//            if (SelectItemData != null)
//            {
//                if (SelectItemData.equipmentEntity.EQUIP_FIELD_SOPHISTICATION_LEVEL != CurrentItemEquiptEntity.EQUIP_FIELD_SOPHISTICATION_LEVEL ||
//                    SelectItemData.equipmentEntity.EQUIP_FIELD_SOPHISTICATION_PERCENT != CurrentItemEquiptEntity.EQUIP_FIELD_SOPHISTICATION_PERCENT ||
//                    SelectItemData.equipmentEntity.EQUIP_FIELD_SKILL_ID1 != CurrentItemEquiptEntity.EQUIP_FIELD_SKILL_ID1 ||
//                    SelectItemData.equipmentEntity.EQUIP_FIELD_SKILL_VALUE1 != CurrentItemEquiptEntity.EQUIP_FIELD_SKILL_VALUE1 ||
//                    SelectItemData.equipmentEntity.EQUIP_FIELD_SKILL_ID2 != CurrentItemEquiptEntity.EQUIP_FIELD_SKILL_ID2 ||
//                    SelectItemData.equipmentEntity.EQUIP_FIELD_SKILL_VALUE2 != CurrentItemEquiptEntity.EQUIP_FIELD_SKILL_VALUE2 ||
//                    SelectItemData.equipmentEntity.EQUIP_FIELD_SKILL_ID3 != CurrentItemEquiptEntity.EQUIP_FIELD_SKILL_ID3 ||
//                    SelectItemData.equipmentEntity.EQUIP_FIELD_SKILL_VALUE3 != CurrentItemEquiptEntity.EQUIP_FIELD_SKILL_VALUE3)
//                {
//                    if ((SelectItemData.equipmentEntity.EQUIP_FIELD_SKILL_ID1 != 0 && SelectItemData.equipmentEntity.EQUIP_FIELD_SKILL_VALUE1 == 0) ||
//                        (SelectItemData.equipmentEntity.EQUIP_FIELD_SKILL_ID2 != 0 && SelectItemData.equipmentEntity.EQUIP_FIELD_SKILL_VALUE2 == 0) ||
//                        (SelectItemData.equipmentEntity.EQUIP_FIELD_SKILL_ID3 != 0 && SelectItemData.equipmentEntity.EQUIP_FIELD_SKILL_VALUE3 == 0))
//                    {
//                        IsUpdate = false;
//                    }
//                    else
//                    {
//                        if (CurrentItemEquiptEntity.EQUIP_FIELD_SOPHISTICATION_LEVEL < SelectItemData.equipmentEntity.EQUIP_FIELD_SOPHISTICATION_LEVEL)
//                        {
//                            CreatUpgradeEffect();//升级动画
//                        }
//                        CurrentItemEquiptEntity = SelectItemData.equipmentEntity;
//                        IsUpdate = true;
//                    }
//                }
//            }
            return IsUpdate;
        }

        void CreatUpgradeEffect()
        {
            GameObject creatObj = CreatObjectToNGUI.InstantiateObj(UpgradeEffectPrefab, CreatUpgradeEffectTransform);
            DoForTime.DoFunForTime(2, DestroyUpgradEffect, creatObj);
        }

        void DestroyUpgradEffect(object obj)
        {
            GameObject destroyEffect = obj as GameObject;
            if (destroyEffect != null)
            {
                Destroy(destroyEffect);
            }
        }

        void TweenAddProgressBar(float number)
        {
            CurrentProgress.sliderValue = number;
            string showStr = (number * 100).ToString();
            ProgressLabel.SetText(string.Format("{0}%", showStr.Length>1?showStr.Substring(0,2):showStr));
        }

        void UpdatePanel()
        {
            if (MyParent != null&&gameObject.active==true)
            {
                MyParent.Init(null);
            }
        }

        public void Show(ItemFielInfo itemFileInfo,EquipStrenManager myParent)
        {
//            this.SelectItemData = itemFileInfo;
//            //TraceUtil.Log("默认洗练等级：" + SelectItemData.equipmentEntity.EQUIP_FIELD_SOPHISTICATION_LEVEL);
//            this.MyParent = myParent;
//            if (itemFileInfo == null)
//            {
//                ResetPanelStatus();
//                return;
//            }
//            CurrentItemEquiptEntity = SelectItemData.equipmentEntity;
//            PassiveSkillItemList[0].Show(itemFileInfo.equipmentEntity.EQUIP_FIELD_SKILL_ID1, itemFileInfo.equipmentEntity.EQUIP_FIELD_SKILL_VALUE1,myParent.PassiveSkillDataBase,this);
//            PassiveSkillItemList[1].Show(itemFileInfo.equipmentEntity.EQUIP_FIELD_SKILL_ID2, itemFileInfo.equipmentEntity.EQUIP_FIELD_SKILL_VALUE2, myParent.PassiveSkillDataBase, this);
//            PassiveSkillItemList[2].Show(itemFileInfo.equipmentEntity.EQUIP_FIELD_SKILL_ID3, itemFileInfo.equipmentEntity.EQUIP_FIELD_SKILL_VALUE3, myParent.PassiveSkillDataBase, this);
//            TraceUtil.Log("设置刷新技能false");
//            IsResetSkill = false;
//            UpdateSkillProgressBar();
//            FullLevelIcon.SetActive(CheckIsFullLevel(itemFileInfo));
        }

        void ResetPanelStatus()
        {
            PassiveSkillItemList.ApplyAllItem(P => P.Show(0, 0,MyParent.PassiveSkillDataBase, this));
            CurrentLevelLabel.text = "";
            ProgressLabel.text = "";
            CurrentProgress.sliderValue = 0;
            AddProgress.sliderValue = 0; 
        }

        /// <summary>
        /// 检测是否满级
        /// </summary>
        /// <returns></returns>
        public bool CheckIsFullLevel(ItemFielInfo itemFileInfo)
        {
            bool flag = false;
            //flag = itemFileInfo.equipmentEntity.EQUIP_FIELD_SOPHISTICATION_LEVEL >= CommonDefineManager.Instance.CommonDefineFile._dataTable.RefiningLevel;
            return flag;
        }

        public void OnEquipmentOperateArtificeBtnClick(object obj)
        {
            if (SelectItemData == null)
            {
                return;
            }
            if (SelectArtificeGoodsPanel.IsShow)
            {
                EquipmentOperateArtificeBtn.SetButtonBackground(SelectArtificeGoodsPanel.IsShow ? 1 : 2);
                SelectArtificeGoodsPanel.TweenClose();
                UpdateAddValueProgressBar();
            }
            else
            {
                if (CheckIsFullLevel(SelectItemData))
                {
                    MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_526"), 1);
                    //提示满级
                }
                else
                {
                    EquipmentOperateArtificeBtn.SetButtonBackground(SelectArtificeGoodsPanel.IsShow ? 1 : 2);
                    SelectArtificeGoodsPanel.TweenShow(SelectItemData);
                }
            }
        }

        void OnResetSkillBtnClick(object obj)
        {
            if (SelectItemData == null)
            {
                return;
            }
            OperateSophisticationMsgPanel.Show(SelectItemData,this);
        }
        /// <summary>
        /// 发送洗练技能到服务器
        /// </summary>
        public void SendResetPassiveSkillToSever(ItemFielInfo selectItem)
        {
            IsResetSkill = true;
            NetServiceManager.Instance.EquipStrengthenService.SendGoodsOperateSophisticationCommoand(selectItem.sSyncContainerGoods_SC.uidGoods);
        }

        public void UpdateSkillProgressBar()
        {
//            //CurrentLevelLabel.SetText(SelectItemData.equipmentEntity.EQUIP_FIELD_SOPHISTICATION_LEVEL);
//            if (CheckIsFullLevel(SelectItemData))
//            {
//                CurrentProgress.sliderValue = 1;
//                ProgressLabel.SetText("100%");
//                return;
//            }
//			float currentProgressValue =0; //(float)SelectItemData.equipmentEntity.EQUIP_FIELD_SOPHISTICATION_PERCENT/100f;
//            if (!IsUpdate||currentProgressValue<CurrentProgress.sliderValue)
//            {
//                CurrentProgress.sliderValue = currentProgressValue;
//                ProgressLabel.SetText(string.Format("{0}%", currentProgressValue*100));
//            }
//            else
//            {
//                TweenFloat.Begin(1, CurrentProgress.sliderValue, currentProgressValue, TweenAddProgressBar, null);
//                IsUpdate = false;
//            }
//            //CurrentProgress.sliderValue = (float)(SelectItemData.equipmentEntity.EQUIP_FIELD_SOPHISTICATION_PERCENT / 100f);
//            AddProgress.sliderValue = (float)SelectItemData.equipmentEntity.EQUIP_FIELD_SOPHISTICATION_PERCENT / 100f;
//            TraceUtil.Log("BaseValue:" + AddProgress.sliderValue);
//            SelectArtificeGoodsPanel.GetSelectItemList().ApplyAllItem(P => AddProgress.sliderValue+=GetEquipItemExpAddProgress(P));
        }

        float GetEquipItemExpAddProgress(ItemFielInfo item)
        {
//            var currentEquipmentRefiningData = EquipmentRefiningDataBase.EquipmentRefiningDatatable.FirstOrDefault(P => P.lGoodsSubClass == SelectItemData.LocalItemData._GoodsSubClass
//                && P.lColorLevel == SelectItemData.LocalItemData._ColorLevel
//                && P.lLevel_Min <= SelectItemData.LocalItemData._AllowLevel && P.lLevel_Max >= SelectItemData.LocalItemData._AllowLevel);
//			if(currentEquipmentRefiningData == null)
//			{
//				TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"找不到对应物品炼化经验data："+item.LocalItemData._goodID);
//			}
//            if (item.LocalItemData._GoodsSubClass == 9)//如果为妖女内丹
//            {
//                float m_GetValue = 0;
//                for (int i = 0; i < item.sSyncContainerGoods_SC.byNum;i++ )
//                {
//                    m_GetValue += float.Parse((item.LocalItemData as MaterielData)._szParam1) / (float)currentEquipmentRefiningData.lLevelUpNeed[SelectItemData.equipmentEntity.EQUIP_FIELD_SOPHISTICATION_LEVEL - 1];
//                    //TraceUtil.Log("内丹预判值：" + m_GetValue);
//                }
//				TraceUtil.Log("内丹获得经验预判值：" + m_GetValue);
//                return m_GetValue;
//            }
//            var commonData = CommonDefineManager.Instance.CommonDefineFile._dataTable;
//            var equipmentRefiningData = EquipmentRefiningDataBase.EquipmentRefiningDatatable.First(P => P.lGoodsSubClass == item.LocalItemData._GoodsSubClass 
//                && P.lColorLevel == item.LocalItemData._ColorLevel
//                &&P.lLevel_Max>=item.LocalItemData._AllowLevel&&P.lLevel_Min<=item.LocalItemData._AllowLevel);
//            TraceUtil.Log(equipmentRefiningData.lColorLevel + "," + equipmentRefiningData.lGoodsSubClass + "," + equipmentRefiningData.lLevelUpNeed);
//            float currentExp = ((float)equipmentRefiningData.lLevelUpNeed[item.equipmentEntity.EQUIP_FIELD_SOPHISTICATION_LEVEL-1]) * (float)(item.equipmentEntity.EQUIP_FIELD_SOPHISTICATION_PERCENT / 100f);
//            for (int i = 0; i < item.equipmentEntity.EQUIP_FIELD_SOPHISTICATION_LEVEL-1; i++)
//            {
//                currentExp += equipmentRefiningData.lLevelUpNeed[i];
//            }
//            //currentExp += equipmentRefiningData.lLevelUpNeed[item.equipmentEntity.EQUIP_FIELD_SOPHISTICATION_LEVEL - 1] * item.equipmentEntity.EQUIP_FIELD_SOPHISTICATION_PERCENT / 100f;
//            float expAdd = (int)(commonData.RefiningPrama_Base * (1 + (float)item.LocalItemData._Level * (float)commonData.RefiningPrama_Level / 100f +
//                (float)item.LocalItemData._ColorLevel * (float)commonData.RefiningPrama_ColorLevel / 100f) + currentExp * commonData.RefiningPrama_Discount / 100);
//
//            float getValue = expAdd / (float)currentEquipmentRefiningData.lLevelUpNeed[SelectItemData.equipmentEntity.EQUIP_FIELD_SOPHISTICATION_LEVEL - 1];
//            TraceUtil.Log("getValue :" + getValue);
//            TraceUtil.Log("currentExp :" +  currentExp * commonData.RefiningPrama_Discount / 100);
//            TraceUtil.Log("CurrentNeedExp"+currentEquipmentRefiningData.lLevelUpNeed[SelectItemData.equipmentEntity.EQUIP_FIELD_SOPHISTICATION_LEVEL - 1]);
            return 0;
        }

        public void UpdateAddValueProgressBar()
        {
            AddProgress.sliderValue = 0;
        }

        protected override void RegisterEventHandler()
        {
            throw new System.NotImplementedException();
        }
    }
}
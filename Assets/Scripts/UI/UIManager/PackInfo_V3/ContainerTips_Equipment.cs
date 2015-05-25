using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI
{
    /// <summary>
    /// 装备类
    /// </summary>
    public class ContainerTips_Equipment : BaseContainerTips_V2
    {

        public Transform CreatItemIconPoint;

        public ContainerTipsEffectLabel EffectLabel1, EffectLabel2;
        public ContainerTipsPassiveSkillLabel[] PassiveSkillLabelList;

        public PassiveSkillDataBase PassiveSkillData;

        public UILabel ProfessionLabel;//职业需求
        public UILabel LevelLabel;//等级需求
        public UILabel OperateArtificeLabel;//炼化等级
        public UILabel PriceLabel;//价格
        public UILabel ItemNameLabel;//装备名字
        public UILabel ItemDesLabel;//装备描述
        public SingleButtonCallBack SellBtn;
        public SingleButtonCallBack CloseBtn;
        public Transform TransitionItem;
        public Vector3[] TransitionItemPos = new Vector3[4];
        public ItemFielInfo MyItemfileInfo { get; private set; }

        void Start()
        {
            SellBtn.SetCallBackFuntion(OnSellBtnClick);
            CloseBtn.SetCallBackFuntion(OnCloseBtnClick);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.OnRoleViewClick, OnCloseBtnClick);

            base.RegBtn(SellBtn.gameObject, CloseBtn.gameObject);
            ////TODO GuideBtnManager.Instance.RegGuideButton(SellBtn.gameObject, UIType.PackInfo, SubType.PackageEquipDescPanel, out m_guideBtnID[0]);
            ////TODO GuideBtnManager.Instance.RegGuideButton(CloseBtn.gameObject, UIType.PackInfo, SubType.PackageEquipDescPanel, out m_guideBtnID[1]);
        }

        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OnRoleViewClick, OnCloseBtnClick);
            base.UnRegBtn();
        }


        public override void Show(ItemFielInfo itemFielInfo)
		{
//            MyItemfileInfo = itemFielInfo;
//            EffectLabel1.ShowEffect(itemFielInfo,0);
//            EffectLabel2.ShowEffect(itemFielInfo,1);
//            //int currentLv = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
//            CreatItemIconPoint.ClearChild();
//            CreatObjectToNGUI.InstantiateObj(itemFielInfo.LocalItemData._picPrefab, CreatItemIconPoint);
//
//            string ItemName = LanguageTextManager.GetString(itemFielInfo.LocalItemData._szGoodsName);
//            int StrengLevel = itemFielInfo.equipmentEntity.EQUIP_FIELD_STRONGE_LEVEL;
//            this.ItemNameLabel.SetText(NGUIColor.SetTxtColor(StrengLevel > 0 ? string.Format("{0}+{1}", ItemName, StrengLevel) : ItemName, GetNameLabelColor(itemFielInfo)));
//            ItemDesLabel.SetText(LanguageTextManager.GetString(itemFielInfo.LocalItemData._szDesc));
//
//            LevelLabel.SetText(string.Format("[9bfc3b]{0}[-]{1}",itemFielInfo.LocalItemData._AllowLevel, LanguageTextManager.GetString("IDS_H1_156")));
//            //LevelLabel.color = currentLv >= itemFielInfo.LocalItemData._AllowLevel ? Color.green : Color.red;
//            PriceLabel.SetText(itemFielInfo.LocalItemData._SaleCost);
//            ProfessionLabel.SetText(GetProfession(itemFielInfo.LocalItemData._AllowProfession));
//            OperateArtificeLabel.SetText(string.Format("[9bfc3b]{0}[-]{1}", itemFielInfo.equipmentEntity.EQUIP_FIELD_SOPHISTICATION_LEVEL, LanguageTextManager.GetString("IDS_H1_156")));
//            PassiveSkillData pasSkill01 = GetPassiveData(itemFielInfo.equipmentEntity.EQUIP_FIELD_SKILL_ID1, itemFielInfo.equipmentEntity.EQUIP_FIELD_SKILL_VALUE1);
//            PassiveSkillLabelList[0].ShowEffect(pasSkill01);
//            PassiveSkillData pasSkill02 = GetPassiveData(itemFielInfo.equipmentEntity.EQUIP_FIELD_SKILL_ID2, itemFielInfo.equipmentEntity.EQUIP_FIELD_SKILL_VALUE2);
//            PassiveSkillLabelList[1].ShowEffect(pasSkill02);
//            PassiveSkillData pasSkill03 = GetPassiveData(itemFielInfo.equipmentEntity.EQUIP_FIELD_SKILL_ID3, itemFielInfo.equipmentEntity.EQUIP_FIELD_SKILL_VALUE3);
//            PassiveSkillLabelList[2].ShowEffect(pasSkill03);
//            TransitionItem.transform.localPosition = TransitionItemPos[pasSkill01 == null?0:(pasSkill02 == null?1:(pasSkill03 == null?2:3))];
//            transform.localPosition = Vector3.zero;
        }

        PassiveSkillData GetPassiveData(int skillID,int level)
        {
            PassiveSkillData getData = PassiveSkillData._dataTable.FirstOrDefault(P=>P.SkillID == skillID&&P.SkillLevel == level);
            return getData;
        }

        TextColor GetNameLabelColor(ItemFielInfo itemFielInfo)
        {
            //TraceUtil.Log(string.Format("显示物品：{0}，品质：{1}",itemFielInfo.LocalItemData._goodID,itemFielInfo.LocalItemData._ColorLevel));
            TextColor labelColor = TextColor.white;
            switch (itemFielInfo.LocalItemData._ColorLevel)
            {
                case 0:
                    //labelColor = Color.green;
                    //labelColor = new Color(47,119,25);
                    labelColor = TextColor.EquipmentGreen;
                    break;
                case 1:
                    //labelColor = Color.blue;
                    //labelColor = new Color(0,162,255);
                    labelColor = TextColor.EquipmentBlue;
                    break;
                case 2:
                    //labelColor = Color.magenta;
                    //labelColor = new Color(215,75,255);
                    labelColor = TextColor.EquipmentMagenta;
                    break;
                case 3:
                    //labelColor = Color.yellow;
                    //labelColor = new Color(255,198,0);
                    labelColor = TextColor.EquipmentYellow;
                    break;
            }
            return labelColor;
        }


        string GetProfession(string ProfessionID)//职业
        {
            string[] professionID = ProfessionID.Split('+');
            string profession = string.Empty;
            //string child = professionID[0];
            foreach (string child in professionID)
            {
                string AddStr = "";
                switch (int.Parse(child))
                {
                    case 0:
                        AddStr = "--";
                        break;
                    case 1:
                        AddStr = LanguageTextManager.GetString("IDS_D2_11");
                        break;
                    case 2:
                        AddStr = LanguageTextManager.GetString("IDS_D2_12");
                        break;
                    case 3:
                        AddStr = LanguageTextManager.GetString("IDS_D2_13");
                        break;
                    case 4:
                        AddStr = LanguageTextManager.GetString("IDS_D2_14");
                        break;
                    case 5:
                        AddStr = LanguageTextManager.GetString("IDS_D2_19");
                        break;
                    default:
                        break;
                }
                profession += AddStr;
            }
            return profession;
        }

        void OnSellBtnClick(object obj)
        {
            base.InitItemFileInfo(MyItemfileInfo);
            base.DiscardItems();
        }

        void OnCloseBtnClick(object obj)
        {
            Close();
        }
    }
}
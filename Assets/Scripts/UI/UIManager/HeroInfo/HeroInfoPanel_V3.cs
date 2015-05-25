using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI
{

    public class HeroInfoPanel_V3 : BaseUIPanel
    {
        public UILabel NameLabel;
        public GameObject SinglePassiveSkillItemPrefab;
        public PassiveSkillDataBase PassiveSkillDataBase;
        public Transform CreatSkillItemTransForm;
        public UIDraggablePanel UIDraggablePanel;
        public RoleAttributePanel RoleAttributePanel;

        public GameObject RoleInfoPanelModelViewPrefab;
        private RoleModelView_WithNewScene roleInfoPanelModelView;
        public SingleButtonCallBack DragButton;
        

        public GameObject UIBottomBtnPrefab;
        public Transform CreatBottomBtnPoint;
        private CommonUIBottomButtonTool commonUIBottomButtonTool;


        public List<HeroInfoPanel_SinglePassiveSkillItem> SinglePassiveSkillItemList { get; private set; }

        void Awake()
        {
            NameLabel.SetText(PlayerManager.Instance.FindHeroDataModel().Name);
            RegisterEventHandler();
        }

        protected override void RegisterEventHandler()
        {
            AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
        }

        void OnDestroy()
        {
            RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
        }

        public void UpdateViaNotify(INotifyArgs inotifyArgs)//设置各种属性
        {
            EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)inotifyArgs;
            if (entityDataUpdateNotify.IsHero && entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_PLAYER)
            {
                UpdateInfoPanelInfo();
            }
        }

        public override void Show(params object[] value)
        {
            base.Show(value);
            gameObject.SetActive(true);
            if (commonUIBottomButtonTool == null)
            {
                commonUIBottomButtonTool = CreatObjectToNGUI.InstantiateObj(UIBottomBtnPrefab, CreatBottomBtnPoint).GetComponent<CommonUIBottomButtonTool>();
                ShowBottomBtn();
            }
            else
            {
                commonUIBottomButtonTool.ShowAnim();
            }
            RoleAttributePanel.ShowAttributePanelInfo();
            RoleAttributePanel.transform.localPosition = Vector3.zero;
            InitPassiveSkill();
            if (roleInfoPanelModelView == null)
            {
                GameObject roleInfoPanelModelViewObj = GameObject.Instantiate(RoleInfoPanelModelViewPrefab) as GameObject;
                roleInfoPanelModelView = roleInfoPanelModelViewObj.GetComponent<RoleModelView_WithNewScene>();
                roleInfoPanelModelView.Init(RoleModelView_WithNewScene.PanelType.HeroInfoPanel);
                DragButton.SetDragCallback(roleInfoPanelModelView.OnDragBtnDrag);
            }
            else
            {
                roleInfoPanelModelView.Show();
            }
        }

        void ShowBottomBtn()
        {
            CommonBtnInfo btnInfo = new CommonBtnInfo(0, "JH_UI_Button_1116_06", "JH_UI_Button_1116_00", OnCloseBtnClick);
            commonUIBottomButtonTool.Show(new List<CommonBtnInfo>() { btnInfo });
        }

        void InitPassiveSkill()
        {
            ResetGrid();
            List<PassiveSkillData> passiveSkillDataList = new List<PassiveSkillData>();
            foreach (var child in ContainerInfomanager.Instance.GetSSyncContainerGoods_SCList(1))
            {
                ItemFielInfo equiptItem = ContainerInfomanager.Instance.itemFielArrayInfo.SingleOrDefault(P => P.sSyncContainerGoods_SC.uidGoods == child.uidGoods);
                if (equiptItem != null&&equiptItem.LocalItemData._GoodsClass ==1)
                {
//                    if (equiptItem.equipmentEntity.EQUIP_FIELD_SKILL_ID1 != 0)
//                    {
//                        TraceUtil.Log(equiptItem.equipmentEntity.EQUIP_FIELD_SKILL_ID1 + "," + equiptItem.equipmentEntity.EQUIP_FIELD_SKILL_VALUE1);
//                        passiveSkillDataList.Add(PassiveSkillDataBase._dataTable.First(P => P.SkillID == equiptItem.equipmentEntity.EQUIP_FIELD_SKILL_ID1 && 
//                            P.SkillLevel == equiptItem.equipmentEntity.EQUIP_FIELD_SKILL_VALUE1));
//                    }
//                    if (equiptItem.equipmentEntity.EQUIP_FIELD_SKILL_ID2 != 0)
//                    {
//                        passiveSkillDataList.Add(PassiveSkillDataBase._dataTable.First(P => P.SkillID == equiptItem.equipmentEntity.EQUIP_FIELD_SKILL_ID2 &&
//                            P.SkillLevel == equiptItem.equipmentEntity.EQUIP_FIELD_SKILL_VALUE2));
//                    }
//                    if (equiptItem.equipmentEntity.EQUIP_FIELD_SKILL_ID3 != 0)
//                    {
//                        passiveSkillDataList.Add(PassiveSkillDataBase._dataTable.First(P => P.SkillID == equiptItem.equipmentEntity.EQUIP_FIELD_SKILL_ID3 &&
//                            P.SkillLevel == equiptItem.equipmentEntity.EQUIP_FIELD_SKILL_VALUE3));
//                    }
                }
            }
            for (int i = 0; i < passiveSkillDataList.Count; i++)
            {
                HeroInfoPanel_SinglePassiveSkillItem passiveSkillItem = CreatObjectToNGUI.InstantiateObj(SinglePassiveSkillItemPrefab, CreatSkillItemTransForm).GetComponent<HeroInfoPanel_SinglePassiveSkillItem>();
                passiveSkillItem.Show(passiveSkillDataList[i]);
                passiveSkillItem.transform.localPosition = new Vector3(0, -130-60*i, 0);
            }
        }

        void ResetGrid()
        {
            CreatSkillItemTransForm.ClearChild();
            UIDraggablePanel.ResetPosition();
        }

        void UpdateInfoPanelInfo()
        {
            RoleAttributePanel.ShowAttributePanelInfo();
        }

        void OnCloseBtnClick(object obj)
        {
            CleanUpUIStatus();
            Close();
        }

        public override void Close()
        {
            if (!IsShow)
                return;
            gameObject.SetActive(false);
            if (roleInfoPanelModelView != null)
            {
                roleInfoPanelModelView.Close();
            }
            base.Close();
        }

    }
}
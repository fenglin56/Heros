using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI
{

    public class FashionPanel_V3 : BaseUIPanel
    {

        public FashionInfoTips_V3 fashionInfoTips;//时装信息显示面板
        public FashionAttributePanel_V3 fashionAttributePanel;//时装属性面板
        //public FashionHeroViewCam fashionHeroViewCam;//时装人物显示面板
        public FashionListPanel_V3 FashionListPanel;//时装列表面板
        public EquipmentData CurrentFashiondata { get; private set; }//当前穿着的时装
        public GameObject RoleInfoPanelModelViewPrefab;
        private RoleModelView_WithNewScene roleInfoPanelModelView;
        public SingleButtonCallBack DragButton;
		public SingleButtonCallBack BackButton;
		//public CommonPanelTitle m_CommonPanelTitle;
		public SingleButtonCallBack HelpTips;

//        public GameObject UIBottomBtnPrefab;
//        public Transform CreatBottomBtnPoint;
//        private CommonUIBottomButtonTool commonUIBottomButtonTool;

        public List<ItemFielInfo> OwnFashionList { get; private set; }//已经拥有的时装列表

        void Awake()
        {
			HelpTips.SetButtonText(LanguageTextManager.GetString("IDS_I8_2"));
            //MainUIController.Instance.SetPanelActivEvent += new MainUIController.SetPanelDelegate(SetPanelActive);
            AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.ResetContainerGoods, SetPanelAttribute);
			BackButton.SetCallBackFuntion(OnBackButtonTapped);
            RegisterEventHandler();
            TaskGuideBtnRegister();
        }

        protected override void RegisterEventHandler()
        {
            base.RegisterEventHandler();
            AddEventHandler(EventTypeEnum.PackStateChange.ToString(),PackageStateChangeHandel);
        }

        void PackageStateChangeHandel(INotifyArgs args)
        {
            switch(PackInfoStateManager.Instance.CurrentState)
            {
                case PackInfoStateType.ClosFashPanel:
                    fashionAttributePanel.TweenClose();
                    FashionListPanel.TweenClose();
                    DoForTime.DoFunForTime(0.1f,ClosePanelForTime,null);
                    break;
            }
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            DragButton.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_FashionPanel_V3_DragButton);
            BackButton.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_FashionPanel_V3_BackButton);
           
//            m_CommonPanelTitle.GoldMoneyLabel.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_FashionPanel_V3_BuyIngot);
//            m_CommonPanelTitle.CopperLabel.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_FashionPanel_V3_BuyMoney);
        }

        void OnDestroy()
        {
            RemoveEventHandler(EventTypeEnum.PackStateChange.ToString(),PackageStateChangeHandel);
            RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ResetContainerGoods, SetPanelAttribute);
        }

        public void UpdateViaNotify(INotifyArgs inotifyArgs)//设置各种属性
        {
            if (!IsShow)
                return;
           
            EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)inotifyArgs;
            if (entityDataUpdateNotify.IsHero && entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_PLAYER)
            {
                int FashionID = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION;
//                if ((CurrentFashiondata == null && FashionID != 0) || (CurrentFashiondata != null && CurrentFashiondata._goodID != FashionID))
//                {
                    SetPanelAttribute(null);
//                }
            }
        }

//        IEnumerator ActiveBackBtn()
//        {
//            yield return new WaitForSeconds(0.3f);
//            BackButton.SetMyButtonActive(true);
//        }
        public override void Show(params object[] value)
        {
			SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Avatar_UIOpen");
//            BackButton.SetMyButtonActive(false);
//            StartCoroutine(ActiveBackBtn());
            base.Show(value);
			//m_CommonPanelTitle.TweenShow();
//            if (commonUIBottomButtonTool == null)
//            {
//                commonUIBottomButtonTool = CreatObjectToNGUI.InstantiateObj(UIBottomBtnPrefab, CreatBottomBtnPoint).GetComponent<CommonUIBottomButtonTool>();
//                ShowBottomBtn();
//            }
//            else
//            {
//                commonUIBottomButtonTool.ShowAnim();
//            }
            if (roleInfoPanelModelView == null)
            {
                GameObject roleInfoPanelModelViewObj = GameObject.Instantiate(RoleInfoPanelModelViewPrefab) as GameObject;
                roleInfoPanelModelView = roleInfoPanelModelViewObj.GetComponent<RoleModelView_WithNewScene>();
                roleInfoPanelModelView.Init(RoleModelView_WithNewScene.PanelType.FashionInfoPanel);
                DragButton.SetDragCallback(roleInfoPanelModelView.OnDragBtnDrag);
            }
            else
            {
                roleInfoPanelModelView.Show();
            }
            SetPanelAttribute(null);
			fashionAttributePanel.TweenShow();
			FashionListPanel.TweenShow();
        }

//        void ShowBottomBtn()
//        {
//            CommonBtnInfo btnInfo = new CommonBtnInfo(0, "JH_UI_Button_1116_06", "JH_UI_Button_1116_00", OnBackButtonTapped);
//            commonUIBottomButtonTool.Show(new List<CommonBtnInfo>() { btnInfo });
//        }

        /// <summary>
        /// 显示面板各项属性
        /// </summary>
        void SetPanelAttribute(object obj)
        {
            if (!IsShow)
                return;
            TraceUtil.Log("设置时装面板");
            List<ItemData> FashionList = GetAllFashionDatas();
            if (FashionList.Count < 1) return;
            OwnFashionList = ContainerInfomanager.Instance.itemFielArrayInfo.FindAll(P => P.LocalItemData._GoodsClass == 1 && P.LocalItemData._GoodsSubClass == 2);
            foreach (var child in OwnFashionList)
            {
                TraceUtil.Log("添加拥有的时装：" + child.sSyncContainerGoods_SC.nPlace);
            }
			FashionList.Sort(delegate(ItemData a, ItemData b) { return (a._goodID).CompareTo(b._goodID); });
			int currentFashionID = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION;
			CurrentFashiondata = currentFashionID == 0 ? (EquipmentData)GetAllFashionDatas()[0] : (EquipmentData)ItemDataManager.Instance.GetItemData(currentFashionID);
			FashionListPanel.InitPanel(FashionList, this);
			SetCurrentFashionData();
        }

        public List<ItemData> GetAllFashionDatas()
        {
            int Profession = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
            List<ItemData> MyFashionList = new List<ItemData>();
            foreach (var child in ItemDataManager.Instance.ItemDatas)
            {
                foreach (var ItemChild in child._equipments)
                {
                    if (ItemChild._GoodsClass == 1 && ItemChild._GoodsSubClass == 2 && int.Parse(ItemChild._AllowProfession.Split('+')[0]) == Profession)
                    {
                        MyFashionList.Add(ItemChild);
                    }
                }
            }
            return MyFashionList;
        }

        /// <summary>
        /// 设置当前穿着的时装
        /// </summary>
        void SetCurrentFashionData()
        {
			if(CurrentFashiondata == null)return;
            FashionListPanel.OnMyBtnClick(CurrentFashiondata);
        }

        /// <summary>
        /// 选中某一套时装
        /// </summary>
        /// <param name="SelectBtn">点击的按钮</param>
        public void SelectFashion(ItemData fashionData)
        {
            fashionInfoTips.ShowFashionInfo(fashionData, this);
            fashionAttributePanel.Show(fashionData, this);
            if (roleInfoPanelModelView != null)
            {
                DoForTime.DoFunForTime(0.1f, roleInfoPanelModelView.ChangeFashion, fashionData._goodID);
                //StartCoroutine(roleInfoPanelModelView.ChangeFashion(fashionData._goodID));
                //roleViewPanel.roleModelPanel.ChangeHeroFashion(SelectBtn.MyFashionData._goodID);
            }
			if(IsNewItem(fashionData))
			{
				ItemFielInfo itemFileData = GetUnlockData(fashionData);
				NetServiceManager.Instance.ContainerService.SendUpdateContainerGoodsNewStatu((int)itemFileData.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID,(byte)itemFileData.sSyncContainerGoods_SC.nPlace);
			}
        }
        /// <summary>
        /// 检测时装是否解锁，返回空即未解锁
        /// </summary>
        /// <param name="fashionData"></param>
        /// <returns></returns>
        public ItemFielInfo GetUnlockData(ItemData fashionData)
        {
            return OwnFashionList.FirstOrDefault(P => P.LocalItemData._goodID == fashionData._goodID);
        }
		/// <summary>
		/// 是否为最新时装
		/// </summary>
		public bool IsNewItem(ItemData itemData)
		{
			bool flag = false;
			ItemFielInfo getData = GetUnlockData(itemData);
			flag = getData!=null&&getData.equipmentEntity.ITEM_FIELD_VISIBLE_NEW ==0;
			return flag;
		}

        void OnBackButtonTapped(object obj)
        {
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Avatar_Leave");

            PackInfoStateManager.Instance.StateChange(PackInfoStateType.PrepareToOutFashion);
        }

		void ClosePanelForTime(object obj)
		{
			Close();
			CleanUpUIStatus();
            PackInfoStateManager.Instance.StateChange(PackInfoStateType.InterPack);
		}

        public override void Close()
        {
            if (!IsShow)
                return;
            UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
            if (roleInfoPanelModelView != null)
            {
                //roleInfoPanelModelView.Close();
                Destroy(roleInfoPanelModelView.gameObject);
            }
            base.Close();
        }

        public EquipmentData CurrentMaxFashionData
        {
            get
            {
                EquipmentData data = null;
                if (OwnFashionList.Count > 0)
                {
                    data = OwnFashionList[OwnFashionList.Count - 1].LocalItemData as EquipmentData;
                }
                return data;
            }
        }


    }
}
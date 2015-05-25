using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{

    public class PackInfoPanel : BaseUIPanel
    {
        public HeroEquiptPanel m_HeroEquiptPanel;//人物装备
		public ContainerItemListPanel m_ContainerItemListPanel;//背包列表
		public HeroAttributePanel m_HeroAttributePanel;//人物属性
		public SellItemsPanel m_SellItemsPanel;//批量出售
		public SellItemConfirmPanel m_SellItemConfirmPanel;//出售确认面板
		//public GameObject ItemTipsManagerPrefab;//物品信息栏
		public CommonPanelTitle m_CommonPanelTitle;
		public PlayerTitlePanel m_PlayerTitlePanel;

        public SingleButtonCallBack TitleButton;//称号
        public SingleButtonCallBack FashionButton;
        public SingleButtonCallBack PackSwithButton;//角色属性

		public SingleButtonCallBack BackButton;
		public GameObject FashionPanelPrefab;//时装面板
		public GameObject EquipmentUpgradPanelPrefab;//装备升级面板
		public Transform NewPanelPos;
        private bool IsClosing;
        public GameObject Mask;
		FashionPanel_V3 m_FashionPanel;
		EquipmentUpgradePanel m_EquipmentUpgradePanel;

		private ItemInfoTipsManager m_ItemTipsManager;

		void Awake()
		{
            Mask.SetActive(false);
			TitleButton.SetCallBackFuntion(OnTitleBtnClick);
			FashionButton.SetCallBackFuntion(OnFashionBtnClick);
			PackSwithButton.SetCallBackFuntion(OnPackSwithBtnClick);
			BackButton.SetCallBackFuntion(OnBackBtnClick);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.ResetContainerGoods,UpdateContainerGoods);

			m_SellItemsPanel.Init(this);
			m_HeroEquiptPanel.Init(this);
			//AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
			RegisterEventHandler();

            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            TitleButton.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_Title);
            FashionButton.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_Fasion);
            PackSwithButton.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_PlayerProperty);
            BackButton.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_Back);

            m_CommonPanelTitle.GoldMoneyLabel.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_BuyIngot);
            m_CommonPanelTitle.CopperLabel.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_BuyMoney);
        }

		void OnDestroy()
		{
			//RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
            RemoveEventHandler(EventTypeEnum.PlayerFashionUpdate.ToString(), UpdateFashionHadel);
            RemoveEventHandler(EventTypeEnum.PlayerTitleUpdate.ToString(), UpdateTitleDisplay);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ResetContainerGoods,UpdateContainerGoods);
		}

		void UpdateContainerGoods(object obj)//刷新背包栏属性
		{
			if(!IsShow)
				return;
			m_ContainerItemListPanel.UpdateItemList(null);
			m_SellItemsPanel.UpdateItemsPanel();
			m_HeroEquiptPanel.UpdatePanel();
		}

        public override void Show(params object[] value)
        {
			base.Show(value);
            CloseSubPage();
			ShowPanel();
            IsClosing=false;
        }

        void CloseSubPage()
        {
            if(m_EquipmentUpgradePanel!=null)
            {
            m_EquipmentUpgradePanel.OnCloseBtnClick(null);
            }
            if(m_FashionPanel!=null)
            {
            m_FashionPanel.Close();
            }
            if(m_PlayerTitlePanel!=null)
            {
            m_PlayerTitlePanel.ClosePanel(null);
            }
        }
		void ShowPanel()
		{

			m_HeroEquiptPanel.Show();
			m_ContainerItemListPanel.Show(this);
			UpdatePackSwthBtnStatus();
			m_CommonPanelTitle.TweenShow();
			UpdateTitleDisplay(null);
		}

        public override void Close()
        {
            if (!IsShow)
            return;
            m_PlayerTitlePanel.ClosePanel(null);
            m_HeroEquiptPanel.Close();
            m_HeroEquiptPanel.m_RoleModelPanel.Close();
            m_HeroAttributePanel.Close();
            m_SellItemsPanel.Close();
            StartCoroutine(DelayClose());
           
			
        }
        IEnumerator DelayClose()
        {
            IsClosing=true;
            if(m_FashionPanel!=null)
			{
                m_FashionPanel.Close();
			}
            m_HeroEquiptPanel.TweenClose();
          
            m_ContainerItemListPanel.TweenClose();
           
            CleanUpUIStatus();
            if(m_ItemTipsManager!=null)
			{
                m_ItemTipsManager.Close(null);
            }
            yield return new WaitForSeconds(0.1f);
            base.Close();
            Close();
           
        }
		public void ShowFashionPanel()
		{
			if(m_FashionPanel == null){m_FashionPanel = NewPanelPos.InstantiateNGUIObj(FashionPanelPrefab).GetComponent<FashionPanel_V3>();}
			m_FashionPanel.Show();
		}

		public void ShowEquipmentUpgradePanel(ItemFielInfo selectItem)
		{
			if(m_EquipmentUpgradePanel == null){m_EquipmentUpgradePanel = NewPanelPos.InstantiateNGUIObj(EquipmentUpgradPanelPrefab).GetComponent<EquipmentUpgradePanel>();}
			m_EquipmentUpgradePanel.Show(selectItem);
		}

		void OnPackSwithBtnClick(object obj)
		{
            if(!IsClosing)
            {
				if(m_HeroAttributePanel.IsShow)
				{
					SoundManager.Instance.PlaySoundEffect("Sound_Button_PackagePlayerReturn");
					m_HeroAttributePanel.TweenClose();
					m_ContainerItemListPanel.SetTitleBtnActive(true);
				}
				else
				{
	                if(m_SellItemsPanel.IsShow)
	                {
	                    m_SellItemsPanel.Close();
	                }
					SoundManager.Instance.PlaySoundEffect("Sound_Button_PackagePlayer");
					m_HeroAttributePanel.TweenShow();
					m_ContainerItemListPanel.SetTitleBtnActive(false);
				}

				UpdatePackSwthBtnStatus();
            }
		}

		void UpdatePackSwthBtnStatus()
		{
			if(!m_HeroAttributePanel.IsShow)
			{
				var m_HeroDataModel = PlayerManager.Instance.FindHeroDataModel();
				int vocationID = m_HeroDataModel.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
				int fashionID = m_HeroDataModel.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION;
				var resData= CommonDefineManager.Instance.CommonDefineFile._dataTable.HeroIcon_TownAndTeam.FirstOrDefault(P=>P.VocationID == vocationID&&P.FashionID == fashionID);
				if (resData == null) { TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"找不到角色头像,fashionID:" + fashionID); }
				PackSwithButton.BackgroundSprite.gameObject.SetActive(true);
				PackSwithButton.spriteSwith.target.gameObject.SetActive(false);
				PackSwithButton.BackgroundSprite.spriteName = resData.ResName;
			}
			else
			{
				PackSwithButton.BackgroundSprite.gameObject.SetActive(false);
				PackSwithButton.spriteSwith.target.gameObject.SetActive(true);
			}
		}
		//更新显示当前称号
		private void UpdateTitleDisplay(INotifyArgs arg)
		{
			if(m_HeroEquiptPanel.TitleGameObject != null)
				Destroy(m_HeroEquiptPanel.TitleGameObject);
			int titleID = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_TITLE;
			var titleData = PlayerDataManager.Instance.GetPlayerTitleConfigData(titleID);
			if(titleData == null)
				return;
			m_HeroEquiptPanel.TitleGameObject = (GameObject)UI.CreatObjectToNGUI.InstantiateObj( titleData._ModelIdPrefab, m_HeroEquiptPanel.TitlePoint);
		}
        private void UpdateFashionHadel(INotifyArgs args)
        {
            UpdatePackSwthBtnStatus();
        }
		public void ShowItemTips(ItemFielInfo itemFielInfo)
		{
			if(itemFielInfo == null)
				return;
//			if(m_ItemTipsManager == null)
//			{
//				m_ItemTipsManager = CreatObjectToNGUI.InstantiateObj(ItemTipsManagerPrefab,transform).GetComponent<ItemInfoTipsManager>();
//			}
            if(itemFielInfo.LocalItemData._GoodsClass==2&&itemFielInfo.LocalItemData._GoodsSubClass==3)
            {
                UseGoodsPanel.Instance.Show(itemFielInfo.LocalItemData._goodID);
            }
            else
            {
	        ItemInfoTipsManager.Instance.Show(itemFielInfo,this);
            }
		}

		/// <summary>
		/// 当点击了某个物品
		/// </summary>
		/// <param name="obj">Object.</param>
		public void OnItemClick(object obj)
		{
			m_ContainerItemListPanel.SetItemSelectStatus(obj);
			ShowItemTips(obj as ItemFielInfo);
		}

		void OnFashionBtnClick(object obj)
		{
            if(!IsClosing)
            {
				SoundManager.Instance.PlaySoundEffect("Sound_Button_PackageFashion");
                PackInfoStateManager.Instance.StateChange(PackInfoStateType.PrepareToFashion);
            }
		}

		void OnTitleBtnClick(object obj)
		{
            if(!IsClosing)
            {
				SoundManager.Instance.PlaySoundEffect("Sound_Button_PackageTitle");
				m_PlayerTitlePanel.Show();
            }
		}

		void OnBackBtnClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_PackageClose");
            Close();
		}

        void PackageStateChangeHandel(INotifyArgs args)
        {
            switch(PackInfoStateManager.Instance.CurrentState)
            {
                case PackInfoStateType.ShowFashionPanel:
                    ShowFashionPanel();
                    break;
            }
            if(PackInfoStateManager.Instance.CurrentState==PackInfoStateType.Showpack||PackInfoStateManager.Instance.CurrentState==PackInfoStateType.ShowFashion)
            {
                Mask.SetActive(false);
            }
            else
            {
                Mask.SetActive(true);
            }
        }
		protected override void RegisterEventHandler ()
		{
			AddEventHandler(EventTypeEnum.PlayerTitleUpdate.ToString(), UpdateTitleDisplay);
            AddEventHandler(EventTypeEnum.PlayerFashionUpdate.ToString(), UpdateFashionHadel);
            AddEventHandler(EventTypeEnum.PackStateChange.ToString(),PackageStateChangeHandel);
			base.RegisterEventHandler ();
		}
    }
}
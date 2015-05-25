using UnityEngine;
using System.Collections;


namespace UI.MainUI{

	public class EquipmentUpgradePanel : MonoBehaviour {

		public UpgradeItemListPanel m_UpgradeItemListPanel;
		public UpgradeItemAtributtePanel m_UpgradeItemAtributtePanel;
		public SingleButtonCallBack CloseButton;
		public SingleButtonCallBack UpgradeButton;
        public GameObject UpgradeSuccess_prefab;
        public Transform UpgradeSuccess_point;
        private GameObject UpgradeSuccess_go;
		public ItemFielInfo SelectItem{get;private set;}

		void Awake()
		{
			CloseButton.SetCallBackFuntion(OnCloseBtnClick);
			UpgradeButton.SetCallBackFuntion(OnUpgradeBtnClick);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.EqipmentLevelUp,UpdatePanel);
            UpgradeSuccess_go=CreatObjectToNGUI.InstantiateObj(UpgradeSuccess_prefab,UpgradeSuccess_point);
            //UpgradeSuccess_go.SetActive(false);
            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            CloseButton.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_EquipmentUpgradePanel_Back);
            UpgradeButton.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_EquipmentUpgradePanel_Upgrade);
        }

		void OnDestroy()
		{
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EqipmentLevelUp,UpdatePanel);
		}

		public void Show(ItemFielInfo selectItem)
		{
            UpgradeSuccess_go.SetActive(false);
            m_UpgradeItemAtributtePanel.TweenShow();
			gameObject.SetActive(true);
			m_UpgradeItemListPanel.Show(selectItem,this);
			//OnItemSelect(selectItem);
			//m_UpgradeItemAtributtePanel.Show(selectItem,this);
		}

		public void OnItemSelect(ItemFielInfo itemFielInfo)
		{
			SelectItem = itemFielInfo;
			m_UpgradeItemAtributtePanel.Show(itemFielInfo,this);
		}

		public void OnCloseBtnClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Upgrade_Cancel");
			m_UpgradeItemListPanel.TweenClose();
			m_UpgradeItemAtributtePanel.TweenClose();
			DoForTime.DoFunForTime(0.2f,ClosePanelForTime,null);
		}

		void ClosePanelForTime(object obj)
		{
			SelectItem = null;
			gameObject.SetActive(false);
		}

		void OnUpgradeBtnClick(object obj)
		{
			int myLevel = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
			if(SelectItem == null)
			{
				return;
			}
			ItemData needItem = null;
            ItemData nextLevelData=ItemDataManager.Instance.GetItemData(SelectEquipmentData.UpgradeID);
			if(SelectEquipmentData.UpgradeID==0)//达到最大等级
			{
				MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_I10_6"),1);
				SoundManager.Instance.PlaySoundEffect("Sound_Button_Upgrade_Warning");
			}else if(IsLackOfMaterial(ref needItem))//材料不足
			{
				MessageBox.Instance.ShowTips(3,string.Format(LanguageTextManager.GetString("IDS_I10_7"),LanguageTextManager.GetString(needItem._szGoodsName)),1);
				SoundManager.Instance.PlaySoundEffect("Sound_Button_Upgrade_Warning");
            }else if(nextLevelData._AllowLevel>myLevel)//等级不足
			{
				MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_I10_8"),1);
				SoundManager.Instance.PlaySoundEffect("Sound_Button_Upgrade_Warning");
			}else
			{
				NetServiceManager.Instance.EquipStrengthenService.SendEquipmentLevelUp(SelectItem.sSyncContainerGoods_SC.uidGoods);
			}
		}
		/// <summary>
		/// 升级成功刷新界面
		/// </summary>
		/// <param name="obj">Object.</param>
		void UpdatePanel(object obj)
		{
            UpgradeSuccess_go.SetActive(false);
            UpgradeSuccess_go.SetActive(true);
			//long newEquipmentID = (long)obj;
			SMsgGoodsOperateEquipLevelUp_SC msg = (SMsgGoodsOperateEquipLevelUp_SC)obj;
			if(msg.bySucess ==1)//升级成功
			{
				TraceUtil.Log(SystemModel.Jiang,"装备升级成功");
				//Show(null);
				m_UpgradeItemListPanel.UpdateItemList();
				ItemFielInfo newItemData = ContainerInfomanager.Instance.GetItemFileInfoBuyUID(msg.NewItemUID);
				m_UpgradeItemListPanel.OnMyItemClick(newItemData);
			}
		}

		bool IsLackOfMaterial(ref ItemData needItem)
		{
			bool flag = false;
			foreach(var child in SelectEquipmentData.UpgradeCost.Split('|'))
			{
				string[] chacheStr = child.Split('+');
				int itemID = int.Parse(chacheStr[0]);
				int itemNum = int.Parse(chacheStr[1]);
				if(ContainerInfomanager.Instance.GetItemNumber(itemID)<itemNum)
				{
					flag = true;
					needItem = ItemDataManager.Instance.GetItemData(itemID);
					break;
				}
			}
			return flag;
		}

		EquipmentData SelectEquipmentData
		{
			get
			{
				EquipmentData getData = null;
				if(SelectItem!=null)
				{
					return (EquipmentData)SelectItem.LocalItemData;
				}
					return getData;
			}
		}
	}
}
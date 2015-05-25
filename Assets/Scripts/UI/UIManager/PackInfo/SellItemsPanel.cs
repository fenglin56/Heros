using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI{

	public class SellItemsPanel : MonoBehaviour {
		
		public GameObject SinglePackItemPrefab;
		public Transform Grid;
		public SingleButtonCallBack SellButton;
		public SingleButtonCallBack FastSelectButton;
		public SingleButtonCallBack CancelButton;
		public UIPanel MyUIPanel;
		public UIPanel ItemListPanel;
		public Vector3 ShowPos;
		public Vector3 HidePos;
		GameObject FloatObj;

		public List<SinglePackItemSlot> MyItemList{get;private set;}
		public List<SinglePackItemSlot> SelectItemList{get;private set;}
		public bool IsShow{get;private set;}
		public PackInfoPanel MyParent{get;private set;}

		void Awake()
		{
			MyItemList = new List<SinglePackItemSlot>();
			SelectItemList = new List<SinglePackItemSlot>();
			SellButton.SetCallBackFuntion(OnSellBtnClick);
			FastSelectButton.SetCallBackFuntion(OnFastSelectBtnClick);
			CancelButton.SetCallBackFuntion(OnCancelBtnClick);
			IsShow = true;
			Close();

            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            SellButton.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_SellItemsPanel_Sell);
            CancelButton.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_SellItemsPanel_Cancel);
        }

		public void Init(PackInfoPanel myParent)
		{
			MyParent = myParent;
		}

		public void TweenShow(PackInfoPanel myParent)
		{
			this.MyParent = myParent;
			if(IsShow)
				return;
			IsShow = true;
			MyParent.m_ContainerItemListPanel.SetTitleBtnActive(false);
			float animTime = 0.3f;
			if(FloatObj!=null){DestroyImmediate(FloatObj);}
			FloatObj = TweenFloat.Begin(animTime,MyUIPanel.alpha,1,SetMyPanelAlpha);
			TweenPosition.Begin(gameObject,animTime,transform.localPosition,ShowPos);
			Init();
		}

		public void UpdateItemsPanel()
		{
			Init();
		}

		public void TweenClose()
		{
			if(!IsShow)
				return;
			float animTime = 0.3f;
			MyParent.m_ContainerItemListPanel.SetTitleBtnActive(true);
			if(FloatObj!=null){DestroyImmediate(FloatObj);}
			FloatObj = TweenFloat.Begin(animTime,MyUIPanel.alpha,0,SetMyPanelAlpha);
			TweenPosition.Begin(gameObject,animTime,transform.localPosition,HidePos);
			IsShow = false;
		}

		void SetMyPanelAlpha(float value)
		{
			MyUIPanel.alpha = value;
			ItemListPanel.alpha = value;
		}

		public void Close()
		{
			if(!IsShow)
				return;
			if(FloatObj!=null){DestroyImmediate(FloatObj);}
			MyUIPanel.alpha = 0;
			ItemListPanel.alpha = 0;
			transform.localPosition = HidePos;
			IsShow = false;
		}

		void Init()
		{
			Grid.ClearChild();
			MyItemList.Clear();
			SelectItemList.Clear();
			List<ItemFielInfo> packItemList = ContainerInfomanager.Instance.GetPackItemList().Where(p=>(!p.GetIfBesetJewel(1)) && (!p.GetIfBesetJewel(2)) && (p.LocalItemData._TradeFlag ==1)).ToList();

			// 所有已装备的装备本地信息
			List<EquipmentData> equipedDataList = new List<EquipmentData>();
			ContainerInfomanager.Instance.GetEquiptItemList().ApplyAllItem(P => equipedDataList.Add(P.LocalItemData as EquipmentData));

			packItemList.Sort((left,right)=> {
				bool leftCanSelectFast = IsCanFastSelect(left.LocalItemData, equipedDataList);
				bool rightCanSelectFast = IsCanFastSelect(right.LocalItemData, equipedDataList);

				if(leftCanSelectFast && !rightCanSelectFast)
				{
					return -1;
				}
				else if(!leftCanSelectFast && rightCanSelectFast)
				{
					return 1;
				}
				else
				{
					if(left.sSyncContainerGoods_SC.nPlace<right.sSyncContainerGoods_SC.nPlace)
					{
						return -1;
					}
					else if(left.sSyncContainerGoods_SC.nPlace==right.sSyncContainerGoods_SC.nPlace)
					{
						return 0;
					}
					else 
					{
						return 1;
					}
				}
			});
			for(int i = 0;i<ContainerInfomanager.Instance.GetAllPackMaxNum();i++)
			{
				int indexX = i%4;
				int indexY = i/4;
				GameObject newObj = CreatObjectToNGUI.InstantiateObj(SinglePackItemPrefab,Grid);
				newObj.transform.localScale = new Vector3(0.85f, 0.85f, 1);
				newObj.transform.localPosition = new Vector3(-136 + 90 * indexX, 140 - 94 * indexY, 0);
				SinglePackItemSlot newItemSlot = newObj.GetComponent<SinglePackItemSlot>();
				ItemFielInfo newItemFielInfo = packItemList.Count>i?packItemList[i]:null;
				newItemSlot.Init(newItemFielInfo,false,SinglePackItemSlot.ItemStatus.Sell, OnItemCLick);
				MyItemList.Add(newItemSlot);

                //引导注入
                if (newItemFielInfo != null)
                {
                    newObj.RegisterBtnMappingId(newItemFielInfo.LocalItemData._goodID, UIType.Package, BtnMapId_Sub.Package_Cell);
                }

			}
			//UpdateSellBtnStatus();
		}

		void OnSellBtnClick(object obj)
		{
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Confirm");
			TraceUtil.Log(SystemModel.Jiang,"SellItems");	
			List<ItemFielInfo> itemList = new List<ItemFielInfo>();
			SelectItemList.ApplyAllItem(P=>itemList.Add(P.MyItemFileInfo));
            if(itemList.Count>0)
            {
				MyParent.m_SellItemConfirmPanel.SellItem(itemList);
            }
            else
            {
                MessageBox.Instance.ShowTips(1,LanguageTextManager.GetString("IDS_I1_26"),1);
            }
		}

		private bool IsCanFastSelect(ItemData goodsLocalData, List<EquipmentData> equipedDataList)
		{
			if(goodsLocalData._TradeFlag  == 1 && goodsLocalData.CanBeFastSelect)
			{
				ItemData.ItemType itemType = ItemData.GetItemType(goodsLocalData);
				if(itemType != ItemData.ItemType.Equipment)
				{
					return true;
				}
				else
				{
					EquipmentData equipGoodsData = (EquipmentData)goodsLocalData;

					// 武器和玩家的职业比对，如果不可用，则直接可以快速出售
					int Profession = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
					string[] allowPro = equipGoodsData._AllowProfession.Split('+');
					if(!allowPro.Contains(Profession.ToString()))
					{
						return true;
					}
			
					EquipmentData equiptedData = equipedDataList.FirstOrDefault(P=>P._vectEquipLoc == equipGoodsData._vectEquipLoc);
					if(equiptedData != null)
					{
						if(equipGoodsData._ColorLevel < equiptedData._ColorLevel ||
						   (equipGoodsData._ColorLevel == equiptedData._ColorLevel && equipGoodsData._AllowLevel < equiptedData._AllowLevel))
						{
							return true;
						}
					}
					
				}

			}

			return false;
		}

		void OnFastSelectBtnClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Confirm");
			List<SinglePackItemSlot> selectableItem = MyItemList.FindAll(P => P.MyItemFileInfo != null);

			// 所有已装备的装备本地信息
			List<EquipmentData> equipedDataList = new List<EquipmentData>();
			ContainerInfomanager.Instance.GetEquiptItemList().ApplyAllItem(P => equipedDataList.Add(P.LocalItemData as EquipmentData));

			foreach(var item in selectableItem)
			{
				ItemData goodsLocalData = item.MyItemFileInfo.LocalItemData;

				// 判断是否可以快速选择
				if( IsCanFastSelect(goodsLocalData, equipedDataList))
				{
					item.SetSelectStatus(true);
					SelectItemList.Add(item);
				}
			}
		}

		void OnCancelBtnClick(object obj)
		{
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Cancel");
			TweenClose();
		}

//		void UpdateSellBtnStatus()
//		{
//			bool isEnabel = SelectItemList.Count>0;
//			SellButton.SetImageButtonComponentActive(isEnabel?true:false);
//			SellButton.SetButtonBackground(isEnabel?1:2);
//			SellButton.gameObject.collider.enabled = isEnabel;
//		}

		void OnItemCLick(object obj)
		{
			ItemFielInfo clickItemInfo = obj as ItemFielInfo;
			SinglePackItemSlot clickItem = MyItemList.First(P=>P.MyItemFileInfo == clickItemInfo);
			bool selectStatus = !clickItem.IsSelect;
			clickItem.SetSelectStatus(selectStatus);
			if(selectStatus&&!SelectItemList.Contains(clickItem))
			{
				SelectItemList.Add(clickItem);
			}else if(!selectStatus&&SelectItemList.Contains(clickItem))
			{
				SelectItemList.Remove(clickItem);
			}
			//UpdateSellBtnStatus();
		}

	}
}
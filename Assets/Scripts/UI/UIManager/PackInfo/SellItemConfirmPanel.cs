using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI{

	public class SellItemConfirmPanel : View {

		public GameObject SelectNumberPanelPrefab;

		SelectNumPanel m_SelectNumPanel;
		List<ItemFielInfo> ChacheItemList;
		int SellItemNum = 0;


		protected override void RegisterEventHandler ()
		{
			throw new System.NotImplementedException ();
		}
		/// <summary>
		/// 出售物品
		/// </summary>
		/// <param name="itemList">Item list.</param>
		public void SellItem(List<ItemFielInfo> itemList)
		{
			SellItemNum = 0;
			ChacheItemList = itemList;
			if(itemList.Count == 1&&itemList[0].sSyncContainerGoods_SC.byNum>1)
			{
				ShowSelectNumPanel();
			}else
			{
				//ContainerInfomanager.Instance.GetEmptyPackBoxNumber=itemList[0].equipmentEntity.EQUIP_FIELD_JEWEL_ID1_ACTIVETYPE;
				ShowEnsureMessageBox(ChacheItemList);
			}
		}
		/// <summary>
		///检查装备镶嵌的宝石数量 
		/// </summary>
		/// <returns>The equip jewel count.</returns>
		/// <param name="item">Item.</param>
		int CheckEquipJewelCount(ItemFielInfo item)
		{
			int count=0;
			if(item.GetIfBesetJewel(1))
				count++;
            if(item.GetIfBesetJewel(2))
				count++;
			return count;
		}
		/// <summary>
		/// 显示选择数量面板
		/// </summary>
		void ShowSelectNumPanel()
		{
			if(m_SelectNumPanel==null)
			{
				m_SelectNumPanel = CreatObjectToNGUI.InstantiateObj(SelectNumberPanelPrefab,transform).GetComponent<SelectNumPanel>();
			}
			m_SelectNumPanel.Show(1,ChacheItemList[0].sSyncContainerGoods_SC.byNum,SelectItemNum);
		}

		void SelectItemNum(int num)
		{
			SellItemNum = num;
          
			ShowEnsureMessageBox(ChacheItemList);
		}

		void ShowEnsureMessageBox(List<ItemFielInfo> itemList)
		{
			Dictionary<int,int> getItemList = new Dictionary<int, int>();//Key为ID，Value为数量
			bool isImportantItem = itemList.FirstOrDefault(P=>P.LocalItemData._ColorLevel>1)!=null;//是否包含重要物品
			int BesetedJewelCount=0;//所有装备所镶嵌的宝石的数量
			int price = 0;//出售价格
      
			foreach(var child in itemList)
			{
				switch(child.LocalItemData._GoodsClass)
				{
				case 1:
					EquipmentData sellData = child.LocalItemData as EquipmentData;
					foreach(SaleItemPrice itemChild in sellData.SaleItem)
					{
                            if(itemChild.ItemID!=0)
                            {
        						if(getItemList.ContainsKey(itemChild.ItemID))
        						{
        							int currentNum = getItemList[itemChild.ItemID];
        							getItemList[itemChild.ItemID] = currentNum+itemChild.Price;
        						}else
        						{
        							getItemList[itemChild.ItemID] = itemChild.Price;
        						}
                            }
					}
					BesetedJewelCount+=CheckEquipJewelCount(child);
                        if(SellItemNum==0)
                        {
                            price+=child.LocalItemData._SaleCost*child.sSyncContainerGoods_SC.byNum+child.equipmentEntity.ITEM_FIELD_VISIBLE_COMM;
                        }
                        else
                        {
					      price+=child.LocalItemData._SaleCost*SellItemNum+child.equipmentEntity.ITEM_FIELD_VISIBLE_COMM;
                        }
					break;
				case 2:
                        if(SellItemNum==0)
                        {
                            price+=child.LocalItemData._SaleCost*child.sSyncContainerGoods_SC.byNum+child.equipmentEntity.ITEM_FIELD_VISIBLE_COMM;
                        }
                        else
                        {
                            price+=child.LocalItemData._SaleCost*SellItemNum+child.equipmentEntity.ITEM_FIELD_VISIBLE_COMM;
                        }
					break;
				case 3:
                        if(SellItemNum==0)
                        {
                            price+=child.LocalItemData._SaleCost*child.sSyncContainerGoods_SC.byNum+child.equipmentEntity.ITEM_FIELD_VISIBLE_COMM;
                        }
                        else
                        {
                            price+=child.LocalItemData._SaleCost*SellItemNum+child.equipmentEntity.ITEM_FIELD_VISIBLE_COMM;
                        }
					break;
				}
			}
			if(BesetedJewelCount>ContainerInfomanager.Instance.GetEmptyPackBoxNumber())
			{

				MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_I9_11"),3);
			}
			else
			{
			string getItemStr = string.Empty;
			if(getItemList.Count>0)
			{
				foreach(var child in getItemList)
				{
					getItemStr += string.Format("{0}{1}{2}",child.Value,LanguageTextManager.GetString(ItemDataManager.Instance.GetItemData(child.Key)._szGoodsName)
					                            ,LanguageTextManager.GetString("IDS_I3_62"));
				}
				getItemStr.Substring(0,getItemStr.Length-1);
			}
			getItemStr+=string.Format(LanguageTextManager.GetString("IDS_H1_202"),price);
			string MsgStr = string.Format(LanguageTextManager.GetString(isImportantItem?"IDS_I3_32":"IDS_I3_31"),getItemStr);
                MessageBox.Instance.Show(3,"",MsgStr,LanguageTextManager.GetString("IDS_I3_44"),LanguageTextManager.GetString("IDS_I3_39"),()=>{SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Cancel");},SendSellItemsToSever);
			}
		}

		void SendSellItemsToSever()
		{
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Confirm");
			Dictionary<byte,byte> sellItemList = new Dictionary<byte, byte>();//Key:位置，Value：数量
			foreach(var child in ChacheItemList)
			{
				sellItemList.Add((byte)child.sSyncContainerGoods_SC.nPlace,SellItemNum == 0?child.sSyncContainerGoods_SC.byNum:(byte)SellItemNum);
			}
			sellItemList.ApplyAllItem(P=>TraceUtil.Log(SystemModel.Jiang,"SellItem:Place:"+P.Key+",Num:"+P.Value));
			SMsgContainerDoff_CS sellItemMsg = new SMsgContainerDoff_CS()
			{
				dwSrcContainerID_Heard = ChacheItemList[0].sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID,
				dwSrcContainerID = ChacheItemList[0].sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID,
				bySrcPlaceNum = (byte)ChacheItemList.Count,
				sellItemList = sellItemList,
			};
			NetServiceManager.Instance.ContainerService.SendContainerDoff(sellItemMsg);
		}

	}
}
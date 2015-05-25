using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.MainUI{

	public class UpgradeItemListPanel : BaseTweenShowPanel {

		public GameObject SingleUpgradeItemPrefab;
		public Transform Grid;

		public UIDraggablePanel m_UIDraggablePanel;

		public List<SigleUpgradeItem> MyItemList{get;private set;}
		public EquipmentUpgradePanel MyParent{get;private set;}

		void Awake()
		{
			MyItemList = new List<SigleUpgradeItem>();
		}

		public void Show(ItemFielInfo selectItem,EquipmentUpgradePanel myParent)
		{
			MyParent = myParent;
			UpdateItemList();
			OnMyItemClick(selectItem);
			base.TweenShow();
		}

		public void UpdateItemList()
		{
			List<ItemFielInfo> packItemList = ContainerInfomanager.Instance.GetPackItemList().FindAll(C=>C.LocalItemData._GoodsClass == 1&&(C.LocalItemData as EquipmentData).lUpgradeFlag);
            SortItemList(packItemList);
            InitMyItemList(packItemList);
		}

		void InitMyItemList(List<ItemFielInfo> itemList)
		{
			MyItemList.Clear();
			Grid.ClearChild();
			m_UIDraggablePanel.ResetPosition();
			int lineNumber = (itemList.Count/3)+(itemList.Count%3>0?1:0);
			int itemIndex = 0;
			for(int line = 0;line<lineNumber;line++)
			{
				for(int row = 0;row<3;row++)
				{
					if(itemList.Count>itemIndex)
					{
						SigleUpgradeItem upgradItem = Grid.InstantiateNGUIObj(SingleUpgradeItemPrefab).GetComponent<SigleUpgradeItem>();
						upgradItem.transform.localPosition = new Vector3(-110+110*row,210-110*line,0);
						upgradItem.Init(itemList[itemIndex],OnMyItemClick);
						MyItemList.Add(upgradItem);
						itemIndex++;
					}
				}
			}
//			if(itemList.Count>0)
//			{
//				OnMyItemClick(itemList[0]);//默认选中第一个
//			}
		}

		public void OnMyItemClick(object obj)
		{
			ItemFielInfo selectIteminfo = obj as ItemFielInfo;
			MyItemList.ApplyAllItem(C=>C.SetSelectStatus(selectIteminfo));
			MyParent.OnItemSelect(selectIteminfo);
		}

		void SortItemList(List<ItemFielInfo> itemList)
		{
            itemList.Sort((left,right)=>{return (right.LocalItemData._ColorLevel*1000+right.LocalItemData._AllowLevel)+-(left.LocalItemData._ColorLevel*1000+left.LocalItemData._AllowLevel);});
            itemList.Sort((left,Right)=>{return Right.LocalItemData._goodID-left.LocalItemData._goodID;});
		}

	}
}
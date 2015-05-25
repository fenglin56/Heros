using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.MainUI{

	public class EctypeDropItemDesPanel : MonoBehaviour {

		public UILabel NameLabel;
		public UILabel ExpLabel;
		private List<SingleEctypedropItem> DropItemList = new List<SingleEctypedropItem>();
		public SingleEctypedropItem singleEctypedropItem;
		public Transform Grid;

		public UIPanel MyPanel;
		public Vector3 ShowPos;
		public Vector3 ClosePos;
		float animTime = 0.1f;
		string goodStr = "SingleDropItem";
		void Awake()
		{
			GetComponent<UIPanel>().alpha = 0;
			transform.localPosition = ClosePos;
			for (int i = 1; i <= 6; i++) {
				Transform tran = Grid.Find (goodStr+i);
				GameObject goods = NGUITools.AddChild(tran.gameObject,singleEctypedropItem.gameObject);
				DropItemList.Add(goods.GetComponent<SingleEctypedropItem>());
			}
			singleEctypedropItem.gameObject.SetActive (false);
		}

		public void TweenShow(EctypeContainerData selectData)
		{
			NameLabel.SetText(LanguageTextManager.GetString(selectData.lEctypeName));
			ExpLabel.SetText(selectData.lExperience);
			TweenAlpha.Begin(gameObject,animTime,1);
			TweenPosition.Begin(gameObject,animTime,ShowPos);
			DropItemList.ApplyAllItem(P=>P.gameObject.SetActive(false));
			List<ItemData> dropItem = new List<ItemData>();
			foreach(var child in selectData.DropListItem)
			{
				ItemData getData = ItemDataManager.Instance.GetItemData(child);
				if(getData!=null){
					dropItem.Add(getData);
				}
			}
			for(int i =0;i<DropItemList.Count;i++)
			{
				if(i<dropItem.Count)
				{
					DropItemList[i].gameObject.SetActive(true);
					DropItemList[i].Init(dropItem[i]);
				}else
				{
					DropItemList[i].gameObject.SetActive(false);
				}
			}
			Grid.transform.localPosition = new Vector3(dropItem.Count%2==0?0:50,0,0);
		}
		
		public void TweenClose()
		{
			TweenAlpha.Begin(gameObject,animTime,0);
			TweenPosition.Begin(gameObject,animTime,ClosePos);
		}
		
		public void Close()
		{
			TweenAlpha.Begin(gameObject,0,0);
			transform.localPosition = ClosePos;
		}

		[ContextMenu("GetDropItems")]
		void GetDropItems()
		{
			//DropItemList = transform.GetComponentsInChildren<SingleEctypedropItem>();//jamfing
		}

	}
}
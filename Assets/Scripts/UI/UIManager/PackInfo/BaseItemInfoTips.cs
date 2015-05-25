using UnityEngine;
using System.Collections;

namespace UI.MainUI{

	public class BaseItemInfoTips : MonoBehaviour {

		public UIPanel[] MyPanelList;
		public UILabel ItemPriceLabel;//出售价格
		public UILabel UnableSaleLabel;
		public AutoAddDragTool m_AutoAddDragTool;//自动添加工具

		public Vector3 LeftShowPos;
		public Vector3 LeftHidePos;
		public Vector3 RightShowPos;
		public Vector3 RightHidePos;

		private GameObject TweenFloatObj;
		public ItemFielInfo m_ItemFielInfo{get;private set;}
        [HideInInspector]
        public bool ISShowing;
		protected bool IsLeftPos;

		public virtual void Init(ItemFielInfo itemFielInfo,bool isLeftPos,bool CanSale)
		{
			IsLeftPos = isLeftPos;
			m_ItemFielInfo = itemFielInfo;
            if(CanSale)
            {
				UnableSaleLabel.gameObject.SetActive(false);
				ItemPriceLabel.gameObject.SetActive(true);
	            ItemPriceLabel.SetText(itemFielInfo.LocalItemData._SaleCost+itemFielInfo.equipmentEntity.ITEM_FIELD_VISIBLE_COMM);
            }
            else
            {
				ItemPriceLabel.gameObject.SetActive(false);
				UnableSaleLabel.gameObject.SetActive(true);
				UnableSaleLabel.SetText(NGUIColor.SetTxtColor( LanguageTextManager.GetString("IDS_I1_50"),TextColor.red));
            }
           // Close();
		}
		public void AddObj(GameObject obj)
		{
			m_AutoAddDragTool.Add(obj);
		}

		public void TweenShow()
		{
			float animTime = 0.1f;
			TweenPosition.Begin(gameObject,animTime,IsLeftPos?LeftShowPos:RightShowPos);
			if(TweenFloatObj!=null){DestroyImmediate(TweenFloatObj);}
			TweenFloatObj = TweenFloat.Begin(animTime,MyPanelList[0].alpha,1,SetMyPanelAlpha);
		}

		public void TweenClose()
		{
            ISShowing=false;
			float animTime = 0.1f;
			TweenPosition.Begin(gameObject,animTime,IsLeftPos?LeftHidePos:RightHidePos);
			if(TweenFloatObj!=null){DestroyImmediate(TweenFloatObj);}
			TweenFloatObj = TweenFloat.Begin(animTime,MyPanelList[0].alpha,0,SetMyPanelAlpha,DestroyMySelf);
		}

		public void Close()
		{
            ISShowing=false;
			transform.localPosition = new Vector3(0,0,-1000);

		}

		void SetMyPanelAlpha(float value)
		{
			MyPanelList.ApplyAllItem(P=>P.alpha = value);
		}

		void DestroyMySelf(object obj)
		{
			DestroyImmediate(gameObject);
		}

		void OnDestroy()
		{
			if(TweenFloatObj!=null){Destroy(TweenFloatObj);}
		}

	}
}
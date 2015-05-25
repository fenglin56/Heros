using UnityEngine;
using System.Collections;
using UI.MainUI;
using UI.Gift;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UI
{
	public class GiftPanel : BaseItemInfoTips 
	{
        public SingleButtonCallBack Btn_PathLink;
		public UILabel Label_GoodName;
		public UILabel Label_AllowLevel;
		public UILabel Label_Num;
		public SpriteSwith Switch_Icon;
		public Transform Point_GiftIcon;

		public UILabel Label_SellPrice;

		public UILabel Label_Introduction;
		public UILabel Label_ContentTitle;
		public UILabel Label_RewardContent;

		public LocalButtonCallBack Button_Use;
		public LocalButtonCallBack Button_AllUse;
		public LocalButtonCallBack Button_Sell;
		//public LocalButtonCallBack Button_Exit;

		//public GameObject MessageItemPrefab;
		//private List<GameObject> m_MessageItemList = new List<GameObject>();

		private int m_curGoodID = 0;
		private int m_curNum = 0;
		private bool m_isArrowLevel = false;
		private MedicamentData m_MedicamentData;
		private PlayerGiftConfigData m_GiftConfigData;

		//private Vector3 m_MessageItemAppearPos = new Vector3(0, 0, -10);

		private ItemFielInfo m_curItemFielInfo;

		public Action<PackBtnType> CallBackOnSellClick;
		public Action<object> CallBackOnCloseHandle;
        void  Awake()
        {
            Btn_PathLink.SetCallBackFuntion(OnPathLinkBtnClick);
        }

        void OnPathLinkBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default_Open");
            ItemInfoTipsManager.Instance.BeigenShowPathLinkPanel();
        }

        void Start()
		{
			Button_Use.SetCallBackFuntion(OnUseClick,null);
			Button_AllUse.SetCallBackFuntion(OnAllUseClick,null);
			Button_Sell.SetCallBackFuntion(OnSellClick,null);
			//Button_Exit.SetCallBackFuntion(OnExitClick,null);

			UIEventManager.Instance.RegisterUIEvent(UIEventType.ResetContainerGoods, ReceiveUpdateHandle);

            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            Button_Use.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_Gift_Use);
            Button_AllUse.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_Gift_UseAll);
            Button_Sell.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_Gift_Sell);
           // Button_Exit.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_Gift_Back);
        }

		void OnDestroy()
		{
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ResetContainerGoods, ReceiveUpdateHandle);
		}

		void ReceiveUpdateHandle(object obj)
		{
			List<SSyncContainerGoods_SC> syncList = (List<SSyncContainerGoods_SC>)obj;
			//var containerGoods = syncList.FirstOrDefault(p=>p.uidGoods == m_curItemFielInfo.sSyncContainerGoods_SC.uidGoods);
			//m_curNum = containerGoods.byNum;
            if(m_curItemFielInfo!=null)
            {
			Show(m_curItemFielInfo);		
            }
		}


		public void Show(ItemFielInfo itemFielInfo )
		{
			this.m_curGoodID = itemFielInfo.LocalItemData._goodID;
			this.m_curItemFielInfo = itemFielInfo;
            m_MedicamentData = ItemDataManager.Instance.GetItemData(m_curGoodID) as MedicamentData;
            Show(m_MedicamentData,false);
        }
      public  void Show(MedicamentData m_MedicamentData,bool ShowPathLink)
        {
            ISShowing=true;
            this.m_curGoodID=m_MedicamentData._goodID;
            if(ShowPathLink)
            {
                Btn_PathLink.gameObject.SetActive(true);
                Button_Use.gameObject.SetActive(false);
                Button_Sell.gameObject.SetActive(false);
               // Button_Exit.gameObject.SetActive(false);
                Button_AllUse.gameObject.SetActive(false);
            }
            else
            {
                Btn_PathLink.gameObject.SetActive(false);
                Button_Use.gameObject.SetActive(true);
				Button_Sell.gameObject.SetActive(m_MedicamentData._TradeFlag == 1);
               // Button_Exit.gameObject.SetActive(true);
                Button_AllUse.gameObject.SetActive(true);
            }
            TextColor textColor = TextColor.white;
            switch( m_MedicamentData._ColorLevel)
            {
                case 0:
                    textColor = TextColor.EquipmentGreen;
                    break;
                case 1:
                    textColor = TextColor.EquipmentBlue;
                    break;
                case 2:
                    textColor = TextColor.EquipmentMagenta;
                    break;
                case 3:
                    textColor= TextColor.EquipmentYellow;
                    break;
            }
            Label_GoodName.text = NGUIColor.SetTxtColor(LanguageTextManager.GetString( m_MedicamentData._szGoodsName), textColor);
            Switch_Icon.ChangeSprite(m_MedicamentData._ColorLevel+1);
            CreatObjectToNGUI.InstantiateObj( m_MedicamentData._DisplayIdSmall,Point_GiftIcon);
            Label_AllowLevel.text = m_MedicamentData._AllowLevel.ToString();
            int playerLevel = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
            
            if(playerLevel>=m_MedicamentData._AllowLevel)
            {
                m_isArrowLevel = true;
                Label_AllowLevel.color = Color.white;
            }
            else
            {
                m_isArrowLevel = false;
                Label_AllowLevel.color = Color.red;
            }
            
            this.m_curNum = ContainerInfomanager.Instance.GetItemNumber(m_curGoodID);
            Label_Num.text = m_curNum.ToString();
            
            Label_Introduction.text = LanguageTextManager.GetString(m_MedicamentData._szDesc).Replace(@"\n","\n");
            Label_SellPrice.text = m_MedicamentData._SaleCost.ToString();
            
            m_GiftConfigData= ItemDataManager.Instance.GetGiftData(m_curGoodID);
            if(m_GiftConfigData._giftType == 1)
            {
                Label_ContentTitle.text = LanguageTextManager.GetString("IDS_I1_40");
            }
            else if(m_GiftConfigData._giftType == 2)
            {
                Label_ContentTitle.text = LanguageTextManager.GetString("IDS_I1_41");
            }
            //
            transform.localPosition = Vector3.zero;
            
            //奖品
            string content = "";
            var goodsDisplay  = m_GiftConfigData._ProfessionGoodsDisplay.SingleOrDefault(p=>p.Profession == PlayerManager.Instance.FindHeroDataModel().GetCommonValue().PLAYER_FIELD_VISIBLE_VOCATION);
            for(int i = 0;i< goodsDisplay.GoodsDisplay.Length;i++)
            {
                var ItemData =  ItemDataManager.Instance.GetItemData(goodsDisplay.GoodsDisplay[i].GoodsID);
                //              if(m_GiftConfigData._giftType == 1)
                //              {
                //                  content += LanguageTextManager.GetString(ItemData._szGoodsName) + "("
                //                      +m_GiftConfigData._GoodsDisplay[i].MinNum.ToString()+ "~" 
                //                          +m_GiftConfigData._GoodsDisplay[i].MinNum.ToString()+")" + "\n";
                //              }
                //              else if(m_GiftConfigData._giftType == 2)
                //              {
                //                  content += LanguageTextManager.GetString(ItemData._szGoodsName) + "("
                //                      +m_GiftConfigData._GoodsDisplay[i].MinNum.ToString()+")" + "\n";
                //              }
                if(goodsDisplay.GoodsDisplay[i].MinNum == goodsDisplay.GoodsDisplay[i].MaxNum)
                {
                    content += LanguageTextManager.GetString(ItemData._szGoodsName) + " X "
                        +goodsDisplay.GoodsDisplay[i].MinNum.ToString() + "\n";
                }
                else
                {
                    content += LanguageTextManager.GetString(ItemData._szGoodsName) + " X "
                        +goodsDisplay.GoodsDisplay[i].MinNum.ToString()+ "-" 
                            +goodsDisplay.GoodsDisplay[i].MaxNum.ToString()+ "\n";
                }
            }
            Label_RewardContent.text = content;

        }
		public void Show(ItemFielInfo itemFielInfo,bool isLeftPos)
		{
			Show(itemFielInfo.LocalItemData as MedicamentData,isLeftPos);
			base.Init(itemFielInfo, isLeftPos, itemFielInfo.LocalItemData._TradeFlag == 1);
			transform.localPosition=new Vector3(240,0,0);
		}	

//		[ContextMenu("ShowGetPropsMessage")]
//		public void ShowGetPropsMessage()
//		{
//			//SoundManager.Instance.PlaySoundEffect("Sound_UIEff_PackageDropGoods");
//
//			int listLength = m_MessageItemList.Count;
//			if(listLength >= /*CommonDefineManager.Instance.CommonDefine.ItemMsgLimit*/8)
//			{
//				GiftMessageItem gm = m_MessageItemList[listLength-1].GetComponent<GiftMessageItem>();
//				gm.Disappear();
//				m_MessageItemList.RemoveAt(listLength-1);
//			}
//
//			m_MessageItemList.ApplyAllItem(p=>
//			                               {
//				GiftMessageItem pItem = p.GetComponent<GiftMessageItem>();
//				pItem.Push();
//			});
//
//			GameObject message = (GameObject)Instantiate(MessageItemPrefab);
//			message.transform.parent = PopupObjManager.Instance.UICamera.transform;
//			message.transform.localScale = Vector3.one;
//
//			m_MessageItemList.Add(message);
//			GiftMessageItem gmItem = message.GetComponent<GiftMessageItem>();
//			gmItem.Show(0,1,m_MessageItemAppearPos);
//
//			StartCoroutine(LateDestroy(message));
//
//		}

//		IEnumerator LateDestroy(GameObject gobj)
//		{
//			yield return new  WaitForSeconds(CommonDefineManager.Instance.CommonDefine.ItemMsgTimeDisappear);
//			m_MessageItemList.Remove(gobj);
//			GiftMessageItem item = gobj.GetComponent<GiftMessageItem>();
//			item.Disappear();
//		}

		void OnUseClick(object obj)
		{
			if(!m_isArrowLevel)
			{
				MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_H1_189"),2f);
				SoundManager.Instance.PlaySoundEffect("Sound_Button_PackageUseFail");
//				StopCoroutine("LateUseNext");
				StopAllCoroutines();
				return;
			}

			if(ContainerInfomanager.Instance.GetEmptyPackBoxNumber() < m_GiftConfigData._packageNeed)
			{
				MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_H1_567"),2f);
				SoundManager.Instance.PlaySoundEffect("Sound_Button_PackageUseFail");
//				StopCoroutine("LateUseNext");
				StopAllCoroutines();
				return;
			}

            ContainerInfomanager.Instance.UseGiftBox(this.m_curItemFielInfo);
			if(m_curNum<=1)
			{
				this.OnExitClick(null);
			}
		}

		void OnAllUseClick(object obj)
		{
			OnUseClick(null);
			int num = m_curNum - 1;
			if(num>0)
			{
				StartCoroutine("LateUseNext", num);
			}
			else
			{
				this.OnExitClick(null);
			}
		}

		IEnumerator LateUseNext(int num)
		{
			yield return new WaitForSeconds(CommonDefineManager.Instance.CommonDefine.GoodsDropAutoTime);
			if(m_isArrowLevel)
			{
				if(ContainerInfomanager.Instance.GetEmptyPackBoxNumber() >= m_GiftConfigData._packageNeed)
				{
					OnAllUseClick(null);
				}
				else
				{
					MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_H1_567"),2f);
				}
			}
			else
			{
				MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_H1_189"),2f);
			}
		}

		void OnSellClick(object obj)
		{
			CallBackOnSellClick(PackBtnType.Sell);
		}

		void OnExitClick(object obj)
		{
			CallBackOnCloseHandle(null);
//			transform.parent.SendMessage("Close",obj);
//			Destroy(gameObject);
		}


	}
}

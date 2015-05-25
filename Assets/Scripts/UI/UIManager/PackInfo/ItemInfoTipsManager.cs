using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

namespace UI.MainUI{

	public class ItemInfoTipsManager : MonoBehaviour {

		public GameObject RightBtnManagerPrefab;
		public SingleButtonCallBack BackBtn;

		public Vector3 RightBtnShowPos;
		public Vector3 RightBtnHidPos;
		private PackRightBtnManager m_PackRightBtnManager; //右侧按钮列表管理

		public GameObject EquiptItemTipsPrefab;
		public GameObject MedicineItemTipsPrefab;
		public GameObject JewelItemTipsPrefab;
		public GameObject GiftItemTipsPrefab;
        public GameObject PathLinkPanelPrefab;
		ItemInfoTips_Equipment EquiptItemTips_normal = null;
		ItemInfoTips_Equipment EquiptItemTips_equipt = null;
		ItemInfoTips_Medicine MedicineItemtips =null;
		ItemInfoTips_Jewel EquiptItemTips_Jewel = null;

        private TweenPosition Tween_pos_mainItem;
		GiftPanel GiftPanelItemtips = null;
		public PackInfoPanel MyParent{get;private set;}
		public ItemFielInfo CurrentItem{get;private set;}
        private bool HasClik;
        private PathLinkPanel Linkpanel;
        private static ItemInfoTipsManager instance;
        public static ItemInfoTipsManager Instance
        {
            get
            {
                
                if (!instance) {
                    instance = (ItemInfoTipsManager)GameObject.FindObjectOfType (typeof(ItemInfoTipsManager));
                    if (!instance)
                        Debug.LogError ("没有附加JewelBesetManager脚本的gameobject在场景中");
                }
                return instance;
            }
        }
		void Awake()
		{
			m_PackRightBtnManager = CreatObjectToNGUI.InstantiateObj(RightBtnManagerPrefab,transform).GetComponent<PackRightBtnManager>();
			m_PackRightBtnManager.transform.localPosition = RightBtnHidPos;
			BackBtn.SetCallBackFuntion(Close);
            Linkpanel = CreatObjectToNGUI.InstantiateObj(PathLinkPanelPrefab,transform).GetComponent<PathLinkPanel>();
            Linkpanel.transform.localPosition=new Vector3(0,0,-1000);
            //TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            //BackBtn.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_ItemInfoTips_Back);
        }

        /// <summary>
        /// 需要展示详细属性的调这个，比如在背包里
        /// </summary>
        /// <param name="itemFielInfo">Item fiel info.</param>
        /// <param name="myParent">My parent.</param>
		public void Show(ItemFielInfo itemFielInfo,PackInfoPanel myParent)
		{
            m_PackRightBtnManager.gameObject.SetActive(true);
            HasClik=false;
            bool ShowPathLinkBtn=false;
			this.MyParent = myParent;
			CurrentItem = itemFielInfo;
			TweenAlpha.Begin(m_PackRightBtnManager.gameObject,0.1f,0,1,null);
			TweenPosition.Begin(m_PackRightBtnManager.gameObject,0.1f,m_PackRightBtnManager.transform.localPosition,RightBtnShowPos);
			transform.localPosition = new Vector3(0,0,-150);
			InitButton(itemFielInfo);
			switch (itemFielInfo.LocalItemData._GoodsClass)
			{
			case 1://装备显示
				bool isEquiptItem = ContainerInfomanager.Instance.GetEquiptItemList().FirstOrDefault(P=>P==itemFielInfo) != null;
				if(isEquiptItem)
				{
                        if(EquiptItemTips_equipt==null)
                        {
                            EquiptItemTips_equipt = CreatObjectToNGUI.InstantiateObj(EquiptItemTipsPrefab,transform).GetComponent<ItemInfoTips_Equipment>();
                        }
                     
                        EquiptItemTips_equipt.Show(itemFielInfo,true,true,ShowPathLinkBtn);	
				}
				else
				{
                        if(EquiptItemTips_normal==null)
                        {
                            EquiptItemTips_normal = CreatObjectToNGUI.InstantiateObj(EquiptItemTipsPrefab,transform).GetComponent<ItemInfoTips_Equipment>();
                        }
                        EquiptItemTips_normal.Show(itemFielInfo,false,false,ShowPathLinkBtn);	
						ItemFielInfo equiptPairItem = ContainerInfomanager.Instance.GetEquiptItemList().FirstOrDefault
						(P=>(P.LocalItemData as EquipmentData)._vectEquipLoc ==(itemFielInfo.LocalItemData as EquipmentData)._vectEquipLoc);
						if(equiptPairItem!=null)
						{
                            if(EquiptItemTips_equipt==null)
                            {
                                EquiptItemTips_equipt = CreatObjectToNGUI.InstantiateObj(EquiptItemTipsPrefab,transform).GetComponent<ItemInfoTips_Equipment>();
                            }
							EquiptItemTips_equipt.Show(equiptPairItem,true,true,ShowPathLinkBtn);	
						}
				}
				break;
			case 2:
				if(itemFielInfo.LocalItemData._GoodsSubClass ==4)				
				{
					//if(GiftPanelItemtips!=null){Destroy(GiftPanelItemtips);}
                        if(GiftPanelItemtips==null)
                        {
                            GiftPanelItemtips = CreatObjectToNGUI.InstantiateObj(GiftItemTipsPrefab,transform).GetComponent<GiftPanel>();
                        }
					GiftPanelItemtips.Show(itemFielInfo);
					GiftPanelItemtips.CallBackOnSellClick = OnButtonClick;
					GiftPanelItemtips.CallBackOnCloseHandle = Close;
				}
				else
				{
                        if(MedicineItemtips==null)
                        {
                            MedicineItemtips = CreatObjectToNGUI.InstantiateObj(MedicineItemTipsPrefab,transform).GetComponent<ItemInfoTips_Medicine>();
                        }
					MedicineItemtips.Show(itemFielInfo);
				}
				break;
			case 3:
				if (itemFielInfo.LocalItemData._GoodsSubClass == 3)
				{
                        if(EquiptItemTips_Jewel==null)
                        {
                            EquiptItemTips_Jewel = CreatObjectToNGUI.InstantiateObj(JewelItemTipsPrefab, transform).GetComponent<ItemInfoTips_Jewel>();
                        }
					EquiptItemTips_Jewel.Show(itemFielInfo, true, true);
				}
				else
				{
                        if(MedicineItemtips==null)
                        {
                            MedicineItemtips = CreatObjectToNGUI.InstantiateObj(MedicineItemTipsPrefab,transform).GetComponent<ItemInfoTips_Medicine>();
                        }
				MedicineItemtips.Show(itemFielInfo);
				}
				break;
			default:
				break;
			}
		}


        /// <summary>
        ///需要展示基础属性的调这个，这个不会考虑物品等级，强化等 
        /// </summary>
        /// <param name="ItemID">Item I.</param>
        public void Show(int ItemID)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default_Open");
            HasClik=false;
            bool ShowPathLinkBtn=true;
           // this.MyParent = myParent;
            //CurrentItem = itemFielInfo;
            ItemFielInfo itemFielInfo=new ItemFielInfo(ItemID);
            CurrentItem=itemFielInfo;
            if(itemFielInfo.LocalItemData==null)
            {
                return;
            }

            switch (itemFielInfo.LocalItemData._GoodsClass)
            {
                case 1://装备显示
                    if(EquiptItemTips_normal==null)
                    {
                        EquiptItemTips_normal = CreatObjectToNGUI.InstantiateObj(EquiptItemTipsPrefab,transform).GetComponent<ItemInfoTips_Equipment>();
                    }
                        EquiptItemTips_normal.Show(itemFielInfo,false,false,ShowPathLinkBtn);  

                    break;
                case 2:
                    if(itemFielInfo.LocalItemData._GoodsSubClass ==4)               
                    {
                        //if(GiftPanelItemtips!=null){Destroy(GiftPanelItemtips);}
                        if(GiftPanelItemtips==null)
                        {
                            GiftPanelItemtips = CreatObjectToNGUI.InstantiateObj(GiftItemTipsPrefab,transform).GetComponent<GiftPanel>();
                        }
                        GiftPanelItemtips.Show(itemFielInfo, true);
//                        GiftPanelItemtips.CallBackOnSellClick = OnButtonClick;
                        GiftPanelItemtips.CallBackOnCloseHandle = Close;
                    }
                    else
                    {
                        if(MedicineItemtips==null)
                        {
                            MedicineItemtips = CreatObjectToNGUI.InstantiateObj(MedicineItemTipsPrefab,transform).GetComponent<ItemInfoTips_Medicine>();
                        }
                        MedicineItemtips.Show(itemFielInfo.LocalItemData as MedicamentData);
                    }
                    break;
                case 3:
                    if(itemFielInfo.LocalItemData._GoodsSubClass==9)
                    {
                        return;
                    }
                        if (itemFielInfo.LocalItemData._GoodsSubClass == 3)
                    {
                        if(EquiptItemTips_Jewel==null)
                        {
                            EquiptItemTips_Jewel = CreatObjectToNGUI.InstantiateObj(JewelItemTipsPrefab, transform).GetComponent<ItemInfoTips_Jewel>();
                        }
                        EquiptItemTips_Jewel.Show(itemFielInfo.LocalItemData as Jewel, true, true);
                    }
                    else
                    {
                        if(MedicineItemtips==null)
                        {
                            MedicineItemtips = CreatObjectToNGUI.InstantiateObj(MedicineItemTipsPrefab,transform).GetComponent<ItemInfoTips_Medicine>();
                        }
                        MedicineItemtips.Show(itemFielInfo.LocalItemData);
                    }
                    break;
                default:
                    return;
            }
            //            TweenAlpha.Begin(m_PackRightBtnManager.gameObject,0.1f,0,1,null);
            //            TweenPosition.Begin(m_PackRightBtnManager.gameObject,0.1f,m_PackRightBtnManager.transform.localPosition,RightBtnShowPos);
            transform.localPosition = new Vector3(0,0,-150);
            //InitButton(itemFielInfo);
            m_PackRightBtnManager.gameObject.SetActive(false);
        }
        public void ClosePathLinkpanel()
        {
            Linkpanel.Isshow=false;
            Linkpanel.transform.localPosition=new Vector3(0,0,-1000);
        }
        public void Close()
        {
            ClosePathLinkpanel();
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Stone_Cancel");
            if(EquiptItemTips_normal!=null){EquiptItemTips_normal.Close();}
            if(EquiptItemTips_equipt!=null){EquiptItemTips_equipt.Close();}
            if(MedicineItemtips!=null){MedicineItemtips.Close();}
            if(GiftPanelItemtips!=null){GiftPanelItemtips.Close();}
            if (EquiptItemTips_Jewel != null) { EquiptItemTips_Jewel.Close(); }
            m_PackRightBtnManager.transform.localPosition=RightBtnHidPos;
            TweenAlpha.Begin(m_PackRightBtnManager.gameObject,0.1f,1,0,null);
            MoveBack(null);
        }
		public void Close(object obj)
		{   
          //  Tween_pos_mainItem.callWhenFinished=null;
            ClosePathLinkpanel();
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Stone_Cancel");
			if(EquiptItemTips_normal!=null){EquiptItemTips_normal.TweenClose();}
			if(EquiptItemTips_equipt!=null){EquiptItemTips_equipt.TweenClose();}
			if(MedicineItemtips!=null){MedicineItemtips.TweenClose();}
			if(GiftPanelItemtips!=null){GiftPanelItemtips.TweenClose();}
			if (EquiptItemTips_Jewel != null) { EquiptItemTips_Jewel.TweenClose(); }
			TweenPosition.Begin(m_PackRightBtnManager.gameObject,0.1f,RightBtnHidPos);
			TweenAlpha.Begin(m_PackRightBtnManager.gameObject,0.1f,1,0,null);
			DoForTime.DoFunForTime(0.2f,MoveBack,null);
		}

        public void BeigenShowPathLinkPanel()
        {
            if(!Linkpanel.Isshow)
            {
                Linkpanel.Isshow=true;
                Vector3 from=new Vector3(0,0,-1);
                Vector3 to=new Vector3(from.x-165,from.y,from.z);
                if(EquiptItemTips_normal!=null&&EquiptItemTips_normal.ISShowing)
                { 
                 Tween_pos_mainItem=   TweenPosition.Begin(EquiptItemTips_normal.gameObject,0.17f,from,to);
                }
                else if(EquiptItemTips_Jewel!=null&&EquiptItemTips_Jewel.ISShowing)
                {
                   
                    Tween_pos_mainItem= TweenPosition.Begin(EquiptItemTips_Jewel.gameObject,0.17f,from,to);
                }
                else if(MedicineItemtips!=null&&MedicineItemtips.ISShowing)
                {
                    Tween_pos_mainItem= TweenPosition.Begin(MedicineItemtips.gameObject,0.17f,from,to);
                }
                else if(GiftPanelItemtips!=null&&GiftPanelItemtips.ISShowing)
                {
                    Vector3 loaclTo=new Vector3(to.x+240,to.y,to.z);
                    Tween_pos_mainItem= TweenPosition.Begin(GiftPanelItemtips.gameObject,0.17f,from,loaclTo);
                }

                Invoke("ShowLinkpanel",0.17f);
            }
        }
        void ShowLinkpanel()
        {

            if(Linkpanel==null)
            {
             Linkpanel = CreatObjectToNGUI.InstantiateObj(PathLinkPanelPrefab,transform).GetComponent<PathLinkPanel>();
            }
            Linkpanel.Show(CurrentItem);
            Vector3 from=new Vector3(250,0,-1);
            Vector3 to=new Vector3(150,0,-1);
            TweenPosition.Begin(Linkpanel.gameObject,0.07f,from,to);
        }
		void MoveBack(object obj)
		{
			transform.localPosition = new Vector3(0,0,-1000);
		}

		public void InitButton(ItemFielInfo itemFileInfo)
		{
			List<PackBtnType> btnList = new List<PackBtnType>();
			switch (itemFileInfo.LocalItemData._GoodsClass)
            {
                case 1://装备
                    if(ContainerInfomanager.Instance.IsItemEquipped(itemFileInfo))
                    {
                        CheckAdd(btnList,PackBtnType.PutOff); //卸下
                    }
                    else
                    {
                        CheckAdd(btnList,PackBtnType.PutOn); //穿上
						if(itemFileInfo.LocalItemData._TradeFlag == 1)
						{
                        	CheckAdd(btnList,PackBtnType.Sell);  
						}
                    }
                   
                      CheckAdd(btnList,PackBtnType.Strength);

                    //btnList.Add(PackBtnType.Strength);  //强化
                   
                    CheckAdd(btnList,PackBtnType.StarUpgrade);

                   // btnList.Add(PackBtnType.StarUpgrade);//升星
                    if (CanUpgrade())
                    {
                        CheckAdd(btnList,PackBtnType.Upgrade);
                       // btnList.Add(PackBtnType.Upgrade);
                    }
                 
                        CheckAdd(btnList,PackBtnType.Diamond);
                        //btnList.Add(PackBtnType.Diamond);   //    }
              
                    break;
                case 2:
                    if (itemFileInfo.LocalItemData._GoodsSubClass == 4)
                    {
                        break;
                    }
					if(itemFileInfo.LocalItemData._TradeFlag == 1)
					{
                    	CheckAdd(btnList,PackBtnType.Sell);
					}
                    //btnList.Add(PackBtnType.Sell);		//出售
                    break;
                case 3:
			
                    if (itemFileInfo.LocalItemData._GoodsSubClass == 3)
                    {
                        CheckAdd(btnList,PackBtnType.Swallow);
                        //btnList.Add(PackBtnType.Swallow);//吞噬升级
                        CheckAdd(btnList,PackBtnType.Diamond);
                       // btnList.Add(PackBtnType.Diamond);//镶嵌
                    }
					if(itemFileInfo.LocalItemData._TradeFlag == 1)
					{
                    	btnList.Add(PackBtnType.Sell);      //出售
					}
                    break;
                default:
                    break;
            }
			m_PackRightBtnManager.PackBtnOnClick = OnButtonClick;

			StartCoroutine(m_PackRightBtnManager.AddBtn(btnList.ToArray()));
            //引导代码
            StartCoroutine(RegisterRightBtn());
		}

//        private bool CanStrengthAndStarUp()
//        {
//            EquipmentData item = CurrentItem.LocalItemData as EquipmentData;
//          
//            if(item._StrengFlag==1)
//            {
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }

//        private bool CanDiamond()
//        {
//            EquipmentData item = CurrentItem.LocalItemData as EquipmentData;
//            
//            if(item._HoleMax>0)
//            {
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }

        private void CheckAdd(List<PackBtnType> btnList,PackBtnType btnType)
        {
            PackBtnType[] uiType =NewUIDataManager.Instance.InitMainButtonList.Single(P => P.ButtonProgress == GameManager.Instance.MainButtonIndex).PackBtnTypeList;
            if (uiType.LocalContains(btnType))
            {
                btnList.Add(btnType);
            }
        }
        private IEnumerator RegisterRightBtn()
        {
            //因为右边按钮的创建，在下一帧进行位置调整，所以这里两帧后再注入引导，保证引导信息会加在正确的按钮上
            yield return null;
            yield return null;
            m_PackRightBtnManager.RegisterGuideBtn(PackBtnType.Strength, UIType.Package, BtnMapId_Sub.Package_ItemInfoTips_Strength);
            m_PackRightBtnManager.RegisterGuideBtn(PackBtnType.PutOn, UIType.Package, BtnMapId_Sub.Package_ItemInfoTips_Puton);
            m_PackRightBtnManager.RegisterGuideBtn(PackBtnType.StarUpgrade, UIType.Package, BtnMapId_Sub.Package_ItemInfoTips_StatUp);
            m_PackRightBtnManager.RegisterGuideBtn(PackBtnType.Upgrade, UIType.Package, BtnMapId_Sub.Package_ItemInfoTips_Upgrade);
            m_PackRightBtnManager.RegisterGuideBtn(PackBtnType.Diamond, UIType.Package, BtnMapId_Sub.Package_ItemInfoTips_Gem);
            m_PackRightBtnManager.RegisterGuideBtn(PackBtnType.Sell, UIType.Package, BtnMapId_Sub.Package_ItemInfoTips_Sell);
            m_PackRightBtnManager.RegisterGuideBtn(PackBtnType.Swallow, UIType.Package, BtnMapId_Sub.Package_ItemInfoTips_Swallow);
        }

		bool CanUpgrade()
		{
			EquipmentData equipmentData = CurrentItem.LocalItemData as EquipmentData;
			return equipmentData!=null&&equipmentData.lUpgradeFlag;
		}

		void OnButtonClick(PackBtnType clickBtnType)
		{

            if(!HasClik)
            {
                HasClik=true;
			switch (clickBtnType)
            {
                case PackBtnType.Sell:
                    SoundManager.Instance.PlaySoundEffect("Sound_Button_PackageSaleItem");
                  
                        List<ItemFielInfo> sellItemList = new List<ItemFielInfo>(){CurrentItem};
                        MyParent.m_SellItemConfirmPanel.SellItem(sellItemList);
                    
                    break;
                   
                case PackBtnType.PutOn:
                    SoundManager.Instance.PlaySoundEffect("Sound_Button_PackageWearEquipment");
                    switch (GetBestItem.GetEquipItemStatus(CurrentItem))
                    {
                        case EquipButtonType.CanEquip:
                            EquiptItem();
                            break;
                        case EquipButtonType.LVNotEnough:
                            MessageBox.Instance.ShowTips(1,LanguageTextManager.GetString("IDS_I1_28"),1);
                            break;
                        case EquipButtonType.ProfesionNotEnough:
                            MessageBox.Instance.ShowTips(1,LanguageTextManager.GetString("IDS_I1_27"),1);
                            break;
                    }
                   
                    break;
                case PackBtnType.PutOff:
                    {
                        if(ContainerInfomanager.Instance.PackIsFull())
                            {
                                MessageBox.Instance.ShowTips(2,LanguageTextManager.GetString("IDS_I3_76"),1.5f);
                            }
                        else
                            {
                                UnEquipItem();
                            }
                    }
                    break;
                case PackBtnType.Strength:
                    MainUIController.Instance.OpenMainUI(UIType.EquipmentUpgrade,UpgradeType.Strength,CurrentItem);
                    break;
                case PackBtnType.StarUpgrade:
                    SoundManager.Instance.PlaySoundEffect("Sound_Button_Stone_Click");
                        MainUIController.Instance.OpenMainUI(UIType.EquipmentUpgrade,UpgradeType.StarUp,CurrentItem);
                    break;
                case PackBtnType.Upgrade:
                        MainUIController.Instance.OpenMainUI(UIType.EquipmentUpgrade,UpgradeType.Upgrade,CurrentItem);
                    break;
                case PackBtnType.Diamond:
                    SoundManager.Instance.PlaySoundEffect("Sound_Button_Stone_Click");
                    MainUIController.Instance.OpenMainUI(UIType.Gem, JewelState.JewelBeset,CurrentItem);
                    break;
                case PackBtnType.Swallow:
                    SoundManager.Instance.PlaySoundEffect("Sound_Button_Stone_Click");
                    MainUIController.Instance.OpenMainUI(UIType.Gem, JewelState.JewelUpgrad,CurrentItem);
                    break;
                default:
                    break;
            }
			Close(null);
            }
		}
		/// <summary>
		/// 装备物品
		/// </summary>
		void EquiptItem()
		{
			SMsgContainerUse_CS dataStruct = new SMsgContainerUse_CS();
			dataStruct.dwContainerID1 = dataStruct.dwContainerID2 = CurrentItem.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID;
			dataStruct.byPlace = (byte)CurrentItem.sSyncContainerGoods_SC.nPlace;
			dataStruct.uidTarget = PlayerManager.Instance.FindHeroEntityModel().EntityDataStruct.SMsg_Header.uidEntity;
			dataStruct.byUseType = CurrentItem.LocalItemData._GoodsClass == 2 ? (byte)1 : (byte)0;
			NetServiceManager.Instance.ContainerService.SendContainerUse(dataStruct);
			TraceUtil.Log(SystemModel.Jiang,"EquiptItem");
		}

        void UnEquipItem()
        {
            SMsgContainerUse_CS dataStruct = new SMsgContainerUse_CS();
            dataStruct.dwContainerID1 = dataStruct.dwContainerID2 = CurrentItem.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID;
            dataStruct.byPlace = (byte)CurrentItem.sSyncContainerGoods_SC.nPlace;
            dataStruct.uidTarget = PlayerManager.Instance.FindHeroEntityModel().EntityDataStruct.SMsg_Header.uidEntity;
            dataStruct.desPlace = -1;
            dataStruct.byUseType = CurrentItem.LocalItemData._GoodsClass == 2 ? (byte)1 : (byte)0;
            NetServiceManager.Instance.ContainerService.SendContainerUse(dataStruct);
            TraceUtil.Log(SystemModel.Jiang,"EquiptItem");
        }
	}
}
using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.Battle
{

    public class BattleMagicAndHealthButton_V2 : View
    {
        
        public GameObject ButtonPrefab;

        HealthAndMagicButton medicine;

        UI.MainUI.ItemFielInfo MedicineButtonItemFielInfo;

        UI.MainUI.ItemFielInfo OnUseritemFilelInfo;

        public Vector3 vCreateBtnPos = new Vector3(-250, 85, 0);
        public Vector3 joyStickCreateBtnPos = new Vector3(-310, 55, 0);

		private bool m_isNotEnoughtIngot = false;

        IEnumerator Start()
        {
            if (GameDataManager.Instance.DataIsNull(DataType.InitializeEctype))
            {
                GameDataManager.Instance.dataEvent.RegisterEvent(DataType.InitializeEctype, Init);
            }
            else
            {
                Init(null);
            }
            yield return null;
            RegisterEventHandler();
            SetButtonStatus(null, medicine);
            SetMyButtons(null);
            GetColdWork();
        }

        void Init(object obj)
        {
            GameDataManager.Instance.dataEvent.RemoveEventHandel(DataType.InitializeEctype, Init);
            SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC = (SMSGEctypeInitialize_SC)GameDataManager.Instance.PeekData(DataType.InitializeEctype);
            EctypeContainerData ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[sMSGEctypeInitialize_SC.dwEctypeContainerId];
            if (ectypeData.MapType == 5)
            {
                gameObject.SetActive(false);
            }
            else
            {
                GameObject creatBtn = CreatObjectToNGUI.InstantiateObj(ButtonPrefab, BattleUIManager.Instance.BottomRight);
                creatBtn.transform.localPosition = GameManager.Instance.UseJoyStick ? joyStickCreateBtnPos : vCreateBtnPos;
                medicine = creatBtn.GetComponent<HealthAndMagicButton>();
            }
        }

        protected override void RegisterEventHandler()
        {
            //AddEventHandler(EventTypeEnum.ColdWork.ToString(),ResetBtnStatus);
			AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(),UpdateViaNotify);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.AddColdWork, AddColdWork);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.UseMedicamentResult, this.UseMedicamentResult);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.ResetContainerGoods, SetMyButtons);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.EctypeMedicamentPropUpdate, UpdateUseMedicamentHandle);
        }
	       
        void OnDestroy()
        {
            //RemoveEventHandler(EventTypeEnum.ColdWork.ToString(), ResetBtnStatus);
			RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(),UpdateViaNotify);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.AddColdWork, AddColdWork);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.UseMedicamentResult, this.UseMedicamentResult);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ResetContainerGoods, SetMyButtons);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EctypeMedicamentPropUpdate, UpdateUseMedicamentHandle);
        }


        public void SetMyButtons(object obj)
        {
            if (MedicineButtonItemFielInfo != null) { return; }
            MedicineButtonItemFielInfo = GetBeLinkedItem();
            if (MedicineButtonItemFielInfo != null)
            {
                SetButtonStatus(MedicineButtonItemFielInfo, medicine);
            }
            //TraceUtil.Log("设置药品按钮：" + MagicButtonItemFielInfo.LocalItemData._goodID);
        }

        void SetButtonStatus(UI.MainUI.ItemFielInfo ItemFileInfo, BattleButton Button)
        {
            if (Button == null) return;
            Button.RecoveSprite.fillAmount = 0;
//            if (ItemFileInfo == null)
//            {
//                Button.SetCallBackFuntion(null,null);
//                Button.SetButtonIcon(null);
//                Button.SetButtonText("");
//                Button.gameObject.SetActive(false);
//            }else
//            {
//                if (Button.gameObject.active == false)
//                {
//                    Button.gameObject.SetActive(true);
//                }
//                Button.SetCallBackFuntion(OnButtonClick, ItemFileInfo);
//                Button.SetButtonIcon(GameManager.Instance.UseJoyStick ? ItemFileInfo.LocalItemData.lDisplayIdRound : ItemFileInfo.LocalItemData._picPrefab);
//                //int ItemNumber = UI.MainUI.ContainerInfomanager.Instance.GetItemNumber(ItemFileInfo.LocalItemData._goodID);
//                int ItemNumber = ItemFileInfo.sSyncContainerGoods_SC.byNum;
//                if (ItemNumber > 99) { ItemNumber = 99; }
//                Button.SetButtonText(ItemNumber.ToString());
//            }

			//new 
			if (Button.gameObject.active == false)
			{
				Button.gameObject.SetActive(true);
			}
			Button.SetCallBackFuntion(OnButtonClick, ItemFileInfo);

			UpdateUseMedicamentHandle(null);
			int vipLevel = PlayerManager.Instance.FindHeroDataModel().GetCommonValue().PLAYER_FIELD_VISIBLE_VIP;
			SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC = (SMSGEctypeInitialize_SC)GameDataManager.Instance.PeekData(DataType.InitializeEctype);
			EctypeContainerData ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[sMSGEctypeInitialize_SC.dwEctypeContainerId];
			int goodsID = ectypeData.MedicamentIDs.SingleOrDefault(p=>p.VipLevel == vipLevel).GoodsID;
			var itemData = ItemDataManager.Instance.GetItemData(goodsID);
			Button.SetButtonIcon(GameManager.Instance.UseJoyStick ? itemData.lDisplayIdRound : itemData._picPrefab);
//			int ItemNumber = ectypeData.FreeMedicaments.SingleOrDefault(p=>p.VipLevel == vipLevel).Num - sMSGEctypeInitialize_SC.dwMedicamentTimes;
//			Button.SetButtonText(ItemNumber.ToString());
        }

        void OnButtonClick(object obj)
        {
            if(BattleManager.Instance.BlockPlayerToIdle)
            {
                return;
            }

			if(m_isNotEnoughtIngot)
			{
				MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I11_1"),1f);
				return;
			}


            OnUseritemFilelInfo = (UI.MainUI.ItemFielInfo)obj;
            //itemFilelInfo.UseButtonCallBack(null);
            //UseItem(OnUseritemFilelInfo);

			SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Medicament_Use");

			//new
			NetServiceManager.Instance.EctypeService.SendUseMedicament();
        }

		void UpdateUseMedicamentHandle(object obj)
		{
			int vipLevel = PlayerManager.Instance.FindHeroDataModel().GetCommonValue().PLAYER_FIELD_VISIBLE_VIP;

			EctypeContainerData ectypeData = EctypeManager.Instance.GetCurrentEctypeData();
			var ectypeInfo = EctypeManager.Instance.GetEctypeProps();
			int goodsID = ectypeData.MedicamentIDs.SingleOrDefault(p=>p.VipLevel == vipLevel).GoodsID;
			var itemData = ItemDataManager.Instance.GetItemData(goodsID);
			int ItemNumber = ectypeData.FreeMedicaments.SingleOrDefault(p=>p.VipLevel == vipLevel).Num - ectypeInfo.dwMedicamentTimes;

			if(ItemNumber>0)
			{
				medicine.SetButtonText(ItemNumber.ToString());
				medicine.ShowCopper(false,0);
			}
			else
			{
				var medicamentPrices = ectypeData.MedicamentPrice;
				int time = ItemNumber * -1 + 1;
				//(向下取整((参数1×〖付费使用次数〗^2+参数2×付费使用次数+参数3)/参数4)×参数4)
				int price = ((int)((medicamentPrices.Param1 * time * time + medicamentPrices.Param2 * time + medicamentPrices.Param3)/medicamentPrices.Param4) )*medicamentPrices.Param4;
				medicine.ShowCopper(true,price);

				m_isNotEnoughtIngot = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY < price;

				if(m_isNotEnoughtIngot)
				{
					medicine.Label_copper.color = Color.red;
				}
				else
				{
					medicine.Label_copper.color = new Color(0.6f,0.478f,0.075f);
				}
			}

		}

        void UseMedicamentResult(object obj)
        {
            TraceUtil.Log("收到药品使用请求");
            if (OnUseritemFilelInfo == null) return;
            var sMsgActionUseMedicamentResult_SC = (SMsgActionUseMedicamentResult_SC)obj;
            if (sMsgActionUseMedicamentResult_SC.byResult == 0) 
            { 
                return; 
            }
            if (medicine != null)
            {
                medicine.SetMyButtonActive(false);
            }
            this.OnUseritemFilelInfo = null;
        }

        void AddColdWork(object obj)
        {
            ColdWorkInfo myColdWork = (ColdWorkInfo)obj;
            if (myColdWork.lMasterID == PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity)
            {
				if (myColdWork .ColdClass== ColdWorkClass.ECold_ClassID_MODEL)
                {
                    ColdItem((int)myColdWork.ColdID, (int)myColdWork .ColdTime/ 1000);
                }
            }
        }

        void GetColdWork()
        {
            if (MedicineButtonItemFielInfo == null)
                return;
            long targetUID = PlayerManager.Instance.FindHeroDataModel().UID;
            ColdWorkInfo myColdWork = ColdWorkManager.Instance.GetColdWorkInfo(targetUID, ColdWorkClass.Goods, (uint)MedicineButtonItemFielInfo.LocalItemData._goodID);
            if (myColdWork != null)
            {
                //TweenFloat.Begin(myColdWork.ColdTime / 1000, 1, 0, SetRecoverProgressBar);
                ColdItem((int)myColdWork.ColdID, (int)myColdWork.ColdTime / 1000);
            }
        }

        //void ResetBtnStatus(INotifyArgs inotifyArgs)
        //{
        //    SmsgActionColdWork smsgActionColdWork = (SmsgActionColdWork)inotifyArgs;
        //    if (smsgActionColdWork.sMsgActionColdWorkHead_SC.lMasterID == PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity)
        //    {
        //        foreach (SMsgActionColdWork_SC child in smsgActionColdWork.sMsgActionColdWork_SCs)
        //        {
        //            //Debug.LogWarning("收到冷却消息：" + child.byClassID);
        //            if (child.byClassID == 1)
        //            {
        //                ColdItem((int)child.dwColdID,(int)child.dwColdTime/1000);
        //            }
        //        }
        //    }
        //}



        void ColdItem(int ItemID,int ColdTime)
        {
            //print("收到药品使用冷却应答");
			var buffID = EctypeManager.Instance.GetCurrentEctypeData().MedicamentBuffIDs.FirstOrDefault(
				p=>p.ColdID == ItemID);

			if(buffID != null)
			{
				medicine.SetMyButtonActive(false);
				medicine.RecoverMyself(ColdTime);
			}

//            if (MedicineButtonItemFielInfo!=null&&ItemID == MedicineButtonItemFielInfo.LocalItemData._goodID)
//            {
//                //int ItemNumber = UI.MainUI.ContainerInfomanager.Instance.GetItemNumber(MedicineButtonItemFielInfo.LocalItemData._goodID);
//                int ItemNumber = MedicineButtonItemFielInfo.sSyncContainerGoods_SC.byNum;
//				//int ItemNumber = 
//                if (ItemNumber > 99) { ItemNumber = 99; }
//                if (ItemNumber < 1||!UI.MainUI.ContainerInfomanager.Instance.itemFielArrayInfo.Contains(MedicineButtonItemFielInfo))
//                {
//                    SetButtonStatus(null,medicine);
//                    medicine = null;
//                    return;
//                }
//                medicine.SetMyButtonActive(false);
//                medicine.SetButtonText(ItemNumber.ToString());
//                medicine.RecoverMyself(ColdTime);
//            }


        }

        UI.MainUI.ItemFielInfo GetBeLinkedItem()
        {
            foreach (var item in UI.MainUI.ContainerInfomanager.Instance.sBuildContainerClientContexts)
            {
                if (item.dwContainerName == 3)
                {
                    var medicineItem = UI.MainUI.ContainerInfomanager.Instance.itemFielArrayInfo.FirstOrDefault(P => P.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID == item.dwContainerID);
                    return medicineItem;
                }
            }
            return null;
            //var medicineContainer = UI.MainUI.ContainerInfomanager.Instance.sBuildContainerClientContexts.FirstOrDefault(P => P.dwContainerName == 3);
            ////if (medicineContainer != null)
            //{
            //    var medicineItem = UI.MainUI.ContainerInfomanager.Instance.itemFielArrayInfo.FirstOrDefault(P => P.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID == medicineContainer.dwContainerID);
            //    return medicineItem;
            //}
            ////else
            ////{
            ////    return null;
            ////}
            ////return null;
        }

        void UseItem(UI.MainUI.ItemFielInfo itemFielInfo)//使用物品,目前暂定目标都为主角
        {   
            SMsgContainerUse_CS dataStruct = new SMsgContainerUse_CS();
            dataStruct.dwContainerID1 = dataStruct.dwContainerID2 = itemFielInfo.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID;
            dataStruct.byPlace = (byte)itemFielInfo.sSyncContainerGoods_SC.nPlace;
            dataStruct.uidTarget = PlayerManager.Instance.FindHeroEntityModel().EntityDataStruct.SMsg_Header.uidEntity;
            NetServiceManager.Instance.ContainerService.SendContainerUse(dataStruct);
            //print("使用药品:" + dataStruct.dwContainerID1 + "," + LanguageTextManager.GetString(itemFielInfo.LocalItemData._szGoodsName));
        }

		void UpdateViaNotify(INotifyArgs inotifyArgs)//设置各种属性
		{
			EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)inotifyArgs;
			if (entityDataUpdateNotify.IsHero && entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_PLAYER)
			{
				UpdateButtonStatus();
			}
		}
		private void UpdateButtonStatus()
		{
			int Hp = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURHP;
			if(Hp > 0)
			{
				if(!medicine.Active)
				{
					medicine.SetAllSpriteAlpha(1);
					SetMyButtonsColliderActive(true);
				}
			}
			else
			{
				if(medicine.Active)
				{
					medicine.SetAllSpriteAlpha(0.5f);
					SetMyButtonsColliderActive(false);
				}
			}
		}

        public void SetMyButtonsColliderActive(bool flag)
        {
            if (medicine != null)
                medicine.Active = flag;
        }


    }
}
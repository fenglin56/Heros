using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{

    public class FashionInfoTips_V3 : MonoBehaviour
    {


        public SpriteSwith InfoTitle;
        public UILabel InfoMainLabel;
        public Transform CreatPayIConPoint;
		public UILabel DesLabel;
        public SingleButtonCallBack EquipBtn;


        public FashionPanel_V3 MyParent { get; private set; }
        public ItemData CurrentFashionData {get;private set; }
        public ItemFielInfo CurrentUnlockData { get; private set; }
        public bool CurrentFashionIsLock { get; private set; }
        public bool IsEquipFashion { get; private set; }
        public ShopConfigData CurrentFashionShopData { get; private set; }

        private int m_guideBtnID;

        void Awake()
        {
            EquipBtn.SetCallBackFuntion(OnEquipBtnClick);
            //TODO GuideBtnManager.Instance.RegGuideButton(EquipBtn.gameObject, UIType.Fashion, SubType.FashionEquipBtn, out m_guideBtnID);

            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            EquipBtn.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_FashionPanel_V3_FashionInfoTips_V3_EquipBtn);
        }

        void OnDestroy()
        {
            //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
        }

        public void ShowFashionInfo(ItemData fashionData, FashionPanel_V3 myParent)
        {
            this.MyParent = myParent;
            CurrentFashionData = fashionData;
            CurrentUnlockData = myParent.GetUnlockData(fashionData);
			CurrentFashionIsLock =fashionData != myParent.GetAllFashionDatas()[0]&&CurrentUnlockData == null;
            int equiptFashionID = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION;
			IsEquipFashion = fashionData._goodID == equiptFashionID||(equiptFashionID==0&&fashionData == myParent.GetAllFashionDatas()[0]);
            CurrentFashionShopData = ShopDataManager.Instance.shopConfigDataBase._dataTable.FirstOrDefault(P=>P.GoodsID == fashionData._goodID);
            CreatPayIConPoint.ClearChild();
			DesLabel.SetText(LanguageTextManager.GetString(fashionData._szDesc));
            if (!CurrentFashionIsLock)
            {
                if (!IsEquipFashion)//未穿上的装备
                {
                    //EquipBtn.SetButtonText(LanguageTextManager.GetString("IDS_H2_69"));//穿上
					EquipBtn.spriteSwithList.ApplyAllItem(C=>C.ChangeSprite(1));
					//SetBtnActive(EquipBtn,true);
                }
                else
                {
                    //EquipBtn.SetButtonText(LanguageTextManager.GetString("IDS_H2_69"));//卸下
					EquipBtn.spriteSwithList.ApplyAllItem(C=>C.ChangeSprite(2));
					//SetBtnActive(EquipBtn,false);
                }
//                InfoTitle.ChangeSprite(1);//小提示
//                EquipBtn.spriteSwith.transform.localPosition = Vector3.zero;
//                if (GetLeftTime() > -1)//显示倒计时提示
//                {
//                    CancelInvoke();
//                    InvokeRepeating("ShowItemLeftTime", 0, 1);
//                }
//                else//显示永久时装提示
//                {
//                    InfoMainLabel.SetText(LanguageTextManager.GetString("IDS_H1_365"));
//                }
            }
            else//未购买
            {
//				InfoTitle.ChangeSprite(2);//购买条件
				SetBtnActive(EquipBtn,true);
				EquipBtn.spriteSwithList.ApplyAllItem(C=>C.ChangeSprite(3));//充值按钮
//                string AllowLevelStr = string.Format(LanguageTextManager.GetString("IDS_H1_361"), fashionData._AllowLevel);
//                string BuyFashionLevelStr = CurrentFashionShopData.BuyLvl > 0 ? string.Format(LanguageTextManager.GetString("IDS_H1_362"), CurrentFashionShopData.BuyLvl) : "";
//                InfoMainLabel.SetText(string.Format("{0}  {1}", AllowLevelStr, BuyFashionLevelStr));
//                SetCostLabel();
            }
        }

		void SetBtnActive(SingleButtonCallBack btn,bool flag)
		{
			btn.SetImageButtonComponentActive(flag);
			btn.SetButtonColliderActive(flag);
			Color enableColor = new Color(1,1,1,1);
			Color disableColor = new Color(1,1,1,0.3f);
			btn.spriteSwith.target.color = flag?enableColor:disableColor;
		}

        /// <summary>
        /// 装备按钮点击
        /// </summary>
        /// <param name="obj"></param>
        void OnEquipBtnClick(object obj)
        {
            if (CurrentFashionIsLock)
            {
//                if (CheckBuyFashion())
//                {
					SoundManager.Instance.PlaySoundEffect("Sound_Button_Avatar_Click");

					MainUIController.Instance.OpenMainUI(UIType.TopUp);
//                    BuyFashion();
//                }
            }
            else
            {
                if (IsEquipFashion)
                {
                    //UnloadFashion();
                }
                else
				{
					SoundManager.Instance.PlaySoundEffect("Sound_Button_Avatar_Equip");
                    MessageBox.Instance.ShowTips(1,LanguageTextManager.GetString("IDS_I8_3"),1);
                    EquipFashion();
                }
            }
            //MyParent.OnEquipBtnClick();
        }

        /// <summary>
        /// 发送装备时装请求
        /// </summary>
        void EquipFashion()
        {
            SMsgContainerUse_CS dataStruct = new SMsgContainerUse_CS();
            dataStruct.dwContainerID1 = dataStruct.dwContainerID2 =CurrentUnlockData.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID;
            dataStruct.byPlace = (byte)CurrentUnlockData.sSyncContainerGoods_SC.nPlace;
            dataStruct.uidTarget = PlayerManager.Instance.FindHeroEntityModel().EntityDataStruct.SMsg_Header.uidEntity;
            dataStruct.byUseType = 0;
            NetServiceManager.Instance.ContainerService.SendContainerUse(dataStruct);
            TraceUtil.Log("发送装备时装信息");
        }
        /// <summary>
        /// 显示人物模型
        /// </summary>
        void ShowHeroModel()
        {
            //MyParent.fashionHeroViewCam.SetHeroModelActive(true);
        }

        void BuyFashion()
        {
            SMsgTradeBuyGoods_CS sMsgTradeBuyGoods_CS = new SMsgTradeBuyGoods_CS()
            {
                dwShopID = (uint)CurrentFashionShopData._shopID,
                uidNPC = 0,
                lShopGoodsID = (uint)CurrentFashionShopData._shopGoodsID,
                GoodsID = (uint)CurrentFashionShopData.GoodsID,
                GoodsNum = 1,
            };
            NetServiceManager.Instance.TradeService.SendTradeBuyGoods(sMsgTradeBuyGoods_CS);
            TraceUtil.Log("发送购买时装信息");
        }
		
		/// <summary>
		/// 检测是否达到条件购买
		/// </summary>
		/// <returns></returns>
		//        bool CheckBuyFashion()
		//        {
		//            switch (CurrentFashionShopData.BuyType)
		//            {
		//                case 3:
		//                    if (PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY < CurrentFashionShopData.Price)
		//                    {
		//                        //MyParent.fashionHeroViewCam.SetHeroModelActive(false);
		//                        //MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H2_44"), LanguageTextManager.GetString("IDS_H2_55"), ShowHeroModel);//元宝不足
		//                        MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_44"),1);//元宝不足
		//                        return false;
		//                    }
		//                    break;
		//                case 4:
		//                    int GoodsID = int.Parse(CurrentFashionShopData.ExChangeGoodID.Split('+')[0]);
		//                    int GoodsNumber = int.Parse(CurrentFashionShopData.ExChangeGoodID.Split('+')[1]);
		//                    var GoodsItemList = ContainerInfomanager.Instance.itemFielArrayInfo.FindAll(P => P.LocalItemData._goodID == GoodsID);
		//                    int m_GoodsNumber = 0;
		//                    foreach (var child in GoodsItemList)
		//                    {
		//                        m_GoodsNumber += child.sSyncContainerGoods_SC.byNum;
		//                    }
		//                    if (m_GoodsNumber < GoodsNumber)
		//                    {
		//                        //MyParent.fashionHeroViewCam.SetHeroModelActive(false);
		//                        //MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H2_3"), LanguageTextManager.GetString("IDS_H2_55"), ShowHeroModel);//材料不足
		//                        MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_3"), 1);//材料不足
		//                        return false;
		//                    }
		//                    break;
		//            }
		//
		//            int CurrentLevel = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
		//            if (CurrentLevel <CurrentFashionData._AllowLevel)
		//            {
		//                //MyParent.fashionHeroViewCam.SetHeroModelActive(false);
		//                //MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H2_6"), LanguageTextManager.GetString("IDS_H2_55"), ShowHeroModel);//等级不够
		//                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_6"), 1);//等级不够
		//                return false;
		//            }
		//            int NeedFashionLevel = CurrentFashionShopData.BuyLvl;
		//            if (NeedFashionLevel != 0 && MyParent.OwnFashionList.FirstOrDefault(P => P.LocalItemData._Level == NeedFashionLevel) == null)
		//            {
		//                //MyParent.fashionHeroViewCam.SetHeroModelActive(false);
		//                //MessageBox.Instance.Show(3, "", string.Format(LanguageTextManager.GetString("IDS_H1_363"), NeedFashionLevel), LanguageTextManager.GetString("IDS_H2_55"), ShowHeroModel);//需购买前置等级时装
		//                MessageBox.Instance.ShowTips(3,string.Format(LanguageTextManager.GetString("IDS_H1_363"), NeedFashionLevel),1);//需购买前置等级时装
		//                return false;
		//            }
		//            return true;
		//        }
		
		/// <summary>
		/// 设置显示花费物品的label
		/// </summary>
		//        void SetCostLabel()
		//        {
		//            int ItemID = 0;
		//            int Price = 0;
		//            switch (CurrentFashionShopData.BuyType)
		//            {
		//                case 1:
		//                    ItemID = 3050001;
		//                    Price = CurrentFashionShopData.Price;
		//                    break;
		//                case 3:
		//                    ItemID = 3050002;
		//                    Price = CurrentFashionShopData.Price;
		//                    break;
		//                case 4:
		//                    string[] str = CurrentFashionShopData.ExChangeGoodID.Split('+');
		//                    ItemID = int.Parse(str[0]);
		//                    Price = int.Parse(str[1]);
		//                    break;
		//                default:
		//                    break;
		//            }
		//            CreatObjectToNGUI.InstantiateObj(ItemDataManager.Instance.GetItemData(ItemID)._picPrefab, CreatPayIConPoint);
		//            PayNumberLabel.SetText(Price);
		//        }

		//        void ShowItemLeftTime()
		//        {
		//            InfoMainLabel.SetText(GetShowFashionTimeString());
		//        }
		
		/// <summary>
		/// 获取装备有效时间
		/// </summary>
		/// <returns></returns>
		//        int GetLeftTime()
		//        {
		//            int LeftTime = 0;
		//            if (CurrentUnlockData.equipmentEntity.ITEM_FIELD_VISIBLE_LEFTTIME > -1)
		//            {
		//                long uid = PlayerManager.Instance.FindHeroDataModel().UID;
		//                ColdWorkInfo coldWorkInfo = ColdWorkManager.Instance.GetColdWorkInfo(uid, ColdWorkClass.Goods, (uint)CurrentUnlockData.LocalItemData._goodID);
		//                if (coldWorkInfo == null)
		//                {
		//                    ColdWorkManager.Instance.AddColdWorkInfo(uid, ColdWorkClass.Goods, (uint)CurrentUnlockData.LocalItemData._goodID, (uint)CurrentUnlockData.equipmentEntity.ITEM_FIELD_VISIBLE_LEFTTIME * 1000);
		//                }
		//                else
		//                {
		//                    LeftTime = (int)coldWorkInfo.ColdTime / 1000;
		//                }
		//            }
		//            else
		//            {
		//                LeftTime = CurrentUnlockData.equipmentEntity.ITEM_FIELD_VISIBLE_LEFTTIME;
		//            }
		//            TraceUtil.Log("获取时装倒计时：" + LeftTime);
		//            return LeftTime;
		//        }
		/// <summary>
		/// 获取显示时装倒计时的文字
		/// </summary>
		/// <returns></returns>
		//        string GetShowFashionTimeString()
		//        {
		//            string LeftString = "";
		//            int LeftTime = GetLeftTime();
		//            int LeftSecond = LeftTime;
		//            int LeftMinute = LeftTime / 60;
		//            int LeftHour = LeftMinute / 60;
		//            int LeftDay = LeftHour / 24;
		//            if (LeftDay > 0)
		//            {
		//                LeftString = string.Format(LanguageTextManager.GetString("IDS_H1_366"), LeftDay);
		//            }
		//            else if (LeftHour > 0)
		//            {
		//                LeftString = string.Format(LanguageTextManager.GetString("IDS_H1_367"), LeftHour);
		//            }
		//            else if (LeftMinute > 0)
		//            {
		//                LeftString = string.Format(LanguageTextManager.GetString("IDS_H1_368"), LeftMinute);
		//            }
		//            else if (LeftSecond > 0)
		//            {
		//                LeftString = string.Format(LanguageTextManager.GetString("IDS_H1_369"), LeftSecond);
		//            }
		//            else
		//            {
		//                LeftString = string.Format(LanguageTextManager.GetString("IDS_H1_370"));
		//            }
		//            return LeftString;
		//        }

		/// <summary>
		/// 发送卸下时装请求
		/// </summary>
		//        void UnloadFashion()
		//        {
		//            TraceUtil.Log("卸下时装");
		//            SMsgContainerUse_CS dataStruct = new SMsgContainerUse_CS();
		//            dataStruct.dwContainerID1 = dataStruct.dwContainerID2 = CurrentUnlockData.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID;
		//            dataStruct.byPlace = (byte)CurrentUnlockData.sSyncContainerGoods_SC.nPlace;
		//            dataStruct.uidTarget = PlayerManager.Instance.FindHeroEntityModel().EntityDataStruct.SMsg_Header.uidEntity;
		//            dataStruct.byUseType = 1;
		//            NetServiceManager.Instance.ContainerService.SendContainerUse(dataStruct);
		//        }
    }
}
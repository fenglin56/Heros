using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UI.MainUI
{

    public enum EquiptSlotType 
    {
        Null = -1,
        Weapon = 0,//武器
        Heard = 11,//头饰
        Body = 12,//衣服
        Shoes = 13,//鞋子
        Accessories = 14,//饰品
        Medicine = 100,//药品
    }

    public enum EquiptType
    {
      //  Null=-1,
        Weapon=1,//,武器
        fashion,//时装、
        Heard,//头饰、
        Clothes,//=衣服、
        Shoes,//靴子、
        Accessories,//=饰品、7=徽章（称号）
    }

    //包裹系统包括以下子系统：人物装备系统（装备在人物身上的物品） + 背包（未装备的物品）
	//人物装备系统： dwContainerName = 1,  dwContainerID = UniqueID,		szContainerName = "装备"
	// 背包：dwContainerName = {2， 3， 4},	dwContainerID = UniqueID,	  szContainerName = "包裹"
    public class ContainerInfomanager : Controller, IEntityManager, ISingletonLifeCycle
    {
        Dictionary<EquiptSlotType,EquiptType> EqPlaceToEqTypeDic=new Dictionary<EquiptSlotType, EquiptType>()
        {
            {EquiptSlotType.Weapon,EquiptType.Weapon},
            {EquiptSlotType.Heard,EquiptType.Heard},
            {EquiptSlotType.Body,EquiptType.Clothes},
            {EquiptSlotType.Shoes,EquiptType.Shoes},
            {EquiptSlotType.Accessories,EquiptType.Accessories},
        };
        public List<SBuildContainerClientContext> sBuildContainerClientContexts { get; private set; }//创建背包栏信息
        //public List<SMsgPropCreateEntity_SC_Container> GoodsInfoArray { get; private set; }//背包物体实体信息
        public List<ItemFielInfo> itemFielArrayInfo { get; private set; }//人物所有物品
        public List<SSyncContainerGoods_SC> sSyncContainerGoods_SCs { get; private set; }//背包栏物品同步信息
        public List<SSyncContainerGoods_SC> sSyncHeroContainerGoods_SCs { get; private set; }//人物装备栏物品同步信息
        public List<SSyncContainerGoods_SC> CurrentsSyncContainerGoods { get; private set; }//当前所有的背包栏物品同步信息
        public EquiptSlotType[] Places=new EquiptSlotType[5]{EquiptSlotType.Weapon,EquiptSlotType.Heard,EquiptSlotType.Body,EquiptSlotType.Shoes,EquiptSlotType.Accessories};
        //public event ButtonCallBack SyncContainerGoodsEvent;

        private static ContainerInfomanager m_instance;

        public static ContainerInfomanager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new ContainerInfomanager();
                    SingletonManager.Instance.Add(m_instance);
                    EntityController.Instance.RegisteManager(TypeID.TYPEID_ITEM, m_instance);

                }
                return m_instance;
            }
        }


        private ContainerInfomanager()
        {
            this.sBuildContainerClientContexts = new List<SBuildContainerClientContext>();
            this.itemFielArrayInfo = new List<ItemFielInfo>();
            sSyncContainerGoods_SCs = new List<SSyncContainerGoods_SC>();
            CurrentsSyncContainerGoods = new List<SSyncContainerGoods_SC>();
            sSyncHeroContainerGoods_SCs = new List<SSyncContainerGoods_SC>();
        }
             


        public void UseGiftBox(ItemFielInfo itemfielInfo)
        {
            //发送开启礼箱
            SMsgContainerUse_CS dataStruct = new SMsgContainerUse_CS();
            dataStruct.dwContainerID1 = dataStruct.dwContainerID2 = itemfielInfo.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID;
            dataStruct.byPlace = (byte)itemfielInfo.sSyncContainerGoods_SC.nPlace;
            dataStruct.uidTarget = PlayerManager.Instance.FindHeroEntityModel().EntityDataStruct.SMsg_Header.uidEntity;
            NetServiceManager.Instance.ContainerService.SendContainerUse(dataStruct);

        }
        public void UseUsableGoods(int goodsID,bool UseAll)
        {

            List<ItemFielInfo> items= GetPackItemList().Where(p=>p.LocalItemData._goodID==goodsID).ToList();
            if(items!=null&&items.Count>0)
            {

                SMsgContainerUse_CS dataStruct = new SMsgContainerUse_CS();
                items.Sort((x,y)=>x.sSyncContainerGoods_SC.byNum-y.sSyncContainerGoods_SC.byNum);
                if(!UseAll)
                {   

                    dataStruct.dwContainerID1 = dataStruct.dwContainerID2 = items[0].sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID;
                    dataStruct.byPlace = (byte)items[0].sSyncContainerGoods_SC.nPlace;
                    dataStruct.uidTarget = PlayerManager.Instance.FindHeroEntityModel().EntityDataStruct.SMsg_Header.uidEntity;
                    NetServiceManager.Instance.ContainerService.SendContainerUse(dataStruct);
                }
                else
                {
                    foreach(var item in items)
                    {
                        //SMsgContainerUse_CS dataStruct = new SMsgContainerUse_CS();
                        dataStruct.dwContainerID1 = dataStruct.dwContainerID2 = item.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID;
                        dataStruct.byPlace = (byte)item.sSyncContainerGoods_SC.nPlace;
                        dataStruct.uidTarget = PlayerManager.Instance.FindHeroEntityModel().EntityDataStruct.SMsg_Header.uidEntity;
                        dataStruct.byGoodsNum=item.sSyncContainerGoods_SC.byNum;
                        NetServiceManager.Instance.ContainerService.SendContainerUse(dataStruct);
                    }
                }
            }

        }
        /// <summary>
        /// 获取玩家当前使用的武器
        /// </summary>
        /// <returns></returns>
        public string GetCurrentWeapon()
        {
            string weapon = "";
            var WeaponInfo = GetSSyncContainerGoods_SCList(1).SingleOrDefault(P => P.nPlace == 0);
            if (WeaponInfo.uidGoods != 0)
            {
                    weapon = GetContainerGoodsInfo(WeaponInfo).LocalItemData._ModelId;
            }
            else 
            {
                byte kind = (byte)PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
                bool inTown = GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN;
                var configData = inTown ?
                    PlayerDataManager.Instance.GetTownItemData(kind)
                    : PlayerDataManager.Instance.GetBattleItemData(kind);
                weapon = configData.DefaultWeapon;
            }
            return weapon;
        }

        public ItemFielInfo GetCurrentWeaponItemInfo()
        {
            return GetEquiptItemList().SingleOrDefault(c=>c.sSyncContainerGoods_SC.nPlace==0);
        }





    

		/// <summary>
		/// 获取药品装备栏物品
		/// </summary>
		/// <returns>The medicine item file info.</returns>
		public ItemFielInfo GetMedicineItemFileInfo()
		{
			var currentPack = ContainerInfomanager.Instance.sBuildContainerClientContexts.FirstOrDefault(P=>P.dwContainerName == 3);
			if (currentPack.SMsgActionSCHead.uidEntity == 0)
				return null;
			var beLinkItem = ContainerInfomanager.Instance.itemFielArrayInfo.FirstOrDefault(P => P.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID == currentPack.dwContainerID);
			return beLinkItem;
		}

		public ItemFielInfo GetItemFileInfoBuyUID(long UID)
		{
			return itemFielArrayInfo.FirstOrDefault(C=>C.sSyncContainerGoods_SC.uidGoods == UID);
		}
        /// <summary>
        /// 获取已拥有的材料的数量
        /// </summary>
        /// <returns>The own material count.</returns>
        /// <param name="materialID">Material I.</param>
        public int GetOwnMaterialCount(int materialID)
        {
            List<ItemFielInfo> items= GetPackItemList().Where(p=>p.LocalItemData._goodID==materialID).ToList();
            int count=0;
            foreach(var item in items )
            {
                count+= item.sSyncContainerGoods_SC.byNum;
                
            }
            return count;
        }

        /// <summary>
        /// 获取身上的装备
        /// </summary>
        /// <returns></returns>
        public List<ItemFielInfo> GetEquiptItemList()
        {
            List<ItemFielInfo> getItemInfo = new List<ItemFielInfo>();
            var equiplist = GetSSyncContainerGoods_SCList(1);
            foreach (var child in equiplist)
            {
                ItemFielInfo getItem = itemFielArrayInfo.FirstOrDefault(P => P.sSyncContainerGoods_SC.uidGoods == child.uidGoods&&P.LocalItemData._GoodsSubClass!=2&&P.LocalItemData._GoodsSubClass!=7);
                if (getItem != null)
                {
                    getItemInfo.Add(getItem);
                }
            }
            //TraceUtil.Log("获取装备物品");
            //getItemInfo.ApplyAllItem(P => TraceUtil.Log(string.Format("装备物品：{0}", LanguageTextManager.GetString(P.LocalItemData._szGoodsName))));
            return getItemInfo;
        }
        /// <summary>
        /// 获取所有的装备（背包和已穿戴）
        /// </summary>
        /// <returns>The all equipment.</returns>
        public List<ItemFielInfo> GetAllEquipment()
        {
            List<ItemFielInfo> items=new List<ItemFielInfo>();
             foreach(var item in itemFielArrayInfo)
            {
                if(item.LocalItemData._GoodsClass==1&&item.LocalItemData._GoodsSubClass!=2&&item.LocalItemData._GoodsSubClass!=7)
                {
                    items.Add(item);
                }
            }
            return items;
        }
		public bool IsItemEquipped(EquiptType type)
		{
			bool isEquipt = this.GetEquiptItemList().SingleOrDefault(C=>C.sSyncContainerGoods_SC.nPlace==(int)type)!=null;
			return isEquipt;
		}
        public bool IsItemEquipped(ItemFielInfo itemFileInto)
        {
            bool isEquipt = this.GetEquiptItemList().SingleOrDefault(C=>C == itemFileInto)!=null;
            return isEquipt;
        }
        public bool IsItemEquipped(int goodsID)
        {
            bool isEquipt = this.GetEquiptItemList().SingleOrDefault(C=>C.LocalItemData._goodID == goodsID)!=null;
            return isEquipt;
        }
		/// <summary>
		/// 获取背包栏内的物品
		/// </summary>
		/// <returns>The pack item list.</returns>
		public List<ItemFielInfo> GetPackItemList()
		{
			var containerPackInco = ContainerInfomanager.Instance.sBuildContainerClientContexts.First(P => P.dwContainerName == 2);
			List<ItemFielInfo> currentContainerInfoList = ContainerInfomanager.Instance.itemFielArrayInfo.FindAll(//当前页面的物品列表
			P => (P.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID == containerPackInco.dwContainerID));
			return currentContainerInfoList;
		}
		/// <summary>
		/// 背包是否已满
		/// </summary>
		public bool PackIsFull()
		{
			return GetPackItemList().Count==GetAllPackMaxNum();
		}

		/// <summary>
		/// 获取背包格总个数
		/// </summary>
		/// <returns>The all pack max number.</returns>
		public int GetAllPackMaxNum()
		{
			return GetContainerClientContsext(2).wMaxSize;
		}

        void ResetContainerInfo(object obj)
        {
            TraceUtil.Log("清空物品栏信息");
            this.sBuildContainerClientContexts.Clear();
            this.itemFielArrayInfo.Clear();
            sSyncContainerGoods_SCs.Clear();
            CurrentsSyncContainerGoods.Clear();
            sSyncHeroContainerGoods_SCs.Clear();
        }

        /// <summary>
        /// 是否有空的背包格
        /// </summary>
        /// <returns></returns>
        public bool IsHaveEmptyContainerBox()
        {
            var ContainerSize = GetContainerClientContsext(2);
            int ItemCount = sSyncContainerGoods_SCs.FindAll((SSyncContainerGoods_SC P) => { return P.uidGoods > 0; }).Count;
            return(ItemCount<ContainerSize.wMaxSize);
        }
        /// <summary>
        /// 获取占用的背包格个数
        /// </summary>
        /// <returns></returns>
        public int GetExistPackBoxNumber()
        {
            int itemCount = 0;
            itemCount = sSyncContainerGoods_SCs.FindAll((SSyncContainerGoods_SC P) => { return P.uidGoods > 0; }).Count;
            return itemCount;
        }

        /// <summary>
        /// 获取空的背包格个数
        /// </summary>
        /// <returns></returns>
        public int GetEmptyPackBoxNumber()
        {
            int itemCount = 0;
            itemCount = this.GetExistPackBoxNumber();
            var ContainerSize = GetContainerClientContsext(2);
            return ContainerSize.wMaxSize - itemCount;
        }

        public void CreatContainerClientContext(SBuildContainerClientContext sBuildContainerClientContext)//创建背包栏信息
        {
            this.sBuildContainerClientContexts.Add(sBuildContainerClientContext);
            //TraceUtil.Log("创建背包栏：sBuildContainerClientContext.dwContainerID : " + sBuildContainerClientContext.dwContainerID);
        }

        public SBuildContainerClientContext GetContainerClientContsext(int DwContainerName)//获取创建的背包栏信息,1是人物装备，2是包裹	
        {
            foreach (SBuildContainerClientContext child in sBuildContainerClientContexts)
            {
                if (child.dwContainerName == DwContainerName)
                {
                    return child;
                }
            }
            return new SBuildContainerClientContext();
        }

        public void UnlockPackBox(SMsgContainerChangeSize_SC smsgContainerChangeSize)//解锁背包格
        {
            for (int i = 0; i < sBuildContainerClientContexts.Count; i++)
            {
				// 判断是否是包裹
                if (sBuildContainerClientContexts[i].dwContainerName == 2)
                {
                    var ChangeData = sBuildContainerClientContexts[i];
                    ChangeData.SetMaxSize(smsgContainerChangeSize.vMaxSize);

                    sBuildContainerClientContexts.RemoveAt(i);
                    sBuildContainerClientContexts.Add(ChangeData);
                    break;
                }
            }
            UIEventManager.Instance.TriggerUIEvent(UIEventType.ResetContainerPack,null);
			ShowPackIsfullEff();
        }

        public void AddGoodsInfo(SMsgPropCreateEntity_SC_Container goodsInfo)//物品实体信息
        {
            //TraceUtil.Log("收到创建物品实体消息："+goodsInfo.ItemTemplateID+","+goodsInfo.ItemtypeID);
            foreach (ItemFielInfo child in itemFielArrayInfo)
            {
                if (child.GoodsInfoModel.EntityDataStruct.SMsg_Header.uidEntity == goodsInfo.SMsg_Header.uidEntity)
                {
                    child.GoodsInfoModel.EntityDataStruct = goodsInfo;
                    child.ResetEntityModel(goodsInfo);
                    //TraceUtil.Log("刷新物品实体");
                    return;
                }
            }
            var itemFileInfo = new ItemFielInfo(goodsInfo);
            itemFielArrayInfo.Add(itemFileInfo);

            //TraceUtil.Log("增加物品实体:"+LanguageTextManager.GetString(itemFileInfo.LocalItemData._szGoodsName));
			//TraceUtil.Log("物品所属包： " + itemFileInfo.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID);
			//TraceUtil.Log("物品位置：" +  itemFileInfo.sSyncContainerGoods_SC.nPlace);
        }



        public void SetContainerGoodsPosition(List<SSyncContainerGoods_SC> sSyncContainerGoods_SCs)//同步物品栏信息
        {
            //TraceUtil.Log("背包栏Count:" + this.sBuildContainerClientContexts.Count);
            //TraceUtil.Log("物品栏信息Count:" + sSyncContainerGoods_SCs.Count);
            bool EquipmentContainerChange=false;
            bool PackgeContainerChange=false;
            foreach (SBuildContainerClientContext buildContainerInfo in this.sBuildContainerClientContexts)//创建的背包栏
            {
                foreach (SSyncContainerGoods_SC child in sSyncContainerGoods_SCs)
                {
                    if (child.SMsgContainerCSCHead.dwContainerID == buildContainerInfo.dwContainerID)//判断容器ID
                    {
                        //TraceUtil.Log("物品栏信息:" + buildContainerInfo.dwContainerName);
                        switch (buildContainerInfo.dwContainerName)
                        {
                            case 1: //人物装备栏
                                this.sSyncHeroContainerGoods_SCs.RemoveAll(P => P.nPlace == child.nPlace);                               
                                this.sSyncHeroContainerGoods_SCs.Add(child);
                                break;
                            case 2://包裹栏
                                PackgeContainerChange=true;
                                this.sSyncContainerGoods_SCs.RemoveAll(P => P.nPlace == child.nPlace); 
                                this.sSyncContainerGoods_SCs.Add(child);
                           
                                break;
                            case 3://药品栏
                                break;
                            default:
                                TraceUtil.Log("收到创建其他类型物品栏信息！");
                                break;
                        }
                        //TraceUtil.Log("更新处理完成" );
                        AddGoodsToCurrentContainer(child);
                        AddSSyncContainerGoodsToItemfielInfo(child);
                        //TraceUtil.Log("第二层更新处理完成");
                    }
                }
            }
            CheckShowEquipmentEff();
            ShowJewelEff();
			UIEventManager.Instance.TriggerUIEvent(UIEventType.ResetContainerGoods,sSyncContainerGoods_SCs);
            ShowPackIsfullEff();
        }


        public void AddSSyncContainerGoodsToItemfielInfo(SSyncContainerGoods_SC sSyncContainerGoods_SC)//将物品栏同步信息和实体关联起来
        {
            foreach (ItemFielInfo child in itemFielArrayInfo)
            {
                if (child.GoodsInfoModel.EntityDataStruct.SMsg_Header.uidEntity == sSyncContainerGoods_SC.uidGoods)
                {
                    child.AddsSyncContainerGoods_SC(sSyncContainerGoods_SC);
                }
            }
        }

        void AddGoodsToCurrentContainer(SSyncContainerGoods_SC GoodsInfo)
        {
            CurrentsSyncContainerGoods.RemoveAll(P => P.nPlace == GoodsInfo.nPlace&&P.SMsgContainerCSCHead.dwContainerID == GoodsInfo.SMsgContainerCSCHead.dwContainerID);  
           
            CurrentsSyncContainerGoods.Add(GoodsInfo);
            //TraceUtil.Log("AddGoodsToContainer:"+GoodsInfo.nPlace+",GoodsID:"+GoodsInfo.uidGoods);
            //TraceUtil.Log("Container.Count:"+CurrentsSyncContainerGoods.Count);
			if(SirenManager.Instance.IsHasSirenSatisfyIncrease())
			{
				UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayMainBtnAnim, UIType.Siren);
			}	
        }

        public List<SSyncContainerGoods_SC> GetSSyncContainerGoods_SCList(int dwContainerName)//获取同步物品栏列表
        {
            switch (dwContainerName)
            {
                case 1://人物装备
                    return sSyncHeroContainerGoods_SCs;
                case 2://包裹
                    return sSyncContainerGoods_SCs;
                default:
                    return null;
            }
        }
        /// <summary>
        /// 判断某个背包格是否有东西
        /// </summary>
        /// <param name="Place">背包位置</param>
        /// <returns></returns>
        public bool ContainerGoodsIsFull(int Place)
        {
            bool flag = this.sSyncContainerGoods_SCs.FirstOrDefault(P=>P.nPlace == Place).uidGoods != 0;
            return flag;
        }

        public ItemFielInfo GetContainerGoodsInfo(SSyncContainerGoods_SC sSyncContainerGoods_SC)//获取某个物品的实体信息
        {
            foreach (ItemFielInfo child in itemFielArrayInfo)
            {
                if (child.GoodsInfoModel.EntityDataStruct.SMsg_Header.uidEntity == sSyncContainerGoods_SC.uidGoods)
                {
                    child.AddsSyncContainerGoods_SC(sSyncContainerGoods_SC);
                    return child;
                }
            }
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"找不到对应物品的实体信息："+sSyncContainerGoods_SC.nPlace+",ID:"+sSyncContainerGoods_SC.uidGoods);
            return null;
        }

        public int GetItemNumber(int ItemID)//获取某个物品数量
        {
            int itemNumber = 0;
            foreach (ItemFielInfo child in itemFielArrayInfo)
            {
                if (child.LocalItemData._goodID == ItemID)
                {
                    itemNumber += child.sSyncContainerGoods_SC.byNum;
                }
            }
            return itemNumber;
        }

        public EntityModel GetEntityMode(long uid)
        {
            EntityModel item;
            for (int i = 0; i < this.itemFielArrayInfo.Count; i++)
            {
                switch (this.itemFielArrayInfo[i].severItemFielType)
                {
                    case SeverItemFielInfoType.Equid:
					case SeverItemFielInfoType.Title:
                        item = this.itemFielArrayInfo[i].EquipmentEntityModel;
                        if (item != null && item.EntityDataStruct.SMsg_Header.uidEntity == uid)
                        {
                            return item;
                        }
                        break;
                    case SeverItemFielInfoType.Materiel:
                        item = this.itemFielArrayInfo[i].MaterielModel;
                        if (item != null && item.EntityDataStruct.SMsg_Header.uidEntity == uid)
                        {
                            return item;
                        }
                        break;
                    case SeverItemFielInfoType.Medicament:
                        item = this.itemFielArrayInfo[i].MedicamentEntityModel;
                        if (item != null && item.EntityDataStruct.SMsg_Header.uidEntity == uid)
                        {
                            return item;
                        }

                        break;
                    case SeverItemFielInfoType.Jewel:
                        item = this.itemFielArrayInfo[i].MaterielModel;
                        if (item != null && item.EntityDataStruct.SMsg_Header.uidEntity == uid)
                        {
                            return item;
                        }
                        
                        break;
                }
            }

            return null;
        }

        public void RegisteEntity(EntityModel playerDataModel)
        {
            //TraceUtil.Log("添加实体");
        }

        public void UnRegisteEntity(long uid)
        {
            var ContainerItemFielInfo = itemFielArrayInfo.SingleOrDefault(P => P.GoodsInfoModel.EntityDataStruct.SMsg_Header.uidEntity == uid);
            TraceUtil.Log("删除实体:" + LanguageTextManager.GetString( ContainerItemFielInfo.LocalItemData._szGoodsName));
            if (ContainerItemFielInfo != null)
            {
                itemFielArrayInfo.Remove(ContainerItemFielInfo);
               // UIEventManager.Instance.TriggerUIEvent(UIEventType.ResetContainerGoods, null);
                //Debug.LogWarning("删除物品栏实体");
            }
        }

        public void Instantiate()
        {
        }

        public void LifeOver()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.TownUIBtnLoadComplete, TownUISceneLoadComplete);
            RemoveEventHandler(EventTypeEnum.PlayerLevelUpdate.ToString(), OnPlayerLvOrMoneyUpdate);
            RemoveEventHandler(EventTypeEnum.PlayerHoldMoneyUpdate.ToString(), OnPlayerLvOrMoneyUpdate);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.AddColdWork, AddColdWork);
           

            ResetContainerInfo(null);
            m_instance = null;
        }

        #region 事件接收和处理
        protected override void RegisterEventHandler()
        {
            AddEventHandler(EventTypeEnum.PlayerLevelUpdate.ToString(), OnPlayerLvOrMoneyUpdate);
            AddEventHandler(EventTypeEnum.PlayerHoldMoneyUpdate.ToString(), OnPlayerLvOrMoneyUpdate);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.AddColdWork, AddColdWork);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.TownUIBtnLoadComplete, TownUISceneLoadComplete);
        }

        void OnPlayerLvOrMoneyUpdate(INotifyArgs agrs)
        {
            IEntityDataStruct data = (IEntityDataStruct)(agrs);
            if(data.SMsg_Header.IsHero)
            {
            CheckShowEquipmentEff();
                ShowJewelEff();
            }
        }

        void AddColdWork(object obj)
        {
            ColdWorkInfo myColdWork = (ColdWorkInfo)obj;
            if (myColdWork.lMasterID== PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity)
            {
                if (myColdWork.ColdClass == ColdWorkClass.Goods)//物品冷却
                {
                    foreach (ItemFielInfo ItemChild in itemFielArrayInfo)
                    {
                        if (ItemChild.LocalItemData._goodID == myColdWork.ColdID)
                        {
                            TweenFloat.Begin(myColdWork.ColdTime / 1000, 1, 0, ItemChild.RecoverMyself);
                        }
                    }
                }
            }
            
        }

        void TownUISceneLoadComplete(object obj)
        {
            ShowPackIsfullEff();
            CheckShowEquipmentEff();
            ShowJewelEff();

        }

     public   void ShowJewelEff()
        {
            if(CheckHasJewelCanBeset())
            {
                UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayMainBtnAnim, UIType.Gem);
            }
            else
            {
                UIEventManager.Instance.TriggerUIEvent(UIEventType.StopMainBtnAnim, UIType.Gem);
            }
        }
        void ShowPackIsfullEff()
        { 
            if(GameManager.Instance.CurrentState==GameManager.GameState.GAME_STATE_TOWN)
            {

                if (ContainerInfomanager.Instance.PackIsFull())
                {
                    UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayMainBtnAnim, UIType.Package);
                }
                else
                {
                    UIEventManager.Instance.TriggerUIEvent(UIEventType.StopMainBtnAnim, UIType.Package);
                }
            }
        }

        public void CheckShowEquipmentEff()
        {
            
            if(GameManager.Instance.CurrentState==GameManager.GameState.GAME_STATE_TOWN)
            {
                if( HasEquipmentCanUpWithDoubleUp())
                {
                    UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayMainBtnAnim, UIType.EquipmentUpgrade);
                }
                else
                {
                    UIEventManager.Instance.TriggerUIEvent(UIEventType.StopMainBtnAnim, UIType.EquipmentUpgrade);
                }
            }
            
        }


        #endregion

        #region 背包检查


        public bool CheckPackBtnIsEnable(PackBtnType btnType)
        {
            PackBtnType[] uiType =NewUIDataManager.Instance.InitMainButtonList.Single(P => P.ButtonProgress == GameManager.Instance.MainButtonIndex).PackBtnTypeList;
            return uiType.LocalContains(btnType);
        }

        public bool CheckHasJewelCanBeset()   
        {
          
            bool CanBeset=false;
            List<ItemFielInfo> JewelList=itemFielArrayInfo.FindAll(c=>c.LocalItemData._GoodsClass==3&&c.LocalItemData._GoodsSubClass==3);
            List<ItemFielInfo>eqList=GetEquiptItemList();
            foreach(var jewelItem in JewelList)
            {

                if(CanBesetOnPosition(jewelItem,eqList))
                {
                    CanBeset=true;
                    break;
                }
            }
            return CanBeset;
        }
        /// <summary>
        ///器魂是否可以被镶嵌到身上的某个位置
        /// </summary>
        /// <returns><c>true</c> if this instance can beset on position the specified Jewelitem equipmentList; otherwise, <c>false</c>.</returns>
        /// <param name="Jewelitem">Jewelitem.</param>
        /// <param name="equipmentList">Equipment list.</param>
        private bool CanBesetOnPosition(ItemFielInfo Jewelitem,List<ItemFielInfo> equipmentList)
        {
            bool can=false;
            foreach(var equipment in equipmentList)
            {
                int place=Convert.ToInt32((equipment.LocalItemData as EquipmentData)._vectEquipLoc); 
                EquiptSlotType eqSlotType=(EquiptSlotType)place;
                List<JewelInfo> JewelInfos=PlayerDataManager.Instance.GetJewelInfo(eqSlotType);
                if(CanBesetOnPosition(EqPlaceToEqTypeDic[eqSlotType],Jewelitem.LocalItemData as Jewel))
                {
                    if(JewelInfos[0].JewelID==0&&JewelInfos[1].JewelID==0)//如果装备没有镶嵌器魂
                    {
                        can=true;
                        break;
                    }
                    else if(JewelInfos[0].JewelID==0&&JewelInfos[1].JewelID!=0&&JewelInfos[1].JewelID!=Jewelitem.LocalItemData._goodID)//如果装备有镶嵌器魂并且与当前器魂id不同
                    {
                        can=true;
                        break;
                    }
                    else if(JewelInfos[1].JewelID==0&&JewelInfos[0].JewelID!=0&&JewelInfos[0].JewelID!=Jewelitem.LocalItemData._goodID)//如果装备有镶嵌器魂并且与当前器魂id不同
                    {
                        can=true;
                        break;
                    }
                    else if(JewelInfos[0].JewelID!=0&&JewelInfos[1].JewelID!=0)//如果装备镶嵌两个器魂则判断当前等级是否大于已镶嵌器魂
                    {
                        Jewel jewel1=ItemDataManager.Instance.GetItemData(JewelInfos[0].JewelID) as Jewel;
                        Jewel jewel2=ItemDataManager.Instance.GetItemData(JewelInfos[1].JewelID) as Jewel;
                        if(Jewelitem.LocalItemData._ColorLevel>jewel1._ColorLevel||Jewelitem.LocalItemData._ColorLevel>jewel2._ColorLevel)
                        {
                            can=true;
                            break;
                        }
                    }
                }
            }
        
            return can ;
        }
        private bool  CanBesetOnPosition( EquiptType type,Jewel jewel)
        {
            bool can=false;
            string s=((int)type).ToString();
            if(jewel.StonePosition.LocalContains(s))
            {
                can=true;
            }
            return can;
        }

        public List<ItemFielInfo> GetAllUsableGoods()
        {
            var list=  itemFielArrayInfo.Where(c=>c.LocalItemData._GoodsClass==2&&c.LocalItemData._GoodsSubClass!=3&&c.LocalItemData._AllowLevel<=PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL).ToList();
//          if(list!=null)
//            {
//            list.Sort((x,y)=>int.Parse((x.LocalItemData as EquipmentData)._vectEquipLoc)-int.Parse((y.LocalItemData as EquipmentData)._vectEquipLoc));
//            }
            return list;
        }
        /// <summary>
        /// Gets the best item list.
        /// </summary>
        /// <returns>The best item list.</returns>
        public Dictionary<EquiptSlotType,ItemFielInfo>  GetBestItemList()
        {
            Dictionary<EquiptSlotType,ItemFielInfo> BestItemList=new Dictionary<EquiptSlotType, ItemFielInfo>();
            foreach(EquiptSlotType child in Enum.GetValues(typeof(EquiptSlotType)))
            {
                if(child == EquiptSlotType.Medicine||child==EquiptSlotType.Null)
                    continue;
                var bestItem = GetBestItem.GetBestItemInPlace(child);
                if(bestItem!=null&&(!IsItemEquipped(bestItem)))
                {
                    BestItemList.Add(child,bestItem);
                }
            }
            return BestItemList;
        }
        /// <summary>
        ///是否装备升星升级或者强化1级
        /// </summary>
        /// <returns><c>true</c> if this instance has equipment can U; otherwise, <c>false</c>.</returns>
        public bool HasEquipmentCanUP()
        {
            return (CheckHasEquipmentStrength()||CheckHasEquipmentCanStarUP()||CheckHasEquipmentCanUpgrade());
        }
        /// <summary>
        /// 当角色等级小于预定等级时是否可连续两次升星。强化或一次升级，否则是否可一次升星.强化.升级
        /// </summary>
        /// <returns><c>true</c> if this instance has equipment can up with double up; otherwise, <c>false</c>.</returns>
        public bool HasEquipmentCanUpWithDoubleUp()
        {
           if(PlayerManager.Instance.FindHeroDataModel().GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL< CommonDefineManager.Instance.CommonDefine.ButtonWeakTipsLevel)
            {
                return (CheckHasEquipmentDoubleStrength()||CheckHasEquipmentCanDoubleStarUP()||CheckHasEquipmentCanUpgrade());
            }
            else
            {
                return (CheckHasEquipmentStrength()||CheckHasEquipmentCanStarUP()||CheckHasEquipmentCanUpgrade());
            }
        }
        /// <summary>
        /// 检查是否有装备可以升星一次
        /// </summary>
        /// <returns><c>true</c>, if has equipment can star U was checked, <c>false</c> otherwise.</returns>
        public bool CheckHasEquipmentCanStarUP()
        {
            bool CanStarUp=false;
            foreach(var item in  ContainerInfomanager.Instance.GetEquiptItemList())
            {
                if(EquipmentCanUp(UpgradeType.Strength,item))
                {
                    CanStarUp=true;
                    break;
                }
                
                
            }
            return CanStarUp;
        }
        /// <summary>
        ///检查是否有装备可以连续升星两次
        /// </summary>
        /// <returns><c>true</c>, if has equipment can double star U was checked, <c>false</c> otherwise.</returns>
        public bool CheckHasEquipmentCanDoubleStarUP()
        {
            bool CanStarUp=false;
            foreach(var item in  ContainerInfomanager.Instance.GetEquiptItemList())
            {
                if(EquipmentCanDoubleUp(UpgradeType.Strength,item))
                {
                    CanStarUp=true;
                    break;
                }
                
                
            }
            return CanStarUp;
        }
        /// <summary>
        /// 检查是否有装备可以强化一次
        /// </summary>
        /// <returns><c>true</c>, if has equipment strength was checked, <c>false</c> otherwise.</returns>
        public bool CheckHasEquipmentStrength()
        {
            bool canStrength=false;
            foreach(var item in  ContainerInfomanager.Instance.GetEquiptItemList())
            {
                if(EquipmentCanUp(UpgradeType.StarUp,item))
                {
                    canStrength=true;
                    break;
                }
                
            }
            return canStrength;
        }
        /// <summary>
        /// 检查是否有装备可以连续强化两次
        /// </summary>
        /// <returns><c>true</c>, if has equipment double strength was checked, <c>false</c> otherwise.</returns>
        private bool CheckHasEquipmentDoubleStrength()
        {
            bool canStrength=false;
            foreach(var item in  ContainerInfomanager.Instance.GetEquiptItemList())
            {
                if(EquipmentCanDoubleUp(UpgradeType.StarUp,item))
                {
                    canStrength=true;
                    break;
                }
                
            }
            return canStrength;
        }
        /// <summary>
        /// 检查是否有装备可以升级
        /// </summary>
        /// <returns><c>true</c>, if has equipment can upgrade was checked, <c>false</c> otherwise.</returns>
        public bool CheckHasEquipmentCanUpgrade()
        {
            bool canUpgrade=false;
            foreach(var item in  ContainerInfomanager.Instance.GetEquiptItemList())
            {
                if(EquipmentCanUp(UpgradeType.Upgrade,item))
                {
                    canUpgrade=true;
                    break;
                }
            }
            return canUpgrade;
        }


        /// <summary>
        /// 某装备是否可以升星。强化。升级
        /// </summary>
        /// <returns><c>true</c>,同时满足等级需求和材料需求, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        /// <param name="itemfileInfo">Itemfile info.</param>
        public bool EquipmentCanUp(UpgradeType type,ItemFielInfo itemfileInfo)
        { 
            EquiptSlotType place=(EquiptSlotType)itemfileInfo.sSyncContainerGoods_SC.nPlace;
            int itemId;
            switch(type)
            {
                case UpgradeType.Strength:
                    return  (HasEnoughMaterial(type,itemfileInfo,out itemId)&&StrengthLevelEnough(place));
                case UpgradeType.StarUp:
                    
                    return  (HasEnoughMaterial(type,itemfileInfo,out itemId)&&StarUpLevelEnough(place));
                    
                case UpgradeType.Upgrade:
                    return  (HasEnoughMaterial(type,itemfileInfo,out itemId)&&UpgradeLevelEnough(itemfileInfo.LocalItemData._goodID)&&(itemfileInfo.LocalItemData as EquipmentData).UpgradeID!=0);
                default:
                    return false;
            }
        }
        /// <summary>
        ///某装备能否连续升两级，只针对升星和强化
        /// </summary>
        /// <returns><c>true</c>, if can double up was equipmented, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        /// <param name="itemfileInfo">Itemfile info.</param>
        public bool EquipmentCanDoubleUp(UpgradeType type,ItemFielInfo itemfileInfo)
        {
            EquiptSlotType place=(EquiptSlotType)itemfileInfo.sSyncContainerGoods_SC.nPlace;
			EquiptType eqtype=(EquiptType)itemfileInfo.LocalItemData._GoodsSubClass;
            int itemId;
            switch(type)
            {
                case UpgradeType.Strength:
				return  (EnoughStrengthMaterial(eqtype,PlayerDataManager.Instance.GetEquipmentStrengthLevel(place),true,out itemId)&&StrengthLevelEnoughDoubleUp(place));
                case UpgradeType.StarUp:
				return  (EnoughStarUpMaterial(eqtype,PlayerDataManager.Instance.GetEquipmentStarLevel(place),true,out itemId)&&StarUpLevelEnoughDoubleUp(place));
                default:
                    return false;
            }
        }
        /// <summary>
        /// 升级。升星。强化某装备1级所需的材料是否足够
        /// </summary>
        /// <returns><c>true</c> if this instance has enough material the specified type itemfileInfo itemId; otherwise, <c>false</c>.</returns>
        /// <param name="type">Type.</param>
        /// <param name="itemfileInfo">Itemfile info.</param>
        /// <param name="itemId">Item identifier.</param>
        public bool HasEnoughMaterial(UpgradeType type,ItemFielInfo itemfileInfo,out int itemId)
        {
            EquiptSlotType place=(EquiptSlotType)itemfileInfo.sSyncContainerGoods_SC.nPlace;
            bool HasEnough=false;
			EquiptType eqtype= (EquiptType)itemfileInfo.LocalItemData._GoodsSubClass;
            itemId=0;
            switch(type)
            {
                case UpgradeType.Strength:
                    
                    HasEnough=EnoughStrengthMaterial(eqtype, PlayerDataManager.Instance.GetEquipmentStrengthLevel(place),false,out itemId);
                    break;
                case UpgradeType.StarUp:
				HasEnough=EnoughStarUpMaterial(eqtype, PlayerDataManager.Instance.GetEquipmentStarLevel(place),false,out itemId);
                    break;
                case UpgradeType.Upgrade:
                    HasEnough=EnoughUpgradeMaterial(itemfileInfo,out itemId);
                    break;
            }
            return HasEnough;
        }
        /// <summary>
        /// 获取已拥有的材料的数量（包括金币）
        /// </summary>
        /// <returns>The own material count.</returns>
        /// <param name="item">Item.</param>
        public int GetOwnMaterialCount(UpgradeRequire item)
        {
            int OwnCount;
            if (item.GoodsId == 3050001)
            {
                OwnCount = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
            }
            else
            {
                OwnCount = ContainerInfomanager.Instance.GetItemNumber(item.GoodsId);
            }
            return OwnCount;
        }
        /// <summary>
        /// 是否满足强化等级小于角色等级才可以强化的条件
        /// </summary>
        /// <returns><c>true</c>, if level enough was strengthed, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        public bool StrengthLevelEnough(EquiptSlotType type)
        {
            bool canStarUp=false;
			int heroLevel=PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
			int strengthLevel=PlayerDataManager.Instance.GetEquipmentStrengthLevel(type);
			int maxLvel=CommonDefineManager.Instance.CommonDefine.StrengthLimit;
			if( strengthLevel<heroLevel&&strengthLevel<maxLvel)
			{
				canStarUp=true;
            }
            return canStarUp;
        }
        /// <summary>
        /// 是否满足强化等级小于角色等级两级才可以连续强化两次的条件
        /// </summary>
        /// <returns><c>true</c>, if level enough double up was strengthed, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        public bool StrengthLevelEnoughDoubleUp(EquiptSlotType type)
        {
            bool canStarUp=false;
			int heroLevel=PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
			int strengthLevel=PlayerDataManager.Instance.GetEquipmentStrengthLevel(type);
			int maxLvel=CommonDefineManager.Instance.CommonDefine.StrengthLimit;
			if( (strengthLevel+1)<heroLevel&& (strengthLevel+1)<maxLvel)
            {
                canStarUp=true;
            }
            return canStarUp;
        }
        /// <summary>
        ///是否满足升星等级小于角色等级两级才可以连续升星两次的条件
        /// </summary>
        /// <returns><c>true</c>, if up level enough double up was stared, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        public bool StarUpLevelEnoughDoubleUp(EquiptSlotType type)
        {
            bool canStarUp=false;
			int heroLevel=PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
			int starLevel=PlayerDataManager.Instance.GetEquipmentStarLevel(type);
			int maxLvel=CommonDefineManager.Instance.CommonDefine.StartStrengthLimit;
			if( (starLevel+1)<heroLevel&&(starLevel+1)<maxLvel)
            {
                canStarUp=true;
            }
            return canStarUp;
        }

        /// <summary>
        ///是否满足升星等级小于角色等级才可以升星的条件
        /// </summary>
        /// <returns><c>true</c>, if up level enough was stared, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        public bool StarUpLevelEnough(EquiptSlotType type)
        {
            bool canStarUp=false;
			int heroLevel=PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
			int starLevel=PlayerDataManager.Instance.GetEquipmentStarLevel(type);
			int maxLvel=CommonDefineManager.Instance.CommonDefine.StartStrengthLimit;
			if( starLevel<heroLevel&&starLevel<maxLvel)
            {
                canStarUp=true;
            }
            return canStarUp;
        }
        /// <summary>
        /// 是否满足升级后新装备的等级小于等于当前玩家等级的条件
        /// </summary>
        /// <returns><c>true</c>, if level enough was upgraded, <c>false</c> otherwise.</returns>
        /// <param name="ItemId">Item identifier.</param>
        public bool UpgradeLevelEnough(int ItemId)
        {
            EquipmentData itemdata=ItemDataManager.Instance.GetItemData(ItemId) as EquipmentData;
            if(itemdata!=null)
            {
                // 如果可以升级， 用升级后的数据比较
                if(itemdata.UpgradeID != 0)
                {
                    itemdata=ItemDataManager.Instance.GetItemData(itemdata.UpgradeID) as EquipmentData;
                }
                if(itemdata._AllowLevel<=PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL)
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// 强化材料是否足够
        /// </summary>
        /// <returns><c>true</c>, if strength material was enoughed, <c>false</c> otherwise.</returns>
        /// <param name="level">当前强化等级.</param>
        /// <param name="isDoubleKill">是否要连续强化两次<c>true</c> is double kill.</param>
        /// <param name="Itemid">返回false时输出数量不足的材料id.</param>
        public bool EnoughStrengthMaterial(EquiptType type, int level,bool isDoubleKill,out int Itemid)
        {
            
            bool isEnough=true;
            int OwnCount;
            Itemid=0;
			List<UpgradeRequire> Requires=PlayerDataManager.Instance.GetStrengCost(type,level);
			if(!isDoubleKill)
            {
                foreach(var item in Requires)
                {
                    OwnCount = GetOwnMaterialCount(item);
                    if(item.Count>OwnCount)
                    {
                        Itemid=item.GoodsId;
                        isEnough=false;
                    }
                    break;
                }
            }
            else
            {
				List<UpgradeRequire> Requires1=PlayerDataManager.Instance.GetStrengCost(type,level+1);
				List<UpgradeRequire> Total=combineRequire(Requires,Requires1);
                foreach(var item in Total)
                {
                    
                    OwnCount= GetOwnMaterialCount(item);
                    if(item.Count>OwnCount)
                    {
                        Itemid=item.GoodsId;
                        isEnough=false;
                    }
                    break;
                }
            }
            
            return isEnough;
        }


        /// <summary>
        /// 升星材料是否足够
        /// </summary>
        /// <returns><c>true</c>, if star up material was enoughed, <c>false</c> otherwise.</returns>
        /// <param name="level">Level.</param>
        public bool EnoughStarUpMaterial(EquiptType type, int level, bool isDoubleKill,out int itemID)
        {
            
            bool isEnough=true;
            itemID=0;
            int OwnCount;
			List<UpgradeRequire> Requires=PlayerDataManager.Instance.GetStarUpCost(type,level);
			if(!isDoubleKill)
            {
                foreach(var item in Requires)
                {
                    OwnCount= GetOwnMaterialCount(item);
                    if(item.Count>OwnCount)
                    {
                        itemID=item.GoodsId;
                        isEnough=false;
                    }
                    break;
                }
            }
            else
            {
                List<UpgradeRequire> Requires1=PlayerDataManager.Instance.GetStarUpCost(type,level+1);
                List<UpgradeRequire> Total=combineRequire(Requires,Requires1);
                foreach(var item in Total)
                {
                   
                    OwnCount= GetOwnMaterialCount(item);
                    if(item.Count>OwnCount)
                    {
                        itemID=item.GoodsId;
                        isEnough=false;
                    }
                    break;
                }
            }
            return isEnough;
        }

        /// <summary>
        /// 合并两个材料表 Combines the require.
        /// </summary>
        /// <returns>The require.</returns>
        /// <param name="Requires1">Requires1.</param>
        /// <param name="Requires2">Requires2.</param>
        private  List<UpgradeRequire> combineRequire( List<UpgradeRequire> Requires1,List<UpgradeRequire> Requires2)
        {
            List<UpgradeRequire> Total= new List<UpgradeRequire>();
            foreach(UpgradeRequire item in Requires2)
            {
                UpgradeRequire re=new UpgradeRequire();
                re.GoodsId=item.GoodsId;
                re.Count=item.Count;
                Total.Add(re);
            }

            foreach (var item in Requires1)
            {
                UpgradeRequire re = Total.SingleOrDefault(c => c.GoodsId == item.GoodsId);
                if (re != null)
                {
                    re.Count += item.Count;
                }
                else
                {
                    Total.Add(item);
                }
            }
            return Total;
        }
        
        
        
        
        /// <summary>
        /// 升级材料是否足够
        /// </summary>
        /// <returns><c>true</c>, if upgrade material was enoughed, <c>false</c> otherwise.</returns>
        /// <param name="itemFileInfo">需要itemFileInfo</param>
        public bool EnoughUpgradeMaterial(ItemFielInfo itemFileInfo,out int itemId)
        {
            bool isEnough=true;
            itemId=0;
            int OwnCount;
            List<UpgradeRequire> Requires=GetUpgradeRequire(itemFileInfo.LocalItemData as EquipmentData);//升级装备材料信息在装备表上，升星和强化在PlayerStrengthCost表上
            foreach(var item in Requires)
            {
                OwnCount= GetOwnMaterialCount(item);
                if(item.Count>OwnCount)
                {
                    itemId=item.GoodsId;
                    isEnough=false;
                    break;
                }
           
            }
            return isEnough;
        }
        
        /// <summary>
        /// 获取装备升级需要的材料列表
        /// </summary>
        /// <returns>The upgrade require.</returns>
        /// <param name="equipment">Equipment.</param>
        public List<UpgradeRequire> GetUpgradeRequire(EquipmentData equipment)
        {
            List<UpgradeRequire> UpgradeRequires=new List<UpgradeRequire>();
            if(equipment!=null)
            {
                string[] UpgradeCost=  equipment.UpgradeCost.Split('|');
                foreach(string item in UpgradeCost)
                {
                    UpgradeRequire ur=new UpgradeRequire();
                    string[] strs=item.Split('+');
                    ur.GoodsId=System.Convert.ToInt32( strs[0]);
                    ur.Count=System.Convert.ToInt32( strs[1]);
                    UpgradeRequires.Add(ur);
                }
                
            }
            return UpgradeRequires;
        }
        #endregion
        
        
    }
    
    public enum SeverItemFielInfoType {Materiel,Equid,Medicament ,Title,Jewel}
    
    public class ItemFielInfo
    {
        public ChangeValueDelegate RecoverValueDelegate;
        public SeverItemFielInfoType severItemFielType{get; private set;}
        public ItemData LocalItemData { get; private set; }//本地读取数据表的属性
        //public SMsgPropCreateEntity_SC_Container GoodsInfo { get; private set; }//背包物体实体信息
        public EntityModel GoodsInfoModel { get; private set; }//背包物体实体信息模型
        public SSyncContainerGoods_SC sSyncContainerGoods_SC{get;private set;}//该实体所在位置
        /// <summary>
        /// 解析后的实体信息
        /// </summary>
        public Materiel materiel { get{return (Materiel)this.MaterielModel.EntityDataStruct;} }//材料
        public EquipmentEntity equipmentEntity { get { return this.EquipmentEntityModel == null?new EquipmentEntity(): (EquipmentEntity)this.EquipmentEntityModel.EntityDataStruct; } }//装备
        public MedicamentEntity medicamentEntity { get{return this.MedicamentEntityModel==null?new MedicamentEntity():(MedicamentEntity)this.MedicamentEntityModel.EntityDataStruct;} }//药品

        public EntityModel MaterielModel;//材料
        public EntityModel EquipmentEntityModel;//装备
        public EntityModel MedicamentEntityModel;//药品
                

        public bool GetIfBesetJewel(int HoleIndex)
        {
            return GetJewelIndex(HoleIndex)!=0;
        }
        public int GetJewelID(int hodeIndex)
        {
            return 3010100+GetJewelIndex(hodeIndex);
        }
        public Jewel GetJewel(int hodeIndex)
        {
            return (Jewel)ItemDataManager.Instance.GetItemData(GetJewelID(hodeIndex));
        }
        /// <summary>
        ///获取宝石的索引，宝石ID=3010100+索引
        /// </summary>
        /// <returns>The jewel index.</returns>
        /// <param name="hodeIndex">Hode index.</param>
        public int GetJewelIndex(int hodeIndex)
        {
            int Res=0;
            switch(hodeIndex)
            {
                case 1:
                    Res=equipmentEntity.EQUIP_FIELD_JEWEL_ID1;
                    break;
                case 2:
                    Res=equipmentEntity.EQUIP_FIELD_JEWEL_ID2;
                    break; 
            }
            return Res;
        }

   
        public void AddsSyncContainerGoods_SC(SSyncContainerGoods_SC sSyncContainerGoods_SC)//添加物品位置信息
        {
            //TraceUtil.Log("设置物品位置："+LanguageTextManager.GetString(this.LocalItemData._szGoodsName)+",Nplace:"+sSyncContainerGoods_SC.nPlace);
            this.sSyncContainerGoods_SC = sSyncContainerGoods_SC;
        }

		public ItemFielInfo(int ItemID)
		{
			LocalItemData=ItemDataManager.Instance.GetItemData(ItemID);
		}
        public ItemFielInfo(SMsgPropCreateEntity_SC_Container GoodsInfo)
        {
            this.LocalItemData = ItemDataManager.Instance.GetItemData(GoodsInfo.ItemTemplateID);
            ResetEntityModel(GoodsInfo);
            
        }

        public void ResetEntityModel(SMsgPropCreateEntity_SC_Container GoodsInfo)
        {
            this.GoodsInfoModel = new EntityModel();
            this.GoodsInfoModel.EntityDataStruct= GoodsInfo;
            switch (LocalItemData._GoodsClass)
            {
                case 1://类型为装备
                    this.EquipmentEntityModel = new EntityModel();
                    if(LocalItemData._GoodsSubClass!=7)
                    {
                    this.severItemFielType = SeverItemFielInfoType.Equid;
                    }
                    else
                    {
                     this.severItemFielType=SeverItemFielInfoType.Title;
                    }
                    this.EquipmentEntityModel.EntityDataStruct = EquipmentEntity.ParsePackage(GoodsInfo.ItemData, GoodsInfo.SMsg_Header);
                    EntityController.Instance.RegisteEntity(GoodsInfoModel.EntityDataStruct.SMsg_Header.uidEntity, this.EquipmentEntityModel);
                    break;
                case 2://类型为药品
                    this.MedicamentEntityModel = new EntityModel();
                    this.severItemFielType = SeverItemFielInfoType.Medicament;
                    this.MedicamentEntityModel.EntityDataStruct = MedicamentEntity.ParsePackage(GoodsInfo.ItemData, GoodsInfo.SMsg_Header);
                    EntityController.Instance.RegisteEntity(GoodsInfoModel.EntityDataStruct.SMsg_Header.uidEntity, this.MedicamentEntityModel);
                    break;
                case 3://类型为材料
                    this.MaterielModel = new EntityModel();
                    if(LocalItemData._GoodsSubClass==3)
                    {
                        this.severItemFielType = SeverItemFielInfoType.Jewel;
                    }
                    else
                    {
                        this.severItemFielType = SeverItemFielInfoType.Materiel;
                    }
                    this.MaterielModel.EntityDataStruct = Materiel.ParsePackage(GoodsInfo.ItemData, GoodsInfo.SMsg_Header);
                    EntityController.Instance.RegisteEntity(GoodsInfoModel.EntityDataStruct.SMsg_Header.uidEntity, this.MaterielModel);
                    break;
                default:
                    break;
            }
        }

        public void RecoverMyself(float Number)
        {
            //TraceUtil.Log("Recover:" + Number+"Item:"+this.LocalItemData._szGoodsName);
            if (RecoverValueDelegate != null)
            {
                RecoverValueDelegate(Number);
            }
        }

        public ChangeValueDelegate GetColdInfo()
        {
            ChangeValueDelegate ColdInfo = this.RecoverValueDelegate;
            this.RecoverValueDelegate = null;
            return ColdInfo;
        }

  

    }

}
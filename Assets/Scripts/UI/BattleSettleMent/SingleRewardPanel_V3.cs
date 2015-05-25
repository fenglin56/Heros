using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace UI.Battle
{

    public class SingleRewardPanel_V3 : MonoBehaviour
    {
        public class RewardData
        {
            public SMSGEctypeSettleAccounts2_SC MyPackageData;
            public bool IsHero;
            public string Name;
            public int Vocation;
            public string Grade;
            public int RewardItem;
            public int RewardItemNum;
            public List<int> PickupItemList;
            public EntityModel MyEntityModel;
            public RewardData(SMSGEctypeSettleAccounts2_SC netPackage)
            {
                MyPackageData = netPackage;
                MyEntityModel = PlayerManager.Instance.GetEntityMode(netPackage.uidPlayer);
                var playerBehaviour = (PlayerBehaviour)MyEntityModel.Behaviour;
                IPlayerDataStruct data = (IPlayerDataStruct)MyEntityModel.EntityDataStruct;
                IsHero = playerBehaviour.IsHero;
                Name = playerBehaviour.IsHero ? ((SMsgPropCreateEntity_SC_MainPlayer)MyEntityModel.EntityDataStruct).Name : ((SMsgPropCreateEntity_SC_OtherPlayer)MyEntityModel.EntityDataStruct).Name;
                Vocation = data.GetCommonValue().PLAYER_FIELD_VISIBLE_VOCATION;
                Grade = netPackage.sGrade;
                RewardItem = netPackage.dwAwardEquipId;
                RewardItemNum = netPackage.dwAwardEquipNum;
                PickupItemList = netPackage.EquipItemList;
            }
            public RewardData()
            {
                IsHero = true;
                Name = "TestName";
                Vocation = 1;
                Grade = "SS";
                RewardItem = 1000004;
                PickupItemList = new List<int>() { 1001010, 1001010, 1001010, 1001010, 1001010 };
            }
        }

        public UILabel Namelabel;
        //public UISprite VocationIcon;
        //public UILabel GradLabel;

        public GameObject GradEffectPreafb;
        //public Transform CreatGradEffectTransform;
        public GameObject RewardEffectPrefab;
        public Transform RewardEffectTransform;
        public GameObject PickupItemEffect;
        public Transform PickupItemEffectTransform;

        public GameObject SingleRewardItemPrefab;
        public Transform RewardItemTransform;
        public Transform PickupItemTransform;

        public GameObject AllElementParent;

        #region//宝箱
        public SingleButtonCallBack TreasureChestsButton;
        public GameObject TreasureChestCloseStatusOBj;
        public GameObject TreasureChestOpenStatusOBj;
        public SpriteSwith CostIcon;
        public SingleButtonCallBack CostLabel;
        public Transform CreatTreasureChestsItemPoint;
        public UILabel TreasureChestsItemNameLabel;
        public GameObject OpenTreasureChestEffect;
        public Transform CreatTreasureChestEffectPoint;
        int CostMoneyType = 0;
        int CostMoney = 0;
        bool IsTreasureChestsOpened = false;
        #endregion

        public List<GameObject> PickupItemObjList { get; private set; }
        public List<int> PickUpItem { get; private set; }

        public RewardData rewardData { get; private set; }
        public BattleSettlementRewardPanel_V3 MyParent { get; private set; }


        void Awake()
        {
            TreasureChestsButton.SetCallBackFuntion(OnTreasureChestsBtnClick);
            GameDataManager.Instance.dataEvent.RegisterEvent(DataType.EctypeTreasureReward, UpdateTreasureChests); 
            AllElementParent.SetActive(false);
        }

        void OnDestroy()
        {
            GameDataManager.Instance.dataEvent.RemoveEventHandel(DataType.EctypeTreasureReward, UpdateTreasureChests);
            GameDataManager.Instance.ClearData(DataType.EctypeTreasureReward);
        }

        public void Show(RewardData rewardData, BattleSettlementRewardPanel_V3 myParent)
        {
            AllElementParent.SetActive(true);
            this.rewardData = rewardData;
            this.MyParent = myParent;
            PickUpItem = new List<int>();
            PickupItemObjList = new List<GameObject>();
            rewardData.PickupItemList.ApplyAllItem(P=>PickUpItem.Add(P));
            Namelabel.SetText(rewardData.Name);
            //VocationIcon.ChangeSprite(rewardData.Vocation);
            UpdateHeroIcon(rewardData.MyPackageData.uidPlayer);
            //GradLabel.SetText(this.rewardData.Grade);
            UpdateTreasureChests(null);
            InitTreasureChests();
            if (rewardData.IsHero)
            {
                TweenShow();
            }
            else
            {
                WithoutAnimShow(); 
            }
        }

        void UpdateHeroIcon(long uid)
        {
            bool isHero = uid == PlayerManager.Instance.FindHeroDataModel().UID;
            TypeID typeID;
            int fashionID = 0;
            int vocationID = 0;
            if (isHero)
            {
                var heroData = PlayerManager.Instance.FindHeroDataModel();
                vocationID = heroData.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
                fashionID = heroData.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION;
            }
            else
            {
                var playerData = EntityController.Instance.GetEntityModel(uid, out typeID);
                SMsgPropCreateEntity_SC_OtherPlayer otherPlayerData = (SMsgPropCreateEntity_SC_OtherPlayer)playerData.EntityDataStruct;
                vocationID = otherPlayerData.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
                fashionID = otherPlayerData.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION;
            }
            //var resData = CommonDefineManager.Instance.CommonDefineFile._dataTable.HeroIcon_BattleReward.FirstOrDefault(P => P.VocationID == vocationID && P.FashionID == fashionID);
            //if (resData == null) { TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"找不到角色头像,fashionID:" + fashionID); }
            //VocationIcon.spriteName = resData.ResName;
        }

        void InitTreasureChests()
        {
            SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC = (SMSGEctypeInitialize_SC)GameDataManager.Instance.PeekData(DataType.InitializeEctype);
            var ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[sMSGEctypeInitialize_SC.dwEctypeContainerId];
            CostIcon.ChangeSprite(ectypeData.ByCostType + 1);
            if (rewardData != null && rewardData.IsHero)
            {
                CostLabel.SetButtonText(ectypeData.ByCost.ToString());
            }
            else
            {
                CostLabel.gameObject.SetActive(false);
            }
            CostMoneyType = ectypeData.ByCostType;
            CostMoney = ectypeData.ByCost;
            TreasureChestCloseStatusOBj.SetActive(true);
            TreasureChestOpenStatusOBj.SetActive(false);
        }

        void WithoutAnimShow()
        {
            SingleRewardItem_V3 getRewardItem = CreatObjectToNGUI.InstantiateObj(SingleRewardItemPrefab, RewardItemTransform).GetComponent<SingleRewardItem_V3>();
            getRewardItem.Init(this.rewardData.RewardItem, this.rewardData.RewardItemNum);
            for (int i = 0; i < 3;i++)
            {
                if (PickUpItem.Count >= i + 1)
                {
                    SingleRewardItem_V3 pickUpItemObj = CreatObjectToNGUI.InstantiateObj(SingleRewardItemPrefab, PickupItemTransform).GetComponent<SingleRewardItem_V3>();
                    pickUpItemObj.Init(PickUpItem[i],1);
                    pickUpItemObj.transform.localPosition = new Vector3(0, 32 * i, 0);
                }
            }
        }

        void TweenShow()
        {
            //Vector3 gradFromScale = new Vector3(60,60,60);
            //Vector3 gradToScale = new Vector3(30,30,30);
            //TweenScale.Begin(GradLabel.gameObject, 0.3f, gradFromScale, gradToScale, ShowGradEffect);
            ShowGetRewardItem(null);
        }

        //void ShowGradEffect(object obj)
        //{
        //    CreatObjectToNGUI.InstantiateObj(GradEffectPreafb,CreatGradEffectTransform);
        //    DoForTime.DoFunForTime(1,ShowGetRewardItem,null);
        //}

        void ShowGetRewardItem(object obj)
        {
            SingleRewardItem_V3 getRewardItem = CreatObjectToNGUI.InstantiateObj(SingleRewardItemPrefab, RewardItemTransform).GetComponent<SingleRewardItem_V3>();
            getRewardItem.Init(this.rewardData.RewardItem, this.rewardData.RewardItemNum);
            Vector3 fromScale = new Vector3(2,2,2);
            Vector3 toScale = new Vector3(1,1,1);
            TweenScale.Begin(getRewardItem.gameObject,0.3f,fromScale,toScale,ShowGetRewardEffect);
        }

        void ShowGetRewardEffect(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_AppraisalNew");
            GameObject effectobj = CreatObjectToNGUI.InstantiateObj(RewardEffectPrefab,RewardEffectTransform);
            DoForTime.DoFunForTime(1,ShowPickupItem,null);
        }

        void ShowPickupItem(object obj)
        {
            float animTime = 0.3f;
            float waitTime = 0.3f;
            if (PickUpItem.Count > 0)
            {
                SoundManager.Instance.PlaySoundEffect("Sound_UIEff_AppraisalFields");
                int showItem = PickUpItem[0];
                PickUpItem.RemoveAt(0);
                SingleRewardItem_V3 newItem = CreatObjectToNGUI.InstantiateObj(SingleRewardItemPrefab, PickupItemTransform).GetComponent<SingleRewardItem_V3>();
                DoForTime.DoFunForTime(0.3f, ShowPickupEffect, null);
                newItem.Init(showItem,1);
                if (PickupItemObjList.Count >= 3)
                {
                    var removeObj = PickupItemObjList[2];
                    Destroy(removeObj);
                    PickupItemObjList.RemoveAt(2);
                }
                for (int i = 0; i < PickupItemObjList.Count; i++)
                {
                    Vector3 fromPosition = new Vector3(0, 32 * i, 0);
                    Vector3 toPosition = new Vector3(0, 32 + 32 * i, 0);
                    TweenPosition.Begin(PickupItemObjList[i], animTime, fromPosition, toPosition);
                }
                PickupItemObjList.Insert(0, newItem.gameObject);
                Vector3 newItemFromPos = new Vector3(-138, 0, 0);
                Vector3 newItemToPos = new Vector3(0, 0, 0);
                TweenPosition.Begin(newItem.gameObject, animTime, newItemFromPos, newItemToPos,PlayGetItemSound);
                DoForTime.DoFunForTime(waitTime, ShowPickupItem, null);
            }
        }

        void PlayGetItemSound(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_AppraisalNew"); 
        }

        void ShowPickupEffect(object obj)
        {
            GameObject effectObj = CreatObjectToNGUI.InstantiateObj(PickupItemEffect,PickupItemEffectTransform);
            DoForTime.DoFunForTime(3,DestroyPickupEffectObj,effectObj);
        }

        void DestroyPickupEffectObj(object obj)
        { 
            GameObject destroyObj = obj as GameObject;
            if (destroyObj != null)
            {
                Destroy(destroyObj);
            }
        }

        void OnTreasureChestsBtnClick(object obj)
        {
            if (rewardData.IsHero)
            {
                bool canPay = false;
                switch (CostMoneyType)
                {
                    case 0://元宝
                        int PayGoldMoney = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY;
                        canPay = PayGoldMoney >= CostMoney;
                        if (!canPay)
                        {
                            MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_44"),1);
                        }
                        break;
                    case 1://铜币
                        int PayCopperMoney = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
                        canPay = PayCopperMoney >= CostMoney;
                        if (!canPay)
                        {
                            MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_231"), 1);
                        }
                        break;
                }
                if (canPay)
                {
                    if (UI.MainUI.ContainerInfomanager.Instance.GetEmptyPackBoxNumber() < 1)
                    {
                        MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_2"), 1);//背包已满
                    }
                    else
                    {
                        TraceUtil.Log("发送打开宝箱请求");
                        //NetServiceManager.Instance.EctypeService.SendSMSGEctypeClickTreasure_CS();
                    }
                }
            }
        }
        /// <summary>
        /// 刷新宝箱状态
        /// </summary>
        void UpdateTreasureChests(object obj)
        {
            if (IsTreasureChestsOpened||rewardData == null)
                return;
            EctypeTreasureRewardList ectypeTreasureRewardList = GameDataManager.Instance.PeekData(DataType.EctypeTreasureReward) as EctypeTreasureRewardList;
            if (ectypeTreasureRewardList != null)
            {
                var myTreasureChestsData = ectypeTreasureRewardList.TreasureList.FirstOrDefault(P => P.dwUID == rewardData.MyPackageData.uidPlayer);
                TraceUtil.Log("myTreasureChestsData.UID:" + myTreasureChestsData.dwUID);
                if (myTreasureChestsData.dwUID != 0)
                {
                    IsTreasureChestsOpened = true;
                    TweenOpenTreasureChests(myTreasureChestsData);
                }
            }
        }
        /// <summary>
        /// 打开宝箱
        /// </summary>
        void TweenOpenTreasureChests(SMSGEctypeTreasureReward_SC data)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_ChooseAwardGoldenCard");
            GameObject effectOBj = CreatObjectToNGUI.InstantiateObj(OpenTreasureChestEffect,CreatTreasureChestEffectPoint);
            DoForTime.DoFunForTime(1.3f, SwithTreasurePanel, data);
        }
        void SwithTreasurePanel(object obj)
        {
            //GameObject effectObj = obj as GameObject;
            //if (effectObj != null) { Destroy(effectObj); }
            SMSGEctypeTreasureReward_SC data = (SMSGEctypeTreasureReward_SC)obj;
            TreasureChestCloseStatusOBj.SetActive(false);
            TreasureChestOpenStatusOBj.SetActive(true);
            ItemData creatData = ItemDataManager.Instance.GetItemData(data.dwEquipId);
            CreatObjectToNGUI.InstantiateObj(creatData._picPrefab,CreatTreasureChestsItemPoint);
            DoForTime.DoFunForTime(0.5f, TweenShowTreasureItemIcon, null);
            SetTreasureItemNameLabel(creatData,data.dwEquipNum);
        }
        void SetTreasureItemNameLabel(ItemData itemData,int number)
        {
            TextColor NameTextColor = TextColor.white;
            switch (itemData._ColorLevel)//物品品质颜色
            {
                case 0:
                    NameTextColor = TextColor.EquipmentGreen;
                    break;
                case 1:
                    NameTextColor = TextColor.EquipmentBlue;
                    break;
                case 2:
                    NameTextColor = TextColor.EquipmentMagenta;
                    break;
                case 3:
                    NameTextColor = TextColor.EquipmentYellow;
                    break;
                default:
                    break;
            }
            string ItemName = LanguageTextManager.GetString(itemData._szGoodsName);
            TreasureChestsItemNameLabel.SetText(NGUIColor.SetTxtColor(string.Format("{0}+{1}",ItemName,number),NameTextColor));
        }
        void TweenShowTreasureItemIcon(object obj)
        {
            Vector3 fromSclae = new Vector3(2,2,2);
            Vector3 toSclae = new Vector3(0.8f,0.8f,0.8f);
            TweenScale.Begin(CreatTreasureChestsItemPoint.gameObject, 0.3f, fromSclae, toSclae, null);
        }
    
    }
}
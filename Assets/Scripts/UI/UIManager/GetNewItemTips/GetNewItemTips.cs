using UnityEngine;
using System.Collections;

namespace UI.MainUI
{

    public class GetNewItemTips : View
    {
       // public GameObject DragItemPrefab;
        public Transform CreatItemPoint;
		public GameObject effObj;
        //public UILabel MsgLabel;
		public GameObject titleEquip;
		public GameObject titleGoods;
        public SingleButtonCallBack EquipBtn;
		public UILabel btnLabel;
        public SingleButtonCallBack[] QuitBtn;
        ItemFielInfo itemFielInfo;
        private int CurrentAtkNumber = 0;
        private bool m_isShowZhanLiAnim = false;
        void Start()
        {
            EquipBtn.SetCallBackFuntion(OnEquipBtnClick);
			QuitBtn.ApplyAllItem(P=>P.SetCallBackFuntion(ClosePanelEvent));
            //UIEventManager.Instance.RegisterUIEvent(UIEventType.OpentMainUI, Close);
            CurrentAtkNumber = HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_Combat, PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHTING);
		}

        void OnDestroy()
        {
            
            //UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpentMainUI, Close);
            RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
        }

        void Awake()
        {
            AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
        }

//        void ShowAtkInfo()
//        {
//            int NewAtk = HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_Combat, PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHTING);
//            if (CurrentAtkNumber >= NewAtk)
//            {
//                CurrentAtkNumber = NewAtk;
//                return;
//            }
//            //TraceUtil.Log("刷新人物信息");
//
//            var addAtkNum = NewAtk - CurrentAtkNumber;
//            var heroPos = PlayerManager.Instance.FindHero().transform.position + new Vector3(CommonDefineManager.Instance.CommonDefine.HitNumber_VectorX, CommonDefineManager.Instance.CommonDefine.HitNumber_VectorY, CommonDefineManager.Instance.CommonDefine.HitNumber_VectorZ);
//            PopupTextController.SettleResult(heroPos, addAtkNum.ToString(), FightEffectType.TOWN_EFFECT_ZHANLI);
//
//            CurrentAtkNumber = NewAtk;
//            m_isShowZhanLiAnim = false;
//        }

        void UpdateViaNotify(INotifyArgs inotifyArgs)
        {
            //if (!m_isShowZhanLiAnim)
            //    return;

            EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)inotifyArgs;
            if (entityDataUpdateNotify.IsHero && entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_PLAYER)
            {
               // ShowAtkInfo();
            }
        }
		void Init()
		{
			effObj.SetActive (true);
			transform.localScale = Vector3.one;
			gameObject.GetComponent<UIPanel> ().alpha = 1;
		}
		public void Show(ItemFielInfo itemFielInfo)
        {
			Init ();
            //transform.localPosition = new Vector3(-204,0,50);
            this.itemFielInfo = itemFielInfo;
            CreatItemPoint.ClearChild();
			GameObject skillIcon = NGUITools.AddChild(CreatItemPoint.gameObject,itemFielInfo.LocalItemData._picPrefab);
			skillIcon.transform.localScale = new Vector3(90, 90, 1);
			if (itemFielInfo.LocalItemData._GoodsClass == 2) {
				btnLabel.text = LanguageTextManager.GetString ("IDS_I31_3");
				titleGoods.SetActive(true);
				titleEquip.SetActive(false);
			} else {
				btnLabel.text = LanguageTextManager.GetString ("IDS_I31_1");
				titleEquip.SetActive(true);
				titleGoods.SetActive(false);
			}
            /*SingleContainerBox singleContainerBox = CreatObjectToNGUI.InstantiateObj(DragItemPrefab, CreatItemPoint).GetComponent<SingleContainerBox>();
            singleContainerBox.collider.enabled = false;
            singleContainerBox.gameObject.layer = 26;
            singleContainerBox.gameObject.GetChildTransforms().ApplyAllItem(P => P.gameObject.layer = 26);
            singleContainerBox.Background.ChangeSprite(0);
            //singleContainerBox.ViewAtbButton.gameObject.SetActive(false);
            singleContainerBox.Init(itemFielInfo, SingleContainerBoxType.HeroEquip);
            */
        }

        public void OnEquipBtnClick(object obj)
        {
            m_isShowZhanLiAnim = true;
			SoundManager.Instance.PlaySoundEffect("Sound_Button_NewEqui_OK");
            Close(null);
			if (itemFielInfo.LocalItemData._GoodsClass == 2) {
				ContainerInfomanager.Instance.UseGiftBox(itemFielInfo);
			} else {
				SendEquiptItemToserver();
			}
        }

        /// <summary>
        /// 装备物品
        /// </summary>
        /// <param name="itemFielInfo"></param>
        public void SendEquiptItemToserver()
        {
            ItemFielInfo equipmentItem = itemFielInfo;
            SMsgContainerUse_CS dataStruct = new SMsgContainerUse_CS();
            dataStruct.dwContainerID1 = dataStruct.dwContainerID2 = equipmentItem.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID;
            dataStruct.byPlace = (byte)equipmentItem.sSyncContainerGoods_SC.nPlace;
            dataStruct.uidTarget = PlayerManager.Instance.FindHeroEntityModel().EntityDataStruct.SMsg_Header.uidEntity;
            NetServiceManager.Instance.ContainerService.SendContainerUse(dataStruct);
			//不等服务器，直接装上装备
			if (equipmentItem.LocalItemData._GoodsSubClass == 1) {
				string weapon = ItemDataManager.Instance.GetItemData (equipmentItem.LocalItemData._goodID)._ModelId;
				GameObject weaponObj = PlayerFactory.Instance.GetWeaponPrefab (weapon);
                var weaponEff=(ItemDataManager.Instance.GetItemData (equipmentItem.LocalItemData._goodID)as EquipmentData).WeaponEff;
				RoleGenerate.ChangeWeapon (PlayerManager.Instance.FindHero (), weaponObj,weaponEff);
			}
	//		var heroPos = PlayerManager.Instance.FindHero().transform.position + new Vector3(CommonDefineManager.Instance.CommonDefine.HitNumber_VectorX, CommonDefineManager.Instance.CommonDefine.HitNumber_VectorY, CommonDefineManager.Instance.CommonDefine.HitNumber_VectorZ);
   //         PopupTextController.SettleResult(heroPos, "5555",   FightEffectType.BATTLE_EFFECT_EXPSHOW);
        }
		void ClosePanelEvent(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_NewEqui_Leave");
			Close (obj);
		}
        public void Close(object obj)
        {
			effObj.SetActive (false);
			TweenRun ();
            /*CloseAnim.animation.CrossFade("JH_EFF_UI_NewEquipment");
            Invoke("HideUI", CloseAnim.animation["JH_EFF_UI_NewEquipment"].length);*/
        }
		void TweenRun()
		{
			TweenScale.Begin(gameObject, tweenTime,transform.localScale,new Vector3 (0.5f,0.5f,1),(obj)=>{
				TweenBack();
			});
			TweenAlpha.Begin (gameObject,tweenTime,1,0,(obj)=>{
				TweenBack();
			});
		}
		bool isAgain = false;
		float tweenTime = 0.167f;
		void TweenBack()
		{
			if (isAgain) {
				HideUI();
			}
			isAgain = true;
		}
        void HideUI()
        {
            Destroy(gameObject);
            //transform.localPosition = new Vector3(0, 0, -1000);
        }

        protected override void RegisterEventHandler()
        {
            return;
        }
    }
}
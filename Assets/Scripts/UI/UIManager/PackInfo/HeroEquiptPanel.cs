using UnityEngine;
using System.Collections;

namespace UI.MainUI
{

    public class HeroEquiptPanel : BaseTweenShowPanel
    {
        public UILabel NameLabel;
        public UILabel Levellabel;
        public UILabel ForceLabel;
        
        public SingleEquiptSlot[] EquiptSlotList;
		public RoleModelPanel m_RoleModelPanel;
		public GameObject RoleEffectObj;
		public SingleButtonCallBack DragRoleModelButton;
		public RoleViewPoint RoleViewPoint;
		public PackInfoPanel MyParent{get;private set;}
        public Transform VipEmblemPoint;
		public Transform TitlePoint;//称号勋章位置
		public GameObject TitleGameObject{get;set;}//当前称号模型
		//private RoleViewPanel roleViewPanel;

        void Start()
        {
			AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateEntityData);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.ResetContainerGoods, ChangeHeroWeapon);
			m_RoleModelPanel.AttachEffect(RoleEffectObj);
			DragRoleModelButton.SetDragCallback(m_RoleModelPanel.OnDragRoleModel);

            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            DragRoleModelButton.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_HeroEquiptPanel_DragRoleModel);           
        }

        void OnDestroy()
		{
			RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateEntityData);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ResetContainerGoods, ChangeHeroWeapon); 
        }

		public void Init(PackInfoPanel myParent)
		{
			MyParent = myParent;
		}

        void UpdateEntityData(INotifyArgs inotifyArgs)
        {
            EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)inotifyArgs;
            if (entityDataUpdateNotify.IsHero && entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_PLAYER)
			{
				m_RoleModelPanel.ChangeHeroFashion();
				UpdateHeroAttribute();
            }
        }

        public void Show()
        {
			base.Close();
			base.TweenShow();
            DoForTime.DoFunForTime(0.5f,ShowDefultForTime,null);
            ShowVipEmblem();

        }

        public void ShowVipEmblem()
        {
            VipEmblemPoint.ClearChild();
            GameObject go=PlayerDataManager.Instance.GetCurrentVipEmblemPrefab();
            if(go!=null)
            {
            CreatObjectToNGUI.InstantiateObj(go,VipEmblemPoint);
            }

        }

      

		public void UpdatePanel()
		{
			ShowForTime(null);
		}

        void ShowDefultForTime(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_PackageAppear");
            var equiplist = ContainerInfomanager.Instance.GetEquiptItemList();
            EquiptSlotList.ApplyAllItem(P => P.Init(equiplist,this));
            SetCameraPanelPosition();
            m_RoleModelPanel.ShowDefult();
            UpdateHeroAttribute();
        }

		void ShowForTime(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_UIEff_PackageAppear");
			var equiplist = ContainerInfomanager.Instance.GetEquiptItemList();
			EquiptSlotList.ApplyAllItem(P => P.Init(equiplist,this));
            EquiptSlotList.ApplyAllItem(p => p.SetSelectStatus(null));
			SetCameraPanelPosition();
			m_RoleModelPanel.Show();
			UpdateHeroAttribute();
		}

		void UpdateHeroAttribute()
		{
			NameLabel.SetText(PlayerManager.Instance.FindHeroDataModel().Name);
			int newAtk = HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_Combat, PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHTING);
			ForceLabel.SetText(newAtk);
			Levellabel.SetText(string.Format("Lv:{0}",PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL));
		}

        public void Close()
        {
            DoForTime.stop();
			m_RoleModelPanel.StopAllCoroutines();
            m_RoleModelPanel.Close();
        }
		
		void ChangeHeroWeapon(object obj)
		{
			if (gameObject.active == false)
				return;
			m_RoleModelPanel.ChangeHeroWeapon(null,true);
		}

		public void SetCameraPanelPosition()
		{
			Camera uiCamera = UICamera.mainCamera;
			Vector3 CameraPosition = uiCamera.WorldToScreenPoint(RoleViewPoint.transform.position);
//			BackGrondPanel.transform.position = BackUICamera.ScreenToWorldPoint(CameraPosition);
//			roleAttributePanel.SetPanelPosition(CameraPosition);
			Vector3 LPosition = uiCamera.WorldToScreenPoint(RoleViewPoint.LBound.position);
			Vector3 RPosition = uiCamera.WorldToScreenPoint(RoleViewPoint.RBound.position);
			var lRoleRec = uiCamera.ScreenToViewportPoint(LPosition);
			var rRoleRec = uiCamera.ScreenToViewportPoint(RPosition);
			var w = rRoleRec.x - lRoleRec.x;
			StartCoroutine(m_RoleModelPanel.SetCameraPosition(lRoleRec, w));
		}
        public void OnItemClick(SingleEquiptSlot selectItem)
        {
			EquiptSlotList.ApplyAllItem(P=>P.SetSelectStatus(selectItem));
			MyParent.ShowItemTips(selectItem.MyItem);
        }

        protected override void RegisterEventHandler()
        {
            throw new System.NotImplementedException();
        }

		[ContextMenu("GetComponentInChild")]
		void GetSingleEequiptComponentInChild()
		{
			this.EquiptSlotList = transform.GetComponentsInChildren<SingleEquiptSlot>();
		}
    }
}
using UnityEngine;
using System.Collections;

namespace UI.MainUI
{

    public class GetNewSkillTips : View
    {
        //public GameObject DragItemPrefab;
        public Transform CreatItemPoint;
		public GameObject effObj;
        //public Animation CloseAnim;
        //public UILabel MsgLabel;

        public SingleButtonCallBack EquipBtn;
		public UILabel btnLabel;
        public SingleButtonCallBack[] QuitBtn;
        SingleSkillInfo itemFielInfo;
		public UILabel labelLevel;
        private bool m_isShowZhanLiAnim = false;
        private int CurrentAtkNumber = 0;

        void Start()
        {
			return;
            EquipBtn.SetCallBackFuntion(OnEquipBtnClick);
			QuitBtn.ApplyAllItem(P => P.SetCallBackFuntion(ClosePanelEvent));
            //UIEventManager.Instance.RegisterUIEvent(UIEventType.OpentMainUI, Close);
            CurrentAtkNumber =  HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_Combat,PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHTING);
			btnLabel.text = LanguageTextManager.GetString ("IDS_I31_2");
		}

        void OnDestroy()
        {
			return;
            //UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpentMainUI, Close);
            RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
        }

        void Awake()
        {
			return;
            AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
        }

        void ShowAtkInfo()
        {
            int NewAtk =  HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_Combat,PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHTING);
            if (CurrentAtkNumber >= NewAtk)
            {
                CurrentAtkNumber = NewAtk;
                return;
            }
            //TraceUtil.Log("刷新人物信息");
            var addAtkNum = NewAtk - CurrentAtkNumber;
            var heroPos = PlayerManager.Instance.FindHero().transform.position + new Vector3(CommonDefineManager.Instance.CommonDefine.HitNumber_VectorX, CommonDefineManager.Instance.CommonDefine.HitNumber_VectorY, CommonDefineManager.Instance.CommonDefine.HitNumber_VectorZ);
            PopupTextController.SettleResult(heroPos, addAtkNum.ToString(), FightEffectType.TOWN_EFFECT_ZHANLI, false);

            CurrentAtkNumber = NewAtk;
            m_isShowZhanLiAnim = false;
        }

        void UpdateViaNotify(INotifyArgs inotifyArgs)
        {
            //if (!m_isShowZhanLiAnim)
            //    return;

            EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)inotifyArgs;
            if (entityDataUpdateNotify.IsHero && entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_PLAYER)
            {
              //  ShowAtkInfo();
            }
        }
		void Init()
		{
			effObj.SetActive (true);
			transform.localScale = Vector3.one;
			gameObject.GetComponent<UIPanel> ().alpha = 1;
		}
        public void Show(SingleSkillInfo itemFielInfo)
        {
			Init();
            //transform.localPosition = new Vector3(-204,-250,50);
            this.itemFielInfo = itemFielInfo;
            CreatItemPoint.ClearChild();
			GameObject skillIcon = NGUITools.AddChild(CreatItemPoint.gameObject,itemFielInfo.localSkillData.m_icon);
			skillIcon.transform.localScale = new Vector3(90, 90, 1);
			labelLevel.text = (itemFielInfo.localSkillData.m_breakLevel-1).ToString();
            /*SkillsItem item = CreatObjectToNGUI.InstantiateObj(DragItemPrefab, CreatItemPoint).GetComponent<SkillsItem>();
            item.gameObject.layer = 26;
            item.gameObject.GetChildTransforms().ApplyAllItem(P => P.gameObject.layer = 26);
            //item.FocusBGSwitch.ChangeSprite(0);
            //item.ViewSkillButton.gameObject.SetActive(false);
            item.InitItemData(itemFielInfo);
            item.InitGuideID(3);*/
        }

        public void OnEquipBtnClick(object obj)
        {
            m_isShowZhanLiAnim = true;
			SoundManager.Instance.PlaySoundEffect("Sound_Button_NewSkill_OK");
            Close(null);
            SendEquiptItemToserver();
            
        }

        SingleSkillInfo[] m_equipSkillList;
        public SingleSkillInfo[] SetEquipSkillList
        {
            set { m_equipSkillList = value; }
        }

        /// <summary>
        /// 装备技能[没有新技能提示弹出了]
        /// </summary>
        /// <param name="itemFielInfo"></param>
        public void SendEquiptItemToserver()
        {
            SkillEquipEntity skillEquipEntity = new SkillEquipEntity();
            skillEquipEntity.Skills = new System.Collections.Generic.Dictionary<byte, ushort>();

            for (int i = 0; i < m_equipSkillList.Length; i++ )
            {
                if (m_equipSkillList[i] == null)
                {
                    m_equipSkillList[i] = itemFielInfo;
                    break;
                }
            }

            for (int i = 0; i < m_equipSkillList.Length; i++ )
            {
                if (m_equipSkillList[i] != null)
                {
                    skillEquipEntity.Skills.Add((byte)i, (byte)m_equipSkillList[i].localSkillData.m_skillId);
                }
                else
                    skillEquipEntity.Skills.Add((byte)i, 0);
            }

            NetServiceManager.Instance.EntityService.SendSkillEquip(skillEquipEntity);
        }
		void ClosePanelEvent(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_NewSkill_Leave");
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
            transform.localPosition = new Vector3(0, 0, -1200);
        }


        protected override void RegisterEventHandler()
        {
            return;
        }
    }
}
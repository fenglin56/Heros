using UnityEngine;
using System.Collections;

namespace UI.Battle
{
//    public enum RoleClass 
//    {
//        Swordsman,//刀客
//        Taoist,//天师
//        MusicMaster,//琴师
//        Assassin,//刺客
//    }
    public class HeroStatusUI : View
    {
        public SpriteSwith HeadIcon ;//头像
        public UILabel LevelNum;//等级
        public UISlider HP;//血值
        public UISlider MP;//魔法值
        public UISlider EXP;//经验值

        private GameObject m_TweenHP;
        private GameObject m_TweenMP;
        private GameObject m_TweenEXP;

        public GameObject GO_HP;
        public GameObject GO_MP;

        public SpriteSmoothFlag MPBackground;

        public SingleButtonCallBack DeathBtn;
        public SpriteSwith TeamleaderIcon;

        public GameObject ResurrectionUIPrefab;
        private HeroResurrectionUI ResurrectionUI;

        public HeroRollStrengthTrough RollStrengthTrough;

        private float CurrentHP=0;
        private float CurrentMP=0;
        private float CurrentEXP=0;
        private int CurrentVocation =0;

        private bool ShowLabel = true;

        private Vector3 m_HPScale;
        private Vector3 m_MPScale;

        public UILabel HPLabel;
        public UILabel MPLabel;
        public UILabel EXPLabe;


        SMsgPropCreateEntity_SC_MainPlayer_PlayerValue PlayerValue;
        SMsgPropCreateEntity_SC_Player_UnitValue UnityValue;

        void Awake()
        {
            RegisterEventHandler();
            DeathBtn.HideMyself();
            TeamleaderIcon.ChangeSprite(0);
            DeathBtn.SetCallBackFuntion(OnDeathBtnClick);

            PlayerRollStrengthManager.Instance.ResetRollStrength();//重置翻滚体力值

            if (GO_HP != null)
            {
                m_HPScale = GO_HP.transform.localScale;
            }
            if(GO_MP != null)
            {
                m_MPScale = GO_MP.transform.localScale;
            }
            
        }

        void Start()
        {
            Show();
        }

        public void Show()
        {
            //transform.localPosition = Vector3.zero;
            SetHeroStatus();
            SetHeroDeathStatus();
            SetTeammateStatus(null);
        }

        protected override void RegisterEventHandler()
        {
            AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), ResetStatus);
            AddEventHandler(EventTypeEnum.EntityCreate_Player.ToString(), CreatHeroEntity);
            //AddEventHandler(EventTypeEnum.EntityDie.ToString(), ReceiveEntityDieHandle);
            AddEventHandler(EventTypeEnum.EntityRelive.ToString(),SetHeroAlive);
            //AddEventHandler(EventTypeEnum.UpdateRollStrength.ToString(), UpdaeRollStrengthHandle);
            //AddEventHandler(EventTypeEnum.NoEnoughRollStrength.ToString(), NoEnoughRollStrengthHandle);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.TeamMemberLeave,SetTeammateStatus);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.FlagMPBar, FlagMPProgressBar);
        }


        void OnDestroy()
        {
            RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), ResetStatus);
            RemoveEventHandler(EventTypeEnum.EntityCreate_Player.ToString(), CreatHeroEntity);
            //RemoveEventHandler(EventTypeEnum.EntityDie.ToString(), ReceiveEntityDieHandle);
            RemoveEventHandler(EventTypeEnum.EntityRelive.ToString(), SetHeroAlive);
            //RemoveEventHandler(EventTypeEnum.UpdateRollStrength.ToString(), UpdaeRollStrengthHandle);
            //RemoveEventHandler(EventTypeEnum.NoEnoughRollStrength.ToString(), NoEnoughRollStrengthHandle);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.TeamMemberLeave, SetTeammateStatus);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.FlagMPBar, FlagMPProgressBar);
        }


        void ResetStatus(INotifyArgs inotifyArgs)
        { 
            EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)inotifyArgs;            
            if (entityDataUpdateNotify.IsHero && entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_PLAYER)
            {                
                SetHeroStatus();
            }
        }

        void CreatHeroEntity(INotifyArgs iNotifyArgs)
        {
            SetHeroStatus();
        }

        void SetTeammateStatus(object obj)
        {
            SMsgTeamProp_SC sMsgTeamProp_SC = TeamManager.Instance.MyTeamProp;
            if (!TeamManager.Instance.IsTeamExist())
            {
                TeamleaderIcon.ChangeSprite(0);
                return;
            }
            foreach (SMsgTeamPropMember_SC child in sMsgTeamProp_SC.TeamMemberNum_SC.SMsgTeamPropMembers)
            {
                if (child.TeamMemberContext.dwActorID == TeamManager.Instance.MyTeamProp.TeamContext.dwCaptainId)
                {
                    if (PlayerManager.Instance.FindHeroDataModel().ActorID == (int)child.TeamMemberContext.dwActorID)
                    {
                        TeamleaderIcon.ChangeSprite(1);
                        return;
                    }
                }
            }
            TeamleaderIcon.ChangeSprite(0);
        }

        void ReceiveEntityDieHandle(INotifyArgs args)
        {
            SMsgActionDie_SC sMsgActionDie_SC = (SMsgActionDie_SC)args;
            //TraceUtil.Log("收到死亡消息："+sMsgActionDie_SC.uidEntity+"玩家ID："+PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity);
            if (sMsgActionDie_SC.uidEntity == PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity)
            {
                SetHeroDeathStatus();
            }
        }
        //void OnGUI()
        //{
        //    GUILayout.Label(CurrentHP.ToString());
        //}

        public void SetHeroStatus()//此处设置主角显示的所有状态
        {
            RollStrengthTrough.UpdateMaxValue();
            PlayerValue = PlayerManager.Instance.FindHeroDataModel().PlayerValues;
            UnityValue = PlayerManager.Instance.FindHeroDataModel().UnitValues;
            //TraceUtil.Log("获取玩家职业:" + PlayerValue.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION);
            //TraceUtil.Log("获取玩家HP:" + UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURHP);
            //TraceUtil.Log(string.Format("获取玩家MP:{0}/{1}" , UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURHP,UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_MAXHP));
            //if (CurrentVocation != PlayerValue.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION)
            //{
            //    HeadIcon.ChangeSprite(PlayerValue.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION);
            //    CurrentVocation = PlayerValue.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
            //}
            if (LevelNum != null)
            {
                this.LevelNum.text = UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL.ToString();
            }            
            //if (CurrentHP != UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURHP)
            //{            
                                
                m_TweenHP = TweenFloat.Begin(1, CurrentHP, UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURHP, ChangeHp);
                CurrentHP = UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURHP;
                if (GO_HP != null)
                {
                    float MaxHp = UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_MAXHP;
                    float hpValue = CurrentHP / MaxHp;
                    GO_HP.transform.localScale = new Vector3(m_HPScale.x * hpValue, m_HPScale.y, m_HPScale.z);
                    //GO_HP.transform.localScale = Vector3.one;
                }
            
            //}
            //if (CurrentMP != UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURMP)
            //{
                
                
                m_TweenMP = TweenFloat.Begin( 1, CurrentMP, UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURMP, ChangeMP);
                CurrentMP = UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURMP;  
                if (GO_MP != null)
                {
                    float MaxMP = UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_MAXMP;
                    float mpValue = CurrentMP / MaxMP;
                    GO_MP.transform.localScale = new Vector3(m_MPScale.x * mpValue, m_MPScale.y, m_MPScale.z);
                }            
            //}
            //if (CurrentEXP != PlayerValue.PLAYER_FIELD_EXP)
            //{               
                if (this.EXP != null)
                {
                    m_TweenEXP = TweenFloat.Begin(1, CurrentEXP, PlayerValue.PLAYER_FIELD_EXP, ChangeEXP);
                    CurrentEXP = PlayerValue.PLAYER_FIELD_EXP;
                }
            //}
            //this.HP.sliderValue = UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURHP/UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_MAXHP;
            //this.MP.sliderValue = UnityValue.sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_CURMP/UnityValue.sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_MAXMP;
            //this.EXP.sliderValue = PlayerValue.PLAYER_FIELD_EXP/PlayerValue.PLAYER_FIELD_NEXT_LEVEL_EXP;

        }

        void SetHeroDeathStatus()
        {
            if (PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UINT_FIELD_STATE == (int)CRT_STATE.enCrt_State_Die)
            {
                //DeathBtn.ShowMyself();
            }
        }

        void SetHeroAlive(INotifyArgs args)
        {

            SMsgActionRelivePlayer_SC sMsgActionRelivePlayer_SC = (SMsgActionRelivePlayer_SC)args;
            //Debug.LogWarning("收到复活角色消息,Target:" + sMsgActionRelivePlayer_SC.UIDTarget + ",MyUID:" + PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity);
            if (sMsgActionRelivePlayer_SC.actorTarget ==PlayerManager.Instance.FindHeroDataModel().ActorID)
            {
                HeadIcon.ChangeSprite(PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION);
                //DeathBtn.HideMyself();
                SetHeroStatus();
            }
        }

        void UpdaeRollStrengthHandle(INotifyArgs args)
        {
            SUpdateRollStrengthStruct updateStruct = (SUpdateRollStrengthStruct)args;
            RollStrengthTrough.SetValue(updateStruct.strengthValue);          
        }

        void NoEnoughRollStrengthHandle(INotifyArgs args)
        {
            RollStrengthTrough.ShowNoEnoughStrengthTip();
        }

        void OnDeathBtnClick(object obj)
        {
            if (ResurrectionUI == null)
            {
                ResurrectionUI = CreatObjectToNGUI.InstantiateObj(ResurrectionUIPrefab,BattleUIManager.Instance.Center).GetComponent<HeroResurrectionUI>();
            }
            ResurrectionUI.ShowMyself();
        }

        void ChangeHp(float Number)
        {
            if (Number < 0)
                return;
            float MaxHp =UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_MAXHP;
            this.HP.sliderValue = Number / MaxHp;
            this.HPLabel.SetText(ShowLabel?string.Format("{0}/{1}",Number,MaxHp):"");
            //TraceUtil.Log("[HPLabel]" + HPLabel.text);
        }
        void ChangeMP(float Number)
        {
            if (Number < 0)
                return;
            float MaxMP = UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_MAXMP;
            this.MP.sliderValue = Number / MaxMP;
            this.MPLabel.SetText(ShowLabel ? string.Format("{0}/{1}", Number, MaxMP) : "");
        }
        void ChangeEXP(float Number)
        {
            if (Number < 0)
                return;
            float MaxExp = PlayerValue.PLAYER_FIELD_NEXT_LEVEL_EXP;
            this.EXP.sliderValue = Number / MaxExp;
            this.EXPLabe.SetText(ShowLabel ? string.Format("{0}/{1}", Number, MaxExp) : "");
        }

        public void FlagMPProgressBar(object obj)
        {
            MPBackground.BeginFlag(3, 0.5f, Color.white, Color.red, null);
        }

        public void Close()
        {
            transform.localPosition = new Vector3(0,0,-800);
        }

    }
}
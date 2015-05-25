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
    public class CompetitorStatusUI : View
    {
        public SpriteSwith HeadIcon;//头像
        public UILabel LevelNum;//等级
        public UISlider HP;//血值
        public UISlider MP;//魔法值
        public UISlider EXP;//经验值
        public SpriteSmoothFlag MPBackground;

        public SingleButtonCallBack DeathBtn;
        public SpriteSwith TeamleaderIcon;

        public GameObject ResurrectionUIPrefab;
        private HeroResurrectionUI ResurrectionUI;

        public HeroRollStrengthTrough RollStrengthTrough;

        private float CurrentHP = 0;
        private float CurrentMP = 0;        
        private int CurrentVocation = 0;

        private bool ShowLabel = false;

        public UILabel HPLabel;
        public UILabel MPLabel;
        public UILabel EXPLabe;


        SMsgPropCreateEntity_SC_OtherPlayer_PlayerValue PlayerValue;
        SMsgPropCreateEntity_SC_Player_UnitValue UnityValue;


        void Awake()
        {
            RegisterEventHandler();
            DeathBtn.HideMyself();
            //TeamleaderIcon.ChangeSprite(0);
            DeathBtn.SetCallBackFuntion(OnDeathBtnClick);

            //PlayerRollStrengthManager.Instance.ResetRollStrength();//重置翻滚体力值

        }

        void Start()
        {
            Show();
        }

        public void Show()
        {
            //transform.localPosition = Vector3.zero;
            SetHeroStatus();
            //SetHeroDeathStatus();
            //SetTeammateStatus(null);
        }

        protected override void RegisterEventHandler()
        {
            AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), ResetStatus);
            AddEventHandler(EventTypeEnum.EntityCreate.ToString(), CreatHeroEntity);
            //AddEventHandler(EventTypeEnum.EntityDie.ToString(), ReceiveEntityDieHandle);
            //AddEventHandler(EventTypeEnum.EntityRelive.ToString(), SetHeroAlive);
            //AddEventHandler(EventTypeEnum.UpdateRollStrength.ToString(), UpdaeRollStrengthHandle);
            //UIEventManager.Instance.RegisterUIEvent(UIEventType.TeamMemberLeave, SetTeammateStatus);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.FlagMPBar, FlagMPProgressBar);
        }


        void OnDestroy()
        {
            RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), ResetStatus);
            RemoveEventHandler(EventTypeEnum.EntityCreate.ToString(), CreatHeroEntity);
            //RemoveEventHandler(EventTypeEnum.EntityDie.ToString(), ReceiveEntityDieHandle);
            //RemoveEventHandler(EventTypeEnum.EntityRelive.ToString(), SetHeroAlive);
            //RemoveEventHandler(EventTypeEnum.UpdateRollStrength.ToString(), UpdaeRollStrengthHandle);
            //UIEventManager.Instance.RemoveUIEventHandel(UIEventType.TeamMemberLeave, SetTeammateStatus);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.FlagMPBar, FlagMPProgressBar);
        }


        void ResetStatus(INotifyArgs inotifyArgs)
        {
            EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)inotifyArgs;
            var pvpPlayerData = PVPBattleManager.Instance.GetPVPPlayerData();
            if (entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_PLAYER &&
                pvpPlayerData.uidEntity == entityDataUpdateNotify.EntityUID)
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
            var pvpPlayerData = PVPBattleManager.Instance.GetPVPPlayerData();
            var pvpEntityModel = PlayerManager.Instance.GetEntityMode(pvpPlayerData.uidEntity);
            if(pvpEntityModel == null)
            {
                TraceUtil.Log("find pvpEntityModel is null!");
                return;
            }
            PlayerValue = ((SMsgPropCreateEntity_SC_OtherPlayer)pvpEntityModel.EntityDataStruct).PlayerValues;
            UnityValue = ((SMsgPropCreateEntity_SC_OtherPlayer)pvpEntityModel.EntityDataStruct).UnitValues;

            //TraceUtil.Log("获取玩家职业:" + PlayerValue.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION);
            //TraceUtil.Log("获取玩家HP:" + UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURHP);
            //TraceUtil.Log("获取玩家MP:" + UnityValue.sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_CURMP);
            if (CurrentVocation != PlayerValue.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION)
            {
                HeadIcon.ChangeSprite(PlayerValue.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION);
                CurrentVocation = PlayerValue.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
            }
            //等级
            //this.LevelNum.text = UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL.ToString();
            if (CurrentHP != UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURHP)
            {
                TweenFloat.Begin(1, CurrentHP, UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURHP, ChangeHp);
                CurrentHP = UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURHP;
            }
            //TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"CurrentMP : " + CurrentMP + " , UNIT_FIELD_CURMP" + UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURMP);
            if (CurrentMP != UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURMP)
            {
                TweenFloat.Begin(1, CurrentMP, UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURMP, ChangeMP);
                CurrentMP = UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURMP;
            }
            //if (CurrentEXP != PlayerValue.PLAYER_FIELD_EXP)
            //{
            //    TweenFloat.Begin(1, CurrentEXP, PlayerValue.PLAYER_FIELD_EXP, ChangeEXP);
            //    CurrentEXP = PlayerValue.PLAYER_FIELD_EXP;
            //}
            //this.HP.sliderValue = UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURHP/UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_MAXHP;
            //this.MP.sliderValue = UnityValue.sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_CURMP/UnityValue.sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_MAXMP;
            //this.EXP.sliderValue = PlayerValue.PLAYER_FIELD_EXP/PlayerValue.PLAYER_FIELD_NEXT_LEVEL_EXP;

        }

        void SetHeroDeathStatus()
        {
            if (PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UINT_FIELD_STATE == (int)CRT_STATE.enCrt_State_Die)
            {
                DeathBtn.ShowMyself();
            }
        }

        void SetHeroAlive(INotifyArgs args)
        {

            SMsgActionRelivePlayer_SC sMsgActionRelivePlayer_SC = (SMsgActionRelivePlayer_SC)args;
            Debug.LogWarning("收到复活角色消息,Target:" + sMsgActionRelivePlayer_SC.actorTarget + ",MyActorID:" + PlayerManager.Instance.FindHeroDataModel().ActorID);
            if (sMsgActionRelivePlayer_SC.actorTarget == PlayerManager.Instance.FindHeroDataModel().ActorID)
            {
                HeadIcon.ChangeSprite(PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION);
                DeathBtn.HideMyself();
                SetHeroStatus();
            }
        }

        //void UpdaeRollStrengthHandle(INotifyArgs args)
        //{
        //    SUpdateRollStrengthStruct updateStruct = (SUpdateRollStrengthStruct)args;
        //    if (updateStruct.strengthValue >= 0)
        //    {
        //        RollStrengthTrough.SetValue(updateStruct.strengthValue);
        //    }
        //    else
        //    {
        //        RollStrengthTrough.ShowTip();
        //    }
        //}

        void OnDeathBtnClick(object obj)
        {
            if (ResurrectionUI == null)
            {
                ResurrectionUI = CreatObjectToNGUI.InstantiateObj(ResurrectionUIPrefab, BattleUIManager.Instance.Center).GetComponent<HeroResurrectionUI>();
            }
            ResurrectionUI.ShowMyself();
        }

        void ChangeHp(float Number)
        {
            float MaxHp = UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_MAXHP;
            this.HP.sliderValue = Number / MaxHp;
            this.HPLabel.SetText(ShowLabel ? string.Format("{0}/{1}", Number, MaxHp) : "");
        }
        void ChangeMP(float Number)
        {
            float MaxMP = UnityValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_MAXMP;
            this.MP.sliderValue = Number / MaxMP;
            this.MPLabel.SetText(ShowLabel ? string.Format("{0}/{1}", Number, MaxMP) : "");
        }
        //void ChangeEXP(float Number)
        //{
        //    float MaxExp = PlayerValue.PLAYER_FIELD_NEXT_LEVEL_EXP;
        //    this.EXP.sliderValue = Number / MaxExp;
        //    this.EXPLabe.SetText(ShowLabel ? string.Format("{0}/{1}", Number, MaxExp) : "");
        //}

        public void FlagMPProgressBar(object obj)
        {
            MPBackground.BeginFlag(3, 0.5f, Color.white, Color.red, null);
        }

        public void Close()
        {
            transform.localPosition = new Vector3(0, 0, -800);
        }

    }
}
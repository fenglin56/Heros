using UnityEngine;
using System.Collections;

namespace UI.Battle
{

    public class HeroResurrectionPanel_V2 : View
    {
        public GameObject ResurrectionBtnPrefab;
        public GameObject ResurrectionPanelPrefab;

        private SingleButtonCallBack resurrectBtn;
        private HeroResurrectionTips heroResurrectionTips;

		private SMSGEctypePlayerRevive_SC m_lastEctypePlayerReviveMsg;

        void Start()
        {
			AddEventHandler(EventTypeEnum.CloseRelivePanel.ToString(), CloseRelivePanelHandle);
            AddEventHandler(EventTypeEnum.EntityDie.ToString(), ReceiveEntityDieHandle);
            AddEventHandler(EventTypeEnum.EntityRelive.ToString(), SetHeroAlive);
			GameDataManager.Instance.dataEvent.RegisterEvent(DataType.CountDownUI, ResetCountDownTimeHandle);
            //StartCoroutine(CheckIsDie());
        }

        IEnumerator CheckIsDie()
        {
            yield return null;
            if ((PlayerManager.Instance.FindHeroEntityModel().Behaviour as PlayerBehaviour).IsDie)
            {
                if (!BattleSceneTrialsEctypeUIManager.Instance.ISTrialsEctype)
                {
                    ShowDeathBtn();
                }
                TraceUtil.Log("角色死亡");
            }
            else
            {
                TraceUtil.Log("角色非死亡");
            }
        }

        void OnDestroy()
        {
			RemoveEventHandler(EventTypeEnum.CloseRelivePanel.ToString(), CloseRelivePanelHandle);
            RemoveEventHandler(EventTypeEnum.EntityDie.ToString(), ReceiveEntityDieHandle);
            RemoveEventHandler(EventTypeEnum.EntityRelive.ToString(), SetHeroAlive); 
			GameDataManager.Instance.dataEvent.RemoveEventHandel(DataType.CountDownUI, ResetCountDownTimeHandle);
			GameDataManager.Instance.ClearData(DataType.CountDownUI);
        }
		void CloseRelivePanelHandle(INotifyArgs inotifyArgs)
		{
			CloseTipsPanel ();		
		}
        /// <summary>
        /// 角色死亡消息
        /// </summary>
        /// <param name="inotifyArgs"></param>
        void ReceiveEntityDieHandle(INotifyArgs inotifyArgs)
        {
            //TraceUtil.Log("收到角色死亡消息:" + BattleSceneTrialsEctypeUIManager.Instance.ISTrialsEctype);
            if (PVPBattleManager.Instance.IsPVPBattle || BattleSceneTrialsEctypeUIManager.Instance.ISTrialsEctype)
                return;
            SMsgActionDie_SC sMsgActionDie_SC = (SMsgActionDie_SC)inotifyArgs;
            if (sMsgActionDie_SC.uidEntity == PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity)
            {
                //Debug.LogWarning("收到死亡消息：" + sMsgActionDie_SC.uidEntity + "玩家ID：" + PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity);
                ShowDeathBtn();
            }
        }
        /// <summary>
        /// 角色复活消息
        /// </summary>
        /// <param name="inotifyArgs"></param>
        void SetHeroAlive(INotifyArgs inotifyArgs)
        {
            SMsgActionRelivePlayer_SC sMsgActionRelivePlayer_SC = (SMsgActionRelivePlayer_SC)inotifyArgs;
            if (sMsgActionRelivePlayer_SC.actorTarget == PlayerManager.Instance.FindHeroDataModel().ActorID)
            {
                //Debug.LogWarning("收到复活角色消息,Target:" + sMsgActionRelivePlayer_SC.UIDTarget + ",MyUID:" + PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity);
                CloseDeathBtn();
                CloseTipsPanel();
            }
        }

        public void ShowDeathBtn()
        {
//            if (resurrectBtn == null)
//            {
//                resurrectBtn = CreatObjectToNGUI.InstantiateObj(ResurrectionBtnPrefab,transform).GetComponent<SingleButtonCallBack>();
//                resurrectBtn.SetCallBackFuntion(ShowHeroResurrectionTips);
//            }
			ShowHeroResurrectionTips(null);
        }

        public void CloseDeathBtn()
        {
            if (resurrectBtn != null && resurrectBtn.gameObject != null)
            {
                Destroy(resurrectBtn.gameObject);
            }
        }

        public void ShowHeroResurrectionTips(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            //CloseDeathBtn();
            if (heroResurrectionTips == null)
            {
                heroResurrectionTips = CreatObjectToNGUI.InstantiateObj(ResurrectionPanelPrefab, transform).GetComponent<HeroResurrectionTips>();
                heroResurrectionTips.ShowMyself(this);


				if(GameDataManager.Instance.DataIsNull(DataType.CountDownUI))
				{
					int CurrentTime = EctypeManager.Instance.GetCurrentEctypeData().ReviveTime;
					heroResurrectionTips.ResetCutDownTime(CurrentTime);
				}
				else//如果重连
				{
					ResetCountDownTimeHandle(GameDataManager.Instance.GetData(DataType.CountDownUI));					
				}
			}
        }

        public void CloseTipsPanel()
        {
            if (heroResurrectionTips != null && heroResurrectionTips.gameObject != null)
            {				 
                Destroy(heroResurrectionTips.gameObject);
				heroResurrectionTips = null;
            }
        }

		void ResetCountDownTimeHandle(object obj)
		{
			SMSGEctypePlayerRevive_SC sMSGEctypePlayerRevive_SC = (SMSGEctypePlayerRevive_SC)obj;
			if(sMSGEctypePlayerRevive_SC.dwActorID != PlayerManager.Instance.FindHeroDataModel().ActorID)
				return;
			m_lastEctypePlayerReviveMsg = sMSGEctypePlayerRevive_SC;
			float CurrentTime = EctypeManager.Instance.GetCurrentEctypeData().ReviveTime -
				(sMSGEctypePlayerRevive_SC.dwReliveTime /1000f - (Time.realtimeSinceStartup - sMSGEctypePlayerRevive_SC.ReceiveMsgTime));
			if(heroResurrectionTips!= null)
			{
				heroResurrectionTips.ResetCutDownTime(CurrentTime);
			}
		}

        protected override void RegisterEventHandler()
        {
            throw new System.NotImplementedException();
        }
    }
}
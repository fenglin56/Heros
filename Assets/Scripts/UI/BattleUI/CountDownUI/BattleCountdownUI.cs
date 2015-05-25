using UnityEngine;
using System.Collections;

namespace UI.Battle
{

    public class BattleCountdownUI : View
    {
        private static BattleCountdownUI m_instance;
        public static BattleCountdownUI Instance { get { return m_instance; } }

        public GameObject CountDownUIPrefab;
        private CountdownUITips CountUIScript;

        private GameObject CountUIObj;

        //private int Timer = 25;
        private int CurrentTime=0;

        bool IsShow = false;

        public void Awake()
        {
            m_instance = this;
            //UIEventManager.Instance.RegisterUIEvent(UIEventType.CountDownUI, Show);
			//GameDataManager.Instance.dataEvent.RegisterEvent(DataType.CountDownUI,Show);
//			if(!GameDataManager.Instance.DataIsNull(DataType.CountDownUI))
//			{
//				Show(GameDataManager.Instance.PeekData(DataType.CountDownUI));
//			}
            AddEventHandler(EventTypeEnum.EntityRelive.ToString(), Close);
            //AddEventHandler(EventTypeEnum.EctypeSettleAccount.ToString(), Close);
        }
        protected override void RegisterEventHandler()
        {
        }

        void OnDestroy()
        {
            if (IsShow)
            {
                SoundManager.Instance.StopSoundEffect("Sound_UIEff_DeadCountdown");
            }
            m_instance = null;
			//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CountDownUI, Show);
			//GameDataManager.Instance.dataEvent.RemoveEventHandel(DataType.CountDownUI,Show);
			GameDataManager.Instance.ClearData(DataType.CountDownUI);
            RemoveEventHandler(EventTypeEnum.EntityRelive.ToString(), Close);
            //RemoveEventHandler(EventTypeEnum.EctypeSettleAccount.ToString(), Close);
        }

        public void Show(object obj)
        {
            IsShow = true;
            SMSGEctypePlayerRevive_SC sMSGEctypePlayerRevive_SC = (SMSGEctypePlayerRevive_SC)obj;
            if (CountUIObj == null)
            {
                CountUIObj = CreatObjectToNGUI.InstantiateObj(CountDownUIPrefab,BattleUIManager.Instance.GetScreenTransform(ScreenPositionType.Center));
                CountUIScript = CountUIObj.GetComponent<CountdownUITips>();
                //CountUIScript.SetButtonText01(LanguageTextManager.GetString("IDS_H1_197"));
            }
            CountUIObj.transform.localPosition = new Vector3(0,0,-50);
			CurrentTime = (int)sMSGEctypePlayerRevive_SC.dwReliveTime - (int)(Time.realtimeSinceStartup - sMSGEctypePlayerRevive_SC.ReceiveMsgTime);
            StopAllCoroutines();
            StartCoroutine(CountDown());
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_DeadCountdown",true);
        }

        IEnumerator CountDown()
        {
            CountUIScript.Show(CurrentTime);
            yield return new WaitForSeconds(1);
            if (CurrentTime > 0)
            {
                CurrentTime--;
                StartCoroutine(CountDown());
            }
            else
            {
                //SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Dead");
                Close(null);
                ShowGameOverPanel();
            }
        }

        void ShowGameOverPanel()
        {
            //TraceUtil.Log("弹出挑战失败画面");
        }

        public void Close(INotifyArgs iNotifyArgs)
        {
            if (!IsShow)
                return;
            IsShow = false;
            SoundManager.Instance.StopSoundEffect("Sound_UIEff_DeadCountdown");
            if (CountUIObj != null)
            {
                //CountUIObj.transform.localPosition = new Vector3(0,0,-1000);
                StopAllCoroutines();
                Destroy(CountUIObj);
            }
            
        }


    }
}
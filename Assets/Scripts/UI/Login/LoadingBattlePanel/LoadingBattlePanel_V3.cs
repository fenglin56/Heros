using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI
{

    public class LoadingBattlePanel_V3 : IUIPanel
    {
        public class TipsStrList
        {
            public int MyWeights_Min;
            public int MyWeights_Max;
            public string MyStrIDS;
            public bool CheckIsMy(float weights)
            {
                return weights >= MyWeights_Min && weights <= MyWeights_Max;
            }
        }

        public LoadingTipsDataBase loadingTipsDataBase;
        public UILabel LabelTips;
        public UISlider ProgressBar;
		public GameObject loadingTip;
		private UILabel loadingTipLabel;
        //public UILabel LabelName;
        //public UILabel ProgressLabel;
        //public Transform IconPoint;

        //LoadSceneData loadSceneData;

        GameObject DontDestroyObj;

        float chacheValue = 0;

        void Awake()
        {
			loadingTipLabel = loadingTip.transform.Find("Tip").GetComponent<UILabel>();
			loadingTip.SetActive (false);
			StartMarkTime ();
			UIEventManager.Instance.RegisterUIEvent(UIEventType.LoadingStartDownTime, LoadingStartDownTimeHandler);
			UIEventManager.Instance.RegisterUIEvent (UIEventType.TeamComplete, OnTeamComplete);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.LoadingComplete, OnSceneLoadingComplete);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.LoadingProgress, UpdateProgress);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.OnLostConectEvent,OnLostConectEvent);
            Show();
        }
		void OnLostConectEvent(object obj)
		{
			Close ();	
		}
        public override void Show()
        {
            UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI, UI.MainUI.UIType.Empty);
            //LoadSceneData loadSceneData = GameDataManager.Instance.PeekData(DataType.LoadingSceneData) as LoadSceneData;
            //if (loadSceneData == null)
            //{
            //    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"空引用");
            //}
            //var LoadSceneInfo = (SMsgActionNewWorld_SC)loadSceneData.LoadSceneInfo;
            //this.loadSceneData = loadSceneData;
            this.LabelTips.text = GetTipsStr();
            transform.localPosition = Vector3.zero;
            GetUIRoot(transform);
        }

        string GetTipsStr()
        {
            string getStr = string.Empty;
            int roleLevel = 1;
            RoleSelectItem selectRoleItem = GameDataManager.Instance.PeekData(DataType.SelectRoleData) as RoleSelectItem;
            if (selectRoleItem != null)
            {
                roleLevel = selectRoleItem.ItemDataInfo.lLevel;
            }
            else
            {
                roleLevel = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
                roleLevel = roleLevel == 0 ? 1 : roleLevel;
            }
            var strList = loadingTipsDataBase.LoadingDataList.Where(P => P.Levels_Min <= roleLevel && roleLevel <= P.Levels_Max).ToList();
            int allWeights = 0;
            List<TipsStrList> tipsStrList = new List<TipsStrList>();
            foreach (var child in strList)
            {
                TipsStrList tipsStrInfo = new TipsStrList() { MyWeights_Min = allWeights, MyWeights_Max = allWeights + child.Weights, MyStrIDS = child.LoadingIDS };
                tipsStrList.Add(tipsStrInfo);
                allWeights = tipsStrInfo.MyWeights_Max;
            }
            int LabelIDSIndex = Random.Range(0, allWeights);
			TipsStrList strTip = tipsStrList.FirstOrDefault (P => P.CheckIsMy (LabelIDSIndex));
			if (strTip == null) {
				getStr = "tip is error!!!";			
			} else {
				getStr = LanguageTextManager.GetString(strTip.MyStrIDS);			
			}
            TraceUtil.Log("介绍文字：" + getStr+",Level:"+roleLevel);
            return getStr;
        }

        void GetUIRoot(Transform Trs)
        {
            if (Trs.parent != null)
            {
                GetUIRoot(Trs.parent);
            }
            else
            {
                this.DontDestroyObj = Trs.gameObject;
                this.DontDestroyObj.AddComponent<DontDestroy>();
            }
        }

        void UpdateProgress(object obj)
        {
            float newValue = (float)obj;
            if (chacheValue < newValue)
            {
                chacheValue = newValue;
                this.ProgressBar.sliderValue = newValue;
            }
            //this.ProgressLabel.text = string.Format("{0}%", (int)(loadSceneData.Progress * 100));

        }
		private float loadingStartTime = -1;
		private void StartMarkTime()
		{
			loadingStartTime = Time.realtimeSinceStartup;
		}
		private float loadDownTime = 0;
		private float loadMarkTime ;
		private bool isRead = false;
		//组队中显示加载倒计时//
		void LoadingStartDownTimeHandler(object obj)
		{
			if (isRead)
				return;
			isRead = true;
			loadingTip.SetActive (true);
			loadDownTime = CommonDefineManager.Instance.CommonDefine.EctypeLoadingWaitingTime - (Time.realtimeSinceStartup - loadingStartTime);
			loadMarkTime = Time.realtimeSinceStartup;
			if (IsInvoking ("DownTimeFun")) {
				CancelInvoke("DownTimeFun");			
			}
			InvokeRepeating ("DownTimeFun",0.1f,0.5f);
			loadingTipLabel.text = loadDownTime.ToString();
		}
		void DownTimeFun()
		{
			float curDownTime = loadDownTime - (Time.realtimeSinceStartup - loadMarkTime) ;
			if (curDownTime < 0.1) {
				curDownTime = 0;
				CancelInvoke("DownTimeFun");
			}
			loadingTipLabel.text = "downtime:"+(int)curDownTime;
		}
		void OnTeamComplete(object obj)
		{
			DestroyPanel();
		}
        void OnSceneLoadingComplete(object obj)
        {
			//当此时为组队时，先不进入//
			//Debug.Log("IsTeamExist()=="+TeamManager.Instance.IsTeamExist()+" isTeamBattleMark="+GameManager.Instance.isTeamBattleMark);
			if (TeamManager.Instance.IsTeamExist()&&GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_BATTLE)//GameManager.Instance.isTeamBattleMark) 
			{
				return;			
			}
            DestroyPanel();
        }

        public override void Close()
        {
			if (IsInvoking ("DownTimeFun")) {
				CancelInvoke("DownTimeFun");			
			}
            transform.localPosition = new Vector3(0, 0, -1000);
        }

        public override void DestroyPanel()
        {
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.LoadingStartDownTime, LoadingStartDownTimeHandler);
			UIEventManager.Instance.RemoveUIEventHandel (UIEventType.TeamComplete, OnTeamComplete);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.LoadingProgress, UpdateProgress);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.LoadingComplete, OnSceneLoadingComplete);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OnLostConectEvent,OnLostConectEvent);
			if (IsInvoking ("DownTimeFun")) {
				CancelInvoke("DownTimeFun");			
			}
			Destroy(DontDestroyObj);
        }
    }
}
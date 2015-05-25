using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.MainUI;

namespace UI
{
    /// <summary>
    /// 在TownUI场景的 Town_Panel->SystemFunctionButtonController 
    /// </summary>
    public class SystemFuntionButton : View
    {
        /// <summary>
        /// 系统右下角常驻按钮，此为按钮总控制器，右下角添加隐藏按钮等动作都在此处执行
        /// </summary>

        static SystemFuntionButton instance;
        public static SystemFuntionButton Instance { get { return instance; } }

        public SysBtnPanel SysButtonPanel_BottomRight;//下排按钮
        public SysBtnPanel SysButtonPanel_TopRight;//右排按钮
        public SysBtnPanel SysButtonPanel_TopLeft;//
        public UI.MainUI.CommonUITitle_v2 CommonUITitle_Top;
        public SingleButtonCallBack SysButton;          //主按钮
        /// <summary>
        /// 功能按钮开放特效1
        /// </summary>
        public GameObject ButtonEnableEffect;
        /// <summary>
        /// 功能按钮开放特效2
        /// </summary>
        public GameObject ButtonEnablePic;
        public Camera UiCamera;
        
        public bool MyBtnEneble = false;
        public bool Show { get; private set; }
        /// <summary>
        /// 已经开放的功能按钮配置信息
        /// </summary>
        private List<MainTownButtonConfigData> m_buttonList = new List<MainTownButtonConfigData>();  
        /// <summary>
        /// 记录要开放的功能按钮类型
        /// </summary>
        private UIType m_addSystemFunc = UIType.Empty;
        private GameObject m_buttonEnablePic;

        //public NewUIDataManager newUIDataManager;

       // private static int m_enableIndex = 0;
        //public UILabel DesLabel;

        private int m_guideBtnID;

        private GameObject m_rotationObj;
        void Awake()
        {
            this.RegisterEventHandler();
            SysButtonPanel_BottomRight.transform.localPosition=new Vector3(CommonDefineManager.Instance.CommonDefine.TownstartPoint3.BasePostion.x,CommonDefineManager.Instance.CommonDefine.TownstartPoint3.BasePostion.y,0);
            SysButtonPanel_TopRight.transform.localPosition=new Vector3(CommonDefineManager.Instance.CommonDefine.TownstartPoint2.BasePostion.x,CommonDefineManager.Instance.CommonDefine.TownstartPoint2.BasePostion.y,0);
            SysButtonPanel_TopLeft.transform.localPosition=new Vector3(CommonDefineManager.Instance.CommonDefine.TownstartPoint1.BasePostion.x,CommonDefineManager.Instance.CommonDefine.TownstartPoint1.BasePostion.y,0);
            instance = this;
            m_rotationObj = SysButton.transform.FindChild("RotationObj").gameObject;
        }

        void Start()
        {
            CloseCommonTop();
            if(SysButtonPanel_BottomRight == null)
            SysButtonPanel_BottomRight = transform.FindChild("ButtonPanel_Bottom").GetComponent<SysBtnPanel>();
            if(SysButtonPanel_TopRight==null)
            SysButtonPanel_TopRight = transform.FindChild("ButtonPanel_Right").GetComponent<SysBtnPanel>();
           // if (GameManager.Instance.MainButtonIndex != 0)
                InitMainUIButton(GameManager.Instance.MainButtonIndex);
            SysButton.SetCallBackFuntion(OnSysBtnClick);
            //如果有新任务，先禁用主按钮
            //if (NewbieGuideManager_V2.Instance.m_isStartTask)
            //{
            //    MyBtnEneble = false;
            //}
			OpenMainUIEvent (null);
			CloseBtnPanelHandle (null);
        }

        void OnDestroy()
        {
            instance = null;
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.InitMainButton, InitMainUIButton);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpentMainUI, OpenMainUIEvent);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CloseMainUI, CloseBtnPanelHandle);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EnableMainButton, AddMainButton);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ShowTopCommonUI, ShowCommonTop);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpenSystemButton, OnSysBtnClick);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.PlayMainBtnAnim, PlayMainBtnAnimation);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.StopMainBtnAnim, StopMainBtnAnimation);

            //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
            //this.RemoveEventHandler(EventTypeEnum.CloseSystemMainBtn.ToString(), CloseBtnPanelHandle);
        }
		
		private bool m_isInit = false;
		
        void InitMainUIButton(object obj)
        {
			if(m_isInit)
				return;
			
            EnableMyself();
            GameManager.Instance.MainButtonIndex = (int)obj;

            UIType[] uiType =NewUIDataManager.Instance.InitMainButtonList.SingleOrDefault(P => P.ButtonProgress == GameManager.Instance.MainButtonIndex).MainButtonList;
            var mainBtnList = NewUIDataManager.Instance.TownMainButtonList;
            m_buttonList.Clear();
        
            for (int i = 0; i < uiType.Length; i++)
            {
                var enableItem = mainBtnList.Single(P => P.ButtonFunc == uiType[i]);
                m_buttonList.Add(enableItem);
            }

            //SysButtonPanel_Bottom.ResetSystemPanel();
            //m_buttonList.Sort(new MainButtonComparer());
            m_buttonList.Sort(SortBtn);
            foreach (var item in m_buttonList)
            {
               AddSysFuntionButton(item);
            }
			
			m_isInit = true;
            UIEventManager.Instance.TriggerUIEvent(UIEventType.TownUIBtnLoadComplete, null);
        }

        void PlayMainBtnAnimation(object obj)
        {
            UIType uitype = (UIType)obj;
            //TraceUtil.Log("按钮收到播放按钮动画消息:"+uitype);
            SysButtonPanel_TopLeft.PlayBtnAnimation(uitype);
            SysButtonPanel_TopRight.PlayBtnAnimation(uitype);
            SysButtonPanel_BottomRight.PlayBtnAnimation(uitype);
            ShowMainBtnEffect(obj);
        }

       

        void StopMainBtnAnimation(object obj)
        {
            UIType uitype = (UIType)obj;
			SysButtonPanel_TopLeft.StopBtnAnimation(uitype);
            SysButtonPanel_BottomRight.StopBtnAnimation(uitype);
            SysButtonPanel_TopRight.StopBtnAnimation(uitype);
            HideMainBtnEffect(obj);
        }

        void ShowMainBtnEffect(object obj)
        {
            UIType uitype = (UIType)obj;
            //TraceUtil.Log("按钮收到播放按钮动画消息:"+uitype);
            SysButtonPanel_TopLeft.ShowBtnEffect(uitype);
            SysButtonPanel_TopRight.ShowBtnEffect(uitype);
            SysButtonPanel_BottomRight.ShowBtnEffect(uitype);
        }

        void HideMainBtnEffect(object obj)
        {
            UIType uitype = (UIType)obj;
            SysButtonPanel_TopLeft.HideBtnEffect(uitype);
            SysButtonPanel_BottomRight.HideBtnEffect(uitype);
            SysButtonPanel_TopRight.HideBtnEffect(uitype);
        }
        /// <summary>
        /// 处理开放主按钮消息，播放开启特效
        /// </summary>
        /// <param name="obj"></param>
        void AddMainButton(object obj)
        {
            m_addSystemFunc = (UIType)obj;
            var buttonList = NewUIDataManager.Instance.TownMainButtonList;
            MainTownButtonConfigData enableItem = null;
            //找到要开放的功能按钮配置信息
            for (int i = 0; i < buttonList.Count; i++)
            {
                if (buttonList[i].ButtonFunc == m_addSystemFunc)
                    enableItem = buttonList[i];
            }

            if (enableItem == null)
            {
                TraceUtil.Log("要开启主按钮，配置表中无对应！");
                return;
            }
            //加入到已开放功能按钮配置信息列表
            if (!m_buttonList.Exists(P => P.ButtonFunc == enableItem.ButtonFunc))
                m_buttonList.Add(enableItem);
            else
                TraceUtil.Log("此功能按钮已经开启！");

            //SysButtonPanel_Bottom.ResetSystemPanel();
            //SysButtonPanel_Right.ResetSystemPanel();
            m_buttonList.Sort(SortBtn);
            switch(enableItem.ButtonArea)
            {
                case MainTownButtonArea.LeftUp:
                    SysButtonPanel_TopLeft.RestPanel();
                    break;
                case MainTownButtonArea.RightUp:
                    SysButtonPanel_TopRight.RestPanel();
                    break;
                case MainTownButtonArea.RightDown:
                    SysButtonPanel_BottomRight.RestPanel();
                    break;
            }
           
       
            if (m_buttonEnablePic != null)
                Destroy(m_buttonEnablePic);

            foreach (var item in m_buttonList)
            {
                //AddSysFuntionButton(item);
                if (item.ButtonFunc == m_addSystemFunc)
                {
                    TraceUtil.Log("开启新功能：" + m_addSystemFunc+"  "+Time.realtimeSinceStartup);
                    m_buttonEnablePic = CreatObjectToNGUI.Instantiate(ButtonEnablePic) as GameObject;
                    m_buttonEnablePic.transform.parent = this.transform.parent;
                    m_buttonEnablePic.transform.localScale = Vector3.one;

                    m_buttonEnablePic.GetComponent<EnableFunctionPanel>().InitPanel(m_addSystemFunc, SysButton.transform,this);
                }

            }
       
            StartCoroutine(Creat());
        }

        IEnumerator Creat()
        {
            yield return null;
            
            if (m_addSystemFunc != UIType.Empty)
            {
                foreach (var item in m_buttonList)
                {
                    AddSysFuntionButton(item);
                }
            }
            UIEventManager.Instance.TriggerUIEvent(UIEventType.TownUIBtnLoadComplete, null);
        }
        private int SortBtn(MainTownButtonConfigData x,MainTownButtonConfigData y)
        {
            int XWh=((int)x.ButtonArea)*1000+x.Button_Row*100+x.Button_ListSequence;
            int YWh=((int)y.ButtonArea)*1000+y.Button_Row*100+y.Button_ListSequence;
            return XWh-YWh;
        }
        /// <summary>
        /// 初始化或点击展开
        /// </summary>
        /// <param name="item"></param>
        public void AddSysFuntionButton(MainTownButtonConfigData item)//MainUI.UIType BtnType, MainBtnArea btnPos, int BtnNum)//添加按钮,分别为按钮类型，按钮所在屏幕位置，按钮所在位置的号码
        {
            if (!MyBtnEneble)
                return;
            switch (item.ButtonArea)
            {
                case MainTownButtonArea.LeftUp:
                    if (!SysButtonPanel_TopLeft.ContainsButton(item.ButtonFunc))
                    {
                        SysButtonPanel_TopLeft.InsertBtn(item);
                        if (m_addSystemFunc != UIType.Empty)
                        {
                            SysButtonPanel_TopLeft.ShowEnableEffect(ButtonEnableEffect, m_addSystemFunc);
                            m_addSystemFunc = UIType.Empty;
                        }
                    }
                    break;
                case MainTownButtonArea.RightUp:
                    if (!SysButtonPanel_TopRight.ContainsButton(item.ButtonFunc))
                    {
                        SysButtonPanel_TopRight.InsertBtn(item);
                        if (m_addSystemFunc != UIType.Empty)
                        {
                            SysButtonPanel_TopRight.ShowEnableEffect(ButtonEnableEffect, m_addSystemFunc);
                            m_addSystemFunc = UIType.Empty;
                        }
                    }
                    break;
                case MainTownButtonArea.RightDown:
                    if (!SysButtonPanel_BottomRight.ContainsButton(item.ButtonFunc))
                    {
                        SysButtonPanel_BottomRight.InsertBtn(item);
                        if (m_addSystemFunc != UIType.Empty)
                        {
                            SysButtonPanel_BottomRight.ShowEnableEffect(ButtonEnableEffect, m_addSystemFunc);
                            m_addSystemFunc = UIType.Empty;
                        }
                    }

                    break;
        
                default:
                    break;
            }
        }

        public void RemoveFuntionButton(MainUI.UIType BtnType,MainBtnArea btnPos)//移除按钮
        {
            if (!MyBtnEneble)
                return;
            switch (btnPos)
            {
                case MainBtnArea.TopLeft:
                    if (SysButtonPanel_TopLeft.ContainsButton(BtnType))
                    {
                        SysButtonPanel_TopLeft.RemoveButton(BtnType);
                    }
                    break;
                case MainBtnArea.TopRight:
                    if (SysButtonPanel_TopRight.ContainsButton(BtnType))
                    {
                        SysButtonPanel_TopRight.RemoveButton(BtnType);
                    }
                    break;
                case MainBtnArea.bottomRight:
                    if (SysButtonPanel_BottomRight.ContainsButton(BtnType))
                    {
                        SysButtonPanel_BottomRight.RemoveButton(BtnType);
                    }
                    break;
                default:
                    break;
            }
        }

        public void ShowBtnPanel()//展开按钮
        {
            if (!MyBtnEneble)
                return;
			//send event to joystick of TownUI
			UIEventManager.Instance.TriggerUIEvent(UIEventType.MainBtnOpenEvent, null);
            //DesLabel.enabled = true;
            Show = true;
//            if (m_addSystemFunc != UIType.Empty)
//            {
//                foreach (var item in m_buttonList)
//                {
//                    AddSysFuntionButton(item);
//                }
//            }
            //Debug.Log("showpanel");
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            SysButtonPanel_BottomRight.ShowPanelBtn();
            SysButtonPanel_TopRight.ShowPanelBtn();
            SysButtonPanel_TopLeft.ShowPanelBtn();
           
            TweenRotation.Begin(m_rotationObj, 0.1f, Quaternion.Euler(new Vector3(0, 0, 120)), OpenRotateStep1);
          
        }

        void OpenRotateStep1(object obj)
        {
            TweenRotation.Begin(m_rotationObj, 0.1f, Quaternion.Euler(new Vector3(0, 0, 240)), OpenRotateStep2);
        }
        void OpenRotateStep2(object obj)
        {
            TweenRotation.Begin(m_rotationObj, 0.1f, Quaternion.Euler(new Vector3(0, 0, 360)));
        }

        public void CloseBtnpanel()//收拢按钮
        {
            if (!Show || !MyBtnEneble)
                return;
			//send event to joystick of TownUI
			UIEventManager.Instance.TriggerUIEvent(UIEventType.MainBtnCloseEvent, null);
            Show = false;
            //DesLabel.enabled = false;
			CloseCommonTop();
            SysButtonPanel_BottomRight.ClosePanleBtn();
            SysButtonPanel_TopRight.ClosePanleBtn();
            SysButtonPanel_TopRight.DelEnableEffect();
            SysButtonPanel_BottomRight.DelEnableEffect();
            SysButtonPanel_TopLeft.ClosePanleBtn();
            TweenRotation.Begin(m_rotationObj, 0.1f, Quaternion.Euler(new Vector3(0, 0, 240)), CloseRotateStep1);

        }

        public void ShowCommonTop(object obj)
        {
            if(CommonUITitle_Top.gameObject != null)
                TweenPosition.Begin(CommonUITitle_Top.gameObject, 0.1f, new Vector3(230,100, 50), new Vector3(230, -60, 50)); 
        }

        public void OpenMainUIEvent(object obj)
        {
            CloseBtnpanel();
        }

        public void CloseCommonTop()
        {
            if (CommonUITitle_Top.gameObject != null)
                TweenPosition.Begin(CommonUITitle_Top.gameObject, 0.1f, new Vector3(230, -60, 50), new Vector3(230, 100, 50)); 
        }

        void CloseRotateStep1(object obj)
        {
            TweenRotation.Begin(m_rotationObj, 0.1f, Quaternion.Euler(new Vector3(0, 0, 120)), CloseRotateStep2);
        }
        void CloseRotateStep2(object obj)
        {
            TweenRotation.Begin(m_rotationObj, 0.1f, Quaternion.Euler(new Vector3(0, 0, 0)));
        }


        void OnSysBtnClick(object obj)//主按钮点击事件
        {
           // TraceUtil.Log("@@@@@@@@@@@@@@@@@@@@@OnSysBtnClick");
            if (!MyBtnEneble)
                return;
            if (!Show)
            {
                ShowBtnPanel();
                ShowCommonTop(null);
                
            }
            else
            {
                CloseBtnpanel();
                CloseCommonTop();
            }
        }

        public void EnableMyself()//显示右下角菜单按钮时调用
        {
            transform.localPosition = new Vector3(0, 0, 600);
            MyBtnEneble = true;
        }

        public void DisableMyself()//隐藏右下角菜单按钮时调用
        { 
            transform.localPosition = new Vector3(0,0,-800);
            CloseBtnpanel();
            MyBtnEneble = false;
        }


        protected override void RegisterEventHandler()
        {
            UIEventManager.Instance.RegisterUIEvent(UIEventType.InitMainButton, InitMainUIButton);  //初始化功能按钮消息
            UIEventManager.Instance.RegisterUIEvent(UIEventType.OpentMainUI, OpenMainUIEvent);      //打开主界面消息（收起功能按钮）
            UIEventManager.Instance.RegisterUIEvent(UIEventType.CloseMainUI, CloseBtnPanelHandle);  //关闭主界面消息（关闭通用标题，收起功能按钮）
            UIEventManager.Instance.RegisterUIEvent(UIEventType.EnableMainButton, AddMainButton);   //开启新功能按钮
            UIEventManager.Instance.RegisterUIEvent(UIEventType.ShowTopCommonUI, ShowCommonTop);    //显示屏幕上方通用标题栏
            UIEventManager.Instance.RegisterUIEvent(UIEventType.OpenSystemButton, OnSysBtnClick);   //主按钮点击展开事件
            UIEventManager.Instance.RegisterUIEvent(UIEventType.PlayMainBtnAnim, PlayMainBtnAnimation);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.StopMainBtnAnim, StopMainBtnAnimation);
//            UIEventManager.Instance.RegisterUIEvent(UIEventType.PlayMainBtnAnim, ShowMainBtnEffect);
//            UIEventManager.Instance.RegisterUIEvent(UIEventType.StopMainBtnAnim, HideMainBtnEffect);

        }

        private void CloseBtnPanelHandle(object obj)
        {
            //TraceUtil.Log("##############CloseBtnPanelHandle###" + obj);
            CloseCommonTop();

            if (Show)
            {
                CloseBtnpanel();
                Show = false;
            }
        }
    }

    public enum MainBtnArea//按钮位置
    {
        TopLeft=1,
        TopRight,
        bottomRight,
       
    }
    
    //public enum SysButtonType
    //{
    //    HeroInfo =1,//人物
    //    PackInfo,//背包
    //    TaskInfo,//任务
    //    SkillsInfo,//技能
    //    EctypeInfo,//队伍
    //    SocialInfo,//社交
    //    ShopInfo,//商城
    //    SystemSetting = 9,//系统设置
    //}

}

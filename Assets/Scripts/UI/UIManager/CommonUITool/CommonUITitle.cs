using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{

    public class CommonUITitle : View
    {

        //public UILabel VocationLabel;
        public UISprite HeadIconSprite;
        public SpriteSwith VocationSprite;
        public UILabel NameLabel;
        public UILabel LevelLabel;
        public UISlider ExpBar;
        public UISlider EnegryBar;
        public UILabel EnegryTimeLabel;
        public UILabel ExpLabel;
        public SingleButtonCallBack GoldMoneyBtn;
        public SingleButtonCallBack CopperCoinBtn;
        public SingleButtonCallBack EnegryBtn;
        public SingleButtonCallBack RoleInfoBtn;

        public bool IsResidentUI = true;//是否为主界面常驻UI，由于后来分成了主界面和每个UI面板上的顶部栏不一样，除了主城镇上的，其他都为false

        public GameObject BuyVigourMessagePrefab;
        private VigourMessagePanel BuyVigourMessagePanel;

        private int currentEnegry=0, currentGoldMoney=0, currentCoppercoin=0;
		private GameObject TweenFloatObj_Enegry,TweenFloatObj_GoldMoney,TweenFloatObj_Coppercoin,TweenFloatObj_Exp;
        private float currentExp=0;

        private int[] m_guideBtnID = new int[4];
        
        void Start()
        {
            GoldMoneyBtn.SetCallBackFuntion(OnGoleMoneyBtnClick);
            EnegryBtn.SetCallBackFuntion(OnAddVigourBtnClick);
            if (RoleInfoBtn != null)
            {
                RoleInfoBtn.SetCallBackFuntion(OnHeroInfoBtnClick);
            }
            if (IsResidentUI)
            {
                UIEventManager.Instance.RegisterUIEvent(UIEventType.NoEnoughActiveLife, ShowNoEnoughVigourPanel);
                UIEventManager.Instance.RegisterUIEvent(UIEventType.NotEnoughGoldMoney, ShowNotEnoughGoldMoneyMsg);
                UIEventManager.Instance.RegisterUIEvent(UIEventType.SwitchOffShowPlayerInfo, SwichOffShowPlayerInfo);
            }
            AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
            DoForTime.DoFunForFrame(1, SetPanelForTime,null);
            //TODO GuideBtnManager.Instance.RegGuideButton(GoldMoneyBtn.gameObject, UIType.Empty, SubType.TopCommon, out m_guideBtnID[0]);
            //TODO GuideBtnManager.Instance.RegGuideButton(CopperCoinBtn.gameObject, UIType.Empty, SubType.TopCommon, out m_guideBtnID[1]);
            //TODO GuideBtnManager.Instance.RegGuideButton(EnegryBtn.gameObject, UIType.Empty, SubType.TopCommon, out m_guideBtnID[2]);
            if (RoleInfoBtn != null)
            {
                //TODO GuideBtnManager.Instance.RegGuideButton(RoleInfoBtn.gameObject, UIType.Empty, SubType.TopCommon, out m_guideBtnID[3]);
            }
            UIEventManager.Instance.RegisterUIEvent(UIEventType.UpdateEnegryTimeEvent, ShowEnemgryLeftTimeBar);
            ShowEnemgryLeftTimeBar(null);
        }

        void SetPanelForTime(object obj)
        {
            var m_HeroDataModel = PlayerManager.Instance.FindHeroDataModel();
            currentEnegry = m_HeroDataModel.PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE-1;
            currentGoldMoney = m_HeroDataModel.PlayerValues.PLAYER_FIELD_BINDPAY-1;
            currentCoppercoin = m_HeroDataModel.PlayerValues.PLAYER_FIELD_HOLDMONEY-1;
            currentExp =( (float)m_HeroDataModel.PlayerValues.PLAYER_FIELD_EXP / (float)m_HeroDataModel.PlayerValues.PLAYER_FIELD_NEXT_LEVEL_EXP)-0.01f;
            SetPanelInfo();
        }

        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.UpdateEnegryTimeEvent, ShowEnemgryLeftTimeBar);
            RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify); if (IsResidentUI)
            {
                UIEventManager.Instance.RemoveUIEventHandel(UIEventType.NoEnoughActiveLife, ShowNoEnoughVigourPanel);
                UIEventManager.Instance.RemoveUIEventHandel(UIEventType.NotEnoughGoldMoney, ShowNotEnoughGoldMoneyMsg);
                UIEventManager.Instance.RemoveUIEventHandel(UIEventType.SwitchOffShowPlayerInfo, SwichOffShowPlayerInfo);
            }
            for (int i = 0; i < m_guideBtnID.Length; i++ )
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }
			if(TweenFloatObj_Coppercoin!=null){DestroyImmediate(TweenFloatObj_Coppercoin);}
			if(TweenFloatObj_Enegry!=null){DestroyImmediate(TweenFloatObj_Enegry);}
			if(TweenFloatObj_Exp!=null){DestroyImmediate(TweenFloatObj_Exp);}
			if(TweenFloatObj_GoldMoney!=null){DestroyImmediate(TweenFloatObj_GoldMoney);}
        }

        private void UpdateViaNotify(INotifyArgs inotifyArgs)//设置各种属性
        {
            EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)inotifyArgs;
            if (entityDataUpdateNotify.IsHero && entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_PLAYER)
            {
                SetPanelInfo();

                int explode = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BREAKOUT_FLAG;

                if (explode == 0)
                {
                    ((PlayerBehaviour)PlayerManager.Instance.FindHeroEntityModel().Behaviour).IsExplodeState = false;
                    PlayerManager.Instance.FindHeroEntityModel().GO.GetComponent<PlayerHurtFlash>().OnBurst(false);
                }
                else
                {
                    ((PlayerBehaviour)PlayerManager.Instance.FindHeroEntityModel().Behaviour).IsExplodeState = true;
                    PlayerManager.Instance.FindHeroEntityModel().GO.GetComponent<PlayerHurtFlash>().OnBurst(true);
                }
            }
        }

        private void SetPanelInfo()
        {
            var m_HeroDataModel = PlayerManager.Instance.FindHeroDataModel();
            int vocationID = m_HeroDataModel.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
            VocationSprite.ChangeSprite(vocationID);
            if (IsResidentUI)
            {
                int fashionID = m_HeroDataModel.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION;
                var resData= CommonDefineManager.Instance.CommonDefineFile._dataTable.HeroIcon_TownAndTeam.FirstOrDefault(P=>P.VocationID == vocationID&&P.FashionID == fashionID);
                if (resData == null) { TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"找不到角色头像,fashionID:" + fashionID); }
                HeadIconSprite.spriteName = resData.ResName;
                //HeadIconSprite.ChangeSprite(m_HeroDataModel.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION);
            }
            NameLabel.SetText(m_HeroDataModel.Name);
            LevelLabel.SetText(string.Format("LV." + m_HeroDataModel.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL));
			float newExpValue = (float)m_HeroDataModel.PlayerValues.PLAYER_FIELD_EXP / (float)m_HeroDataModel.PlayerValues.PLAYER_FIELD_NEXT_LEVEL_EXP;
			if(currentExp!=newExpValue)
			{
				if(TweenFloatObj_Exp!=null){DestroyImmediate(TweenFloatObj_Exp);}
				TweenFloatObj_Exp = TweenFloat.Begin(1, currentExp, newExpValue, SetExpBar);
			}
			int newGoldMoney = m_HeroDataModel.PlayerValues.PLAYER_FIELD_BINDPAY;
			if(currentGoldMoney!=newGoldMoney)
			{
				if(TweenFloatObj_GoldMoney!=null){DestroyImmediate(TweenFloatObj_GoldMoney);}
				TweenFloatObj_GoldMoney = TweenFloat.Begin(1, currentGoldMoney, newGoldMoney, showGoldMoneyLabel);
			}
			int newCoppercoin = m_HeroDataModel.PlayerValues.PLAYER_FIELD_HOLDMONEY;
			if(currentCoppercoin!=newCoppercoin)
			{
				if(TweenFloatObj_Coppercoin!=null){DestroyImmediate(TweenFloatObj_Coppercoin);}
				TweenFloatObj_Coppercoin = TweenFloat.Begin(1,currentCoppercoin,newCoppercoin,ShowCopperCoinLabel);
			}
			int newEnegry = m_HeroDataModel.PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE;
			if(currentEnegry!=newEnegry)
			{
				if(TweenFloatObj_Enegry!=null){DestroyImmediate(TweenFloatObj_Enegry);}
				TweenFloatObj_Enegry = TweenFloat.Begin(1,currentEnegry,newEnegry,ShowEnegryLabel);
			}
        }

        void ShowEnemgryLeftTimeBar(object obj)
        {
            int targetTime = CommonDefineManager.Instance.CommonDefineFile._dataTable.VitRecoverTime*60;
            var m_HeroDataModel = PlayerManager.Instance.FindHeroDataModel();
            int passTime = (int)EnegryColdWorkData.Instance.CurrentPassTime;
            if (PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE >=CommonDefineManager.Instance.CommonDefineFile._dataTable.EnergyMax)
            {
                EnegryBar.sliderValue = 0;
                EnegryTimeLabel.SetText(0);
            }
            else
            {
                //TraceUtil.Log("自动增加活力时间：" + passTime+",GetTime:"+EnegryColdWorkData.Instance.CurrentPassTime);
                EnegryBar.sliderValue = (float)passTime / (float)targetTime;
                int timeLeft = targetTime - passTime;
                EnegryTimeLabel.SetText(timeLeft/60+(timeLeft%60 == 0?0:1));
            }
        }

        string GetClass(int ID)//设置职业
        {
            string Vocation = "";
            switch (ID)
            {
                case 1:
                    Vocation = LanguageTextManager.GetString("IDS_D2_11");
                    break;
                case 2:
                    Vocation = LanguageTextManager.GetString("IDS_D2_12");
                    break;
                case 3:
                    Vocation = LanguageTextManager.GetString("IDS_D2_13");
                    break;
                case 4:
                    Vocation = LanguageTextManager.GetString("IDS_D2_14");
                    break;
                default:
                    break;
            }
            return Vocation;
        }


        void showGoldMoneyLabel(float number)
        {
			currentGoldMoney = (int)number;
            if (number > 9999999)
            {
                GoldMoneyBtn.SetButtonText("9999999+");
            }
            else
            {
                GoldMoneyBtn.SetButtonText(((int)number).ToString());
            }
        }

        void ShowCopperCoinLabel(float number)
        {
			currentCoppercoin = (int)number;
            if (number > 9999999)
            {
                CopperCoinBtn.SetButtonText("9999999+");
            }
            else
            {
                CopperCoinBtn.SetButtonText(((int)number).ToString());
            }
        }

        void ShowEnegryLabel(float number)
        {
			currentEnegry = (int)number;
            if (number > 9999999)
            {
                EnegryBtn.SetButtonText("9999999+");
            }
            else
            {
                EnegryBtn.SetButtonText(((int)number).ToString());
            }
        }

        void SetExpBar(float number)
        {
            if (float.IsNaN(number))
                return;
			currentExp = number;
            ExpBar.sliderValue = number;
            ExpLabel.SetText(string.Format("{0}%", (int)(number*100)));
        }
        

        void OnGoleMoneyBtnClick(object obj)
        {
            TraceUtil.Log("打开充值界面");
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");            
			UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI, UIType.TopUp);
        }

        void OnAddVigourBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            if (BuyVigourMessagePanel == null) { BuyVigourMessagePanel = CreatObjectToNGUI.InstantiateObj(BuyVigourMessagePrefab, transform).GetComponent<VigourMessagePanel>(); }
            string ShowStr = string.Format(LanguageTextManager.GetString("IDS_H1_158"), CommonDefineManager.Instance.CommonDefine.EnergyAdd);
            BuyVigourMessagePanel.Show(string.Format("{0}\n{1}", LanguageTextManager.GetString("IDS_H1_166"), ShowStr));
        }
        //新版本点人物头像没反应
        void OnHeroInfoBtnClick(object obj)
        {
            //UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI,UIType.HeroInfo);
        }

        void ShowNoEnoughVigourPanel(object obj)
        {
            if (BuyVigourMessagePanel == null) { BuyVigourMessagePanel = CreatObjectToNGUI.InstantiateObj(BuyVigourMessagePrefab, transform).GetComponent<VigourMessagePanel>(); }
            string ShowStr = string.Format(LanguageTextManager.GetString("IDS_H1_158"), CommonDefineManager.Instance.CommonDefine.EnergyAdd);
            BuyVigourMessagePanel.Show(string.Format("{0}\n{1}", LanguageTextManager.GetString("IDS_H1_161"), ShowStr));
        }

        void ShowNotEnoughGoldMoneyMsg(object obj)
        {
            //MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H2_44"), LanguageTextManager.GetString("IDS_H2_55"), null);
            //MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_H2_44"),1);
			MessageBox.Instance.ShowNotEnoughGoldMoneyMsg();
        }

        void SwichOffShowPlayerInfo(object obj)
        {
            bool isShow = (bool)obj;
            VocationSprite.gameObject.SetActive(isShow);
            NameLabel.gameObject.SetActive(isShow);
            LevelLabel.gameObject.SetActive(isShow);
            ExpBar.gameObject.SetActive(isShow);
        }

        protected override void RegisterEventHandler()
        {
            throw new System.NotImplementedException();
        }
    }
}
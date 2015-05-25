using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.Battle
{

    public class BattleSettlementScorePanel_V3 : MonoBehaviour
    {


        public UISprite RoleIcon;
        public SpriteSwith ProfesionIcon;
        public UILabel NameLabel;
        public UILabel LevelLabel;
        public UISlider ExpSlider;
//        public UILabel DoubleKillLabel;
//        public UISlider DoubleKillSlider;
        public UILabel ForceLabel;

        public UILabel GetMoneyLabel;
        public UILabel GetExpLabel;
		public GameObject VipExpTipsPrefab;
        public Transform VipExpEffectPoint;
		public UILabel MaxHitNumberLabel;
        public UILabel RemainHPLabel;
        public UILabel PassLevelTimeLabel_Minute;
        public UILabel PassLevelTimeLabel_Second;
        //public UILabel PassLevelTimeLabel_Ms;
        public UILabel GradLabel;
        public UISlider GradBar;
		public Transform GradPos;

        public UIPanel UpPanel;
        public UIPanel DownPanel;

        public Transform CreatUpgradeEffectPoint;
        public GameObject UpgradeLevelEffectPrefab;

        public SMSGEctypeSettleAccounts sMSGEctypeSettleAccounts { get; private set; }
        public BattleSettlementPanel_V3 MyParent { get; private set; }
        public EctypeContainerData MyEctypeData { get; private set; }

        public Vector3 upFromPos = new Vector3(-450,50,0);
        public Vector3 upToPos = new Vector3(220,50,0);
        public Vector3 downFromPos = new Vector3(250,-150,0);
        public Vector3 downToPos = new Vector3(220,-150,0);
        float TweenAnimTime = 0.2f;

        void Awake()
        {
            ExpSlider.sliderValue = 0;
            //DoubleKillSlider.sliderValue = 0;
            NameLabel.text = "";
            LevelLabel.text = "";
            //DoubleKillLabel.text = "";
            ForceLabel.text = "";

            GetMoneyLabel.text = "";
            GetExpLabel.text = "";
            MaxHitNumberLabel.text = "";
            PassLevelTimeLabel_Minute.text = "";
            PassLevelTimeLabel_Second.text = "";
            RemainHPLabel.text = "";
            GradLabel.SetText("");
            GradBar.sliderValue = 0;
            //PassLevelTimeLabel_Ms.text = "";
        }

        public void Show(SMSGEctypeSettleAccounts sMSGEctypeSettleAccounts, BattleSettlementPanel_V3 myParent)
        {
            SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC = (SMSGEctypeInitialize_SC)GameDataManager.Instance.PeekData(DataType.InitializeEctype);
            MyEctypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[sMSGEctypeInitialize_SC.dwEctypeContainerId];
            UpPanel.alpha = 0;
            DownPanel.alpha = 0;
            this.sMSGEctypeSettleAccounts = sMSGEctypeSettleAccounts;
            this.MyParent = myParent;
            var playerData = PlayerManager.Instance.FindHeroDataModel();
            int vocation = playerData .PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
            //RoleIcon.ChangeSprite(vocation);
            ProfesionIcon.ChangeSprite(vocation);
            NameLabel.SetText(playerData.Name);
            TweenMovePanel();
            UpdateHeroIcon();
        }

        void UpdateHeroIcon()
        {
            var m_HeroDataModel = PlayerManager.Instance.FindHeroDataModel();
            int vocationID = m_HeroDataModel.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
            int fashionID = m_HeroDataModel.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION;
            var resData = CommonDefineManager.Instance.CommonDefineFile._dataTable.HeroIcon_BattleReward.FirstOrDefault(P => P.VocationID == vocationID && P.FashionID == fashionID);
            if (resData == null) { TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"找不到角色头像,fashionID:" + fashionID); }
            RoleIcon.spriteName = resData.ResName;
        }

        void TweenMovePanel()
        {
            float animTime = 0.1f;
           
            TweenAlpha.Begin(UpPanel.gameObject, animTime, 0, 1, null);
            TweenAlpha.Begin(DownPanel.gameObject,animTime,0,1,null);
            TweenPosition.Begin(UpPanel.gameObject,animTime,upFromPos,upToPos,null);
            TweenPosition.Begin(DownPanel.gameObject, animTime, downFromPos, downToPos, null);
            DoForTime.DoFunForTime(3, TweenShowPanel01Value01, null);
        }

        void TweenShowPanel01Value01(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_AppraisalFields");
            var playerData = PlayerManager.Instance.FindHeroDataModel();
            int level = playerData.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
            float exp = (float)playerData.PlayerValues.PLAYER_FIELD_EXP / (float)playerData.PlayerValues.PLAYER_FIELD_NEXT_LEVEL_EXP;
            TweenFloat.Begin(TweenAnimTime,0,exp,setExp);
            TweenFloat.Begin(TweenAnimTime,0,level,SetLevel,TweenShowPanel01Value03);
        }
        void setExp(float value)
        {
            ExpSlider.sliderValue = value;
        }
        void SetLevel(float value)
        {
            LevelLabel.SetText((int)value);
        }

//        void TweenShowPanel01Value02(object obj)
//        {
//            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_AppraisalFields");
//            TweenFloat.Begin(TweenAnimTime,0,this.sMSGEctypeSettleAccounts.dwKillPercent,SetDoubleKillLabel,TweenShowPanel01Value03);
//        }
//        void SetDoubleKillLabel(float value)
//        {
//            DoubleKillLabel.SetText((int)value);
//            DoubleKillSlider.sliderValue = value / 100f;
//        }

        void TweenShowPanel01Value03(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_AppraisalFields");
            int atk = HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_Combat, PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHTING);
            TweenFloat.Begin(TweenAnimTime,0,atk,SetForceLabel,null);
            DoForTime.DoFunForTime(TweenAnimTime, TweenShowPanel02Value01, null);
        }
        void SetForceLabel(float value)
        {
            ForceLabel.SetText((int)value);
        }

        void TweenShowPanel02Value01(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_AppraisalFields");
            TweenFloat.Begin(TweenAnimTime, 0, this.sMSGEctypeSettleAccounts.dwBaseMoney+ this.sMSGEctypeSettleAccounts.dwGradeMoney, SetGradMoney, TweenShowPanel02Value02);
        }
        void SetGradMoney(float value)
        {
            string format = LanguageTextManager.GetString("IDS_I4_21");
            string msg = string.Format(format, (int)value);
            GetMoneyLabel.SetText(msg);
        }

        void TweenShowPanel02Value02(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_AppraisalFields");
            TweenFloat.Begin(TweenAnimTime, 0, this.sMSGEctypeSettleAccounts.dwBaseExp+this.sMSGEctypeSettleAccounts.dwGradeExp, SetGradExp, null);
            DoForTime.DoFunForTime(TweenAnimTime, TweenShowPanel03Value01, null);
			if(PlayerDataManager.Instance.GetEctypeExpBonus()>0)
			{
			    //SingleButtonCallBack vipGetExpTips = GetExpLabel.transform.parent.InstantiateNGUIObj(VipExpTipsPrefab).GetComponent<SingleButtonCallBack>();
			    //vipGetExpTips.SetButtonText(string.Format(LanguageTextManager.GetString("IDS_I21_8"),PlayerDataManager.Instance.GetEctypeExpBonus()));
			    //vipGetExpTips.transform.localPosition = GetExpLabel.transform.localPosition+new Vector3(0,0,-50);
			    //TweenPosition.Begin(vipGetExpTips.gameObject,1,vipGetExpTips.transform.localPosition+new Vector3(0,50,0));
				//TweenAlpha.Begin(vipGetExpTips.gameObject,1,0);


                TweenPosition.Begin(GetExpLabel.gameObject, 0.166f, GetExpLabel.transform.localPosition + new Vector3(0, 11, 0) );
                DoForTime.DoFunForTime(0.333f, ShowVipAddExpEffect, null);
			}
        }

        void ShowVipAddExpEffect(object obj)
        {
            SingleButtonCallBack vipGetExpTips = VipExpEffectPoint.InstantiateNGUIObj(VipExpTipsPrefab).GetComponent<SingleButtonCallBack>();
            vipGetExpTips.SetButtonText(string.Format(LanguageTextManager.GetString("IDS_I21_8"),PlayerDataManager.Instance.GetEctypeExpBonus()));
        }

        void SetGradExp(float value)
        {
            string format = LanguageTextManager.GetString("IDS_I4_21");
            string msg = string.Format(format, (int)value);
            GetExpLabel.SetText(msg);
        }

        void TweenShowPanel03Value01(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_AppraisalFields");
			TweenFloat.Begin(TweenAnimTime,0,this.sMSGEctypeSettleAccounts.dwHighestCombo,SetDoubeHitNumber,TweenShowPanel03Value02);
        }
        void SetDoubeHitNumber(float value)
        {
			MaxHitNumberLabel.SetText((int)value);
        }

        void TweenShowPanel03Value02(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_AppraisalFields");
            TweenFloat.Begin(TweenAnimTime,0,this.sMSGEctypeSettleAccounts.dwHPRate,SetBeHitNumber,TweenShowPanel03Value03);
        }
        void SetBeHitNumber(float value)
        {
            RemainHPLabel.SetText((int)value);
        }

        void TweenShowPanel03Value03(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_AppraisalFields");
            int minute = (int)this.sMSGEctypeSettleAccounts.dwTime/1000/60;
            int sconds = (int)this.sMSGEctypeSettleAccounts.dwTime/1000%60;
            int ms = Random.Range(1,99) ;
            TweenFloat.Begin(TweenAnimTime, 0, minute, SetTimeMinute);
            TweenFloat.Begin(TweenAnimTime, 0, sconds, SetTimeSconds,TweenShowPanel03Value04);
            //TweenFloat.Begin(TweenAnimTime,0,ms,SetTimeMs);
        }
         
        void SetTimeMinute(float value)
        {
            PassLevelTimeLabel_Minute.SetText((int)value);
        }
        void SetTimeSconds(float value)
        {
            PassLevelTimeLabel_Second.SetText((int)value);
        }
        //void SetTimeMs(float value)
        //{
        //    PassLevelTimeLabel_Ms.SetText((int)value);
        //}

        void TweenShowPanel03Value04(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_AppraisalFields");
			GradLabel.SetText(this.sMSGEctypeSettleAccounts.sGrade);
            float targetvalue = (float)this.sMSGEctypeSettleAccounts.dwGradeCount/200;
			TweenFloat.Begin(TweenAnimTime, 0, targetvalue, SetGradBar,TweenShowGradStar);
            if (CheckIsUpgrade())
            {
                ShowUpgradeLevelEffect();
                TraceUtil.Log("ShowUpgradEffect");
            }
            else
            {
                TraceUtil.Log("UnShowUpgradEffect");
                DoForTime.DoFunForTime(2, TweenDisCardPanel, null);
            }
        }
        void SetGradBar(float value)
        {
            GradBar.sliderValue = value;
        }

		void TweenShowGradStar(object obj)
		{
			var starPrefab = MyParent.EvaluateData.EvaluateDataList.First(P=>P.Evaluate == this.sMSGEctypeSettleAccounts.sGrade).StarIconPrefab;
			GameObject newObj = GradPos.InstantiateNGUIObj(starPrefab);
			TweenScale.Begin(GradPos.gameObject,TweenAnimTime,new Vector3(3,3,3),Vector3.one,null);
		}

        bool CheckIsUpgrade()
        {
            bool isUpgrade = false;
            TypeID entityType = TypeID.TYPEID_BOX;
            long uid = PlayerManager.Instance.FindHeroDataModel().UID;
            EntityModel HeroEntityModel = EntityController.Instance.GetEntityModel(uid, out entityType);
            if (HeroEntityModel != null && UI.Town.HeroUpgradeLevelData.Instance.GetLevel() != 0)
            {
                int LastLevel = UI.Town.HeroUpgradeLevelData.Instance.GetLevel();
                int currentLevel = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
                isUpgrade = currentLevel > LastLevel;
            }
            return isUpgrade;
        }

        void ShowUpgradeLevelEffect()
        {
            GameObject newObj =  CreatObjectToNGUI.InstantiateObj(UpgradeLevelEffectPrefab, CreatUpgradeEffectPoint);
            DoForTime.DoFunForTime(4, DestroyObj, newObj);
        }

        void DestroyObj(object obj)
        {
            GameObject destroyObj = obj as GameObject;
            if (destroyObj != null)
            {
                Destroy(destroyObj);
            }
            TweenDisCardPanel(null);
        }
        
        void TweenDisCardPanel(object obj)
        {
            TweenFloat.Begin(TweenAnimTime,1,0,SetPanelAlpha);
            MyParent.ShowPanel01Complete(null);
            VipExpEffectPoint.gameObject.SetActive(false);
        }
        void SetPanelAlpha(float value)
        {
            UpPanel.alpha = value;
            DownPanel.alpha = value;
        }
    }
}
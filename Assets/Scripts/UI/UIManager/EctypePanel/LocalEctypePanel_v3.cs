using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{
    public class LocalEctypePanel_v3 : MonoBehaviour
    {


        public SpriteSwith Background_NorMal;
        public SpriteSwith Background_Hard;
        public SpriteSwith Background_Siren;
        public SingleButtonCallBack NameTitle;
        public SingleButtonCallBack DayTimesLabel;
        public SingleButtonCallBack GetItemLabel;
        public Transform CreatEctypeIconTransform;
        UISprite EctypeIcon;
        public Transform CreatSirenIconTransform;
        UISprite SirenEctypeIcon;
        public UILabel Label_SirenTime;
        public UILabel SuggestionLabel;
        public SingleButtonCallBack lockLvLabel;
        public SingleButtonCallBack GradLabel;//评级面板

        public GameObject Eff_Refining_Broken;
        public GameObject Eff_Refining_AppearLoop;
        public Transform Eff_emission;

        EctypePanel_V4 myParent;
        public EctypeContainerData ectypeContainerData { get; private set; }

        //public SMSGEctypeData_SC sMSGEctypeData_SC { get; private set; }

        bool IsLock = true;

        private int m_guideBtnID;
        public int ContainerID { set; get; }  //引导用

        private float m_SirenTime;  //妖女副本存活时间(秒)
        private int m_lastMinuteTime;   //上一分钟时间
        private bool m_IsCountDown = false;

        public bool IsSirenEctype { get; private set; }

        public void InitGuide(GameObject go)
        {
            //TODO GuideBtnManager.Instance.RegGuideButton(go, UIType.Battle, SubType.EctypeCard, out m_guideBtnID);
        }

        void OnDestroy()
        {
            //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
        }

        public void InitPanel(int EctypeID, EctypePanel_V4 myParent)
        {
            this.myParent = myParent;
            ectypeContainerData = EctypeConfigManager.Instance.EctypeContainerConfigList[EctypeID];
            CreatEctypeIconTransform.ClearChild();
            this.EctypeIcon = CreatObjectToNGUI.InstantiateObj(myParent.EctypeContainerIconData.iconDataList.First(P=>P.lEctypeContainerID == ectypeContainerData.lEctypeContainerID&&
                P.lDifficulty == ectypeContainerData.lDifficulty).EctypeIconPrefab,CreatEctypeIconTransform).GetComponent<UISprite>();
            this.EctypeIcon.color = Color.gray;
            this.SuggestionLabel.SetText("");
            this.NameTitle.SetButtonText(ectypeContainerData.MapType == 1 ?"":LanguageTextManager.GetString(ectypeContainerData.lEctypeName));
            GradLabel.gameObject.SetActive(false);
            this.lockLvLabel.SetButtonText(string.Format("{0}{1}{2}", ectypeContainerData.lMinActorLevel, LanguageTextManager.GetString("IDS_H1_156"), LanguageTextManager.GetString("IDS_H2_56")));
            this.NameTitle.SetTextColor(ectypeContainerData.MapType == 0 || ectypeContainerData.MapType == 7 ? Color.white : Color.red);
            this.Background_NorMal.gameObject.SetActive(ectypeContainerData.MapType == 0 || ectypeContainerData.MapType == 7 ? true : false);
            //TraceUtil.Log("ectypeContainerData.MapType:" + ectypeContainerData.MapType);
            this.Background_Hard.gameObject.SetActive(ectypeContainerData.MapType == 1? true : false);
            this.Background_Siren.gameObject.SetActive(false);
            InitGuide(this.gameObject);
            //TraceUtil.Log("初始话副本：" + LanguageTextManager.GetString(ectypeContainerData.lEctypeName) + "," + EctypeID);
            SetPosition();
        }

        void SetPosition()
        {
            int Line = int.Parse(ectypeContainerData.lEctypePos[1]);
            int Row = int.Parse(ectypeContainerData.lEctypePos[2]);
            transform.localPosition = new Vector3(-250+250*(Row-1),100-180*(Line-1),0);            
        }

        //public GameObject UnlockObj;
        void Update()
        {
            if (m_IsCountDown)
            {
                m_SirenTime -= Time.deltaTime;
                int curMinuteTime = (int)(m_SirenTime / 60);
                if (curMinuteTime != m_lastMinuteTime)
                {
                    m_lastMinuteTime = curMinuteTime;
                    Label_SirenTime.text = ParseTime(curMinuteTime);
                }
            }
        }

//        public void UnlockMyself(SMSGEctypeData_SC sMSGEctypeData_SC)
//        {
//            //if (ectypeContainerData.lMinActorLevel > PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL)
//            //    return;
//            //UnlockObj = gameObject;
//            this.sMSGEctypeData_SC = sMSGEctypeData_SC;
//            this.IsLock = false;
//            if (this.EctypeIcon != null)
//            {
//                this.EctypeIcon.color = Color.white;
//            }
//            else
//            {
//                TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"未初始副本收到解锁消息：" + sMSGEctypeData_SC.dwEctypeID + "," + sMSGEctypeData_SC.byDiff + "," + LanguageTextManager.GetString(this.ectypeContainerData.lEctypeName));
//                //TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"emptyEctypeIcon");
//            }
//            this.SuggestionLabel.SetText(string.Format(LanguageTextManager.GetString("IDS_H1_501"),NGUIColor.SetTxtColor(this.ectypeContainerData.PlayerNum.ToString(),TextColor.red)));
//            this.lockLvLabel.gameObject.SetActive(false);
//            //TraceUtil.Log("已经解锁副本评级:" + sMSGEctypeData_SC.byGrade);
//            if (sMSGEctypeData_SC.byGrade != 0)
//            {
//                GradLabel.gameObject.SetActive(true);
//                GradLabel.SetButtonBackground(sMSGEctypeData_SC.byGrade);
//            }
//            this.DayTimesLabel.gameObject.SetActive(sMSGEctypeData_SC.byMaxDayTimes > 0 ? true : false);
//            this.DayTimesLabel.SetButtonText(string.Format(LanguageTextManager.GetString("IDS_H1_315"), sMSGEctypeData_SC.byCurDayTimes, sMSGEctypeData_SC.byMaxDayTimes));
//            bool DropInfoIsNull = ectypeContainerData.DropInf.Equals("0") ;
//            this.GetItemLabel.gameObject.SetActive(!DropInfoIsNull);
//            if (!DropInfoIsNull)
//            {
//                //bool FirstFlag = sMSGEctypeData_SC.byFirstFlag == 1 ? true : false;
//                //TraceUtil.Log("封魔副本是否首杀：" + sMSGEctypeData_SC.byFirstFlag);
//                //TraceUtil.Log("封魔副本日使用次数：" + sMSGEctypeData_SC.byCurDayTimes + "," + sMSGEctypeData_SC.byMaxDayTimes);
//                this.GetItemLabel.SetButtonText(LanguageTextManager.GetString(ectypeContainerData.DropInf.Split('+')[sMSGEctypeData_SC.byFirstFlag]));
//            }
//            if (this.sMSGEctypeData_SC.dwEctypeID == 120&&this.sMSGEctypeData_SC.byDiff == 1)
//            {
//                this.OnClick();
//            }
//        }
//
//        public void UnSelectMyself(SMSGEctypeData_SC data)
//        {
//            if (sMSGEctypeData_SC.byDiff!=data.byDiff||sMSGEctypeData_SC.dwEctypeID!=data.dwEctypeID)
//            {
//                Background_NorMal.ChangeSprite(1);
//                Background_Hard.ChangeSprite(1);
//                Background_Siren.ChangeSprite(1);
//            }
//        }
        /// <summary>
        /// 创建妖女副本
        /// </summary>
        /// <param name="regionID">区域id</param>
        /// <param name="ectypeID">副本id</param>
        /// <param name="myParent">父体</param>
        /// <param name="time">时间(毫秒)</param>
        public void CreateSirenPanel(int regionID, int ectypeID, EctypePanel_V4 myParent, int time)
        {
            IsSirenEctype = true;
            this.myParent = myParent;
            ectypeContainerData = EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeID];
            //this.EctypeIcon.spriteName = ectypeContainerData.lEctypeIcon;
            if (EctypeIcon != null)
            {
                this.EctypeIcon.gameObject.SetActive(false);
                this.EctypeIcon.color = Color.gray;
            }
            //this.SirenEctypeIcon.gameObject.SetActive(true);
            CreatSirenIconTransform.ClearChild();
            this.SirenEctypeIcon = CreatObjectToNGUI.InstantiateObj( myParent.EctypeContainerIconData.iconDataList.First(
                P=>P.lEctypeContainerID == ectypeContainerData.lEctypeContainerID&&P.lDifficulty == ectypeContainerData.lDifficulty
                ).EctypeIconPrefab,CreatSirenIconTransform).GetComponent<UISprite>();
            //this.SirenEctypeIcon.spriteName = ectypeContainerData.lEctypeIcon;
            this.SuggestionLabel.SetText("");
            //this.NameTitle.SetButtonText(LanguageTextManager.GetString(ectypeContainerData.lEctypeName));
            this.lockLvLabel.SetButtonText(string.Format("{0}{1}{2}", ectypeContainerData.lMinActorLevel, LanguageTextManager.GetString("IDS_H1_156"), LanguageTextManager.GetString("IDS_H2_56")));
            //this.NameTitle.SetButtonBackground(ectypeContainerData.MapType == 0?1:2);
            //this.NameTitle.SetTextColor(ectypeContainerData.MapType == 0 ? Color.white : Color.red);
            this.NameTitle.gameObject.SetActive(false);
            this.GradLabel.gameObject.SetActive(false);
            this.Background_NorMal.gameObject.SetActive(false);
            //TraceUtil.Log("CreatSirenPanel");
            this.Background_Hard.gameObject.SetActive(false);
            this.Background_Siren.gameObject.SetActive(true);

            SetPosition();
            transform.localPosition += new Vector3(0, 0, -50);//盖住前一个ectypeCard

//            this.sMSGEctypeData_SC = new SMSGEctypeData_SC()
//            {
//                dwEctypeID = (uint)regionID,
//                byDiff = (byte)ectypeContainerData.lDifficulty,
//            };
            this.IsLock = false;
            if (EctypeIcon != null)
            { this.EctypeIcon.color = Color.white; }
            this.lockLvLabel.gameObject.SetActive(false);

            //时间
            m_SirenTime = time / 1000f;
            m_lastMinuteTime = (int)(m_SirenTime / 60);
            m_IsCountDown = true;
            Label_SirenTime.gameObject.SetActive(true);
            Label_SirenTime.text = ParseTime(m_lastMinuteTime);

            //特效
            GameObject eff_broke = (GameObject)Instantiate(Eff_Refining_Broken);
            eff_broke.transform.parent = Eff_emission;
            eff_broke.transform.localPosition = Vector3.zero;
            eff_broke.transform.localScale = Eff_Refining_Broken.transform.localScale;
            var containEctype = EctypeConfigManager.Instance.EctypeSelectConfigList.Values.SingleOrDefault(p => p._sirenEctypeContainerID == ectypeID);
            var ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[containEctype._vectContainer.First()];
            EctypeAppearBrokenChangeTexture cTex = eff_broke.GetComponent<EctypeAppearBrokenChangeTexture>();
            cTex.ChangeTexture(ectypeData.lEctypeIcon);
            GameObject eff_loop = (GameObject)Instantiate(Eff_Refining_AppearLoop);
            eff_loop.transform.parent = Eff_emission;
            eff_loop.transform.localPosition = Vector3.zero;
            eff_loop.transform.localScale = Eff_Refining_AppearLoop.transform.localScale;
			
			InitGuide(this.gameObject);
        }

        private string ParseTime(int minutes)
        {
            int hour = minutes / 60;
            int min = minutes % 60;
            return ParseClock(hour) + ":" + ParseClock(min);
        }
        string ParseClock(int time)
        {
            if (time < 10)
            {
                return "0" + time.ToString();
            }
            return time.ToString();
        }

        public bool SelectMyself()
        {
            if (IsLock)
                return false;
            else
            {
                OnClick();
                return true;
            }
        }

        void OnClick()
        {
            if (IsLock)
                return;
            else
            {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
                Background_NorMal.ChangeSprite(2);
                Background_Hard.ChangeSprite(2);
                Background_Siren.ChangeSprite(2);
                myParent.OnSelectEctype(this);
            }
        }


    }
}
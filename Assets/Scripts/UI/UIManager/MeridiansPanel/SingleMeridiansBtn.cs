using UnityEngine;
using System.Collections;

namespace UI.MainUI
{

    public class SingleMeridiansBtn : MonoBehaviour
    {
        public UISlider SliderBar;
        public SpriteSwith SelectSprite;
        public SpriteSwith Background;

        MeridiansDragPanel MyParent;
        PlayerMeridiansData MyData;
        private int MyDataLvUpNeed = 0;

        public int m_MeridiansID { get; private set; }//当前经脉对应的经脉ID

        public bool IsUnlock { get; private set; }//是否已经解锁经脉
        private int CurrentUpLv = 0;//当前经脉修为值
        private GameObject TweenObj;

        private int m_guideBtnID;
        void Awake()
        {
            //TODO GuideBtnManager.Instance.RegGuideButton(this.gameObject, UIType.Meridians, SubType.MeridiansPoint, out m_guideBtnID);
            OnSelectBtn(-1);
        }

        void OnDestroy()
        {
            //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
        }

        public void Init(int meridiansID, MeridiansDragPanel myParent)
        {
            //if (meridiansID == 0)//经脉ID为0时特殊处理
            //{
            //    this.MyParent = myParent;
            //    this.m_MeridiansID = meridiansID;
            //    IsUnlock = true;
            //    Background.ChangeSprite(1);
            //    return;
            //}
            int MeridiansLv = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MERIDIANS_LEVEL+1;
            CurrentUpLv = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HAVEPRACTICE_NUM;
            MyData = myParent.MyParent.PlayerMeridiansDataManager.GetMeridiansData(meridiansID);
            MyDataLvUpNeed = myParent.MyParent.PlayerMeridiansDataManager.GetMeridiansData(meridiansID - 1).LevelUpNeed;
            this.m_MeridiansID = meridiansID;
            this.MyParent = myParent;
            IsUnlock = m_MeridiansID < MeridiansLv ;
            this.Background.ChangeSprite(IsUnlock?1:2);
            SetSliderBar(IsUnlock ?1 :  MeridiansLv == this.m_MeridiansID ? (float)CurrentUpLv / (float)MyDataLvUpNeed : 0);
            if (MeridiansLv == MyData.MeridiansLevel && MyParent.MyParent.CurrentPageID == myParent.PanelPositionID)
            {
                OnClick();
                //TraceUtil.Log("当前经脉ID：" + MeridiansLv + ",当前修炼进度：" + CurrentUpLv + ",当前经脉所需经脉值：" + MyDataLvUpNeed); 
            }
            else if (MyParent.MyParent.CurrentPageID == myParent.PanelPositionID && MeridiansLv == MyData.MeridiansLevel && !myParent.CheckIsInMyPanel(MeridiansLv))//检测是否该面板最后一个
            {
                TraceUtil.Log("选中当前面板最后一个经脉");
                OnClick();
            }
        }

        public void OnSelectBtn(int meridiansID)
        {
            SelectSprite.ChangeSprite(meridiansID == this.m_MeridiansID ? 1 : 0);
        }

        void OnClick()
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            MyParent.MyParent.OnMeridiansBtnClick(this.m_MeridiansID);
            int MeridiansLv = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MERIDIANS_LEVEL+1;
            int needAddEffeNum = m_MeridiansID == MeridiansLv ? (MyDataLvUpNeed - CurrentUpLv) : 0;
            MyParent.MyParent.ShowMeridiansDesInfo(this.m_MeridiansID, IsUnlock, needAddEffeNum.ToString());
            //TraceUtil.Log("点击按钮："+IsUnlock);
        }
        /// <summary>
        /// 增加经脉修为值，升级当前经脉时增加
        /// </summary>
        public void AddLevelUpNeed(bool Flag)
        {
            //TraceUtil.Log("修炼经脉:当前ID：" + (PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MERIDIANS_LEVEL + 1)+",MyID:"+m_MeridiansID);int maxMeridiansID = 0;
            if (PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MERIDIANS_LEVEL+1 != m_MeridiansID)
            {
                return;
            }
            OnClick();
            int fromValue = CurrentUpLv;
            int ToValue = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HAVEPRACTICE_NUM+PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_PRACTICE_NUM;
            ToValue = ToValue > MyDataLvUpNeed ? MyDataLvUpNeed : ToValue;
            if(ToValue<fromValue)
                return;
            //float duration = MyDataLvUpNeed > 5 ? (float)(MyDataLvUpNeed - fromValue) / (float)MyDataLvUpNeed * 5 : 1;
            if (Flag)
            {
                if (MyParent.playerKongfuData.LevelNeed > PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL)
                {//提示等级不足
                    //TraceUtil.Log("当前内功："+ LanguageTextManager.GetString(MyParent.playerKongfuData.KongfuName)+",当前内功等级："+MyParent.playerKongfuData.LevelNeed);
                    MessageBox.Instance.ShowTips(3, string.Format(LanguageTextManager.GetString("IDS_H1_374"), MyParent.playerKongfuData.LevelNeed),1);
                    return;
                }
                //if (fromValue == ToValue && ToValue < MyDataLvUpNeed)//提示修为不足
                if (ToValue < MyDataLvUpNeed)//提示修为不足
                {
                    MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_377"), 1);
                    return;
                }
                if (TweenObj != null)
                {
                    Destroy(TweenObj);
                }
                //TweenObj = TweenFloat.Begin(duration, fromValue, ToValue, SetCurrentUpLv, AddUpLvNeedComplete);
                SetCurrentUpLv(ToValue);
                AddUpLvNeedComplete(null);
            }
            else if(TweenObj!=null)
            {
                Destroy(TweenObj);
                AddUplevelFinish();
            }
            //SetSliderBar(CurrentUpLv / MyDataLvUpNeed);
        }

        void SetCurrentUpLv(float Lv)
        {
            //TraceUtil.Log("AddNumber:"+Lv);
            int lv = (int) Lv;
            SetSliderBar(Lv / MyDataLvUpNeed);
            if (CurrentUpLv == lv)
                return;
            CurrentUpLv = lv;
            MyParent.MyParent.MeridiansNumLabel.SetButtonText((PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_PRACTICE_NUM -
                (CurrentUpLv - PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HAVEPRACTICE_NUM)).ToString());
            MyParent.MyParent.ShowMeridiansDesInfo(this.m_MeridiansID, IsUnlock, (MyDataLvUpNeed - CurrentUpLv).ToString());
        }

        void AddUpLvNeedComplete(object obj)
        {
            TweenObj = null;
            if (CurrentUpLv < MyDataLvUpNeed)//提示修为不足
            {
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_377"),1);
            }
            else//提示修炼完毕
            {
                SoundManager.Instance.PlaySoundEffect("Sound_UIEff_MeridiansSuccess");
                //MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_373"), 1);
                MyParent.MyParent.CreatKonfuThroughEffect(transform);
                if (MyData.MeridiansLevel.ToString() == MyParent.playerKongfuData.MeridiansList[MyParent.playerKongfuData.MeridiansList.Length - 1])
                {
                    //提示内功修炼完毕
                    MyParent.MyParent.CreatMeridianSeccessEffect(MyParent.transform);
                }
                //IsUnlock = true;
            }
            AddUplevelFinish();
            MyParent.MyParent.AddMeridians(false);
        }

        void AddUplevelFinish()
        {
            SendChangeMeridiansToServer();
        }

        /// <summary>
        /// 发送修改经脉到服务器
        /// </summary>
        void SendChangeMeridiansToServer()
        {
            int konfuID = MyParent.playerKongfuData.KongfuLevel;
            int meridianID = MyData.MeridiansLevel;
            int parcticeNum = CurrentUpLv - PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HAVEPRACTICE_NUM;
            if (parcticeNum > 0)
            {
                NetServiceManager.Instance.EntityService.SendMsgActionMeridianParctice(konfuID, meridianID-1, parcticeNum);
                TraceUtil.Log("发送修改修为到服务器,Add:" + meridianID +","+ parcticeNum);
            }
        }

        ///// <summary>
        ///// 由于最初需求导致前后台差了一个点，最后一个经脉ID修炼后自己设置状态
        ///// </summary>
        //void SetLastMeridiansBtnUnlockStatus()
        //{
        //    if (m_MeridiansID == MyParent.MyParent.MaxMeridiansID)
        //    {
        //        this.IsUnlock = true;
        //        this.Background.ChangeSprite(IsUnlock ? 1 : 2);
        //    }
        //}

        /// <summary>
        /// 设置经脉进度条
        /// </summary>
        /// <param name="Number"></param>
        void SetSliderBar(float Value)
        {
            this.SliderBar.sliderValue = 1-Value;
        }


    }
}